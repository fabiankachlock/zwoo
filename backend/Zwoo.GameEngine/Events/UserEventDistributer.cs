using Zwoo.GameEngine.Events.Handler;
using Zwoo.GameEngine.Notifications;
using Zwoo.GameEngine.ZRP;
using Zwoo.Api.ZRP;
using Zwoo.GameEngine.Game.Cards;

namespace Zwoo.GameEngine.Events;

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
        distribute(context, message);
    }

    public void DistributeEvent(ILocalZRPMessage message)
    {
        IPlayer? player = _room.GetPlayer(message.LobbyId);
        if (player == null) return;

        var context = new UserContext(player.RealId, player.LobbyId, player.Username, player.Role, _room.Id, _room);
        distribute(context, message);
    }

    private void distribute(UserContext context, IIncomingEvent message)
    {
        if (!_handles.ContainsKey(message.Code)) return;

        try
        {
            _handles[message.Code](context, message, _room.NotificationDistributer);
        }
        catch (EmptyPileException)
        {
            _room.NotificationDistributer.SendPlayer(context.LobbyId, ZRPCode.EmptyPileError, new EmptyPileError((int)ZRPCode.EmptyPileError, "the pile is empty"));
        }
        catch (Exception e)
        {
            _room.NotificationDistributer.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
