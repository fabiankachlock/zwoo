using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Feedback;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Cards;

namespace Zwoo.GameEngine.Game.Rules;

public class BaseWildCardRule : BaseCardRule
{
    public override int Priority
    {
        get => RulePriority.DefaultRule;
    }

    public override string Name
    {
        get => "BaseWildRule";
    }

    private record struct StoredEvent(long Player, Card Card);

    private StoredEvent? _storedEvent = null;

    private readonly List<CardColor> _optionsMapper = new List<CardColor>()
    {
         CardColor.Red,
         CardColor.Yellow,
         CardColor.Blue,
         CardColor.Green,
    };

    public BaseWildCardRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return (gameEvent.Type == ClientEventType.PlaceCard && CardUtilities.IsWild(gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card))
            || (gameEvent.Type == ClientEventType.SendPlayerDecision && gameEvent.CastPayload<ClientEvent.PlayerDecissionEvent>().Decission == PlayerDecision.SelectColor);
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, IPlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);

        if (gameEvent.Type == ClientEventType.PlaceCard)
        {
            ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
            bool isAllowed = IsAllowedCard(state.TopCard, payload.Card) && IsActivePlayer(state, payload.Player) && PlayerHasCard(state, payload.Player, payload.Card);
            if (isAllowed)
            {
                return PerformHandlePlaceWild(gameEvent, state);
            }
            else
            {
                return GameStateUpdate.NoneWithEvents(state, GameEvent.Error(payload.Player, GameError.CantPlaceCard));
            }
        }
        return PerformHandleDecission(gameEvent, state, playerOrder);
    }

    /// <summary>
    /// place a players wild card
    /// </summary>
    /// <param name="gameEvent">incoming client event</param>
    /// <param name="state">game state object</param>
    /// <returns>the game state update to return to the client</returns>
    protected GameStateUpdate PerformHandlePlaceWild(ClientEvent gameEvent, GameState state)
    {
        ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
        _storedEvent = new StoredEvent(payload.Player, payload.Card);

        return GameStateUpdate.NoneWithEvents(state, GameEvent.GetPlayerDecision(
            state.CurrentPlayer,
            PlayerDecision.SelectColor,
            _optionsMapper.Select(k => ((int)k).ToString()).ToList()
        ));
    }

    /// <summary>
    /// handle an incoming player decision
    /// </summary>
    /// <param name="gameEvent">incoming client event</param>
    /// <param name="state">game state object</param>
    /// <param name="playerOrder">cycle of players</param>
    /// <returns></returns>
    protected GameStateUpdate PerformHandleDecission(ClientEvent gameEvent, GameState state, IPlayerCycle playerOrder)
    {
        List<GameEvent> events;
        ClientEvent.PlayerDecissionEvent payload = gameEvent.CastPayload<ClientEvent.PlayerDecissionEvent>();

        if (_storedEvent.HasValue && _storedEvent?.Player == payload.Player)
        {
            CardColor color = _optionsMapper[payload.Value];
            Card newCard = new Card(color, _storedEvent.Value.Card.Type);
            state.PlayerDecks[payload.Player].Remove(_storedEvent.Value.Card);
            state = AddCardToStack(state, newCard);
            (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
            events.Add(GameEvent.RemoveCard(payload.Player, _storedEvent.Value.Card));
            state.Ui.CurrentDrawAmount = GetActiveDrawAmount(state.TopCard);
            _storedEvent = null;
            return GameStateUpdate.New(state, events, UIFeedback.Individual(UIFeedbackType.ColorChanged, payload.Player));
        }
        return GameStateUpdate.None(state);
    }

    // TODO: provide IsResponsible helpers
}
