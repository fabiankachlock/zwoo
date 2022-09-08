using System.Net.WebSockets;
using ZwooBackend.Websockets.Interfaces;
using ZwooBackend.ZRP;
using ZwooGameLogic.Game.Events;
using ZwooBackend;
using ZwooGameLogic.Game;
using PlayerWonDTO = ZwooGameLogic.Game.Events.PlayerWonDTO;

namespace ZwooBackend.Websockets;

public class WebSocketNotificationManager : NotificationManager
{

    private SendableWebSocketManager _webSockets;

    private long _gameId;

    private Func<long, string> _resolvePlayerName;

    public WebSocketNotificationManager(SendableWebSocketManager webSocketManager, long gameId, Func<long, string> playerNameResolver)
    {
        _webSockets = webSocketManager;
        _gameId = gameId;
        _resolvePlayerName = playerNameResolver;
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
            _webSockets.SendPlayer((long)data.Player, ZRPEncoder.EncodeToBytes(ErrorToZRPCode(data.Error), new ErrorDTO((int)data.Error, data.Message)), WebSocketMessageType.Text, true);
        }
        else
        {
            _webSockets.BroadcastGame(this._gameId, ZRPEncoder.EncodeToBytes(ErrorToZRPCode(data.Error), new ErrorDTO((int)data.Error, data.Message)), WebSocketMessageType.Text, true);
        }
    }

    public void GetPlayerDecission(ZwooGameLogic.Game.Events.PlayerDecissionDTO data)
    {
        _webSockets.SendPlayer(data.Player, ZRPEncoder.EncodeToBytes(ZRPCode.GetPlayerDecision, new GetPlayerDecisionDTO((int)data.Decission)), WebSocketMessageType.Text, true);
    }

    public void PlayerWon(PlayerWonDTO data, GameMeta gameMeta)
    {
        uint winnerWins = Globals.ZwooDatabase.IncrementWin((ulong)data.Winner);
        Globals.ZwooDatabase.SaveGame(data.Scores, gameMeta);
        _webSockets.BroadcastGame(
            _gameId,
            ZRPEncoder.EncodeToBytes(
                ZRPCode.PlayerWon,
                new ZRP.PlayerWonDTO(
                    _resolvePlayerName(data.Winner),
                    (int)winnerWins,
                    data.Scores.Select(score => new PlayerWon_SummaryDTO(_resolvePlayerName(score.Key), data.Scores.Where(s => s.Value < score.Value).Count() + 1, score.Value)).OrderBy(s => s.Position).ToArray()
                )
            )
        );
    }

    public void RemoveCard(ZwooGameLogic.Game.Events.RemoveCardDTO data)
    {
        _webSockets.SendPlayer(data.Player, ZRPEncoder.EncodeToBytes(ZRPCode.RemoveCard, new ZRP.RemoveCardDTO(data.Card.Color, data.Card.Type)), WebSocketMessageType.Text, true);
    }

    public void SendCard(ZwooGameLogic.Game.Events.SendCardDTO data)
    {
        _webSockets.SendPlayer(data.Player, ZRPEncoder.EncodeToBytes(ZRPCode.SendCard, new ZRP.SendCardDTO(data.Card.Color, data.Card.Type)), WebSocketMessageType.Text, true);
    }

    public void StartTurn(long player)
    {
        _webSockets.SendPlayer(player, ZRPEncoder.EncodeToBytes(ZRPCode.StartTurn, new StartTurnDTO()), WebSocketMessageType.Text, true);
    }

    public void StateUpdate(ZwooGameLogic.Game.Events.StateUpdateDTO data)
    {
        // TODO: resolve player names
        _webSockets.BroadcastGame(
            _gameId,
            ZRPEncoder.EncodeToBytes(
                ZRPCode.StateUpdated,
                new ZRP.StateUpdatedDTO(new StateUpdated_PileTopDTO(data.PileTop.Color, data.PileTop.Type), _resolvePlayerName(data.ActivePlayer), data.ActivePlayerCardAmount, _resolvePlayerName(data.LastPlayer), data.LastPlayerCardAmount)
            ),
            WebSocketMessageType.Text,
            true
        );
    }

    public void StopGame()
    {
        // TODO: disconnect websocket / handle stop
    }

    private ZRPCode ErrorToZRPCode(GameError error)
    {
        switch (error)
        {
            case GameError.CantPlaceCard:
                return ZRPCode.PlaceCardError;
            default:
                return ZRPCode.GeneralError;
        }
    }
}
