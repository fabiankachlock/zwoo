using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Bots.State;

/// <summary>
/// a bot state manager managing only the bare minimum state need for the bot to make decisions
/// </summary>
public class BasicBotStateManager
{

    /// <summary>
    /// the state object
    /// </summary>
    public struct BotState
    {
        /// <summary>
        /// a list of all cards the bot has currently
        /// </summary>
        public List<Card> Deck = new List<Card>();

        /// <summary>
        /// the card thats on top of the current stack
        /// </summary>
        public Card StackTop = new Card();

        /// <summary>
        /// whether the bot is the active player or not
        /// </summary>
        public bool IsActive = false;

        public BotState() { }
    }

    /// <summary>
    /// the current state
    /// </summary>
    private BotState _state;

    public BasicBotStateManager()
    {
        _state = new BotState();
    }

    /// <summary>
    /// create a snapshot of the current state
    /// </summary>
    /// <returns>the current state</returns>
    public BotState GetState()
    {
        return _state;
    }

    /// <summary>
    /// handle an incoming zrp notification
    /// </summary>
    /// <param name="message">the incoming message</param>
    public void AggregateNotification(BotZRPNotification<object> message)
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
            case ZRPCode.SendHand:
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
    public void Reset()
    {
        _state = new BotState();
    }

    private void aggregateNewCards(SendCardsNotification data)
    {
        foreach (var card in data.Cards)
        {
            _state.Deck.Add(new Card(card.Type, card.Symbol));
        }
    }

    private void aggregateRemoveCard(RemoveCardNotification data)
    {
        foreach (var card in data.Cards)
        {
            int index = _state.Deck.FindIndex(c => c.Color == card.Type && c.Type == card.Symbol);
            if (index >= 0)
            {
                _state.Deck.RemoveAt(index);
            }
        }
    }

    private void aggregateSendHand(SendDeckNotification data)
    {
        _state.Deck = data.Hand.Select(card => new Card(card.Type, card.Symbol)).ToList();
    }

    private void aggregatePileTop(SendPileTopNotification data)
    {
        _state.StackTop = new Card(data.Type, data.Symbol);
    }

    private void aggregateStateUpdate(StateUpdateNotification data)
    {
        _state.StackTop = new Card(data.PileTop.Type, data.PileTop.Symbol);
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