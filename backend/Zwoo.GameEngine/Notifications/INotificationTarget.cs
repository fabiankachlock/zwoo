using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Notifications;

/// <summary>
/// an individual object capable of receiving notifications
/// </summary>
public interface INotificationTarget
{
    /// <summary>
    /// the target player id
    /// </summary>
    public long PlayerId { get; }

    /// <summary>
    /// the targets current game
    /// </summary>
    public long GameId { get; }

    /// <summary>
    /// send a notification to the target
    /// </summary>
    /// <param name="code">zrp code of the notification</param>
    /// <param name="payload">zrp payload of the notification</param>
    /// <typeparam name="T">type of the zrp payload</typeparam>
    public void ReceiveMessage<T>(ZRPCode code, T payload);

    /// <summary>
    /// disconnect the target from a game
    /// </summary>
    public void ReceiveDisconnect();
}