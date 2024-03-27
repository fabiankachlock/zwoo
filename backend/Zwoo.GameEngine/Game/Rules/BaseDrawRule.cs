using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Feedback;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Cards;

namespace Zwoo.GameEngine.Game.Rules;

public class BaseDrawRule : BaseRule
{
    public override int Priority
    {
        get => RulePriority.BaseRule;
    }

    public override string Name
    {
        get => "BaseDrawRule";
    }

    public override RuleMeta? Meta => null;

    public BaseDrawRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.DrawCard;
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
        state.Ui.CurrentDrawAmount = null;
        events.Add(GameEvent.SendCards(payload.Player, newCards));

        return GameStateUpdate.New(state, events, UIFeedback.Individual(UIFeedbackType.PlayerHasDrawn, payload.Player).WithArg(UIFeedbackArgKey.DrawAmount, newCards.Count));
    }

    // Rule utilities

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
