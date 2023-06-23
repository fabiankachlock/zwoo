using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Rules;

internal class TestChangeCardRule : BaseCardRule
{
    public override int Priority
    {
        get => RulePriority.OverrideAll;
    }

    public override string Name
    {
        get => "TestChangeCardRule";
    }

    public override GameSettingsKey? AssociatedOption
    {
        get => GameSettingsKey.DEFAULT_RULE_SET;
    }

    private record struct StoredEvent(long Player, List<long> Options, Card Card);

    private StoredEvent? _storedEvent = null;

    public TestChangeCardRule() : base() { }

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
                return GameStateUpdate.WithEvents(state, new List<GameEvent>() { GameEvent.Error(payload.Player, GameError.CantPlaceCard) });
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

        return new GameStateUpdate(state, events);
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

            var targetPlayerDeck = state.PlayerDecks[targetPlayer];
            state.PlayerDecks[targetPlayer] = state.PlayerDecks[payload.Player];
            state.PlayerDecks[payload.Player] = targetPlayerDeck;

            state = AddCardToStack(state, _storedEvent.Value.Card);
            (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
            events.Add(GameEvent.RemoveCard(payload.Player, _storedEvent.Value.Card));
            _storedEvent = null;
            return new GameStateUpdate(state, events);
        }
        return GameStateUpdate.None(state);
    }

    // TODO: provide IsResponsible helpers
}
