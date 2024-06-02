using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Events;

public interface IUserEventReceiver
{
    void DistributeEvent(IIncomingZRPMessage message);
    void DistributeEvent(ILocalZRPMessage message);
}
