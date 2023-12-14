using System.Text.Json.Serialization;
using Zwoo.Backend.LocalServer.Authentication;
using Zwoo.Backend.LocalServer.Services;
using Zwoo.Backend.Shared.Api;
using Zwoo.Backend.Shared.Api.Discover;
using Zwoo.Backend.Shared.Api.Game;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Backend.Shared.Services;
using Zwoo.Database.Dao;
using Zwoo.GameEngine.Lobby.Features;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApiJsonSerializerContext.Default);
});

builder.AddZwooLogging();
var conf = builder.AddZwooConfiguration(args, new ZwooAppConfiguration()
{
    AppVersion = "1.0.0-beta.17"
});
builder.Services.AddCors(s =>
{
    s.AddDefaultPolicy(b => b
        .WithOrigins("localhost", "zwoo.igd20.de")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});
// TODO: get server id
builder.Services.AddLocalAuthentication("server");

builder.Services.AddSingleton<IGameDatabaseAdapter, Mock>();
builder.Services.AddSingleton<IExternalGameProfileProvider, EmptyGameProfileProvider>();
builder.Services.AddSingleton<ILocalUserManager, LocalUserManager>();

// backend services
builder.Services.AddZwooServices();
builder.Services.AddGameServices();

var app = builder.Build();

app.UseZwooHttpLogging();
app.UseZwooCors();
app.UseLocalAuthentication();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
webSocketOptions.AllowedOrigins.Add(conf.Server.Cors);

app.UseWebSockets(webSocketOptions);
app.UseDiscover();
app.UseGame();

app.Run();


[JsonSerializable(typeof(GuestLogin))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

class Mock : IGameDatabaseAdapter
{
    public void IncrementUserWin(long userId)
    {
        throw new NotImplementedException();
    }

    public void SaveGame(GameInfoDao gameInfo)
    {
        throw new NotImplementedException();
    }
}
