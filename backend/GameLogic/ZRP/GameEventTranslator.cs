using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game;
using ZwooGameLogic.Notifications;
using GamePlayerWonDTO = ZwooGameLogic.Game.Events.PlayerWonDTO;

namespace ZwooGameLogic.ZRP;

public class GameEventTranslator : IGameEventManager
{

    private INotificationAdapter _wsAdapter;
    private ZwooRoom? _game;


    public GameEventTranslator(INotificationAdapter wsAdapter, ZwooRoom? game = null)
    {
        _wsAdapter = wsAdapter;
        _game = game;
    }

    public void SetGame(ZwooRoom game)
    {
        _game = game;
    }

    public void EndTurn(long player)
    {
        _wsAdapter.SendPlayer(player, ZRPCode.EndTurn, new EndTurnNotification());
    }

    public void Error(ErrorDto data)
    {
        // TODO: Use zrp errors or remove them from zrp
        if (data.Player != null)
        {
            _wsAdapter.SendPlayer((long)data.Player, ErrorToZRPCode(data.Error), new Error((int)data.Error, data.Message));
        }
        else
        {
            _wsAdapter.BroadcastGame(this._game!.Id, ErrorToZRPCode(data.Error), new Error((int)data.Error, data.Message));
        }
    }

    public void GetPlayerDecision(ZwooGameLogic.Game.Events.PlayerDecisionDTO data)
    {
        _wsAdapter.SendPlayer(data.Player, ZRPCode.GetPlayerDecision, new GetPlayerDecisionNotification((int)data.Decision));
    }

    public void PlayerWon(GamePlayerWonDTO data, GameMeta gameMeta)
    {
        _wsAdapter.BroadcastGame(
            _game!.Id,
            ZRPCode.PlayerWon,
            new PlayerWonNotification(
                _game.Lobby.ResolvePlayer(data.Winner),
                0, // TODO: (int)winnerWins,
                data.Scores.Select(score => new PlayerWon_PlayerSummaryDTO(_game.Lobby.ResolvePlayer(score.Key), data.Scores.Where(s => s.Value < score.Value).Count() + 1, score.Value)).OrderBy(s => s.Position).ToArray()
            )
        );
    }

    public void RemoveCard(ZwooGameLogic.Game.Events.RemoveCardDTO data)
    {
        _wsAdapter.SendPlayer(data.Player, ZRPCode.RemoveCard, new ZRP.RemoveCardNotification(data.Card.Color, data.Card.Type));
    }

    // TODO: utilize, that multiple cards can be sent at once
    public void SendCard(ZwooGameLogic.Game.Events.SendCardDTO data)
    {
        _wsAdapter.SendPlayer(data.Player, ZRPCode.SendCard, new ZRP.SendCardsNotification(new SendCard_CardDTO[] { new SendCard_CardDTO(data.Card.Color, data.Card.Type) }));
    }

    public void StartTurn(long player)
    {
        _wsAdapter.SendPlayer(player, ZRPCode.StartTurn, new StartTurnNotification());
    }

    public void StateUpdate(ZwooGameLogic.Game.Events.StateUpdateDTO data)
    {
        _wsAdapter.BroadcastGame(
            _game!.Id,
            ZRPCode.StateUpdated,
            new ZRP.StateUpdateNotification(new StateUpdate_PileTopDTO(data.PileTop.Color, data.PileTop.Type), _game.Lobby.ResolvePlayer(data.ActivePlayer), data.ActivePlayerCardAmount, _game.Lobby.ResolvePlayer(data.LastPlayer), data.LastPlayerCardAmount)
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
