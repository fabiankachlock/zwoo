using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Logging;
using Microsoft.ClearScript.V8;
using Zwoo.GameEngine.Bots.State;

namespace Zwoo.GameEngine.Bots.Decisions;

internal interface ScriptedBot
{
    void AggregateNotification(BotZRPNotification<object> message);
    void Reset();
}

internal static class BotScripts
{
    public static string SetupScript = @"
var globals = { 
        logger: _logger, 
        rand: _rand, 
        onEvent: (code, payload) => {
            _logger.Info('!!! invoking event: ' + code);
            _onEvent.Invoke(code, payload); 
        },
        toInt: (value) => {
            return _helper.CastToInt(value);
        }
}; 
var _currentBot = new SetupZwooBot();";

    public static string DummyScriptBot = @"
function SetupZwooBot() {
    this.onEvent = globals.onEvent;
    this.state = new WholeGameBotStateManager();
    this.placedCard = -1;

    this.AggregateNotification = function(message) {
        globals.logger.Info('Received message: ' + message);

        switch (message.Code)
        {
            case ZRPCode.GameStarted:
                this.onEvent(ZRPCode.GetHand, new GetDeckEvent());
                break;
            case ZRPCode.GetPlayerDecision:
                globals.logger.Info('making decision');
                this.makeDecision(message.Payload);
                return;
            case ZRPCode.PlaceCardError:
                this.placeCard();
                return;
            default:
                this.state.AggregateNotification(message);
                break;
        }

        var currentState = this.state.GetState();
        if (currentState.IsActive && message.Code != ZRPCode.StateUpdated)
        {
            globals.logger.Info('starting turn');
            this.placedCard = -1;
            this.placeCard();
        }

    }

    this.Reset = function() {
        globals.logger.Info('Resetting bot');
    }

    this.placeCard = function()
    {
        if (globals.rand.Next(10) < 1)
        {
            // bad luck - be dump, just draw
            this.onEvent(ZRPCode.DrawCard, new DrawCardEvent());
            return;
        }

        var state = this.state.GetState();
        this.placedCard = this.placedCard + 1;

        if (this.placedCard >= state.Deck.Count)
        {
            globals.logger.Info('bailing with draw');
            this.onEvent(ZRPCode.DrawCard, new DrawCardEvent());
            return;
        }

        try
        {
            this.onEvent(ZRPCode.PlaceCard, new PlaceCardEvent(
                globals.toInt(state.Deck[this.placedCard].Color),
                globals.toInt(state.Deck[this.placedCard].Type)
            ));

            if (state.Deck.Count == 2 && globals.rand.Next(10) > 4)
            {
                // after placing this card only on card will be left + 50% chance to miss
                this.onEvent(ZRPCode.RequestEndTurn, new RequestEndTurnEvent());
            }
        }
        catch (ex)
        {
            globals.logger.Error('cant place card [' + this.placedCard + ']: ' + ex);
        }
    }

    this.makeDecision = function(data) {
        const decision = globals.rand.Next(data.Options.Count);
        this.onEvent(ZRPCode.ReceiveDecision, new PlayerDecisionEvent(data.Type, decision));
    }
}
";
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

    private dynamic _bot
    {
        get => _engine.Script._currentBot;
    }

    public ScriptedBotDecisionManager(string script, ILogger logger)
    {
        _logger = logger;
        _engine = new V8ScriptEngine();
        var delegateWrapper = new DelegateWrapper(TriggerEvent, logger);

        _engine.Execute(script);
        _engine.AddRestrictedHostObject("_logger", _logger);
        _engine.AddRestrictedHostObject("_rand", _rand);
        _engine.AddHostObject("_onEvent", delegateWrapper);
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

        _engine.Execute(BotScripts.SetupScript);
    }

    public void AggregateNotification(BotZRPNotification<object> message)
    {
        _logger.Info("### Received message: " + message.Code);
        try
        {
            _bot.AggregateNotification(message);
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
        _bot.Reset();
    }

    public void Dispose()
    {
        _engine.Dispose();
    }
}