﻿using System;
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
    public new readonly int Priority = RulePriorirty.BaseRule;

    public new readonly string Name = "BaseCardRule";

    public new readonly GameSettingsKey? AssociatedOption = GameSettingsKey.DEFAULT_RULE_SET;

    public BaseCardRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.PlaceCard;
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new List<GameEvent>();

        // TODO: check if player is active && in game

        ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
        bool isAllowed = CanThrowCard(state.TopCard.Card, payload.Card) && state.CurrentPlayer == payload.Player;
        if (isAllowed && PlayerHasCard(state, payload.Player, payload.Card))
        {
            state = PlayPlayerCard(state, payload.Player, payload.Card);
            state.CurrentPlayer = playerOrder.Next();
            events.Add(GameEvent.RemoveCard(payload.Player, payload.Card));
            return new GameStateUpdate(state, events);
        }

        // TODO: may send not allowed to throw or error if not active player 
        return GameStateUpdate.None(state);
    }

    // Rule utilities
    protected bool CanThrowCard(Card top, Card newCard)
    {
        if (CardUtilities.IsWild(newCard))
        {
            return top.Type == CardType.WildFour ? newCard.Type == CardType.WildFour : true;
        }
        return top.Type == newCard.Type || top.Color == newCard.Color;
    }

    protected GameState PlaceCardOnStack(GameState state, Card card)
    {
        state.TopCard = new StackCard(card);
        state.CardStack.Add(state.TopCard);
        return state;
    }

    protected GameState PlayPlayerCard(GameState state, long player, Card card)
    {
        state.PlayerDecks[player].Remove(card);
        return PlaceCardOnStack(state, card);
    }
}
