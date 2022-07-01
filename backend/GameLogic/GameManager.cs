using log4net;

namespace ZwooGameLogic;

public sealed class GameManager
{
    private readonly ILog _logger;

    private Dictionary<long, Game> ActiveGames;
    private long GameId;

    public GameManager(ILog logger)
    {
        ActiveGames = new Dictionary<long, Game>();
        GameId = 0;
        _logger = logger;
    }

    public Game CreateGame(
        string name,
        bool isPublic
    )
    {
        Game newGame = new Game(nextGameId(), name, isPublic);
        ActiveGames.Add(newGame.Id, newGame);
        _logger.Info($"Created Game ${newGame.Id}");
        return newGame;
    }

    public Game? GetGame(long id)
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

    public List<Game> FindGames(string search)
    {
        return ActiveGames
            .Where(pair => pair.Key.ToString().Contains(search) || pair.Value.Name.Contains(search))
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<Game> GetAllGames()
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