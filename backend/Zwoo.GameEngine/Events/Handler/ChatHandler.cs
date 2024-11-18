using Zwoo.GameEngine.Notifications;
using Zwoo.GameEngine.ZRP;
using Zwoo.Api.ZRP;

namespace Zwoo.GameEngine.Events.Handler;

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
        ChatMessageEvent payload = message.DecodePayload<ChatMessageEvent>();
        websocketManager.BroadcastGame(context.GameId, ZRPCode.BroadcastChatMessage, new ChatMessageNotification(context.LobbyId, payload.Message));
    }
}
