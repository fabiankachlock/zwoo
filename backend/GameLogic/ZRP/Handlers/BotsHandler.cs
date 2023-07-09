using ZwooGameLogic.Lobby;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.Bots;


namespace ZwooGameLogic.ZRP.Handlers;

public class BotsHandler : IEventHandler
{
    private INotificationAdapter _webSocketManager;

    public BotsHandler(INotificationAdapter websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, IIncomingZRPMessage message)
    {
        if (message.Code == ZRPCode.CreateBot)
        {
            CreateBot(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.UpdateBot)
        {
            UpdateBot(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.DeleteBot)
        {
            DeleteBot(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.GetBots)
        {
            GetBots(context, message);
            return true;
        }
        return false;
    }

    private void CreateBot(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            CreateBotEvent data = message.DecodePayload<CreateBotEvent>();
            if (context.BotManager.HasBotWithName(data.Username))
            {
                _webSocketManager.SendPlayer(context.Id, ZRPCode.BotNameExistsError, new Error((int)ZRPCode.BotNameExistsError, "bot name already exists"));
                return;
            }

            Bot newBot = context.BotManager.CreateBot(data.Username, new BotConfig()
            {
                Type = data.Config.Type
            });
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.BotJoined, new BotJoinedNotification(newBot.AsPlayer().LobbyId, newBot.Username, 0));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void UpdateBot(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            UpdateBotEvent data = message.DecodePayload<UpdateBotEvent>();
            Bot? botToUpdate = context.BotManager.ListBots().Find(b => b.AsPlayer().LobbyId == data.Id);
            if (botToUpdate != null)
            {
                botToUpdate.SetConfig(new BotConfig()
                {
                    Type = data.Config.Type
                });
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void DeleteBot(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            DeleteBotEvent data = message.DecodePayload<DeleteBotEvent>();
            context.BotManager.RemoveBot(data.Id);
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.BotLeft, new BotLeftNotification(data.Id));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void GetBots(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.SendBots, new AllBotsNotification(context.BotManager.ListBots().Select(bot => new AllBots_BotDTO(bot.AsPlayer().LobbyId, bot.Username, new BotConfigDTO(bot.Config.Type), 0)).ToArray()));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }


}
