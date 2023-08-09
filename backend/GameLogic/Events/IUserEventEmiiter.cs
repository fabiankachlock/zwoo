using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Events;

public interface IUserEventEmitter
{
    public delegate void EventHandler(ILocalZRPMessage message);

    public event EventHandler OnEvent;
}