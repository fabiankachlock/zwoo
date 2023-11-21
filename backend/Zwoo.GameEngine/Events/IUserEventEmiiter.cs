using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Events;

public interface IUserEventEmitter
{
    public delegate void EventHandler(ILocalZRPMessage message);

    public event EventHandler OnEvent;
}