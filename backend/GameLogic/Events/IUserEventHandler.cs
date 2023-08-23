using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Events;

public interface IUserEventHandler
{
    bool HandleMessage(UserContext context, IIncomingEvent message);
}
