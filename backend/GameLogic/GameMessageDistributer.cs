using System.Text;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.ZRP.Handlers;
using ZwooGameLogic.Lobby;

namespace ZwooGameLogic;

// should this be handled in ZwooRoom? - would make more sense
public class GameMessageDistributer
{

    private INotificationAdapter _webSocketManager;

    private IMessageHandler[] _handlers;

    public GameMessageDistributer(INotificationAdapter webSocketManager, GameManager gameManager)
    {
        _webSocketManager = webSocketManager;
        _handlers = new IMessageHandler[] {
            new ChatHandler(_webSocketManager),
            new LobbyHandler(_webSocketManager, gameManager),
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
