using BackendHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Mongo.Migration.Documents;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Quartz;
using Quartz.Impl;
using ZwooBackend;
using ZwooBackend.Websockets;
using ZwooBackend.Games;
using ZwooBackend.Database;
using ZwooBackend.Services;
using ZwooDatabaseClasses;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(s => s.AddPolicy("Zwoo",
    b => b.WithOrigins(Globals.Cors).AllowAnyHeader().AllowAnyMethod()
        .AllowCredentials()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromDays(90);
    // Dont use SlidingExpiration, because its an security issue!
    o.Cookie.Name = "auth";
    o.Cookie.HttpOnly = Globals.UseSsl;
    o.Cookie.MaxAge = o.ExpireTimeSpan;
    if (Globals.UseSsl)
    {
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.Cookie.SameSite = SameSiteMode.None;
    }
    o.Cookie.Domain = Globals.ZwooCookieDomain;
});

// Database
builder.Services.AddSingleton(Globals.ZwooDatabase._client);
builder.Services.Configure<MongoMigrationSettings>(options =>
{
    options.ConnectionString = Globals.ConnectionString;
    options.Database = "zwoo";
    options.DatabaseMigrationVersion = new DocumentVersion(Globals.Version);
});
builder.Services.AddMigration(new MongoMigrationSettings
{
    ConnectionString = Globals.ConnectionString,
    Database = "zwoo",
    DatabaseMigrationVersion = new DocumentVersion(Globals.Version)
}
);

builder.Services.AddSingleton<IGameLogicService, GameLogicService>();
builder.Services.AddSingleton<IWebSocketManager, ZwooBackend.Websockets.WebSocketManager>();
builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<ILanguageService, LanguageService>();

builder.Services.AddHostedService<EmailService>();

var app = builder.Build();

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

// Configure the HTTP request pipeline.
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


var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
scheduler.Start();
scheduler.ScheduleJob(
    JobBuilder.Create<DatabaseCleanupJob>().WithIdentity("db_cleanup", "db").Build(),
    TriggerBuilder.Create().WithCronSchedule("0 1 1 1/1 * ? *").Build()); // Every Day at 00:01 UTC+1


app.Run();

scheduler.Shutdown();
