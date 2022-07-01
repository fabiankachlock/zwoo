using log4net;

namespace ZwooGameLogic;

public sealed class GameManager
{
    private readonly ILog _logger;

    private Dictionary<long, Game.Game> ActiveGames;
    private long GameId;

    public GameManager()
    {
        ActiveGames = new Dictionary<long, Game.Game>();
        GameId = 0;
        _logger = LogManager.GetLogger("GameManager");
    }

    public Game.Game CreateGame(
        string name,
        bool isPublic
    )
    {
        Game.Game newGame = new Game.Game(nextGameId(), name, isPublic);
        ActiveGames.Add(newGame.Id, newGame);
        _logger.Info($"Created Game ${newGame.Id}");
        return newGame;
    }

    public Game.Game? GetGame(long id)
    {
        if (ActiveGames.ContainsKey(id))
        {
            return ActiveGames[id];
        }
        _logger.Debug($"Game find game ${id}");
        return null;
    }

    public bool RemoveGame(long id)
    {
        _logger.Debug($"Removing game {id}");
        return ActiveGames.Remove(id);
    }

    public List<Game.Game> FindGames(string search)
    {
        return ActiveGames
            .Where(pair => pair.Key.ToString().Contains(search) || pair.Value.Name.Contains(search))
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<Game.Game> GetAllGames()
    {
        return ActiveGames
            .Select(pair => pair.Value)
            .ToList();
    }

    private long nextGameId()
    {
        return ++GameId;
    }
}