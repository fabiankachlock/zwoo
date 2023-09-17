using ZwooGameLogic.ZRP;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.Bots;

namespace ZwooGameLogic.Events.Handler;

public class BotsHandler : IUserEventHandler
{

    public Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> GetHandles()
    {
        return new Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>>() {
            { ZRPCode.CreateBot, CreateBot},
            { ZRPCode.UpdateBot, UpdateBot},
            { ZRPCode.DeleteBot, DeleteBot},
            { ZRPCode.GetBots, GetBots},
        };
    }

    private void CreateBot(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        CreateBotEvent data = message.DecodePayload<CreateBotEvent>();
        if (context.BotManager.HasBotWithName(data.Username))
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.BotNameExistsError, new Error((int)ZRPCode.BotNameExistsError, "bot name already exists"));
            return;
        }

        Bot newBot = context.BotManager.CreateBot(context.Room.NextId(), data.Username, new BotConfig()
        {
            Type = data.Config.Type
        });
        websocketManager.BroadcastGame(context.GameId, ZRPCode.BotJoined, new BotJoinedNotification(newBot.AsPlayer().LobbyId, newBot.Username, 0));
        // FIXME: notify to host (ZWOO-373)
        GetBots(context, message, websocketManager);
    }

    private void UpdateBot(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        UpdateBotEvent data = message.DecodePayload<UpdateBotEvent>();
        Bot? botToUpdate = context.BotManager.ListBots().Find(b => b.AsPlayer().LobbyId == data.Id);
        botToUpdate?.SetConfig(new BotConfig()
        {
            Type = data.Config.Type
        });
    }

    private void DeleteBot(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        DeleteBotEvent data = message.DecodePayload<DeleteBotEvent>();
        context.BotManager.RemoveBot(data.Id);
        websocketManager.BroadcastGame(context.GameId, ZRPCode.BotLeft, new BotLeftNotification(data.Id));
    }

    private void GetBots(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendBots, new AllBotsNotification(context.BotManager.ListBots().Select(bot => new AllBots_BotDTO(bot.AsPlayer().LobbyId, bot.Username, new BotConfigDTO(bot.Config.Type), 0)).ToArray()));
    }
}
