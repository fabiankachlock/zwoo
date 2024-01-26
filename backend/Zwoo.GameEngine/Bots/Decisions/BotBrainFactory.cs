using Zwoo.GameEngine.Logging;

namespace Zwoo.GameEngine.Bots.Decisions;

internal class BotBrainFactory
{
    // TODO: evaluate based on strength and co
    internal static IBotDecisionHandler CreateDecisionHandler(BotConfig config, ILogger logger)
    {
        return new BasicBotDecisionManager(logger);
    }

}