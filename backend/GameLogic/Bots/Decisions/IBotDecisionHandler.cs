namespace ZwooGameLogic.Bots.Decisions;


public interface IBotDecisionHandler
{
    BotZRPNotification<TOut>? AggregateNotification<TIn, TOut>(BotZRPNotification<TIn> message);

    void Reset();
}