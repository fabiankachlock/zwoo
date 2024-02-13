using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
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

var config = builder.AddServerConfiguration(args);
builder.WebHost.ConfigureKestrel(options =>
{
    var port = config.UseDynamicPort ? 0 : config.Port;

    if (config.UseAllIPs)
    {
        options.ListenAnyIP(port);
        return;
    }

    if (config.UseLocalhost)
    {
        options.ListenLocalhost(port);
    }

    if (IPAddress.TryParse(config.IP, out var ip))
    {
        options.Listen(ip, port);
    }

    options.Listen(new IPAddress(0), port);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApiJsonSerializerContext.Default);
});

builder.AddZwooLogging(false);
var conf = builder.AddZwooConfiguration(args, new ZwooAppConfiguration()
{
    AppVersion = "1.0.0-beta.17"
});
builder.Services.AddCors(s =>
{
    s.AddDefaultPolicy(b => b
        .WithOrigins("http://localhost:8080", "zwoo.igd20.de")
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

// group all api endpoints under /api
var api = app.MapGroup("/api");
app.UseLocalAuthentication(api);
// require authentication only for all api endpoints
api.RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

// serve frontend files
var provider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "frontend");
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = provider,
    ServeUnknownFileTypes = true,
});

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
webSocketOptions.AllowedOrigins.Add(conf.Server.Cors);

app.UseWebSockets(webSocketOptions);
api.UseDiscover();
api.UseGame();

// serve index.html for all other requests
var index = provider.GetFileInfo("index.html");
app.MapGet("", async context =>
{
    await context.Response.SendFileAsync(index);
});

app.Run();

[JsonSerializable(typeof(GuestLogin))]
[JsonSerializable(typeof(ProblemDetails))]
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
