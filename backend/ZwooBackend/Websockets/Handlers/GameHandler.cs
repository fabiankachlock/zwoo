using System.Net.WebSockets;
using ZwooBackend.Websockets.Interfaces;
using ZwooBackend.ZRP;
using ZwooGameLogic.Game.Settings;

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
    }

    private void HandleCardPlace(UserContext context, ZRPMessage message)
    {
    }
    private void HandleCardDraw(UserContext context, ZRPMessage message)
    {
    }

    private void HandleSendDecission(UserContext context, ZRPMessage message)
    {
    }

    private void SendHand(UserContext context, ZRPMessage message)
    {
        _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.SendHand, new SendHandDTO(context.GameRecord.Game.State.GetPlayerDeck(context.Id)!.Select(card => new SendHand_HandDTO(card.Color, card.Type)).ToArray())));
    }

    private void SendCardAmount(UserContext context, ZRPMessage message)
    {
        List<SendCardAmount_PlayersDTO> amounts = new List<SendCardAmount_PlayersDTO>();

        foreach (long player in context.GameRecord.Game.AllPlayers)
        {
            amounts.Add(new SendCardAmount_PlayersDTO(context.GameRecord.Lobby.GetPlayer(player)!.Username, context.GameRecord.Game.State.GetPlayerCardAmount(player)!.Value, context.GameRecord.Game.State.ActivePlayer() == player));
        }

        _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.SendCardAmount, new SendCardAmountDTO(amounts.ToArray())));
    }

    private void SendPileTop(UserContext context, ZRPMessage message)
    {
        var top = context.GameRecord.Game.State.GetPileTop();
        _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.SendPileTop, new SendPileTopDTO(top.Color, top.Type)));
    }
}
