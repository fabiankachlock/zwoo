using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Events;

public interface IUserEventReceiver
{
    Task DistributeEvent(IIncomingZRPMessage message);
    Task DistributeEvent(ILocalZRPMessage message);
}
