using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Feedback;

namespace Zwoo.GameEngine.Game.Rules;

public class SkipCardRule : BaseCardRule
{
    public override int Priority
    {
        get => RulePriority.DefaultRule;
    }

    public override string Name
    {
        get => "SkipCardRule";
    }

    public SkipCardRule() : base() { }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.PlaceCard && gameEvent.CastPayload<ClientEvent.PlaceCardEvent>().Card.Type == CardType.Skip;
    }


    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, IPlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = [];

        ClientEvent.PlaceCardEvent payload = gameEvent.CastPayload<ClientEvent.PlaceCardEvent>();
        bool isAllowed = IsFittingCard(state.TopCard.Card, payload.Card) && !(CardUtilities.IsDraw(state.TopCard.Card) && !state.TopCard.EventActivated);
        if (IsActivePlayer(state, payload.Player) && isAllowed && PlayerHasCard(state, payload.Player, payload.Card))
        {
            events.Add(GameEvent.EndTurn(state.CurrentPlayer));

            state = PlayPlayerCard(state, payload.Player, payload.Card);
            (state, _) = ChangeActivePlayerByAmount(state, playerOrder, 1);
            long skippedPlayer = state.CurrentPlayer;
            (state, _) = ChangeActivePlayerByAmount(state, playerOrder, 1);

            events.Add(GameEvent.StartTurn(state.CurrentPlayer));
            events.Add(GameEvent.RemoveCard(payload.Player, payload.Card));
            return GameStateUpdate.New(state, events, UIFeedback.Individual(UIFeedbackType.Skipped, skippedPlayer));
        }

        return GameStateUpdate.NoneWithEvents(state, GameEvent.Error(payload.Player, GameError.CantPlaceCard));
    }

    // Rule utilities
    /// <summary>
    /// change the current player by a certain amount of hops
    /// </summary>
    /// <param name="state">game state object</param>
    /// <param name="playerOrder">cycle of players</param>
    /// <param name="amount">amount of hops</param>
    /// <returns>the updates game state and events for the client</returns>
    protected (GameState, List<GameEvent>) ChangeActivePlayerByAmount(GameState state, IPlayerCycle playerOrder, int amount)
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
