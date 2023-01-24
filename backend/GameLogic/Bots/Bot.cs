using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Bots.Decisions;
using ZwooGameLogic.Logging;
using ZwooGameLogic.Lobby;

namespace ZwooGameLogic.Bots;


public class Bot : INotificationTarget
{

    public long GameId { get; private set; }

    public long PlayerId { get; private set; }

    public string Username { get; private set; }

    public BotConfig Config { get; private set; }

    private IBotDecisionHandler _handler;

    private Action<BotZRPEvent> _sendMessage;

    private ILogger _logger;

    public Bot(long gameId, string username, BotConfig config, ILogger logger, Action<BotZRPEvent> sendMessage)
    {
        GameId = gameId;
        Username = username;
        Config = config;
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

    public LobbyManager.PlayerEntry AsPlayer()
    {
        return new LobbyManager.PlayerEntry(PlayerId, Username, ZRPRole.Bot, ZRPPlayerState.Connected);
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
        _sendMessage(new BotZRPEvent(PlayerId, code, payload));
    }
}