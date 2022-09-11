using System.Net.WebSockets;
using ZwooBackend.Websockets.Interfaces;
using ZwooBackend.ZRP;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Cards;

namespace ZwooBackend.Websockets.Handlers;

public class GameHandler : MessageHandler
{
    private SendableWebSocketManager _webSocketManager;

    public GameHandler(SendableWebSocketManager websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, ZRPMessage message)
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
            HandleSendDecission(context, message);
            return true;
        }
        return false;
    }

    private void StartGame(UserContext context, ZRPMessage message)
    {
        context.GameRecord.Game.Reset();
        foreach (long player in context.GameRecord.Lobby.Players())
        {
            context.GameRecord.Game.AddPlayer(player);
        }
        context.GameRecord.Game.Start();

        _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.GameStarted, new GameStartedDTO()));
        _webSocketManager.SendPlayer(context.GameRecord.Game.State.ActivePlayer(), ZRPEncoder.EncodeToBytes(ZRPCode.StartTurn, new StartTurnDTO()));
    }

    private void HandleCardPlace(UserContext context, ZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            PlaceCardDTO payload = message.DecodePyload<PlaceCardDTO>();
            context.GameRecord.Game.HandleEvent(ClientEvent.PlaceCard(context.Id, new Card(payload.Type, payload.Symbol)));
        }
        catch
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, "cant parse")));
        }
    }

    private void HandleCardDraw(UserContext context, ZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            DrawCardDTO payload = message.DecodePyload<DrawCardDTO>();
            context.GameRecord.Game.HandleEvent(ClientEvent.DrawCard(context.Id));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, "cant parse")));
        }
    }

    private void HandleSendDecission(UserContext context, ZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        try
        {
            ReceiveDecisionDTO payload = message.DecodePyload<ReceiveDecisionDTO>();
            context.GameRecord.Game.HandleEvent(ClientEvent.PlayerDecission(context.Id, (PlayerDecission)payload.Type, payload.Decision));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, "cant parse")));
        }
    }

    private void SendHand(UserContext context, ZRPMessage message)
    {
        if (context.Role == ZRPRole.Spectator) return;
        _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.SendHand, new SendHandDTO(context.GameRecord.Game.State.GetPlayerDeck(context.Id)!.Select(card => new SendHand_HandDTO(card.Color, card.Type)).ToArray())));
    }

    private void SendCardAmount(UserContext context, ZRPMessage message)
    {
        List<SendCardAmount_PlayersDTO> amounts = new List<SendCardAmount_PlayersDTO>();

        foreach (long player in context.GameRecord.Game.AllPlayers)
        {
            amounts.Add(new SendCardAmount_PlayersDTO(
                context.GameRecord.Lobby.GetPlayer(player)!.Username, 
                context.GameRecord.Game.State.GetPlayerCardAmount(player)!.Value,
                context.GameRecord.Game.State.GetPlayerOrder(player)!.Value,
                context.GameRecord.Game.State.ActivePlayer() == player
            ));
        }

        _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.SendCardAmount, new SendCardAmountDTO(amounts.ToArray())));
    }

    private void SendPileTop(UserContext context, ZRPMessage message)
    {
        var top = context.GameRecord.Game.State.GetPileTop();
        _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.SendPileTop, new SendPileTopDTO(top.Color, top.Type)));
    }
}
