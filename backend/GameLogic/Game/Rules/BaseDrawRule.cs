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

internal class BaseDrawRule : BaseRule
{
    public override int Priority
    {
        get => RulePriorirty.BaseRule;
    }

    public override string Name
    {
        get => "BaseDrawRule";
    }

    public override GameSettingsKey? AssociatedOption
    {
        get => GameSettingsKey.DEFAULT_RULE_SET;
    }

    public BaseDrawRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.DrawCard;
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new List<GameEvent>();

        int amount = 0;
        ClientEvent.DrawCardEvent payload = gameEvent.CastPayload<ClientEvent.DrawCardEvent>();

        if (!IsActivePlayer(state, payload.Player))
        {
            return GameStateUpdate.None(state);
        }

        if (CardUtilities.IsDraw(state.TopCard.Card) && !state.TopCard.EventActivated)
        {
            amount = GetDrawAmount(state.TopCard.Card);
            state.TopCard.ActivateEvent();
        }
        else
        {
            amount = 1;
        }

        List<Card> newCards;
        (state, newCards) = DrawCardsForPlayer(state, payload.Player, amount, cardPile);
        (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
        events.Add(GameEvent.SendCards(payload.Player, newCards));


        return new GameStateUpdate(state, events);
    }

    // Rule utilities
    /// <summary>
    /// get the draw amount of a card
    /// </summary>
    /// <param name="card">card</param>
    /// <returns>the amount of card a player should draw</returns>
    protected int GetDrawAmount(Card card)
    {
        if (card.Type == CardType.DrawTwo) return 2;
        else if (card.Type == CardType.WildFour) return 4;
        else return 0;
    }

    /// <summary>
    /// draw a certain amount of cards for a player
    /// </summary>
    /// <param name="state">game state object</param>
    /// <param name="player">ID of the player</param>
    /// <param name="amount">amount of cards to draw</param>
    /// <param name="pile">games card pile</param>
    /// <returns>updated game state and drawn cards</returns>
    protected (GameState, List<Card>) DrawCardsForPlayer(GameState state, long player, int amount, Pile pile)
    {
        List<Card> newCards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            newCards.Add(pile.DrawCard());
        }
        state.PlayerDecks[player].AddRange(newCards);
        return (state, newCards);
    }

    // TODO: add PerformBasicDraw() method as ApplyRule wrapper
}
