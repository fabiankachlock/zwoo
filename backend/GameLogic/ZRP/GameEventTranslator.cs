using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game;
using ZwooGameLogic.Notifications;
using GamePlayerWonDTO = ZwooGameLogic.Game.Events.PlayerWonDTO;

namespace ZwooGameLogic.ZRP;

public class GameEventTranslator : IGameEventManager
{

    private INotificationAdapter _wsAdapter;
    private ZwooRoom _game;


    public GameEventTranslator(ZwooRoom game, INotificationAdapter wsAdapter)
    {
        _wsAdapter = wsAdapter;
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
        _wsAdapter.SendPlayer(data.Player, ZRPCode.GetPlayerDecision, new GetPlayerDecisionNotification((int)data.Decision, data.Options));
    }

    public void PlayerWon(GamePlayerWonDTO data, GameMeta gameMeta)
    {

        _wsAdapter.BroadcastGame(
            _game!.Id,
            ZRPCode.PlayerWon,
            new PlayerWonNotification(
                data.Winner,
                data.Scores.Select(score => new PlayerWon_PlayerSummaryDTO(
                    score.Key,
                    data.Scores.Where(s => s.Value < score.Value).Count() + 1, score.Value
                )).OrderBy(s => s.Position).ToArray()
            )
        );
    }

    public void RemoveCard(ZwooGameLogic.Game.Events.RemoveCardDTO data)
    {
        _wsAdapter.SendPlayer(data.Player, ZRPCode.RemoveCards, new ZRP.RemoveCardNotification(data.Cards.Select(card => new RemoveCard_CardDTO(card.Color, card.Type)).ToArray()));
    }

    // TODO: utilize, that multiple cards can be sent at once
    public void SendCard(ZwooGameLogic.Game.Events.SendCardDTO data)
    {
        _wsAdapter.SendPlayer(data.Player, ZRPCode.SendCards, new ZRP.SendCardsNotification(data.Cards.Select(card => new SendCard_CardDTO(card.Color, card.Type)).ToArray()));
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
            new ZRP.StateUpdateNotification(
                new StateUpdate_PileTopDTO(data.PileTop.Color, data.PileTop.Type),
                data.ActivePlayer,
                data.CardAmounts,
                data.Feedback.Select(f => new StateUpdate_FeedbackDTO(f.Type, f.Kind, f.Args)).ToList(),
                data.CurrentDrawAmount
            )
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
