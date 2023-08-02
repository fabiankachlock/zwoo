using ZwooGameLogic.Lobby;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.Bots;
using ZwooGameLogic.Events;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic;

public class ZwooRoom
{
    public readonly Game.Game Game;
    public readonly LobbyManager Lobby;
    public readonly ZRPPlayerManager PlayerManager;
    public readonly BotManager BotManager;

    public delegate void ClosedHandler();
    public event ClosedHandler OnClosed = delegate { };

    private readonly UserEventDistributer _eventDistributer;
    public IUserEventReceiver EventDistributer
    {
        get => _eventDistributer;
    }

    private readonly IdBasedNotificationRouter _notificationDistributer;
    public INotificationAdapter NotificationDistributer
    {
        get => _notificationDistributer;
    }

    private long _runningId = 0;

    public long Id
    {
        get => Game.Id;
    }

    public ZwooRoom(Game.Game game, LobbyManager lobby, IdBasedNotificationRouter notificationrouter, ILoggerFactory loggerFactory)
    {
        Game = game;
        Lobby = lobby;
        BotManager = new BotManager(Game, loggerFactory);

        _eventDistributer = new UserEventDistributer(this);
        BotManager.OnEvent += _eventDistributer.DistributeEvent;

        _notificationDistributer = new IdBasedNotificationRouter(this, notificationrouter);
        _notificationDistributer.RegisterTarget(BotManager);
        PlayerManager = new ZRPPlayerManager(_notificationDistributer, this, loggerFactory.CreateLogger("PlayerManager"));
    }

    public long NextId() => ++_runningId;

    public IPlayer? GetPlayer(long lobbyId)
    {
        return Lobby.HasLobbyId(lobbyId) ? Lobby.GetPlayer(lobbyId) : BotManager.GetBot(lobbyId)?.AsPlayer();
    }

    public IPlayer? GetPlayerByRealId(long lobbyId)
    {
        return Lobby.GetPlayerByUserId(lobbyId);
    }

    public void Close()
    {
        Game.Stop();
        OnClosed.Invoke();
    }
}

