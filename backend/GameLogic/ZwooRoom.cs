using ZwooGameLogic.Lobby;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.Bots;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic;

public class ZwooRoom
{
    public readonly Game.Game Game;
    public readonly LobbyManager Lobby;
    public readonly UserEventDistributer EventDistributer;
    public readonly ZRPPlayerManager PlayerManager;
    public readonly BotManager BotManager;

    private readonly NotificationDistributer _notificationDistributer;

    public delegate void ClosedHandler();
    public event ClosedHandler OnClosed = delegate { };

    public long Id
    {
        get => Game.Id;
    }

    public ZwooRoom(Game.Game game, LobbyManager lobby, NotificationDistributer notificationDistributer, ILoggerFactory loggerFactory)
    {
        Game = game;
        Lobby = lobby;
        BotManager = new BotManager(Game, loggerFactory);
        _notificationDistributer = notificationDistributer;
        _notificationDistributer.AddTarget(BotManager);
        PlayerManager = new ZRPPlayerManager(notificationDistributer, this, loggerFactory.CreateLogger("PlayerManager"));
        EventDistributer = new UserEventDistributer(_notificationDistributer);
        BotManager.OnEvent += DistributeEvent;
    }

    public string ResolvePlayerName(long id)
    {
        return (Lobby.HasPlayerId(id) ? Lobby.GetPlayer(id)?.Username : BotManager.GetBot(id)?.Username) ?? "unknown player";
    }

    public IPlayer? GetPlayer(long id)
    {
        return Lobby.HasPlayerId(id) ? Lobby.GetPlayer(id) : BotManager.GetBot(id)?.AsPlayer();
    }

    public void Close()
    {
        Game.Stop();
        OnClosed.Invoke();
    }

    public void DistributeEvent(IIncomingZRPMessage msg)
    {
        Console.WriteLine("### distributing");
        EventDistributer.Distribute(this, msg);
    }
}

