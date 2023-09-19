using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Feedback;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Game.Rules;

internal class AddUpDrawRule : BaseRule
{
    public override int Priority
    {
        get => RulePriority.GameLogicExtendions;
    }

    public override string Name
    {
        get => "AddUpDrawRule";
    }

    public override RuleMeta? Meta => RuleMetaBuilder.New("addUpDraw")
        .IsTogglable()
        .Default(GameSettingsValue.On)
        .Localize("de", "Ziehkarten addieren", "Auf eine Ziehkarte können weitere Ziehkarten gelegt werden. Dabei wird die Anzahl der zu ziehenden Karten addiert. Es können sowohl +2 Karten auf +4 Karten, als auch +4 Karten auf +2 Karten gelegt werden.")
        .Localize("en", "Add draw amounts", "On top of a draw card, further draw cards can be placed. The number of cards to be drawn is added. You can put +2 cards on top of +4 cards, as well as +4 cards on top of +2 cards.")
        .ToMeta();

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
                    List<GameEvent> events;
                    state = PlayPlayerCard(state, payload.Player, payload.Card);
                    (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
                    int currentDrawAmount = GetRecursiveDrawAmount(state.CardStack);
                    state.Ui.CurrentDrawAmount = currentDrawAmount == 0 ? null : currentDrawAmount;
                    events.Add(GameEvent.RemoveCard(payload.Player, payload.Card));
                    return GameStateUpdate.New(state, events);
                }
            }
            else
            {
                return GameStateUpdate.NoneWithEvents(state, GameEvent.Error(payload.Player, GameError.CantPlaceCard));
            }
        }
        // override draw amount to recursive one
        var stateUpdate = PerformHandleDecission(gameEvent, state, playerOrder);
        stateUpdate.NewState.Ui.CurrentDrawAmount = GetRecursiveDrawAmount(stateUpdate.NewState.CardStack);
        return stateUpdate;
    }

    // TODO: duplicated code
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
        List<GameEvent> events;
        int amount;

        ClientEvent.DrawCardEvent payload = gameEvent.CastPayload<ClientEvent.DrawCardEvent>();


        if (!IsActivePlayer(state, payload.Player))
        {
            return GameStateUpdate.None(state);
        }

        if (CardUtilities.IsDraw(state.TopCard.Card) && !state.TopCard.EventActivated)
        {
            amount = GetRecursiveDrawAmount(state.CardStack);
            state.CardStack = ActivateCardsRecursive(state.CardStack);
            state.TopCard.ActivateEvent();
        }
        else
        {
            amount = 1;
        }


        List<Card> newCards;
        (state, newCards) = DrawCardsForPlayer(state, payload.Player, amount, cardPile);
        (state, events) = ChangeActivePlayer(state, playerOrder.Next(state.Direction));
        state.Ui.CurrentDrawAmount = null;
        events.Add(GameEvent.SendCards(payload.Player, newCards));

        return GameStateUpdate.New(state, events, UIFeedback.Individual(UIFeedbackType.PlayerHasDrawn, payload.Player).WithArg(UIFeedbackArgKey.DrawAmount, amount));
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

    protected List<StackCard> ActivateCardsRecursive(List<StackCard> stack)
    {
        for (int i = stack.Count - 1; i >= 0; i--)
        {
            if (stack[i].EventActivated || !CardUtilities.IsDraw(stack[i].Card))
            {
                break;
            }
            stack[i] = new StackCard(stack[i].Card, true);
        }
        return stack;
    }

}
