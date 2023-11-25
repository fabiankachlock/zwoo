using Quartz;
using Quartz.Simpl;
using Zwoo.Backend;
using Zwoo.Backend.Websockets;
using Zwoo.Backend.Games;
using Zwoo.Backend.Services;
using Zwoo.GameEngine.Lobby.Features;
using Zwoo.Backend.Shared.Services;
using Zwoo.Backend.Shared.Api;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Backend.Shared.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.AddZwooLogging();
var conf = builder.AddZwooConfiguration(args, new ZwooAppConfiguration()
{
    AppVersion = "1.0.0-beta.17"
});
builder.AddZwooCors(conf);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddZwooAuthentication(conf);
builder.AddZwooDatabase(conf, new ZwooDatabaseOptions()
{
    EnableMigrations = true
});

// backend services
builder.AddZwooServices();
builder.Services.AddSingleton<IExternalGameProfileProvider, BackendGameProfileProvider>();
builder.Services.AddSingleton<IGameEngineService, GameEngineService>();
builder.Services.AddSingleton<IWebSocketManager, Zwoo.Backend.Websockets.WebSocketManager>();
builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();

// scheduler
builder.AddZwooScheduler(q =>
{
    q.ScheduleJob<DatabaseCleanupJob>(t => t
                .WithIdentity("cleanup", "db")
                .WithCronSchedule("0 1 1 1/1 * ? *"));
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

