using System.Net.WebSockets;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game;
using ZwooGameLogic.Lobby;
using ZwooGameLogic.Game.Settings;
using GamePlayerWonDTO = ZwooGameLogic.Game.Events.PlayerWonDTO;

namespace ZwooGameLogic.ZRP;

public class GameEventTranslator : NotificationManager
{

    private INotificationAdapter _wsAdapter;
    private ZwooRoom _game;


    public GameEventTranslator(INotificationAdapter wsAdapter, ZwooRoom game)
    {
        _wsAdapter = wsAdapter;
        _game = game;
    }

    public GameEventTranslator(INotificationAdapter wsAdapter)
    {
        _wsAdapter = wsAdapter;
        _game = new ZwooRoom(new Game.Game(0, "", true, this), new LobbyManager(0, new GameSettings()));
    }

    public void SetGame(ZwooRoom game)
    {
        _game = game;
    }

    public void EndTurn(long player)
    {
        _wsAdapter.SendPlayer(player, ZRPCode.EndTurn, new EndTurnDTO());
    }

    public void Error(ErrorDto data)
    {
        // TODO: Use zrp errors or remove them from zrp
        if (data.Player != null)
        {
            _wsAdapter.SendPlayer((long)data.Player, ErrorToZRPCode(data.Error), new ErrorDTO((int)data.Error, data.Message));
        }
        else
        {
            _wsAdapter.BroadcastGame(this._game.Id, ErrorToZRPCode(data.Error), new ErrorDTO((int)data.Error, data.Message));
        }
    }

    public void GetPlayerDecision(ZwooGameLogic.Game.Events.PlayerDecisionDTO data)
    {
        _wsAdapter.SendPlayer(data.Player, ZRPCode.GetPlayerDecision, new GetPlayerDecisionDTO((int)data.Decision));
    }

    public void PlayerWon(GamePlayerWonDTO data, GameMeta gameMeta)
    {
        // uint winnerWins = Globals.ZwooDatabase.IncrementWin((ulong)data.Winner);
        // Globals.ZwooDatabase.SaveGame(data.Scores, gameMeta); // TODO: should not be here
        _wsAdapter.BroadcastGame(
            _game.Id,
            ZRPCode.PlayerWon,
            new PlayerWonDTO(
                _game.Lobby.ResolvePlayer(data.Winner),
                0, // TODO: (int)winnerWins,
                data.Scores.Select(score => new PlayerWon_SummaryDTO(_game.Lobby.ResolvePlayer(score.Key), data.Scores.Where(s => s.Value < score.Value).Count() + 1, score.Value)).OrderBy(s => s.Position).ToArray()
            )
        );
        // _webSockets.FinishGame(_gameId);
    }

    public void RemoveCard(ZwooGameLogic.Game.Events.RemoveCardDTO data)
    {
        _wsAdapter.SendPlayer(data.Player, ZRPCode.RemoveCard, new ZRP.RemoveCardDTO(data.Card.Color, data.Card.Type));
    }

    public void SendCard(ZwooGameLogic.Game.Events.SendCardDTO data)
    {
        _wsAdapter.SendPlayer(data.Player, ZRPCode.SendCard, new ZRP.SendCardDTO(data.Card.Color, data.Card.Type));
    }

    public void StartTurn(long player)
    {
        _wsAdapter.SendPlayer(player, ZRPCode.StartTurn, new StartTurnDTO());
    }

    public void StateUpdate(ZwooGameLogic.Game.Events.StateUpdateDTO data)
    {
        _wsAdapter.BroadcastGame(
            _game.Id,
            ZRPCode.StateUpdated,
            new ZRP.StateUpdatedDTO(new StateUpdated_PileTopDTO(data.PileTop.Color, data.PileTop.Type), _game.Lobby.ResolvePlayer(data.ActivePlayer), data.ActivePlayerCardAmount, _game.Lobby.ResolvePlayer(data.LastPlayer), data.LastPlayerCardAmount)
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
