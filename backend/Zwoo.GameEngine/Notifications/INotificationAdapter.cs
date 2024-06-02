using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Notifications;

/// <summary>
/// an object capable of receiving & distributing game notifications
/// </summary>
public interface INotificationAdapter
{
    /// <summary>
    /// send a notification targeted to a player
    /// </summary>
    /// <param name="playerId">the target players id</param>
    /// <param name="code">zrp code of the notification</param>
    /// <param name="payload">zrp payload of the notification</param>
    /// <typeparam name="T">type of the payload</typeparam>
    public Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload);

    /// <summary>
    /// send a notification targeted to a game
    /// </summary>
    /// <param name="gameId">the target games id</param>
    /// <param name="code">zrp code of the notification</param>
    /// <param name="payload">zrp payload of the notification</param>
    /// <typeparam name="T">type of the payload</typeparam>
    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload);

    /// <summary>
    /// send a request to disconnect a player
    /// </summary>
    /// <param name="playerId">the players id to disconnect</param>
    public Task<bool> DisconnectPlayer(long playerId);

    /// <summary>
    /// send a request to disconnect a game
    /// </summary>
    /// <param name="gameId">the games id to disconnect</param>
    public Task<bool> DisconnectGame(long gameId);
}