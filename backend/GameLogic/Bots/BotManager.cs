using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Bots.Decisions;

namespace ZwooGameLogic.Bots;


/// <summary>
/// an object managing multiple bot instances within a game
/// </summary>
public class BotManager : INotificationAdapter, IUserEventEmitter
{

    private readonly Game.Game _game;

    private readonly INotificationAdapter _notificationManager;

    private List<Bot> _bots;


    public event IUserEventEmitter.EventHandler OnEvent = delegate { };

    public BotManager(Game.Game game)
    {
        _game = game;
        _notificationManager = new NotificationManager();
        _bots = new List<Bot>();
    }

    public List<Bot> ListBots()
    {
        return _bots;
    }

    public Bot CreateBot(string username, BotConfig config)
    {
        Bot bot = new Bot(_game.Id, username, config, (long botId, BotZRPNotification<object> msg) =>
        {
            OnEvent.Invoke(new BotZRPEvent(botId, msg.Code, msg.Payload));
        });
        _bots.Add(bot);

        return bot;
    }

    public void RemoveBot(string username)
    {
        Bot? botToRemove = _bots.Find(bot => bot.Username == username);
        if (botToRemove != null)
        {
            _bots.Remove(botToRemove);
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