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

internal class SkipCardRule : BaseCardRule
{
    public override int Priority
    {
        get => RulePriority.DefaultRule;
    }

    public override string Name
    {
        get => "BaseCardRule";
    }

    public override GameSettingsKey? AssociatedOption
    {
        get => GameSettingsKey.DEFAULT_RULE_SET;
    }

    public SkipCardRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.PlaceCard && gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card.Type == CardType.Skip;
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new List<GameEvent>();

        ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
        bool isAllowed = IsFittingCard(state.TopCard.Card, payload.Card) && !(CardUtilities.IsDraw(state.TopCard.Card) && !state.TopCard.EventActivated);
        if (IsActivePlayer(state, payload.Player) && isAllowed && PlayerHasCard(state, payload.Player, payload.Card))
        {
            state = PlayPlayerCard(state, payload.Player, payload.Card);
            (state, events) = ChangeActivePlayerByAmount(state, playerOrder, 2);
            events.Add(GameEvent.RemoveCard(payload.Player, payload.Card));
            return new GameStateUpdate(state, events);
        }

        return GameStateUpdate.WithEvents(state, new List<GameEvent>() { GameEvent.Error(payload.Player, GameError.CantPlaceCard) });
    }

    // Rule utilities
    /// <summary>
    /// change the current player by a certain amount of hops
    /// </summary>
    /// <param name="state">game state object</param>
    /// <param name="playerOrder">cycle of players</param>
    /// <param name="amount">amount of hops</param>
    /// <returns>the updates game state and events for the client</returns>
    protected (GameState, List<GameEvent>) ChangeActivePlayerByAmount(GameState state, PlayerCycle playerOrder, int amount)
    {
        long nextPlayer = playerOrder.Next(state.Direction);
        if (amount > 1)
        {
            for (int i = 0; i < amount - 1; i++)
            {
                nextPlayer = playerOrder.Next(state.Direction);
            }
        }

        List<GameEvent> events = new List<GameEvent>() {
            GameEvent.EndTurn(state.CurrentPlayer),
            GameEvent.StartTurn(nextPlayer),
        };
        state.CurrentPlayer = nextPlayer;
        return (state, events);
    }
}
