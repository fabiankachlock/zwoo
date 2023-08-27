using ZwooGameLogic.Events.Handler;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Events;

public class UserEventDistributer : IUserEventReceiver
{
    private readonly ZwooRoom _room;
    private static Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> _handles
    {
        get
        {
            Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> allHandles = new();

            var handlers = new IUserEventHandler[] {
                new ChatHandler(),
                new LobbyHandler(),
                new SettingsHandler(),
                new GameHandler(),
                new BotsHandler(),
            };

            foreach (var handler in handlers)
            {
                foreach (var handle in handler.GetHandles())
                {
                    allHandles[handle.Key] = handle.Value;
                }
            }
            return allHandles;
        }
    }


    public UserEventDistributer(ZwooRoom room)
    {
        _room = room;
    }

    public void DistributeEvent(IIncomingZRPMessage message)
    {
        IPlayer? player = _room.GetPlayerByRealId(message.UserId);
        if (player == null) return;

        var context = new UserContext(player.RealId, player.LobbyId, player.Username, player.Role, _room.Id, _room);
        if (!_handles.ContainsKey(message.Code)) return;

        _handles[message.Code](context, message, _room.NotificationDistributer);
    }

    public void DistributeEvent(ILocalZRPMessage message)
    {
        IPlayer? player = _room.GetPlayer(message.LobbyId);
        if (player == null) return;

        var context = new UserContext(player.RealId, player.LobbyId, player.Username, player.Role, _room.Id, _room);
        if (!_handles.ContainsKey(message.Code)) return;

        _handles[message.Code](context, message, _room.NotificationDistributer);
    }
}
