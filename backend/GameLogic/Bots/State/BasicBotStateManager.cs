using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Bots.State;

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
        internal List<Card> Deck = new List<Card>();

        /// <summary>
        /// the card thats on top of the current stack
        /// </summary>
        internal Card StackTop = new Card();

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
            case ZRPCode.SendCard:
                aggregateNewCard((SendCardDTO)message.Payload);
                break;
            case ZRPCode.StateUpdated:
                aggregateStateUpdate((StateUpdatedDTO)message.Payload);
                break;
            case ZRPCode.RemoveCard:
                aggregateRemoveCard((RemoveCardDTO)message.Payload);
                break;
            case ZRPCode.SendHand:
                aggregateSendHand((SendHandDTO)message.Payload);
                break;
            case ZRPCode.SendPileTop:
                aggregatePileTop((SendPileTopDTO)message.Payload);
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

    private void aggregateNewCard(SendCardDTO data)
    {
        _state.Deck.Add(new Card(data.Type, data.Symbol));
    }

    private void aggregateRemoveCard(RemoveCardDTO data)
    {
        int index = _state.Deck.FindIndex(c => c.Color == data.Type && c.Type == data.Symbol);
        if (index >= 0)
        {
            _state.Deck.RemoveAt(index);
        }
    }

    private void aggregateSendHand(SendHandDTO data)
    {
        _state.Deck = data.Hand.Select(card => new Card(card.Type, card.Symbol)).ToList();
    }

    private void aggregatePileTop(SendPileTopDTO data)
    {
        _state.StackTop = new Card(data.Type, data.Symbol);
    }

    private void aggregateStateUpdate(StateUpdatedDTO data)
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