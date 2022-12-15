using log4net;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Lobby;

namespace ZwooGameLogic;

public sealed class GameManager
{
    private readonly ILog _logger;

    private long _gameId;
    private Dictionary<long, ZwooRoom> _activeGames;
    private Func<long, NotificationManager> _notificationManagerFactory;


    public GameManager(Func<long, NotificationManager> notificationManagerFactory)
    {
        _gameId = 0;
        _activeGames = new Dictionary<long, ZwooRoom>();
        _notificationManagerFactory = notificationManagerFactory;
        _logger = LogManager.GetLogger("GameManager");
    }

    public ZwooRoom CreateGame(string name, bool isPublic)
    {
        long id = nextGameId();
        Game.Game newGame = new Game.Game(id, name, isPublic, _notificationManagerFactory(id));
        LobbyManager lobby = new LobbyManager(newGame.Id, newGame.Settings);
        ZwooRoom room = new ZwooRoom(newGame, lobby);
        _activeGames.Add(newGame.Id, room);
        _logger.Info($"created game {newGame.Id}");
        return room;
    }

    public ZwooRoom? GetGame(long id)
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

    public List<ZwooRoom> FindGames(string search)
    {
        return _activeGames
            .Where(pair => pair.Key.ToString().Contains(search) || pair.Value.Name.Contains(search))
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<ZwooRoom> GetAllGames()
    {
        return _activeGames
            .Select(pair => pair.Value)
            .ToList();
    }

    private long nextGameId()
    {
        return ++_gameId;
    }
}