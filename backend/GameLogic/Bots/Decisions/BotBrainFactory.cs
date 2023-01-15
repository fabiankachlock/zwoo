namespace ZwooGameLogic.Bots.Decisions;


internal class BotBrainFactory
{
    // TODO: evaluate based on strength and co
    internal static IBotDecisionHandler CreateDecisionHandler(BotConfig config)
    {
        return new BasicBotDecisionManager();
    }

}