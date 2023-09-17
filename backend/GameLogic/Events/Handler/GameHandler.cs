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

        List<SendPlayerState_PlayerDTO> amounts = GetCardAmounts(context);
        var top = context.Game.State.GetPileTop();
        var pileTopNotification = new SendPileTopNotification(top.Color, top.Type);

        foreach (var player in context.Game.AllPlayers)
        {
            websocketManager.SendPlayer(player, ZRPCode.GameStarted, new GameStartedNotification(
                context.Game.State.GetPlayerDeck(player)?.Select(card => new SendDeck_CardDTO(card.Color, card.Type)).ToArray() ?? new List<SendDeck_CardDTO>().ToArray(),
                amounts.ToArray(),
                pileTopNotification
            ));
        }

        websocketManager.SendPlayer(context.Game.State.ActivePlayer(), ZRPCode.StartTurn, new StartTurnNotification());
    }

    private void HandleCardPlace(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;

        PlaceCardEvent payload = message.DecodePayload<PlaceCardEvent>();
        context.Game.HandleEvent(ClientEvent.PlaceCard(context.LobbyId, new Card(payload.Type, payload.Symbol)));
    }

    private void HandleCardDraw(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;

        context.Game.HandleEvent(ClientEvent.DrawCard(context.LobbyId));
    }

    private void HandleSendDecision(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;

        PlayerDecisionEvent payload = message.DecodePayload<PlayerDecisionEvent>();
        context.Game.HandleEvent(ClientEvent.PlayerDecision(context.LobbyId, (PlayerDecision)payload.Type, payload.Decision));
    }

    private void SendHand(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;

        websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendHand, new SendDeckNotification(context.Game.State.GetPlayerDeck(context.LobbyId)!.Select(card => new SendDeck_CardDTO(card.Color, card.Type)).ToArray()));
    }

    private void SendCardAmount(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        List<SendPlayerState_PlayerDTO> amounts = GetCardAmounts(context);

        websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendCardAmount, new SendPlayerStateNotification(amounts.ToArray()));
    }

    private void SendPileTop(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        var top = context.Game.State.GetPileTop();
        websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendPileTop, new SendPileTopNotification(top.Color, top.Type));
    }

    private void HandleRequestEndTurn(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role == ZRPRole.Spectator) return;

        context.Game.HandleEvent(ClientEvent.RequestEndTurn(context.LobbyId));
    }
    private List<SendPlayerState_PlayerDTO> GetCardAmounts(UserContext context)
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

        return amounts;
    }
}
