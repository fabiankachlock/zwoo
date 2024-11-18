using Zwoo.GameEngine.Game.Cards;
using Zwoo.Api.ZRP;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Bots.State;

/// <summary>
/// a bot state manager managing only the bare minimum state need for the bot to make decisions
/// </summary>
internal class BasicBotStateManager
{

    /// <summary>
    /// the state object
    /// </summary>
    internal struct BotState
    {
        /// <summary>
        /// a list of all cards the bot has currently
        /// </summary>
        internal List<GameCard> Deck = new List<GameCard>();

        /// <summary>
        /// the card thats on top of the current stack
        /// </summary>
        internal GameCard StackTop = new GameCard();

        /// <summary>
        /// whether the bot is the active player or not
        /// </summary>
        internal bool IsActive = false;

        public BotState() { }
    }

    /// <summary>
    /// the current state
    /// </summary>
    private BotState _state;

    internal BasicBotStateManager()
    {
        _state = new BotState();
    }

    /// <summary>
    /// create a snapshot of the current state
    /// </summary>
    /// <returns>the current state</returns>
    internal BotState GetState()
    {
        return _state;
    }

    /// <summary>
    /// handle an incoming zrp notification
    /// </summary>
    /// <param name="message">the incoming message</param>
    internal void AggregateNotification(BotZRPNotification<object> message)
    {
        switch (message.Code)
        {
            case ZRPCode.SendCards:
                aggregateNewCards((SendCardsNotification)message.Payload);
                break;
            case ZRPCode.StateUpdated:
                aggregateStateUpdate((StateUpdateNotification)message.Payload);
                break;
            case ZRPCode.RemoveCards:
                aggregateRemoveCard((RemoveCardNotification)message.Payload);
                break;
            case ZRPCode.SendDeck:
                aggregateSendHand((SendDeckNotification)message.Payload);
                break;
            case ZRPCode.SendPileTop:
                aggregatePileTop((SendPileTopNotification)message.Payload);
                break;
            case ZRPCode.StartTurn:
                setActive();
                break;
            case ZRPCode.EndTurn:
                setInactive();
                break;
        }
    }

    /// <summary>
    /// reset the current state
    /// </summary>
    internal void Reset()
    {
        _state = new BotState();
    }

    private void aggregateNewCards(SendCardsNotification data)
    {
        foreach (var card in data.Cards)
        {
            _state.Deck.Add(card.ToGame());
        }
    }

    private void aggregateRemoveCard(RemoveCardNotification data)
    {
        foreach (var card in data.Cards)
        {
            int index = _state.Deck.FindIndex(c => c.Color == card.Color.ToGame() && c.Type == card.Type.ToGame());
            if (index >= 0)
            {
                _state.Deck.RemoveAt(index);
            }
        }
    }

    private void aggregateSendHand(SendDeckNotification data)
    {
        _state.Deck = data.Hand.Select(card => card.ToGame()).ToList();
    }

    private void aggregatePileTop(SendPileTopNotification data)
    {
        _state.StackTop = data.Card.ToGame();
    }

    private void aggregateStateUpdate(StateUpdateNotification data)
    {
        _state.StackTop = data.PileTop.ToGame();
    }

    private void setActive()
    {
        _state.IsActive = true;
    }

    private void setInactive()
    {
        _state.IsActive = false;
    }
}