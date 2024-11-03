using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Logging;
using Microsoft.ClearScript.V8;
using Zwoo.GameEngine.Bots.State;
using Zwoo.GameEngine.Game.Cards;
using Microsoft.ClearScript;
using Zwoo.GameEngine.Bots.JS;

namespace Zwoo.GameEngine.Bots.Decisions;


public class ScriptedBotDecisionManager : IBotDecisionHandler
{

    public event IBotDecisionHandler.EventHandler OnEvent = delegate { };
    private V8ScriptEngine _engine;
    private ILogger _logger;
    private Random _rand { get; set; } = new();
    private ScriptObject _bot;

    public ScriptedBotDecisionManager(string script, ILogger logger)
    {
        var flags = V8ScriptEngineFlags.EnableDynamicModuleImports | V8ScriptEngineFlags.EnableTaskPromiseConversion;
        _logger = logger;
        _engine = new V8ScriptEngine(flags);

        var delegateWrapper = new DelegateWrapper(TriggerEvent, logger);
        _engine.DocumentSettings.Loader = new StaticModuleLoader(new Dictionary<string, string>
        {
            { "internal--bot.js", ScriptConstants.InjectGlobals + script },
        });

        _engine.AddRestrictedHostObject("_logger", _logger);
        _engine.AddRestrictedHostObject("_rand", _rand);
        _engine.AddHostObject("_triggerEvent", delegateWrapper);
        _engine.AddHostObject("_helper", new ScriptHelper());
        _engine.AddHostType(typeof(WholeGameBotStateManager));
        _engine.AddHostType(typeof(BasicBotStateManager));
        _engine.AddHostType(typeof(ZRPCode));
        _engine.AddHostType(typeof(IBotDecisionHandler.EventHandler));
        _engine.AddHostType(typeof(GetDeckEvent));
        _engine.AddHostType(typeof(PlayerDecisionEvent));
        _engine.AddHostType(typeof(RequestEndTurnEvent));
        _engine.AddHostType(typeof(PlaceCardEvent));
        _engine.AddHostType(typeof(DrawCardEvent));
        _engine.AddHostType(typeof(Card));
        _engine.AddHostType(typeof(CardColor));
        _engine.AddHostType(typeof(CardType));

        dynamic currentBot = ((Task<object>)_engine.Evaluate(ScriptConstants.SetupScript)).Result;
        _bot = (ScriptObject)currentBot;
    }

    public void AggregateNotification(BotZRPNotification<object> message)
    {
        _logger.Info("### Received message: " + message.Code);
        try
        {
            // use InvokeMethod to avoid trimming warnings 
            _bot.InvokeMethod("AggregateNotification", message);
        }
        catch (Exception ex)
        {
            _logger.Error("### Error in bot: " + ex);
        }
        _logger.Info("### AFTER Received message: " + message.Code);
    }

    private void TriggerEvent(ZRPCode code, object payload)
    {
        OnEvent.Invoke(code, payload);
    }

    public void Reset()
    {
        // use InvokeMethod to avoid trimming warnings 
        _bot.InvokeMethod("Reset");
    }

    public void Dispose()
    {
        _engine.Dispose();
    }
}