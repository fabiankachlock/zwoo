using Zwoo.Backend.Shared.Api.Model;

namespace Zwoo.Backend.Shared.Api.Game;

public class GameQueryOptions
{
    public string Filter { get; set; } = string.Empty;
    public bool PublicOnly { get; set; }
    public bool Recommended { get; set; }
}

public interface IGameProviderService
{
    public List<GameMeta> QueryGames(int offset, int limit, GameQueryOptions options);

    public GameMeta? GetGame(long id);
}

// TODO: the game engine game manager should be deprecated
public class GameProviderService : IGameProviderService
{
    private readonly GameEngineService _gameEngine;

    public GameProviderService(GameEngineService gameEngine)
    {
        _gameEngine = gameEngine;
    }

    public List<GameMeta> QueryGames(int offset, int limit, GameQueryOptions options)
    {
        return _gameEngine.ListAll().Select(g => new GameMeta()
        {
            Id = g.Id,
            Name = g.Game.Name,
            IsPublic = g.Game.IsPublic,
            PlayerCount = g.Lobby.PlayerCount()
        }).ToList();
    }

    public GameMeta? GetGame(long id)
    {
        var game = _gameEngine.GetGame(id);
        return game == null ? null : new GameMeta()
        {
            Id = game.Id,
            Name = game.Game.Name,
            IsPublic = game.Game.IsPublic,
            PlayerCount = game.Lobby.PlayerCount()
        };
    }
}