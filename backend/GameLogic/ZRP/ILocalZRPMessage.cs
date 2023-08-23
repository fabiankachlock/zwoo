using ZwooGameLogic.Events;

namespace ZwooGameLogic.ZRP;

public interface ILocalZRPMessage : IIncomingEvent
{
    /// <summary>
    /// the lobby id of the player who sent this message
    /// </summary>
    public long LobbyId { get; }
}

