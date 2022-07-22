using System.Net.WebSockets;
using ZwooBackend.Websockets.Interfaces;
using ZwooBackend.ZRP;

namespace ZwooBackend.Websockets.Handlers
{
    public class ChatHandler : MessageHandler
    {

        private SendableWebSocketManager _webSocketManager;

        public ChatHandler(SendableWebSocketManager websocketManager)
        {
            _webSocketManager = websocketManager;
        }

        public bool HandleMessage(UserContext context, ZRPMessage message)
        {
            if (message.Code != ZRPCode.PushMessage)
            {
                return false;
            }

            try
            {
                PushMessageDTO payload = message.DecodePyload<PushMessageDTO>();
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.DistributeMessage, new DistributeMessageDTO(payload.Message, context.UserName, context.Role)), WebSocketMessageType.Text, true);
                return false;
            }
            catch
            {
                // TODO: handle errors correctly
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, "cant parse")), WebSocketMessageType.Text, true);
                return true;
            }
        }
    }
}
