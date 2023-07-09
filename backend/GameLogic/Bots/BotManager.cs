using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Bots;


/// <summary>
/// an object managing multiple bot instances within a game
/// </summary>
public class BotManager : INotificationAdapter, IUserEventEmitter
{

    private readonly Game.Game _game;

    private readonly NotificationManager _notificationManager;

    private readonly ILoggerFactory _loggerFactory;

    private readonly ILogger _logger;

    private List<Bot> _bots;


    public event IUserEventEmitter.EventHandler OnEvent = delegate { };

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
        return _bots.Where(bot => bot.PlayerId == id).FirstOrDefault();
    }

    public bool HasBotWithName(string name)
    {
        return _bots.Where(bot => bot.Username == name).Count() > 0;
    }

    public Bot CreateBot(string username, BotConfig config)
    {
        _logger.Info($"creating bot {username}");
        var botLogger = _loggerFactory.CreateLogger($"Bot-{username}");
        Bot bot = new Bot(_game.Id, username, config, botLogger, (BotZRPEvent evt) =>
        {
            botLogger.Debug($"sending event {evt.Code} {evt.Payload}");
            OnEvent.Invoke(evt);
        });
        _bots.Add(bot);
        _notificationManager.AddTarget(bot);
        return bot;
    }

    public void RemoveBot(string publicId)
    {
        _logger.Info($"removing bot {publicId}");
        Bot? botToRemove = _bots.Find(bot => bot.AsPlayer().PublicId == publicId);
        if (botToRemove != null)
        {
            _bots.Remove(botToRemove);
            _notificationManager.RemoveTarget(botToRemove);
        }
    }

    /// <summary>
    /// since there is no reserved space for bot ids and bot ids should not collide with player ids
    /// bots get a dynamic id assigned before each game starts
    ///  
    /// these dynamic ids get selected based on the current players of the game
    /// </summary>
    public void PrepareBotsForGame()
    {
        _logger.Info($"preparing bots for game");
        long i = 1000; // FIXME: ZWOO-326 player cant receive bot notification if they happen to have the same id and are currently playing another game
        foreach (Bot bot in _bots)
        {
            while (_game.HasPlayer(i))
            {
                i++;
            }
            _logger.Info($"assigning id {i} to bot {bot.Username}");
            bot.PrepareForGame(i);
            i++;
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