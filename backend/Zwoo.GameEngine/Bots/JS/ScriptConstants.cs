namespace Zwoo.GameEngine.Bots.JS;


internal static class ScriptConstants
{

    public static string BotScriptIdentifier = "internal--bot.js";

    /// <summary>
    /// A snipped of javascript code that sets up the bot
    /// </summary>
    public static string SetupScript = $$"""
const globals = { 
        logger: _logger, 
        random: _rand, 
        triggerEvent: (code, payload) => {
            _triggerEvent.Invoke(code, payload); 
        },
        helper: _helper,
};

import("{{BotScriptIdentifier}}").then(module => {
    module.default._setupEnvironment(globals);
    return new module.default();
})
""";

    public static string DummyScriptBot = """
var o={};function C(t){o=t}var n=class{static _setupEnvironment=C;triggerEvent=o.triggerEvent;logger=o.logger;helper=o.helper;random=o.random;placeCard(e){this.triggerEvent(ZRPCode.PlaceCard,new PlaceCardEvent(o.helper.toInt(e.color),o.helper.toInt(e.type)))}drawCard(){this.triggerEvent(ZRPCode.DrawCard,new DrawCardEvent)}endTurn(){this.triggerEvent(ZRPCode.RequestEndTurn,new RequestEndTurnEvent)}setChatMessage(e){this.triggerEvent(ZRPCode.CreateChatMessage,new ChatMessageEvent(e))}makeDecision(e,a){this.triggerEvent(ZRPCode.ReceiveDecision,new PlayerDecisionEvent(e,a))}requestSettings(){this.triggerEvent(ZRPCode.GetAllSettings,new GetSettingsEvent)}requestDeck(){this.triggerEvent(ZRPCode.GetHand,new GetDeckEvent)}requestPlayers(){this.triggerEvent(ZRPCode.GetCardAmount,new GetPlayerStateEvent)}requestPile(){this.triggerEvent(ZRPCode.GetPileTop,new GetPileTopEvent)}},c=(t=>(t[t.None=0]="None",t[t.Red=1]="Red",t[t.Yellow=2]="Yellow",t[t.Blue=3]="Blue",t[t.Green=4]="Green",t[t.Black=5]="Black",t))(c||{}),h=(t=>(t[t.None=0]="None",t[t.Zero=1]="Zero",t[t.One=2]="One",t[t.Two=3]="Two",t[t.Three=4]="Three",t[t.Four=5]="Four",t[t.Five=6]="Five",t[t.Six=7]="Six",t[t.Seven=8]="Seven",t[t.Eight=9]="Eight",t[t.Nine=10]="Nine",t[t.Skip=11]="Skip",t[t.Reverse=12]="Reverse",t[t.DrawTwo=13]="DrawTwo",t[t.Wild=14]="Wild",t[t.WildFour=15]="WildFour",t))(h||{}),i=class t{type;color;constructor(e,a){typeof e=="object"?"Color"in e&&"Type"in e?(this.color=e.Color,this.type=e.Type):"Type"in e&&"Symbol"in e?(this.color=e.Type,this.type=e.Symbol):"color"in e&&"type"in e?(this.color=e.color,this.type=e.type):(this.color=c.None,this.type=h.None):e&&a?(this.color=e,this.type=a):(this.color=c.None,this.type=h.None)}static equals(e,a){let r=new t(e),s=new t(a);return r.color===s.color&&r.type===s.type}equals(e){return t.equals(this,e)}},l=class{_state;constructor(){this._state={deck:[],topCard:new i,isActive:!1}}get state(){return this._state}getState(){return this.state}aggregateNotification(e){switch(e.Code){case ZRPCode.SendCards:this.aggregateNewCards(e.Payload);break;case ZRPCode.StateUpdated:this.aggregateStateUpdate(e.Payload);break;case ZRPCode.RemoveCards:this.aggregateRemoveCard(e.Payload);break;case ZRPCode.SendHand:this.aggregateSendHand(e.Payload);break;case ZRPCode.SendPileTop:this.aggregatePileTop(e.Payload);break;case ZRPCode.StartTurn:this._state.isActive=!0;break;case ZRPCode.EndTurn:this._state.isActive=!1;break}}reset(){this._state={deck:[],topCard:new i,isActive:!1}}aggregateNewCards(e){for(let a of e.Cards)this._state.deck.push(new i(a))}aggregateRemoveCard(e){for(let a of e.Cards){let r=this._state.deck.findIndex(s=>s.equals(a));r>=0&&this._state.deck.splice(r,1)}}aggregateSendHand(e){this._state.deck=[];for(let a of e.Hand)this._state.deck.push(new i(a))}aggregatePileTop(e){this._state.topCard=new i(e)}aggregateStateUpdate(e){this._state.topCard=new i(e.PileTop)}},d=class{_basicState;_players;_currentDraw;constructor(){this._basicState=new l,this._players={},this._currentDraw=void 0}get state(){return this._basicState.state}get players(){return this._players}get currentDraw(){return this._currentDraw}getState(){return this.state}aggregateNotification(e){switch(this._basicState.aggregateNotification(e),e.Code){case ZRPCode.StateUpdated:let a=e.Payload;this._currentDraw=a.CurrentDrawAmount;for(let s of a.CardAmounts)this._players[s.Key]=s.Value;break;case ZRPCode.GameStarted:let r=e.Payload;for(let s of r.Players)this._players[s.Id]=s.Cards;break}}reset(){this._basicState.reset(),this._players={},this._currentDraw=void 0}};var g=class extends n{stateManager=new d;placedCard=-1;lastTriedCard=void 0;constructor(){super()}AggregateNotification(e){this.logger.Info("Received message: "+e);let a=this.stateManager.state.isActive;switch(e.Code){case ZRPCode.GameStarted:this.requestDeck();break;case ZRPCode.GetPlayerDecision:this.logger.Info("making decision"),this.makeRandomDecision(e.Payload);return;case ZRPCode.PlaceCardError:this.selectCard();return;default:this.stateManager.aggregateNotification(e);break}var r=this.stateManager.state;!a&&r.isActive&&(this.setChatMessage("Ich bin dran!"),this.lastTriedCard=void 0),a&&!r.isActive&&(this.lastTriedCard?.type===CardType.DrawTwo||this.lastTriedCard?.type===CardType.WildFour)&&this.setChatMessage("Haha du looser ;)"),r.isActive&&e.Code!=ZRPCode.StateUpdated&&(this.logger.Info("starting turn"),this.placedCard=-1,this.selectCard())}Reset(){this.logger.Info("Resetting bot"),this.stateManager.reset()}selectCard(){if(this.random.Next(10)<1){this.drawCard();return}var e=this.stateManager.state;if(this.placedCard=this.placedCard+1,this.placedCard>=e.deck.length){this.logger.Info("bailing with draw"),this.drawCard();return}try{this.lastTriedCard=e.deck[this.placedCard],this.placeCard(e.deck[this.placedCard]),e.deck.length==2&&this.random.Next(10)>4&&this.endTurn()}catch(a){this.logger.Error("cant place card ["+this.placedCard+"]: "+a)}}makeRandomDecision(e){let a=this.random.Next(e.Options.Count);this.makeDecision(e.Type,a)}},w=g;export{g as MyBot,w as default};
""";
}