using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Events.Handler;

public class GameHandler : IUserEventHandler
{
    public Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> GetHandles()
    {
        return new Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>>() {
            { ZRPCode.StartGame, StartGame},
            { ZRPCode.GetHand, SendHand},
            { ZRPCode.GetPileTop, SendPileTop},
            { ZRPCode.GetCardAmount, SendCardAmount},
            { ZRPCode.PlaceCard, HandleCardPlace},
            { ZRPCode.DrawCard, HandleCardDraw},
            { ZRPCode.ReceiveDecision, HandleSendDecision},
            { ZRPCode.RequestEndTurn, HandleRequestEndTurn},
        };
    }

    private void StartGame(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            context.Game.Reset();
            foreach (IPlayer player in context.Lobby.GetPlayers())
            {
                context.Game.AddPlayer(player.LobbyId);
            }
            foreach (var bot in context.BotManager.ListBots())
            {
                context.Game.AddPlayer(bot.LobbyId);
            }
            context.Game.Start();

            websocketManager.BroadcastGame(context.GameId, ZRPCode.GameStarted, new GameStartedNotification());
            websocketManager.SendPlayer(context.Game.State.ActivePlayer(), ZRPCode.StartTurn, new StartTurnNotification());
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleCardPlace(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            PlaceCardEvent payload = message.DecodePayload<PlaceCardEvent>();
            context.Game.HandleEvent(ClientEvent.PlaceCard(context.LobbyId, new Card(payload.Type, payload.Symbol)));
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleCardDraw(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            DrawCardEvent payload = message.DecodePayload<DrawCardEvent>();
            context.Game.HandleEvent(ClientEvent.DrawCard(context.LobbyId));
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleSendDecision(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            PlayerDecisionEvent payload = message.DecodePayload<PlayerDecisionEvent>();
            context.Game.HandleEvent(ClientEvent.PlayerDecision(context.LobbyId, (PlayerDecision)payload.Type, payload.Decision));
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendHand(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            if (context.Role == ZRPRole.Spectator) return;
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendHand, new SendDeckNotification(context.Game.State.GetPlayerDeck(context.LobbyId)!.Select(card => new SendDeck_CardDTO(card.Color, card.Type)).ToArray()));
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendCardAmount(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            List<SendPlayerState_PlayerDTO> amounts = new List<SendPlayerState_PlayerDTO>();

            foreach (long playerId in context.Game.AllPlayers)
            {
                var player = context.Room.GetPlayer(playerId);
                if (player != null)
                {
                    amounts.Add(new SendPlayerState_PlayerDTO(
                        player.LobbyId,
                        player.Username,
                        context.Game.State.GetPlayerCardAmount(player.LobbyId)!.Value,
                        context.Game.State.GetPlayerOrder(player.LobbyId)!.Value,
                        context.Game.State.ActivePlayer() == player.LobbyId
                    ));
                }
            }

            websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendCardAmount, new SendPlayerStateNotification(amounts.ToArray()));
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendPileTop(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            var top = context.Game.State.GetPileTop();
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendPileTop, new SendPileTopNotification(top.Color, top.Type));
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleRequestEndTurn(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            RequestEndTurnEvent payload = message.DecodePayload<RequestEndTurnEvent>();
            context.Game.HandleEvent(ClientEvent.RequestEndTurn(context.LobbyId));
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
