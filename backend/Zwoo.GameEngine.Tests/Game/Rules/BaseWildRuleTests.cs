using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Tests.Game.Rules;

[TestFixture(typeof(BaseWildCardRule))]
public class BaseWildRuleTests
{
    private BaseRule rule;
    private Type _ruleImplementation;

    public BaseWildRuleTests(Type ruleImplementation)
    {
        _ruleImplementation = ruleImplementation;
        rule = (BaseRule)Activator.CreateInstance(ruleImplementation)!;
    }

    [SetUp]
    public void Setup()
    {
        rule = (BaseRule)Activator.CreateInstance(_ruleImplementation)!;
    }

    [Test]
    public void ShouldBeTriggered()
    {
        GameScenario.Create($"triggers {rule.Name}")
         .WithTopCard(CardColor.Red, CardType.Two)
         .ShouldTriggerRule(rule, TestClient.PlaceCard(CardColor.Black, CardType.Wild))
         .ShouldTriggerRule(rule, TestClient.PlaceCard(CardColor.Black, CardType.WildFour))
         .ShouldTriggerRule(rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(rule, TestClient.PlayerDecision(PlayerDecision.SelectPlayer, 0))
         .ShouldNotTriggerRule(rule, TestClient.PlaceCard(CardColor.Blue, CardType.Two))
         .ShouldNotTriggerRule(rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(rule, TestClient.RequestEndTurn());
    }

    [Test]
    public void ShouldSendDecisionEvent()
    {
        Card card = new Card(CardColor.Black, CardType.Wild);
        GameScenario.Create($"{rule.Name} sends decisoon event")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
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
        Card card = new Card(CardColor.Black, CardType.Wild);
        GameScenario.Create($"{rule.Name} places card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck([card, card])
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectTopCard(CardColor.Red, CardType.Wild);
    }


    [Test]
    public void ShouldCheckActivePlayer()
    {
        Card card = new Card(CardColor.Black, CardType.Wild);
        GameScenario.Create($"{rule.Name} accepts only active player")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck(card)
         .Trigger(TestClient.As(1).PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldCheckPlayerHasCard()
    {
        Card card = new Card(CardColor.Black, CardType.Wild);
        GameScenario.Create($"{rule.Name} checks player has card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldRemoveCardFromDeck()
    {
        Card card = new Card(CardColor.Black, CardType.Wild);
        GameScenario.Create($"{rule.Name} removes card from deck")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck([card, card])
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectPlayerDeck(TestClient.ID, [card]);
    }

    [Test]
    public void ShouldSwitchPlayer()
    {
        Card card = new Card(CardColor.Black, CardType.Wild);
        GameScenario.Create($"{rule.Name} switches player")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithPlayersAndCards(new Dictionary<long, List<Card>> { { 0, [card] }, { 1, [] } })
         .WithActivePlayer(0)
         .WithRule(rule)
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectActivePlayer(1);
    }

    [Test]
    public void ShouldPassNonExistingDecisions()
    {
        GameScenario.Create($"{rule.Name} passes if no decision is active")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectNoEvents();
    }

    [Test]
    public void ShouldCheckDecisionOrigin()
    {
        Card card = new Card(CardColor.Black, CardType.Wild);
        GameScenario.Create($"{rule.Name} check decision origin")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.As(1).PlayerDecision(PlayerDecision.SelectColor, 1))
         .ExpectNoEvents();
    }

    [TestCase(CardColor.Black, CardType.Wild, null)]
    [TestCase(CardColor.Black, CardType.WildFour, 4)]
    public void ShouldDisplayCurrentDrawAmount(CardColor color, CardType type, int? amount)
    {
        var card = new Card(color, type);
        GameScenario.Create($"{rule.Name} updates current draw amount")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithDeck([card])
         .WithRule(rule)
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 1))
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == amount);
    }

    [Test]
    public void ShouldResetInternalEvent()
    {
        Card card = new Card(CardColor.Black, CardType.Wild);
        GameScenario.Create($"{rule.Name} reset internal state")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectTopCard(CardColor.Red, CardType.Wild)
         .Trigger(TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ExpectNoEvents();
    }

    [Test]
    public void ShouldRejectDisallowedCards()
    {
        Card card = new Card(CardColor.Black, CardType.WildFour);
        GameScenario.Create($"{rule.Name} reject disallowed cards")
         .WithTopCard(CardColor.Red, CardType.WildFour)
         .WithRule(rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

}