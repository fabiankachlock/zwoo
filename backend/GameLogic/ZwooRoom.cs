using ZwooGameLogic.Lobby;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.Bots;

namespace ZwooGameLogic;

public class ZwooRoom
{
    public readonly Game.Game Game;
    public readonly LobbyManager Lobby;
    public readonly UserEventDistributer EventDistributer;
    public readonly ZRPPlayerManager PlayerManager;
    public readonly BotManager BotManager;

    private readonly INotificationAdapter _notificationDistributer;

    public delegate void ClosedHandler();
    public event ClosedHandler OnClosed = delegate { };

    public long Id
    {
        get => Game.Id;
    }

    public ZwooRoom(Game.Game game, LobbyManager lobby, INotificationAdapter notificationAdapter)
    {
        Game = game;
        Lobby = lobby;
        BotManager = new BotManager(Game);

        _notificationDistributer = new NotificationDistributer(notificationAdapter, BotManager);
        EventDistributer = new UserEventDistributer(_notificationDistributer);
        PlayerManager = new ZRPPlayerManager(_notificationDistributer, this);

        BotManager.OnEvent += DistributeEvent;
    }

    public void Close()
    {
        OnClosed.Invoke();
    }

    public void DistributeEvent(IIncomingZRPMessage msg)
    {
        EventDistributer.Distribute(this, msg);
    }
}

