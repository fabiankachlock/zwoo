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

internal class ReverseCardRule : BaseCardRule
{
    public override int Priority
    {
        get => RulePriority.DefaultRule;
    }

    public override string Name
    {
        get => "BaseCardRule";
    }

    public ReverseCardRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.PlaceCard && gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card.Type == CardType.Reverse;
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
            state.Direction = state.Direction == GameDirection.Left ? GameDirection.Rigth : GameDirection.Left;
            (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
            events.Add(GameEvent.RemoveCard(payload.Player, payload.Card));
            return GameStateUpdate.WithEvents(state, events);
        }

        return GameStateUpdate.WithEvents(state, new List<GameEvent>() { GameEvent.Error(payload.Player, GameError.CantPlaceCard) });
    }

    // TODO: add PerformReversDirection() method as ApplyRule wrapper
}
