using ZwooGameLogic.Events;

namespace ZwooGameLogic.ZRP;

public interface IIncomingZRPMessage : IIncomingEvent
{
    /// <summary>
    /// the user id of the player who sent this message
    /// </summary>
    public long UserId { get; }
}

