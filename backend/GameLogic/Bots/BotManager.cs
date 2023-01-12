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

    public event IUserEventEmitter.EventHandler OnEvent = delegate { };

    public BotManager(Game.Game game)
    {
        _game = game;
        _notificationManager = new NotificationManager();
    }

    public Bot CreateBot()
    {
        Bot bot = new Bot(_game.Id, BotBrainFactory.CreateDecisionHandler(), (long botId, BotZRPNotification<object> msg) =>
        {
            OnEvent.Invoke(new BotZRPEvent(botId, msg.Code, msg.Payload));
        });
        return bot;
    }

    public void RemoveBot()
    {

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