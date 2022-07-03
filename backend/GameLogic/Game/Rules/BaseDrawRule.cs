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
    public new readonly int Priority = RulePriorirty.BaseRule;

    public new readonly string Name = "BaseDrawRule";

    public new readonly GameSettingsKey? AssociatedOption = GameSettingsKey.DEFAULT_RULE_SET;

    public BaseDrawRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.DrawCard;
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new List<GameEvent>();

        // TODO: check if player is active && in game
        int amount = 0;
        ClientEvent.DrawCardEvent payload = gameEvent.CastPayload<ClientEvent.DrawCardEvent>();
        if (CardUtilities.IsDraw(state.TopCard.Card) && !state.TopCard.EventActivated)
        {
            amount = GetDrawAmount(state.TopCard.Card);
        }
        else
        {
            amount = 1;
        }

        List<Card> newCards;
        (state, newCards) = DrawCardForPlayer(state, payload.Player, amount, cardPile);
        state.CurrentPlayer = playerOrder.Next();
        foreach (Card card in newCards)
        {
            events.Add(GameEvent.SendCard(payload.Player, card));
        }


        // TODO: may send not allowed to throw or error if not active player 
        return new GameStateUpdate(state, events);
    }

    // Rule utilities
    protected int GetDrawAmount(Card card)
    {
        if (card.Type == CardType.DrawTwo) return 2;
        else if (card.Type == CardType.WildFour) return 4;
        else return 0;
    }

    protected (GameState, List<Card>) DrawCardForPlayer(GameState state, long player, int amount, Pile pile)
    {
        List<Card> newCards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            newCards.Add(pile.DrawCard());
        }
        state.PlayerDecks[player].AddRange(newCards);
        return (state, newCards);
    }
}
