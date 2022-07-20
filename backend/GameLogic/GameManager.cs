using log4net;
using ZwooGameLogic.Game.Events;

namespace ZwooGameLogic;

public sealed class GameManager
{
    private readonly ILog _logger;

    private long _gameId;
    private Dictionary<long, Game.Game> _activeGames;
    private Func<long, NotificationManager> _notificationManagerFactory;


    public GameManager(Func<long, NotificationManager> notificationManagerFactory)
    {
        _gameId = 0;
        _activeGames = new Dictionary<long, Game.Game>();
        _notificationManagerFactory = notificationManagerFactory;
        _logger = LogManager.GetLogger("GameManager");
    }

    public Game.Game CreateGame(string name, bool isPublic)
    {
        long id = nextGameId();
        Game.Game newGame = new Game.Game(id, name, isPublic, _notificationManagerFactory(id));
        _activeGames.Add(newGame.Id, newGame);
        _logger.Info($"created game {newGame.Id}");
        return newGame;
    }

    public Game.Game? GetGame(long id)
    {
        if (_activeGames.ContainsKey(id))
        {
            return _activeGames[id];
        }
        _logger.Debug($"game find game {id}");
        return null;
    }

    public bool RemoveGame(long id)
    {
        _logger.Debug($"removing game {id}");
        return _activeGames.Remove(id);
    }

    public List<Game.Game> FindGames(string search)
    {
        return _activeGames
            .Where(pair => pair.Key.ToString().Contains(search) || pair.Value.Name.Contains(search))
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<Game.Game> GetAllGames()
    {
        return _activeGames
            .Select(pair => pair.Value)
            .ToList();
    }

    public void SendEvent(long gameId, ClientEvent clientEvent)
    {
        Game.Game? game = GetGame(gameId);
        if (game != null)
        {
            game.HandleEvent(clientEvent);
        }
    }

    private long nextGameId()
    {
        return ++_gameId;
    }
}