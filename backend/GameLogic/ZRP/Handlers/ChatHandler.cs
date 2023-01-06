using ZwooGameLogic.Notifications;

namespace ZwooGameLogic.ZRP.Handlers;

public class ChatHandler : IMessageHandler
{

    private INotificationAdapter _webSocketManager;

    public ChatHandler(INotificationAdapter websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, IIncomingZRPMessage message)
    {
        if (message.Code != ZRPCode.PushMessage)
        {
            return false;
        }

        try
        {
            PushMessageDTO payload = message.DecodePayload<PushMessageDTO>();
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.DistributeMessage, new DistributeMessageDTO(payload.Message, context.UserName, context.Role));
            return false;
        }
        catch (Exception e)
        {
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString()));
            return true;
        }
    }
}
