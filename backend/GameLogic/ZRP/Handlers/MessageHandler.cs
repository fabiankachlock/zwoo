using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.ZRP.Handlers;

public interface IMessageHandler
{
    bool HandleMessage(UserContext context, IIncomingZRPMessage message);
}
