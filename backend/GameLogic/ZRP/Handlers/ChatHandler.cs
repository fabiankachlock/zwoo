using ZwooGameLogic.Notifications;

namespace ZwooGameLogic.ZRP.Handlers;

public class ChatHandler : IEventHandler
{

    private INotificationAdapter _webSocketManager;

    public ChatHandler(INotificationAdapter websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, IIncomingZRPMessage message)
    {
        if (message.Code != ZRPCode.CreateChatMessage)
        {
            return false;
        }

        try
        {
            ChatMessageEvent payload = message.DecodePayload<ChatMessageEvent>();
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SendChatMessage, new ChatMessageNotification(payload.Message, context.UserName, context.Role));
            return false;
        }
        catch (Exception e)
        {
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
            return true;
        }
    }
}
