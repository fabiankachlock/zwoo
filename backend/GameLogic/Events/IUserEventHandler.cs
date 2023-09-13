using ZwooGameLogic.ZRP;
using ZwooGameLogic.Notifications;

namespace ZwooGameLogic.Events;

public interface IUserEventHandler
{
    Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> GetHandles();
}
