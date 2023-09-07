using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Feedback;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Game.Settings;

namespace ZwooGameLogic.Game.Rules;

internal class DeckChangeRule : BaseCardRule
{
    public override int Priority
    {
        get => RulePriority.OverrideAll;
    }

    public override string Name
    {
        get => "DeckChangeRule";
    }

    public override RuleMeta? Meta => RuleMetaBuilder.New("deckChange")
        .IsTogglable()
        .Default(GameSettingsValue.Off)
        .Localize("de", "Hand-Tausch", "Wenn ein Spieler eine 2 legt, kann er sich einen beliebigen Spieler aussuchen, mit dem dieser alle Karten tauscht.")
        .Localize("en", "Deck-swap", "When a player lays a 2, he can choose any player with whom to exchange all cards.")
        .ToMeta();

    private record struct StoredEvent(long Player, List<long> Options, Card Card);

    private StoredEvent? _storedEvent = null;

    public DeckChangeRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return (gameEvent.Type == ClientEventType.PlaceCard && gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card.Type == CardType.Two)
            || (gameEvent.Type == ClientEventType.SendPlayerDecision && gameEvent.CastPayload<ClientEvent.PlayerDecissionEvent>().Decission == PlayerDecision.SelectPlayer);
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);

        if (gameEvent.Type == ClientEventType.PlaceCard)
        {
            ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
            bool isAllowed = IsAllowedCard(state.TopCard, payload.Card) && IsActivePlayer(state, payload.Player) && PlayerHasCard(state, payload.Player, payload.Card);
            if (isAllowed)
            {
                return PerformHandlePlaceCard(gameEvent, state);
            }
            else
            {
                return GameStateUpdate.WithEvents(state, GameEvent.Error(payload.Player, GameError.CantPlaceCard));
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
    protected GameStateUpdate PerformHandlePlaceCard(ClientEvent gameEvent, GameState state)
    {
        List<GameEvent> events = new List<GameEvent>();
        ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();

        var options = state.PlayerDecks.Keys.Where(id => id != payload.Player).ToList();
        _storedEvent = new StoredEvent(payload.Player, options, payload.Card);
        events.Add(GameEvent.GetPlayerDecision(
            state.CurrentPlayer,
            PlayerDecision.SelectPlayer,
            options.Select(k => ((int)k).ToString()).ToList()
        ));

        return GameStateUpdate.WithEvents(state, events);
    }

    /// <summary>
    /// handle an incoming player decision
    /// </summary>
    /// <param name="gameEvent">incoming client event</param>
    /// <param name="state">game state object</param>
    /// <param name="playerOrder">cycle of players</param>
    /// <returns></returns>
    protected GameStateUpdate PerformHandleDecission(ClientEvent gameEvent, GameState state, PlayerCycle playerOrder)
    {
        List<GameEvent> events = new List<GameEvent>();
        ClientEvent.PlayerDecissionEvent payload = gameEvent.CastPayload<ClientEvent.PlayerDecissionEvent>();

        if (_storedEvent.HasValue && _storedEvent?.Player == payload.Player)
        {
            long targetPlayer = _storedEvent.Value.Options[payload.Value];
            state.PlayerDecks[payload.Player].Remove(_storedEvent.Value.Card);

            List<GameEvent> swapEvents = new List<GameEvent>();
            swapEvents.Add(GameEvent.RemoveCard(targetPlayer, state.PlayerDecks[targetPlayer]));
            swapEvents.Add(GameEvent.SendCards(targetPlayer, state.PlayerDecks[payload.Player]));
            swapEvents.Add(GameEvent.RemoveCard(payload.Player, state.PlayerDecks[payload.Player]));
            swapEvents.Add(GameEvent.SendCards(payload.Player, state.PlayerDecks[targetPlayer]));

            var targetPlayerDeck = state.PlayerDecks[targetPlayer];
            state.PlayerDecks[targetPlayer] = state.PlayerDecks[payload.Player];
            state.PlayerDecks[payload.Player] = targetPlayerDeck;

            state = AddCardToStack(state, _storedEvent.Value.Card);
            (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
            events.AddRange(swapEvents);
            _storedEvent = null;
            return GameStateUpdate.New(state, events, UIFeedback.Interaction(UIFeedbackType.DeckSwapped, payload.Player, targetPlayer));
        }
        return GameStateUpdate.None(state);
    }
}
