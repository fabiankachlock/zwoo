using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Events;

public interface IUserEventReceiver
{
    void DistributeEvent(IIncomingZRPMessage message);
    void DistributeEvent(ILocalZRPMessage message);
}
