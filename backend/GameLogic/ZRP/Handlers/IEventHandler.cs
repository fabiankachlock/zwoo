namespace ZwooGameLogic.ZRP.Handlers;

public interface IEventHandler
{
    bool HandleMessage(UserContext context, IIncomingZRPMessage message);
}
