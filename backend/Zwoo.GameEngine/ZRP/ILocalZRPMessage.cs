using Zwoo.GameEngine.Events;

namespace Zwoo.GameEngine.ZRP;

public interface ILocalZRPMessage : IIncomingEvent
{
    /// <summary>
    /// the lobby id of the player who sent this message
    /// </summary>
    public long LobbyId { get; }
}

