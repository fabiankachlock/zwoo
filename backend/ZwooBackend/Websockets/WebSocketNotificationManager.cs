using System.Net.WebSockets;
using ZwooBackend.Websockets.Interfaces;
using ZwooBackend.ZRP;
using ZwooGameLogic.Game.Events;

namespace ZwooBackend.Websockets;

public class WebSocketNotificationManager : NotificationManager
{

    private SendableWebSocketManager _webSockets;

    private long _gameId;

    public WebSocketNotificationManager(SendableWebSocketManager webSocketManager, long gameId)
    {
        _webSockets = webSocketManager;
        _gameId = gameId;
    }

    public void EndTurn(long player)
    {
        _webSockets.SendPlayer(player, ZRPEncoder.EncodeToBytes(ZRPCode.EndTurn, new EndTurnDTO()), WebSocketMessageType.Text, true);
    }

    public void Error(ErrorDto data)
    {
        // TODO: Use zrp errors or remove them from zrp
        if (data.Player != null)
        {
            _webSockets.SendPlayer((long)data.Player, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)data.Error, "")), WebSocketMessageType.Text, true);
        }
        else
        {
            _webSockets.BroadcastGame(this._gameId, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)data.Error, "")), WebSocketMessageType.Text, true);
        }
    }

    public void GetPlayerDecission(ZwooGameLogic.Game.Events.PlayerDecissionDTO data)
    {
        _webSockets.SendPlayer(data.Player, ZRPEncoder.EncodeToBytes(ZRPCode.GetPlayerDecision, new GetPlayerDecisionDTO((int)data.Decission)), WebSocketMessageType.Text, true);
    }

    public void PlayerWon(ZwooGameLogic.Game.Events.PlayerWonDTO data)
    {
        // TODO: construct message
        _webSockets.BroadcastGame(_gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerWon, new ZRP.PlayerWonDTO()), WebSocketMessageType.Text, true);
    }

    public void RemoveCard(ZwooGameLogic.Game.Events.RemoveCardDTO data)
    {
        _webSockets.SendPlayer(data.Player, ZRPEncoder.EncodeToBytes(ZRPCode.RemoveCard, new ZRP.RemoveCardDTO(data.Card.Color, data.Card.Type)), WebSocketMessageType.Text, true);
    }

    public void SendCard(ZwooGameLogic.Game.Events.SendCardDTO data)
    {
        _webSockets.SendPlayer(data.Player, ZRPEncoder.EncodeToBytes(ZRPCode.SendCard, new ZRP.SendCardDTO(data.Card.Color, data.Card.Type)), WebSocketMessageType.Text, true);
    }

    public void StartGame()
    {
        _webSockets.BroadcastGame(_gameId, ZRPEncoder.EncodeToBytes(ZRPCode.EndTurn, new StartGameDTO()), WebSocketMessageType.Text, true);
    }

    public void StartTurn(long player)
    {
        _webSockets.SendPlayer(player, ZRPEncoder.EncodeToBytes(ZRPCode.EndTurn, new StartTurnDTO()), WebSocketMessageType.Text, true);
    }

    public void StateUpdate(ZwooGameLogic.Game.Events.StateUpdateDTO data)
    {
        // TODO: resolve player names
        _webSockets.BroadcastGame(
            _gameId,
            ZRPEncoder.EncodeToBytes(
                ZRPCode.StateUpdated,
                new ZRP.StateUpdatedDTO(new StateUpdated_PileTopDTO(data.PileTop.Color, data.PileTop.Type), data.ActivePlayer.ToString(), data.ActivePlayerCardAmount, data.LastPlayer.ToString(), data.LastPlayerCardAmount)
            ),
            WebSocketMessageType.Text,
            true
        );
    }

    public void StopGame()
    {
        // TODO: disconnect websocket / handle stop
    }
}
