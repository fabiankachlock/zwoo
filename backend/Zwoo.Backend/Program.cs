using Microsoft.AspNetCore.Authentication.Cookies;
using Mongo.Migration.Documents;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Quartz;
using Quartz.Simpl;
using Zwoo.Backend;
using Zwoo.Backend.Websockets;
using Zwoo.Backend.Games;
using Zwoo.Backend.Services;
using Zwoo.Database;
using Zwoo.GameEngine.Lobby.Features;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(s =>
{
    s.AddPolicy("Zwoo", b => b
        .WithOrigins(Globals.Cors)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());

    s.AddPolicy("ContactForm", b => b
        .WithOrigins(Globals.ContactFormExtraCorsOrigin)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromDays(90);
    // Dont use SlidingExpiration, because its an security issue!
    o.Cookie.Name = "auth";
    o.Cookie.HttpOnly = true;
    o.Cookie.MaxAge = o.ExpireTimeSpan;
    if (Globals.UseSsl)
    {
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.Cookie.SameSite = SameSiteMode.None;
    }
    o.Cookie.Domain = Globals.ZwooCookieDomain;
});

// database
var db = new Zwoo.Database.Database(Globals.ConnectionString, Globals.DatabaseName, Globals.DatabaseLogger);

builder.Services.AddSingleton<IDatabase>(db);
builder.Services.AddSingleton<IAuditTrailService, AuditTrailService>();
builder.Services.AddSingleton<IAccountEventService, AccountEventService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IBetaCodesService, BetaCodesService>();
builder.Services.AddSingleton<IChangelogService, ChangelogService>();
builder.Services.AddSingleton<IGameInfoService, GameInfoService>();
builder.Services.AddSingleton<IContactRequestService, ContactRequestService>();

// migrations
builder.Services.AddSingleton(db.Client);
builder.Services.Configure<MongoMigrationSettings>(options =>
{
    options.ConnectionString = Globals.ConnectionString;
    options.Database = Globals.DatabaseName;
    options.DatabaseMigrationVersion = new DocumentVersion(Globals.Version);
});
builder.Services.AddMigration(new MongoMigrationSettings
{
    ConnectionString = Globals.ConnectionString,
    Database = Globals.DatabaseName,
    DatabaseMigrationVersion = new DocumentVersion(Globals.Version)
});

// backend services
builder.Services.AddSingleton<IExternalGameProfileProvider, BackendGameProfileProvider>();
builder.Services.AddSingleton<IGameLogicService, GameLogicService>();
builder.Services.AddSingleton<IWebSocketManager, Zwoo.Backend.Websockets.WebSocketManager>();
builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<ILanguageService, LanguageService>();
builder.Services.AddSingleton<ICaptchaService, CaptchaService>();

builder.Services.AddHostedService<EmailService>();

// scheduler
builder.Services.AddQuartz(q =>
{
    q.SchedulerId = "zwoo-scheduler";
    q.UseThreadPool<DefaultThreadPool>(c =>
    {
        c.MaxConcurrency = 1;
    });
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.ScheduleJob<DatabaseCleanupJob>(t => t
        .WithIdentity("cleanup", "db")
        .WithCronSchedule("0 1 1 1/1 * ? *"));
});
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});
builder.Services.AddTransient<DatabaseCleanupJob>();

var app = builder.Build();

// http logging
app.Use(async (context, next) =>
{
    if (context.Request.Method != "OPTIONS")
    {
        Globals.HttpLogger.Info($"[{context.Request.Method}] {context.Request.Path}");
    }
    await next.Invoke();
    if (context.Response.StatusCode >= 300)
    {
        Globals.HttpLogger.Info($"sending error response: {context.Response.StatusCode} ([{context.Request.Method}] {context.Request.Path})");
    }
});

if (app.Environment.IsDevelopment())
{
    Globals.Logger.Debug("adding swagger");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
};
webSocketOptions.AllowedOrigins.Add(Globals.Cors);

app.UseCors("Zwoo");
app.UseWebSockets(webSocketOptions);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

