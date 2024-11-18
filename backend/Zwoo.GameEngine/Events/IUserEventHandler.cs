using Zwoo.GameEngine.ZRP;
using Zwoo.Api.ZRP;
using Zwoo.GameEngine.Notifications;

namespace Zwoo.GameEngine.Events;

public interface IUserEventHandler
{
    Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> GetHandles();
}
