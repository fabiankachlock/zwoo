namespace Zwoo.GameEngine.Bots.JS;


internal static class ScriptConstants
{

    public static string BotScriptIdentifier = "internal--bot.js";

    /// <summary>
    /// A snipped of javascript code that sets up the bot
    /// </summary>
    public static string SetupScript = $$"""
import({{BotScriptIdentifier}}).then(module => {
    const Bot = module.default;
    const currentInstance = new Bot();
    return currentInstance;
})
""";

    /// <summary>
    /// A snipped of javascript code that injects the global variables into the module
    /// </summary>
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