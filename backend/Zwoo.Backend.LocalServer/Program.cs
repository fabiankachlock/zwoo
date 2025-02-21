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
using Zwoo.GameEngine.ZRP;

if (args.Length >= 1 && args[0] == "-h")
{
    new ServerConfig().PrintHelp();
    return;
}

var builder = WebApplication.CreateSlimBuilder(args);

var config = builder.AddServerConfiguration(args);
// resolve listening options
int listeningPort = config.Port == 0 ? 8001 : config.Port;
if (config.UseDynamicPort) listeningPort = 0;

string listeningIp = "0.0.0.0";
if (config.UseAllIPs) listeningIp = "127.0.0.1";
else if (config.UseLocalhost) listeningIp = "127.0.0.1";
else if (IPAddress.TryParse(config.IP, out var ip)) listeningIp = config.IP;

builder.WebHost.ConfigureKestrel(options =>
{
    Console.WriteLine($"[Config] Configuring Server to listen on {listeningIp}:{listeningPort}");
    if (config.UseAllIPs) options.ListenAnyIP(listeningPort);
    else if (config.UseLocalhost) options.ListenLocalhost(listeningPort);
    else if (IPAddress.TryParse(config.IP, out var ip)) options.Listen(ip, listeningPort);
    else options.Listen(new IPAddress(0), listeningPort);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApiJsonSerializerContext.Default);
});

builder.AddZwooLogging(false);
var conf = builder.AddZwooConfiguration(args, new ZwooAppConfiguration()
{
    AppVersion = Zwoo.Backend.LocalServer.VersionProvider.VERSION,
    AppVersionHash = Zwoo.Backend.LocalServer.VersionProvider.HASH,
    ZRPVersion = ZRPVersion.CURRENT,
    ServerMode = "local"
});

builder.Services.AddLocalAuthentication(config.ServerId);

builder.Services.AddSingleton<IGameDatabaseAdapter, Mock>();
builder.Services.AddSingleton<IExternalGameProfileProvider, EmptyGameProfileProvider>();
builder.Services.AddSingleton<ILocalUserManager, LocalUserManager>();


// backend services
builder.Services.AddZwooServices();
builder.Services.AddGameServices();

var app = builder.Build();


var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

if (config.UseStrictOrigins)
{
    Console.WriteLine($"[Config] Restricting websockets to: http://{listeningIp}:{listeningPort}");
    webSocketOptions.AllowedOrigins.Add($"http://{listeningIp}:{listeningPort}");
}
else if (config.AllowedOrigins != string.Empty)
{
    Console.WriteLine($"[Config] Allowing websockets from: {config.AllowedOrigins}");
    foreach (var origin in config.AllowedOrigins.Split(','))
    {
        webSocketOptions.AllowedOrigins.Add(origin);
    }
}
else
{
    Console.WriteLine($"[Config] Allowing websockets from: *everywhere*");
}
app.UseWebSockets(webSocketOptions);

app.UseZwooHttpLogging("/api");

if (!config.UseStrictOrigins)
{
    var allowedOrigins = config.AllowedOrigins == string.Empty ? null : config.AllowedOrigins;
    Console.WriteLine($"[Config] Allowing origins: {allowedOrigins ?? "*all*"}");
    app.Use((context, next) =>
    {
        if (context.WebSockets.IsWebSocketRequest) return next(context);

        context.Response.Headers.Append("Access-Control-Allow-Origin", allowedOrigins ?? context.Request.Headers["Origin"]);
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
        context.Response.Headers.Append("Access-Control-Max-Age", "86400");

        // check if preflight request
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return Task.CompletedTask;
        }
        return next(context);
    });
}


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
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = provider,
    DefaultFileNames = ["index.html"]
});
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = provider,
    ServeUnknownFileTypes = true,
});


api.UseDiscover();
api.UseGame();
api.MapGet("/stats", (HttpContext context) =>
{
    if (context.Request.Headers["X-Api-Secret"] != config.SecretKey)
    {
        return Results.Unauthorized();
    }
    return Results.Ok("##server stats");
}).AllowAnonymous();

app.MapFallbackToFile("index.html", new StaticFileOptions
{
    FileProvider = provider,
    ServeUnknownFileTypes = true,
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
