using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Tests.Game.Rules;

[TestFixture(typeof(BaseWildCardRule))]
public class BaseWildRuleTests
{
    private BaseRule _rule;
    private Type _ruleImplementation;

    public BaseWildRuleTests(Type ruleImplementation)
    {
        _ruleImplementation = ruleImplementation;
        _rule = (BaseRule)Activator.CreateInstance(ruleImplementation)!;
    }

    [SetUp]
    public void Setup()
    {
        _rule = (BaseRule)Activator.CreateInstance(_ruleImplementation)!;
    }

    [Test]
    public void ShouldBeTriggered()
    {
        GameScenario.Create($"triggers {_rule.Name}")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .ShouldTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Black, GameCardType.Wild))
         .ShouldTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Black, GameCardType.WildFour))
         .ShouldTriggerRule(_rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(_rule, TestClient.PlayerDecision(PlayerDecision.SelectPlayer, 0))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Blue, GameCardType.Two))
         .ShouldNotTriggerRule(_rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(_rule, TestClient.RequestEndTurn());
    }

    [Test]
    public void ShouldSendDecisionEvent()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.Wild);
        GameScenario.Create($"{_rule.Name} sends decisoon event")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectEventLike(e =>
         {
             var payload = e.CastPayload<GameEvent.PlayerDecisionEvent>();
             return e.Type == GameEventType.GetPlayerDecision
                && payload.Player == TestClient.ID
                && payload.Decision == PlayerDecision.SelectColor;
         });
    }

    [Test]
    public void ShouldPlaceCard()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.Wild);
        GameScenario.Create($"{_rule.Name} places card")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck([card, card])
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectTopCard(GameCardColor.Red, GameCardType.Wild);
    }


    [Test]
    public void ShouldCheckActivePlayer()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.Wild);
        GameScenario.Create($"{_rule.Name} accepts only active player")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.As(1).PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldCheckPlayerHasCard()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.Wild);
        GameScenario.Create($"{_rule.Name} checks player has card")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldRemoveCardFromDeck()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.Wild);
        GameScenario.Create($"{_rule.Name} removes card from deck")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck([card, card])
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectPlayerDeck(TestClient.ID, [card]);
    }

    [Test]
    public void ShouldSwitchPlayer()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.Wild);
        GameScenario.Create($"{_rule.Name} switches player")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithPlayersAndCards(new Dictionary<long, List<GameCard>> { { 0, [card] }, { 1, [] } })
         .WithActivePlayer(0)
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectActivePlayer(1);
    }

    [Test]
    public void ShouldPassNonExistingDecisions()
    {
        GameScenario.Create($"{_rule.Name} passes if no decision is active")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectNoEvents();
    }

    [Test]
    public void ShouldCheckDecisionOrigin()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.Wild);
        GameScenario.Create($"{_rule.Name} check decision origin")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.As(1).PlayerDecision(PlayerDecision.SelectColor, 1))
         .ExpectNoEvents();
    }

    [TestCase(GameCardColor.Black, GameCardType.Wild, null)]
    [TestCase(GameCardColor.Black, GameCardType.WildFour, 4)]
    public void ShouldDisplayCurrentDrawAmount(GameCardColor color, GameCardType type, int? amount)
    {
        var card = new GameCard(color, type);
        GameScenario.Create($"{_rule.Name} updates current draw amount")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 1))
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == amount);
    }

    [Test]
    public void ShouldResetInternalEvent()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.Wild);
        GameScenario.Create($"{_rule.Name} reset internal state")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectTopCard(GameCardColor.Red, GameCardType.Wild)
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectNoEvents();
    }

    [Test]
    public void ShouldRejectDisallowedCards()
    {
        GameCard card = new GameCard(GameCardColor.Black, GameCardType.WildFour);
        GameScenario.Create($"{_rule.Name} reject disallowed cards")
         .WithTopCard(GameCardColor.Red, GameCardType.WildFour)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

}