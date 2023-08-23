using ZwooGameLogic.Events.Handler;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Events;

public class UserEventDistributer : IUserEventReceiver
{
    private readonly ZwooRoom _room;
    private readonly IUserEventHandler[] _handlers;


    public UserEventDistributer(ZwooRoom room)
    {
        _room = room;
        _handlers = new IUserEventHandler[] {
            new ChatHandler(room.NotificationDistributer),
            new LobbyHandler(room.NotificationDistributer),
            new SettingsHandler(room.NotificationDistributer),
            new GameHandler(room.NotificationDistributer),
            new BotsHandler(room.NotificationDistributer),
        };
    }

    public void DistributeEvent(IIncomingZRPMessage message)
    {
        IPlayer? player = _room.GetPlayerByRealId(message.UserId);
        if (player == null) return;

        var context = new UserContext(player.RealId, player.LobbyId, player.Username, player.Role, _room.Id, _room);
        foreach (IUserEventHandler handler in _handlers)
        {
            if (handler.HandleMessage(context, message))
            {
                break;
            }
        }
    }

    public void DistributeEvent(ILocalZRPMessage message)
    {
        IPlayer? player = _room.GetPlayer(message.LobbyId);
        if (player == null) return;

        var context = new UserContext(player.RealId, player.LobbyId, player.Username, player.Role, _room.Id, _room);
        foreach (IUserEventHandler handler in _handlers)
        {
            if (handler.HandleMessage(context, message))
            {
                break;
            }
        }
    }
}
