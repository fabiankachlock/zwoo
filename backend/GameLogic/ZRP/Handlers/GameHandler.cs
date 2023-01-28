using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Notifications;

namespace ZwooGameLogic.ZRP.Handlers;


public class GameHandler : IEventHandler
{
    private INotificationAdapter _webSocketManager;

    public GameHandler(INotificationAdapter websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, IIncomingZRPMessage message)
    {
        if (message.Code == ZRPCode.StartGame)
        {
            StartGame(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.GetHand)
        {
            SendHand(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.GetPileTop)
        {
            SendPileTop(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.GetCardAmount)
        {
            SendCardAmount(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.PlaceCard)
        {
            HandleCardPlace(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.DrawCard)
        {
            HandleCardDraw(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.ReceiveDecision)
        {
            HandleSendDecision(context, message);
            return true;
        }
        return false;
    }

    private void StartGame(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            context.Game.Reset();
            foreach (IPlayer player in context.Lobby.GetPlayers())
            {
                context.Game.AddPlayer(player.Id);
            }
            context.BotManager.PrepareBotsForGame();
            foreach (var bot in context.BotManager.ListBots())
            {
                context.Game.AddPlayer(bot.PlayerId);
            }
            context.Game.Start();

            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.GameStarted, new GameStartedNotification());
            _webSocketManager.SendPlayer(context.Game.State.ActivePlayer(), ZRPCode.StartTurn, new StartTurnNotification());
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleCardPlace(UserContext context, IIncomingZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            PlaceCardEvent payload = message.DecodePayload<PlaceCardEvent>();
            context.Game.HandleEvent(ClientEvent.PlaceCard(context.Id, new Card(payload.Type, payload.Symbol)));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleCardDraw(UserContext context, IIncomingZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            DrawCardEvent payload = message.DecodePayload<DrawCardEvent>();
            context.Game.HandleEvent(ClientEvent.DrawCard(context.Id));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleSendDecision(UserContext context, IIncomingZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            PlayerDecisionEvent payload = message.DecodePayload<PlayerDecisionEvent>();
            context.Game.HandleEvent(ClientEvent.PlayerDecision(context.Id, (PlayerDecision)payload.Type, payload.Decision));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendHand(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            if (context.Role == ZRPRole.Spectator) return;
            _webSocketManager.SendPlayer(context.Id, ZRPCode.SendHand, new SendDeckNotification(context.Game.State.GetPlayerDeck(context.Id)!.Select(card => new SendDeck_CardDTO(card.Color, card.Type)).ToArray()));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendCardAmount(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            List<SendPlayerState_PlayerDTO> amounts = new List<SendPlayerState_PlayerDTO>();

            foreach (long player in context.Game.AllPlayers)
            {
                amounts.Add(new SendPlayerState_PlayerDTO(
                    context.Room.ResolvePlayerName(player) ?? "",
                    context.Game.State.GetPlayerCardAmount(player)!.Value,
                    context.Game.State.GetPlayerOrder(player)!.Value,
                    context.Game.State.ActivePlayer() == player
                ));
            }

            _webSocketManager.SendPlayer(context.Id, ZRPCode.SendCardAmount, new SendPlayerStateNotification(amounts.ToArray()));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendPileTop(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            var top = context.Game.State.GetPileTop();
            _webSocketManager.SendPlayer(context.Id, ZRPCode.SendPileTop, new SendPileTopNotification(top.Color, top.Type));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
