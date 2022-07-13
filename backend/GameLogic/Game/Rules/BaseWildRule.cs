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

internal class BaseWildCardRule : BaseCardRule
{
    public new readonly int Priority = RulePriorirty.BaseRule;

    public new readonly string Name = "BaseWildCardRule";

    public new readonly GameSettingsKey? AssociatedOption = GameSettingsKey.DEFAULT_RULE_SET;

    private record struct StoredEvent(long Player, Card Card);

    private StoredEvent? _storedEvent = null;

    public BaseWildCardRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return (gameEvent.Type == ClientEventType.PlaceCard && CardUtilities.IsWild(gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card))
            || (gameEvent.Type == ClientEventType.SendPlayerDecission && gameEvent.CastPayload<GameEvent.PlayerDecissionEvent>().Decission == PlayerDecission.SelectColor);
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new List<GameEvent>();

        if (gameEvent.Type == ClientEventType.PlaceCard)
        {
            ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
            bool isAllowed = CanThrowCard(state.TopCard.Card, payload.Card) && IsActivePlayer(state, payload.Player);
            if (isAllowed)
            {
                // TODO: enum for decisions;
                _storedEvent = new StoredEvent(payload.Player, payload.Card);
                events.Add(GameEvent.GetPlayerDecission(state.CurrentPlayer, PlayerDecission.SelectColor));
                return new GameStateUpdate(state, events);
            }
            else
            {
                // TODO: send error
            }
        }
        else
        {
            ClientEvent.PlayerDecissionEvent payload = gameEvent.CastPayload<ClientEvent.PlayerDecissionEvent>();
            if (_storedEvent.HasValue && _storedEvent?.Player == payload.Player)
            {
                Card newCard = new Card((CardColor)payload.Value, _storedEvent.Value.Card.Type);
                state.PlayerDecks[payload.Player].Remove(_storedEvent.Value.Card);
                state = PlaceCardOnStack(state, newCard);
                (state, events) = ChangeActivePlayer(state, playerOrder.Next());
                events.Add(GameEvent.RemoveCard(payload.Player, _storedEvent.Value.Card));
                _storedEvent = null;
                return new GameStateUpdate(state, events);
            }
        }
        return GameStateUpdate.None(state);
    }
}