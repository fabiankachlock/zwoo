namespace Zwoo.GameEngine.Bots.JS;


internal static class ScriptConstants
{

    public static string BotScriptIdentifier = "internal--bot.js";

    /// <summary>
    /// A snipped of javascript code that sets up the bot
    /// </summary>
    public static string SetupScript = $$"""
import("{{BotScriptIdentifier}}").then(module => {
    return new module.default();
})
""";

    /// <summary>
    /// A snipped of javascript code that injects the global variables into the module
    /// </summary>
    public static string InjectGlobals = """
export const globals = { 
        logger: _logger, 
        random: _rand, 
        triggerEvent: (code, payload) => {
            _triggerEvent.Invoke(code, payload); 
        },
        helper: _helper,
};
""";

    public static string DummyScriptBot = """
var i=(e=>(e[e.None=0]="None",e[e.Red=1]="Red",e[e.Yellow=2]="Yellow",e[e.Blue=3]="Blue",e[e.Green=4]="Green",e[e.Black=5]="Black",e))(i||{}),d=(e=>(e[e.None=0]="None",e[e.Zero=1]="Zero",e[e.One=2]="One",e[e.Two=3]="Two",e[e.Three=4]="Three",e[e.Four=5]="Four",e[e.Five=6]="Five",e[e.Six=7]="Six",e[e.Seven=8]="Seven",e[e.Eight=9]="Eight",e[e.Nine=10]="Nine",e[e.Skip=11]="Skip",e[e.Reverse=12]="Reverse",e[e.DrawTwo=13]="DrawTwo",e[e.Wild=14]="Wild",e[e.WildFour=15]="WildFour",e))(d||{}),r=class _r{type;color;constructor(t,a){typeof t=="object"?"Color"in t&&"Type"in t?(this.color=t.Color,this.type=t.Type):"Type"in t&&"Symbol"in t?(this.color=t.Type,this.type=t.Symbol):"color"in t&&"type"in t?(this.color=t.color,this.type=t.type):(this.color=i.None,this.type=d.None):t&&a?(this.color=t,this.type=a):(this.color=i.None,this.type=d.None)}static equals(t,a){let o=new _r(t),s=new _r(a);return o.color===s.color&&o.type===s.type}equals(t){return _r.equals(this,t)}},n=class{_state;constructor(){this._state={deck:[],topCard:new r,isActive:!1}}get state(){return this._state}getState(){return this.state}aggregateNotification(t){switch(t.Code){case ZRPCode.SendCards:this.aggregateNewCards(t.Payload);break;case ZRPCode.StateUpdated:this.aggregateStateUpdate(t.Payload);break;case ZRPCode.RemoveCards:this.aggregateRemoveCard(t.Payload);break;case ZRPCode.SendHand:this.aggregateSendHand(t.Payload);break;case ZRPCode.SendPileTop:this.aggregatePileTop(t.Payload);break;case ZRPCode.StartTurn:this._state.isActive=!0;break;case ZRPCode.EndTurn:this._state.isActive=!1;break}}reset(){this._state={deck:[],topCard:new r,isActive:!1}}aggregateNewCards(t){for(let a of t.Cards)this._state.deck.push(new r(a))}aggregateRemoveCard(t){for(let a of t.Cards){let o=this._state.deck.findIndex(s=>s.equals(a));o>=0&&this._state.deck.splice(o,1)}}aggregateSendHand(t){this._state.deck=[];for(let a of t.Hand)this._state.deck.push(new r(a))}aggregatePileTop(t){this._state.topCard=new r(t)}aggregateStateUpdate(t){this._state.topCard=new r(t.PileTop)}},h=class{_basicState;_players;_currentDraw;constructor(){this._basicState=new n,this._players={},this._currentDraw=void 0}get state(){return this._basicState.state}get players(){return this._players}get currentDraw(){return this._currentDraw}getState(){return this.state}aggregateNotification(t){switch(this._basicState.aggregateNotification(t),t.Code){case ZRPCode.StateUpdated:let a=t.Payload;this._currentDraw=a.CurrentDrawAmount;for(let s of a.CardAmounts)this._players[s.Key]=s.Value;break;case ZRPCode.GameStarted:let o=t.Payload;for(let s of o.Players)this._players[s.Id]=s.Cards;break}}reset(){this._basicState.reset(),this._players={},this._currentDraw=void 0}},l=class{};import{globals}from"@zwoo/bots-builder/globals";var MyBot=class extends l{triggerEvent=globals.triggerEvent;stateManager=new h;placedCard=-1;lastTriedCard=void 0;constructor(){super()}AggregateNotification(message){globals.logger.Info("Received message: "+message);let wasActive=this.stateManager.state.isActive;switch(message.Code){case ZRPCode.GameStarted:this.triggerEvent(ZRPCode.GetHand,new GetDeckEvent);break;case ZRPCode.GetPlayerDecision:globals.logger.Info("making decision"),this.makeDecision(message.Payload);return;case ZRPCode.PlaceCardError:this.placeCard();return;default:this.stateManager.aggregateNotification(message);break}var currentState=this.stateManager.state;!wasActive&&currentState.isActive&&this.triggerEvent(ZRPCode.CreateChatMessage,new ChatMessageEvent("Ich bin dran!!")),wasActive&&!currentState.isActive&&(this.lastTriedCard?.type===CardType.DrawTwo||this.lastTriedCard?.type===CardType.WildFour)&&this.triggerEvent(ZRPCode.CreateChatMessage,new ChatMessageEvent("Haha du looser ;)")),currentState.isActive&&message.Code!=ZRPCode.StateUpdated&&(globals.logger.Info("starting turn"),this.placedCard=-1,this.placeCard())}Reset(){globals.logger.Info("Resetting bot"),this.stateManager.reset()}placeCard(){if(globals.random.Next(10)<1){this.triggerEvent(ZRPCode.DrawCard,new DrawCardEvent);return}var state=this.stateManager.state;if(this.placedCard=this.placedCard+1,this.placedCard>=state.deck.length){globals.logger.Info("bailing with draw"),this.triggerEvent(ZRPCode.DrawCard,new DrawCardEvent);return}try{this.lastTriedCard=state.deck[this.placedCard],this.triggerEvent(ZRPCode.PlaceCard,new PlaceCardEvent(globals.helper.toInt(state.deck[this.placedCard].color),globals.helper.toInt(state.deck[this.placedCard].type))),state.deck.length==2&&globals.random.Next(10)>4&&this.triggerEvent(ZRPCode.RequestEndTurn,new RequestEndTurnEvent)}catch(ex){globals.logger.Error("cant place card ["+this.placedCard+"]: "+ex)}}makeDecision(data){let decision=globals.random.Next(data.Options.Count);this.triggerEvent(ZRPCode.ReceiveDecision,new PlayerDecisionEvent(data.Type,decision))}},main_default=MyBot;export{MyBot,main_default as default};
""";
}