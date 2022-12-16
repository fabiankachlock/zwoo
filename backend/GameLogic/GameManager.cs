using log4net;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Lobby;

namespace ZwooGameLogic;

public sealed class GameManager
{
    private readonly ILog _logger;
    private long _gameId;
    private Dictionary<long, ZwooRoom> _activeGames;
    private INotificationAdapter _notificationAdapter;
    private GameMessageDistributer _messageDistributer;


    public GameManager(INotificationAdapter notificationAdapter)
    {
        _gameId = 0;
        _activeGames = new Dictionary<long, ZwooRoom>();
        _notificationAdapter = notificationAdapter;
        _logger = LogManager.GetLogger("GameManager");
        _messageDistributer = new GameMessageDistributer(_notificationAdapter, this);
    }

    public ZwooRoom CreateGame(string name, bool isPublic)
    {
        long id = nextGameId();
        GameEventTranslator notificationManager = new GameEventTranslator(this._notificationAdapter);

        Game.Game newGame = new Game.Game(id, name, isPublic, notificationManager);
        LobbyManager lobby = new LobbyManager(newGame.Id, newGame.Settings);
        ZwooRoom room = new ZwooRoom(newGame, lobby);
        notificationManager.SetGame(room);

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
            .Where(pair => pair.Key.ToString().Contains(search) || pair.Value.Game.Name.Contains(search))
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<ZwooRoom> GetAllGames()
    {
        return _activeGames
            .Select(pair => pair.Value)
            .ToList();
    }

    public void SendEvent(long gameId, IIncomingZRPMessage msg)
    {
        ZwooRoom? room = GetGame(gameId);
        if (room == null) return;
        _messageDistributer.Distribute(room, msg);
    }

    private long nextGameId()
    {
        return ++_gameId;
    }
}