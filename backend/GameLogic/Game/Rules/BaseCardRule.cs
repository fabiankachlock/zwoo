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

internal class BaseCardRule : BaseRule
{
    public override int Priority
    {
        get => RulePriority.BaseRule;
    }

    public override string Name
    {
        get => "BaseCardRule";
    }

    public override RuleMeta? Setting => null;

    public BaseCardRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.PlaceCard && !CardUtilities.IsWild(gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card);
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new List<GameEvent>();

        ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
        bool isAllowed = IsAllowedCard(state.TopCard, payload.Card);
        if (IsActivePlayer(state, payload.Player) && isAllowed && PlayerHasCard(state, payload.Player, payload.Card))
        {
            state = PlayPlayerCard(state, payload.Player, payload.Card);
            (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
            events.Add(GameEvent.RemoveCard(payload.Player, payload.Card));
            state.Ui.CurrentDrawAmount = GetActiveDrawAmount(state.TopCard);
            return GameStateUpdate.WithEvents(state, events);
        }

        return GameStateUpdate.WithEvents(state, new List<GameEvent>() { GameEvent.Error(payload.Player, GameError.CantPlaceCard) });
    }

    // Rule utilities
    /// <summary>
    /// does a basic card compatibility check on following conditions
    /// - same color or type
    /// - is wild
    /// </summary>
    /// <param name="top">stack top card</param>
    /// <param name="newCard">card to throw</param>
    /// <returns></returns>
    protected bool IsFittingCard(Card top, Card newCard)
    {
        return top.Type == newCard.Type || top.Color == newCard.Color || CardUtilities.IsWild(newCard);
    }

    /// <summary>
    /// does a deeper card compatibility check on following conditions
    /// - is fitting
    /// - top card has no active event
    /// </summary>
    /// <param name="top">stack top card</param>
    /// <param name="newCard">card to throw</param>
    /// <returns></returns>
    protected bool IsAllowedCard(StackCard top, Card newCard)
    {
        // only allowed when the topmost card is no draw card or the event is already activated
        return IsFittingCard(top.Card, newCard) && (!CardUtilities.IsDraw(top.Card) || top.EventActivated);
    }

    /// <summary>
    /// place a new card onto the card stack
    /// </summary>
    /// <param name="state">games state object</param>
    /// <param name="card">card to put onto the stack</param>
    /// <returns>updated game state</returns>
    protected GameState AddCardToStack(GameState state, Card card)
    {
        state.TopCard = new StackCard(card);
        state.CardStack.Add(state.TopCard);
        return state;
    }

    /// <summary>
    /// place a card from a player onto the card stack
    /// </summary>
    /// <param name="state">games state object</param>
    /// <param name="player">players ID</param>
    /// <param name="card">card to play</param>
    /// <returns>updated game state</returns>
    protected GameState PlayPlayerCard(GameState state, long player, Card card)
    {
        state.PlayerDecks[player].Remove(card);
        return AddCardToStack(state, card);
    }

    // TODO: add PerformBasicPlaceCard() Method as ApplyRule Wrapper
}
