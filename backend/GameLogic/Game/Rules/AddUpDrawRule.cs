using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Game.Rules;

internal class AddUpDrawRule : BaseRule
{
    public override int Priority
    {
        get => RulePriorirty.GameLogicExtendions;
    }

    public override string Name
    {
        get => "AddUpDrawRule";
    }

    public override GameSettingsKey? AssociatedOption
    {
        get => GameSettingsKey.AddUpDraw;
    }

    private BaseRule _placeCardRule;
    private BaseRule _drawRule;

    public AddUpDrawRule() : base()
    {
        _placeCardRule = new AddUpDrawRule_PlaceCard();
        _drawRule = new AddUpDrawRule_Draw();
    }

    public override bool IsResponsible(ClientEvent clientEvent, GameState state)
    {
        return _placeCardRule.IsResponsible(clientEvent, state) || _drawRule.IsResponsible(clientEvent, state);
    }

    public override GameStateUpdate ApplyRule(ClientEvent clientEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (_placeCardRule.IsResponsible(clientEvent, state))
        {
            return _placeCardRule.ApplyRule(clientEvent, state, cardPile, playerOrder);
        }
        if (_drawRule.IsResponsible(clientEvent, state))
        {
            return _drawRule.ApplyRule(clientEvent, state, cardPile, playerOrder);
        }
        return GameStateUpdate.None(state);
    }
}

internal class AddUpDrawRule_PlaceCard : BaseWildCardRule
{
    public override string Name
    {
        get => "AddUpDrawRule_PlaceCard";
    }

    public AddUpDrawRule_PlaceCard() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return (gameEvent.Type == ClientEventType.PlaceCard
            && CardUtilities.IsDraw(gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card)
        ) || base.IsResponsible(gameEvent, state);
    }

    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);

        if (gameEvent.Type == ClientEventType.PlaceCard)
        {
            ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
            bool isNormalWild = CardUtilities.IsWild(payload.Card) && !CardUtilities.IsDraw(payload.Card);
            bool isAllowed = isNormalWild ? IsAllowedCard(state.TopCard, payload.Card) : IsFittingCard(state.TopCard.Card, payload.Card);
            if (IsActivePlayer(state, payload.Player) && isAllowed && PlayerHasCard(state, payload.Player, payload.Card))
            {
                if (CardUtilities.IsWild(payload.Card))
                {
                    return PerformHandlePlaceWild(gameEvent, state);
                }
                else
                {
                    List<GameEvent> events = new List<GameEvent>();
                    state = PlayPlayerCard(state, payload.Player, payload.Card);
                    (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
                    events.Add(GameEvent.RemoveCard(payload.Player, payload.Card));
                    return new GameStateUpdate(state, events);
                }
            }
            else
            {
                return GameStateUpdate.WithEvents(state, new List<GameEvent>() { GameEvent.Error(payload.Player, GameError.CantPlaceCard) });
            }
        }
        return PerformHandleDecission(gameEvent, state, playerOrder);
    }
}

internal class AddUpDrawRule_Draw : BaseDrawRule
{
    public override string Name
    {
        get => "AddUpDrawRule_Draw";
    }

    public AddUpDrawRule_Draw() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.DrawCard && CardUtilities.IsDraw(state.TopCard.Card);
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
            amount = GetRecursiveDrawAmount(state.CardStack);
            state.CardStack = ActiveCardsRecursive(state.CardStack);
            state.TopCard.ActivateEvent();
        }
        else
        {
            amount = 1;
        }


        List<Card> newCards;
        (state, newCards) = DrawCardsForPlayer(state, payload.Player, amount, cardPile);
        (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
        foreach (Card card in newCards)
        {
            events.Add(GameEvent.SendCard(payload.Player, card));
        }


        return new GameStateUpdate(state, events);
    }

    protected int GetRecursiveDrawAmount(List<StackCard> stack)
    {
        int amount = 0;
        for (int i = stack.Count - 1; i >= 0; i--)
        {
            if (stack[i].EventActivated || !CardUtilities.IsDraw(stack[i].Card))
            {
                break;
            }
            amount += GetDrawAmount(stack[i].Card);
        }
        return amount;
    }

    protected List<StackCard> ActiveCardsRecursive(List<StackCard> stack)
    {
        for (int i = stack.Count - 1; i >= 0; i--)
        {
            if (stack[i].EventActivated || !CardUtilities.IsDraw(stack[i].Card))
            {
                break;
            }
            stack[i].ActivateEvent();
        }
        return stack;
    }

}
