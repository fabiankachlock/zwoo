using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Events.Handler;

public class ChatHandler : IUserEventHandler
{

    public Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> GetHandles()
    {
        return new Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>>() {
            { ZRPCode.CreateChatMessage, HandleMessage},

        };
    }

    public void HandleMessage(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            ChatMessageEvent payload = message.DecodePayload<ChatMessageEvent>();
            websocketManager.BroadcastGame(context.GameId, ZRPCode.SendChatMessage, new ChatMessageNotification(context.LobbyId, payload.Message));
        }
        catch (Exception e)
        {
            websocketManager.BroadcastGame(context.GameId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
