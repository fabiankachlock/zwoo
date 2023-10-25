using ZwooGameLogic.ZRP;
using ZwooGameLogic.Logging;
using ZwooGameLogic.Lobby;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.Lobby.Features;

namespace ZwooGameLogic;

public sealed class GameManager
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    private long _gameId;
    private Dictionary<long, ZwooRoom> _activeGames;
    private INotificationAdapter _notificationAdapter;
    private IExternalGameProfileProvider _gameProfileProvider;


    public GameManager(INotificationAdapter notificationAdapter, IExternalGameProfileProvider gameProfileProvider, ILoggerFactory loggerFactory)
    {
        _gameId = 0;
        _activeGames = new Dictionary<long, ZwooRoom>();
        _notificationAdapter = notificationAdapter;
        _gameProfileProvider = gameProfileProvider;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger("GameManager");
    }

    public ZwooRoom CreateGame(string name, bool isPublic)
    {
        long id = nextGameId();
        var room = new ZwooRoom(id, name, isPublic, _notificationAdapter, _gameProfileProvider, _loggerFactory);
        room.OnClosed += () =>
        {
            RemoveGame(room.Id);
            _notificationAdapter.DisconnectGame(room.Id);
        };

        _activeGames.Add(room.Game.Id, room);
        _logger.Info($"created game {room.Game.Id}");
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
        room.EventDistributer.DistributeEvent(msg);
    }

    private long nextGameId()
    {
        return ++_gameId;
    }
}