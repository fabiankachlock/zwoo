namespace ZwooGameLogic.ZRP;


public interface IUserEventEmitter
{
    public delegate void EventHandler(IIncomingZRPMessage message);

    public event EventHandler OnEvent;
}