using Zwoo.GameEngine.Notifications;
using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Events;
using Zwoo.GameEngine.Logging;

namespace Zwoo.GameEngine.Bots;


/// <summary>
/// an object managing multiple bot instances within a game
/// </summary>
public class BotManager : INotificationAdapter, IRoutableNotificationAdapter, IUserEventEmitter
{

    private readonly Game.Game _game;

    private readonly NotificationManager _notificationManager;

    private readonly ILoggerFactory _loggerFactory;

    private readonly ILogger _logger;

    private List<Bot> _bots;


    public event IUserEventEmitter.EventHandler OnEvent = delegate { };

    public long TargetId
    {
        get => Bot.MOCK_REAL_ID;
    }

    public BotManager(Game.Game game, ILoggerFactory loggerFactory)
    {
        _game = game;
        _notificationManager = new NotificationManager(loggerFactory.CreateLogger("NotificationManager"));
        _bots = new List<Bot>();
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger("BotManager");
    }

    public List<Bot> ListBots()
    {
        return _bots;
    }

    public Bot? GetBot(long id)
    {
        return _bots.Where(bot => bot.LobbyId == id).FirstOrDefault();
    }

    public bool HasBotWithName(string name)
    {
        return _bots.Where(bot => bot.Username == name).Any();
    }

    public Bot CreateBot(long lobbyId, string username, BotConfig config)
    {
        _logger.Info($"creating bot {username}");
        var botLogger = _loggerFactory.CreateLogger($"Bot-{username}");
        Bot bot = new Bot(_game.Id, lobbyId, username, config, botLogger, (BotZRPEvent evt) =>
        {
            botLogger.Debug($"sending event {evt.Code} {evt.Payload}");
            OnEvent.Invoke(evt);
        });
        _bots.Add(bot);
        _notificationManager.AddTarget(bot);
        return bot;
    }

    public void RemoveBot(long lobbyId)
    {
        _logger.Info($"removing bot {lobbyId}");
        Bot? botToRemove = _bots.Find(bot => bot.LobbyId == lobbyId);
        if (botToRemove != null)
        {
            _bots.Remove(botToRemove);
            _notificationManager.RemoveTarget(botToRemove);
        }
    }

    #region Interface Implementation

    public Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload)
    {
        return _notificationManager.SendPlayer(playerId, code, payload);
    }

    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        return _notificationManager.BroadcastGame(gameId, code, payload);
    }

    public Task<bool> DisconnectPlayer(long playerId)
    {
        return _notificationManager.DisconnectPlayer(playerId);
    }

    public Task<bool> DisconnectGame(long gameId)
    {
        return _notificationManager.DisconnectGame(gameId);
    }

    #endregion Interface Implementation

}