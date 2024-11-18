using Zwoo.GameEngine.Lobby;
using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Notifications;
using Zwoo.GameEngine.Bots;
using Zwoo.GameEngine.Events;
using Zwoo.GameEngine.Logging;

namespace Zwoo.GameEngine;

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

    // id 1 is reserved for the host that creates the game
    private long _runningId = 1;

    public long Id
    {
        get => Game.Id;
    }

    public ZwooRoom(long id, string name, bool isPublic, INotificationAdapter notificationAdapter, ILoggerFactory loggerFactory)
    {
        _notificationDistributer = new(this, notificationAdapter);
        _eventDistributer = new UserEventDistributer(this);

        GameEventTranslator notificationTranslator = new(this, _notificationDistributer);
        Game = new(id, name, isPublic, notificationTranslator, loggerFactory);
        Lobby = new(Game.Id, Game.Settings);
        PlayerManager = new ZRPPlayerManager(_notificationDistributer, this, loggerFactory.CreateLogger("PlayerManager"));

        BotManager = new BotManager(Game, loggerFactory);
        BotManager.OnEvent += _eventDistributer.DistributeEvent;
        _notificationDistributer.RegisterTarget(BotManager);
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

    public bool ShouldClose()
    {
        if (Game.IsRunning)
        {
            return
                Lobby.ActivePlayerCount() == 0
                || Lobby.PlayerCount() == 0
                || (Lobby.PlayerCount() == 1 && BotManager.ListBots().Count == 0);
        }
        else
        {
            return Lobby.PlayerCount() == 0;
        }
    }

    public void Close()
    {
        Game.Stop();
        OnClosed.Invoke();
    }
}

