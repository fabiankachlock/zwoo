using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Bots.Decisions;
using ZwooGameLogic.Logging;
using ZwooGameLogic.Lobby;

namespace ZwooGameLogic.Bots;


public class Bot : INotificationTarget
{

    public long GameId { get; private set; }

    /// <summary>
    /// the lobby id is an lobby internal unique identifier
    /// its scoped to the current lobby and used to identify the bot in zrp notifications
    /// </summary>
    /// <value></value>
    public int LobbyId { get; private set; }

    /// <summary>
    /// the player id is a temporary in scope of the lobby unique identifiers used for notification targeting
    /// its equivalent is the users actual id
    /// its scoped to a single game and may change based on the actual players currently in the game
    /// </summary>
    /// <value></value>
    public long PlayerId { get; private set; }

    public string Username { get; private set; }

    public BotConfig Config { get; private set; }

    private IBotDecisionHandler _handler;

    private Action<BotZRPEvent> _sendMessage;

    private ILogger _logger;

    public Bot(long gameId, int lobbyId, string username, BotConfig config, ILogger logger, Action<BotZRPEvent> sendMessage)
    {
        GameId = gameId;
        Username = username;
        Config = config;
        LobbyId = lobbyId;
        _sendMessage = sendMessage;
        _logger = logger;
        _handler = BotBrainFactory.CreateDecisionHandler(config, _logger);
        _handler.OnEvent += forwardMessage;
    }

    public void PrepareForGame(long assignedId)
    {
        _logger.Debug($"received id {assignedId}");
        PlayerId = assignedId;
        _handler.Reset();
    }

    public void SetConfig(BotConfig config)
    {
        Config = config;
        _handler.OnEvent -= forwardMessage;
        _handler = BotBrainFactory.CreateDecisionHandler(config, _logger);
        _handler.OnEvent += forwardMessage;
    }

    public IPlayer AsPlayer()
    {
        return new LobbyEntry(0, LobbyId, Username, ZRPRole.Bot, ZRPPlayerState.Connected);
    }

    public void ReceiveMessage<T>(ZRPCode code, T payload)
    {
        _logger.Debug($"received message {code} {payload}");
        BotZRPNotification<object> msg = new BotZRPNotification<object>(code, payload != null ? (object)payload : new object());
        _handler.AggregateNotification(msg);
    }

    public void ReceiveDisconnect()
    {
        _handler.Reset();
    }

    private void forwardMessage(ZRPCode code, object payload)
    {
        _sendMessage(new BotZRPEvent(LobbyId, code, payload));
    }
}