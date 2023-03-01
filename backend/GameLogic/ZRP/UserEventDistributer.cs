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
        Console.WriteLine($"### getting player {msg.UserId} {room.Lobby.HasPlayerId(1)} {room.Lobby.GetPlayer(1) == null}");
        if (player == null) return;
        Console.WriteLine($"### player not null");

        UserContext context = new UserContext(player.Id, player.PublicId, player.Username, player.Role, room.Id, room);
        foreach (IEventHandler handler in _handlers)
        {
            if (handler.HandleMessage(context, msg))
            {
                break;
            }
        }
    }

}
