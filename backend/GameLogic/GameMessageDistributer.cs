using System.Text;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.ZRP.Handlers;
using ZwooGameLogic.Lobby;

namespace ZwooGameLogic;

public class GameMessageDistributer
{

    private INotificationAdapter _webSocketManager;

    private IMessageHandler[] _handlers;

    public GameMessageDistributer(INotificationAdapter webSocketManager)
    {
        _webSocketManager = webSocketManager;
        _handlers = new IMessageHandler[] {
            new ChatHandler(_webSocketManager),
            new LobbyHandler(_webSocketManager),
            new SettingsHandler(_webSocketManager),
            new GameHandler(_webSocketManager),
        };
    }

    public void Distribute(ZwooRoom room, IIncomingZRPMessage msg)
    {
        LobbyManager.PlayerEntry? player = room.Lobby.GetPlayer(msg.UserId);
        if (player == null) return;

        UserContext context = new UserContext(player.Id, player.Username, player.Role, 0, room.Id, room);
        foreach (IMessageHandler handler in _handlers)
        {
            if (handler.HandleMessage(context, msg))
            {
                break;
            }
        }
    }

}
