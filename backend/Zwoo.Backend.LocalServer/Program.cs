using System.Text.Json.Serialization;
using Zwoo.Backend.Shared.Api;
using Zwoo.Backend.Shared.Api.Contact;
using Zwoo.Backend.Shared.Api.Discover;
using Zwoo.Backend.Shared.Api.Game;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Backend.Shared.Services;
using Zwoo.Database;
using Zwoo.Database.Dao;

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
builder.Services.AddZwooCors(conf);

builder.Services.AddSingleton<IGameDatabaseAdapter, Mock>();

// backend services
builder.Services.AddZwooServices();
builder.Services.AddGameServices();

var app = builder.Build();

app.UseZwooHttpLogging();
app.UseZwooCors();

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

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
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
