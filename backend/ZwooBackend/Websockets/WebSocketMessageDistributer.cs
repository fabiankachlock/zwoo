using System.Text;
using ZwooBackend.ZRP;
using ZwooBackend.Games;
using ZwooBackend.Websockets.Handlers;
using ZwooBackend.Websockets.Interfaces;

namespace ZwooBackend.Websockets;

public class WebSocketMessageDistributer
{

    private SendableWebSocketManager _webSocketManager;

    private MessageHandler[] _handlers;

    public WebSocketMessageDistributer(SendableWebSocketManager webSocketManager)
    {
        _webSocketManager = webSocketManager;
        _handlers = new MessageHandler[] {
            new ChatHandler(_webSocketManager),
        };
    }

    public void Distribute(byte[] message, int length, long playerId, long gameId)
    {
        string stringMessage = Encoding.UTF8.GetString(message, 0, length);
        ZRPCode? code = ZRPDecoder.GetCode(stringMessage);
        if (code != null)
        {
            LobbyManager.PlayerEntry player = GameManager.Global.GetGame(gameId)!.Lobby.GetPlayer(playerId)!;
            UserContext context = new UserContext(playerId, player.Username, player.Role, gameId);
            ZRPMessage zrpMessage = new ZRPMessage(code.Value, stringMessage);

            foreach (MessageHandler handler in _handlers)
            {
                if (handler.HandleMessage(context, zrpMessage))
                {
                    break;
                }
            }
        }
    }

}
