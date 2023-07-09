using ZwooGameLogic.ZRP.Handlers;
using ZwooGameLogic.Lobby;
using ZwooGameLogic.Notifications;

namespace ZwooGameLogic.ZRP;

public class UserEventDistributer
{
    private INotificationAdapter _webSocketManager;

    private IEventHandler[] _handlers;

    public UserEventDistributer(INotificationAdapter webSocketManager)
    {
        _webSocketManager = webSocketManager;
        _handlers = new IEventHandler[] {
            new ChatHandler(_webSocketManager),
            new LobbyHandler(_webSocketManager),
            new SettingsHandler(_webSocketManager),
            new GameHandler(_webSocketManager),
            new BotsHandler(_webSocketManager),
        };
    }

    public void Distribute(ZwooRoom room, IIncomingZRPMessage msg)
    {
        IPlayer? player = room.GetPlayer(msg.UserId);
        if (player == null) return;

        UserContext context = new UserContext(player.Id, player.LobbyId, player.Username, player.Role, room.Id, room);
        foreach (IEventHandler handler in _handlers)
        {
            if (handler.HandleMessage(context, msg))
            {
                break;
            }
        }
    }

}
