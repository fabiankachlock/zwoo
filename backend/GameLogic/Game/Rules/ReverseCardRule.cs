using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Feedback;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Game.Settings;

namespace ZwooGameLogic.Game.Rules;

internal class ReverseCardRule : BaseCardRule
{
    public override int Priority
    {
        get => RulePriority.DefaultRule;
    }

    public override string Name
    {
        get => "ReverseCardRule";
    }

    public override RuleMeta? Meta => RuleMetaBuilder.New("reverse")
    .Localize("de", "DummeR", "Regel")
    .Localize("en", "DummyR", "Rule")
            .ConfigureParameter("test1", setting =>
        {
            setting.Type = GameSettingsType.Readonly;
            setting.Localize("de", "testde", "longde");
            setting.Localize("en", "testen", "longen");
        })
        .ConfigureParameter("test2", setting =>
        {
            setting.Type = GameSettingsType.Boolean;
            setting.Localize("de", "bool", "longde");
            setting.Localize("en", "bool", "longen");
        })
        .ConfigureParameter("test3", setting =>
        {
            setting.Type = GameSettingsType.Numeric;
            setting.Localize("de", "number", "longde");
            setting.Localize("en", "number", "longen");
        })
    .ToMeta();

    public ReverseCardRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.PlaceCard && gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card.Type == CardType.Reverse;
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events;

        ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
        bool isAllowed = IsFittingCard(state.TopCard.Card, payload.Card) && !(CardUtilities.IsDraw(state.TopCard.Card) && !state.TopCard.EventActivated);
        if (IsActivePlayer(state, payload.Player) && isAllowed && PlayerHasCard(state, payload.Player, payload.Card))
        {
            state = PlayPlayerCard(state, payload.Player, payload.Card);
            state.Direction = state.Direction == GameDirection.Left ? GameDirection.Rigth : GameDirection.Left;
            (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
            events.Add(GameEvent.RemoveCard(payload.Player, payload.Card));
            return GameStateUpdate.New(state, events, UIFeedback.Unaffected(UIFeedbackType.DirectionChanged));
        }

        return GameStateUpdate.NoneWithEvents(state, GameEvent.Error(payload.Player, GameError.CantPlaceCard));
    }

    // TODO: add PerformReversDirection() method as ApplyRule wrapper
}
