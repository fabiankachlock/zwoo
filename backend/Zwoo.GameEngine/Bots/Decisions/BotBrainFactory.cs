using Zwoo.GameEngine.Logging;

namespace Zwoo.GameEngine.Bots.Decisions;

internal class BotBrainFactory
{
    internal static IBotDecisionHandler CreateDecisionHandler(BotConfig config, ILogger logger)
    {
        return config.Type switch
        {
            -1 => new DumpBotDecisionManager(logger),
            1 => new SmartBotDecisionManager(logger),
            200 => new ScriptedBotDecisionManager(config.JSScript ?? BotScripts.DummyScriptBot, logger),
            _ => new BasicBotDecisionManager(logger),
        };
    }
}