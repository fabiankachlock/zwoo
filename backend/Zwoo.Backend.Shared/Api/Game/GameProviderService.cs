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