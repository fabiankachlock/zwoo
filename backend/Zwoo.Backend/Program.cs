using Quartz;
using Zwoo.Backend.Websockets;
using Zwoo.Backend.Games;
using Zwoo.Backend.Services;
using Zwoo.GameEngine.Lobby.Features;
using Zwoo.Backend.Shared.Services;
using Zwoo.Backend.Shared.Api;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Backend.Shared.Authentication;
using Zwoo.Backend.Shared.Api.Discover;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddZwooLogging();
var conf = builder.AddZwooConfiguration(args, new ZwooAppConfiguration()
{
    AppVersion = "1.0.0-beta.17"
});
builder.Services.AddZwooCors(conf);
builder.Services.AddZwooAuthentication(conf);
builder.Services.AddZwooDatabase(conf, new ZwooDatabaseOptions()
{
    EnableMigrations = true
});

// backend services
builder.Services.AddZwooServices();
builder.Services.AddSingleton<IExternalGameProfileProvider, BackendGameProfileProvider>();
builder.Services.AddSingleton<IGameEngineService, GameEngineService>();
builder.Services.AddSingleton<IWebSocketManager, Zwoo.Backend.Websockets.WebSocketManager>();
builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();

// scheduler
builder.Services.AddZwooScheduler(q =>
{
    q.ScheduleJob<DatabaseCleanupJob>(t => t
                .WithIdentity("cleanup", "db")
                .WithCronSchedule("0 1 1 1/1 * ? *"));
});
builder.Services.AddTransient<DatabaseCleanupJob>();

var app = builder.Build();

app.UseZwooHttpLogging();
app.UseZwooCors();
app.UseZwooAuthentication();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
webSocketOptions.AllowedOrigins.Add(conf.Server.Cors);

app.UseWebSockets(webSocketOptions);
app.MapControllers();
app.UseDiscover();

app.Run();

