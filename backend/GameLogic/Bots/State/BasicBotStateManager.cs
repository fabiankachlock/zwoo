using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Bots.State;

internal class BasicBotStateManager
{

    internal struct BotState
    {
        internal List<Card> Deck;

        internal Card StackTop;
    }

    private BotState _state;

    internal BasicBotStateManager()
    {
        _state = new BotState();
    }

    internal BotState GetState()
    {
        return new BotState();
    }

    internal void AggregateEvent<T>(BotZRPNotification<T> message) { }

    internal void Reset() { }
}