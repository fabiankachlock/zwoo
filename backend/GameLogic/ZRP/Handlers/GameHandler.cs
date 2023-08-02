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
        else if (message.Code == ZRPCode.RequestEndTurn)
        {
            HandleRequestEndTurn(context, message);
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
                context.Game.AddPlayer(player.LobbyId);
            }
            foreach (var bot in context.BotManager.ListBots())
            {
                context.Game.AddPlayer(bot.LobbyId);
            }
            context.Game.Start();

            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.GameStarted, new GameStartedNotification());
            _webSocketManager.SendPlayer(context.Game.State.ActivePlayer(), ZRPCode.StartTurn, new StartTurnNotification());
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleCardPlace(UserContext context, IIncomingZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            PlaceCardEvent payload = message.DecodePayload<PlaceCardEvent>();
            context.Game.HandleEvent(ClientEvent.PlaceCard(context.LobbyId, new Card(payload.Type, payload.Symbol)));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleCardDraw(UserContext context, IIncomingZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            DrawCardEvent payload = message.DecodePayload<DrawCardEvent>();
            context.Game.HandleEvent(ClientEvent.DrawCard(context.LobbyId));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleSendDecision(UserContext context, IIncomingZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            PlayerDecisionEvent payload = message.DecodePayload<PlayerDecisionEvent>();
            context.Game.HandleEvent(ClientEvent.PlayerDecision(context.LobbyId, (PlayerDecision)payload.Type, payload.Decision));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendHand(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            if (context.Role == ZRPRole.Spectator) return;
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.SendHand, new SendDeckNotification(context.Game.State.GetPlayerDeck(context.LobbyId)!.Select(card => new SendDeck_CardDTO(card.Color, card.Type)).ToArray()));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendCardAmount(UserContext context, IIncomingZRPMessage message)
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

            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.SendCardAmount, new SendPlayerStateNotification(amounts.ToArray()));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void SendPileTop(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            var top = context.Game.State.GetPileTop();
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.SendPileTop, new SendPileTopNotification(top.Color, top.Type));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void HandleRequestEndTurn(UserContext context, IIncomingZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            RequestEndTurnEvent payload = message.DecodePayload<RequestEndTurnEvent>();
            context.Game.HandleEvent(ClientEvent.RequestEndTurn(context.LobbyId));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
