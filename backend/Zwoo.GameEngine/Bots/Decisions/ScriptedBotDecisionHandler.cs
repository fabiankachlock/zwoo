using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Logging;
using Microsoft.ClearScript.V8;
using Zwoo.GameEngine.Bots.State;
using Zwoo.GameEngine.Game.Cards;
using Microsoft.ClearScript;

namespace Zwoo.GameEngine.Bots.Decisions;

internal interface ScriptedBot
{
    void AggregateNotification(BotZRPNotification<object> message);
    void Reset();
}

internal static class BotScripts
{
    public static string SetupScript = """
import("internal--bot.js").then(module => {
    const Bot = module.default;
    const currentInstance = new Bot();
    return currentInstance;
})
""";

    public static string InjectGlobals = """
var globals = { 
        logger: _logger, 
        rand: _rand, 
        triggerEvent: (code, payload) => {
            _logger.Info('!!! invoking event: ' + code);
            _triggerEvent.Invoke(code, payload); 
        },
        toInt: (value) => {
            return _helper.CastToInt(value);
        }
};
""";

    public static string DummyScriptBot = """
var s=class{};var Bot=class extends s{triggerEvent=globals.triggerEvent;state=new WholeGameBotStateManager;placedCard=-1;AggregateNotification(message){switch(globals.logger.Info("Received message: "+message),message.Code){case ZRPCode.GameStarted:this.triggerEvent(ZRPCode.GetHand,new GetDeckEvent);break;case ZRPCode.GetPlayerDecision:globals.logger.Info("making decision"),this.makeDecision(message.Payload);return;case ZRPCode.PlaceCardError:this.placeCard();return;default:this.state.AggregateNotification(message);break}var currentState=this.state.GetState();currentState.IsActive&&message.Code!=ZRPCode.StateUpdated&&(globals.logger.Info("starting turn"),this.placedCard=-1,this.placeCard())}Reset(){globals.logger.Info("Resetting bot")}placeCard(){if(globals.rand.Next(10)<1){this.triggerEvent(ZRPCode.DrawCard,new DrawCardEvent);return}var state=this.state.GetState();if(this.placedCard=this.placedCard+1,this.placedCard>=state.Deck.Count){globals.logger.Info("bailing with draw"),this.triggerEvent(ZRPCode.DrawCard,new DrawCardEvent);return}try{this.triggerEvent(ZRPCode.PlaceCard,new PlaceCardEvent(globals.toInt(state.Deck[this.placedCard].Color),globals.toInt(state.Deck[this.placedCard].Type))),state.Deck.Count==2&&globals.rand.Next(10)>4&&this.triggerEvent(ZRPCode.RequestEndTurn,new RequestEndTurnEvent)}catch(ex){globals.logger.Error("cant place card ["+this.placedCard+"]: "+ex)}}makeDecision(data){let decision=globals.rand.Next(data.Options.Count);this.triggerEvent(ZRPCode.ReceiveDecision,new PlayerDecisionEvent(data.Type,decision))}},main_default=Bot;export{Bot,main_default as default};
""";

}

class MyCustomModuleLoader : DocumentLoader
{
    private readonly Dictionary<string, string> _modules;

    public MyCustomModuleLoader(Dictionary<string, string> modules)
    {
        _modules = modules;
    }

    public override Task<Document> LoadDocumentAsync(DocumentSettings settings, DocumentInfo? sourceInfo, string specifier, DocumentCategory category, DocumentContextCallback contextCallback)
    {
        if (_modules.TryGetValue(specifier, out var content))
        {
            return Task.FromResult<Document>(
                new StringDocument(
                    new DocumentInfo(specifier)
                    {
                        Category = category,
                        ContextCallback = contextCallback
                    },
                    content
                )
            );
        }

        throw new FileNotFoundException($"Module '{specifier}' not found in predefined modules.");
    }
}

public class DelegateWrapper
{

    private readonly Action<ZRPCode, object> _action;
    private readonly ILogger _logger;

    public DelegateWrapper(Action<ZRPCode, object> action, ILogger logger)
    {
        _action = action;
        _logger = logger;
    }

    public void Invoke(ZRPCode code, object payload)
    {
        _logger.Info("??? Invoking event in wrapper: " + code);
        _action(code, payload);
        _logger.Info("??? AFTER Invoking event in wrapper: " + code);
    }
}

public class Helper
{
    public int CastToInt(dynamic value)
    {
        return (int)value;
    }
}


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
        _engine.DocumentSettings.Loader = new MyCustomModuleLoader(new Dictionary<string, string>
        {
            { "internal--bot.js", BotScripts.InjectGlobals + script },
        });
        // _engine.DocumentSettings.AddSystemDocument("internal--bot.js", BotScripts.InjectGlobals + script);

        _engine.AddRestrictedHostObject("_logger", _logger);
        _engine.AddRestrictedHostObject("_rand", _rand);
        _engine.AddHostObject("_triggerEvent", delegateWrapper);
        _engine.AddHostObject("_helper", new Helper());
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

        dynamic currentBot = ((Task<object>)_engine.Evaluate(BotScripts.SetupScript)).Result;
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