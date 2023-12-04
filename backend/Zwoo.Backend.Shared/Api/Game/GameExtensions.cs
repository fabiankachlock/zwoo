using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zwoo.Backend.Games;
using Zwoo.GameEngine;
using Zwoo.GameEngine.Lobby.Features;

namespace Zwoo.Backend.Shared.Api.Game;

public static partial class AppExtensions
{
    public static void AddGameServices(this IServiceCollection services)
    {
        services.AddSingleton<IGameConnectionsService, GameConnectionsService>();
        services.AddSingleton<IGameConnectionHandlerService, GameConnectionHandlerService>();
        services.AddSingleton<IGameEngineService, GameEngineService>();
        services.AddSingleton<GameEngineService>();
        services.AddSingleton<IGameProviderService, GameProviderService>();
        services.AddSingleton<IExternalGameProfileProvider, BackendGameProfileProvider>();
        services.AddSingleton(sp =>
        {
            var connections = sp.GetRequiredService<IGameConnectionsService>();
            var gameProfileProvider = sp.GetRequiredService<IExternalGameProfileProvider>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new GameManager(connections, gameProfileProvider, new BackendLoggerFactory(loggerFactory));
        });
    }

    public static void UseGame(this WebApplication app)
    {
        GameEndpoints.Map(app);
    }
}