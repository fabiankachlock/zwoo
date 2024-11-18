using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zwoo.Backend.Games;
using Zwoo.GameEngine;

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
        services.AddSingleton(sp =>
        {
            var connections = sp.GetRequiredService<IGameConnectionsService>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new GameManager(connections, new BackendLoggerFactory(loggerFactory));
        });
    }

    public static void AddGameDatabaseAdapter(this IServiceCollection services)
    {
        services.AddSingleton<IGameDatabaseAdapter, GameDatabaseAdapter>();
    }

    public static void UseGame(this IEndpointRouteBuilder app)
    {
        GameEndpoints.Map(app);
    }
}