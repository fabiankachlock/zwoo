namespace ZwooGameLogic.Bots.Decisions;


public interface IBotDecisionHandler
{
    BotZRPNotification<object>? AggregateNotification(BotZRPNotification<object> message);

    void Reset();
}