using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Events;

namespace Zwoo.GameEngine.Tests.Game.Rules;

[TestFixture(typeof(BaseDrawRule))]
public class BaseDrawRuleTests
{
    private BaseRule _rule;
    private Type _ruleImplementation;

    public BaseDrawRuleTests(Type ruleImplementation)
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
         .WithTopCard(CardColor.Red, CardType.Two)
         .ShouldTriggerRule(_rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(CardColor.Red, CardType.Zero))
         .ShouldNotTriggerRule(_rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(_rule, TestClient.RequestEndTurn());
    }

    [Test]
    public void ShouldDrawCard()
    {
        GameScenario.Create($"{_rule.Name} draws card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 1);
    }

    [Test]
    public void ShouldDrawDrawTwo()
    {
        GameScenario.Create($"{_rule.Name} rule draws draw two")
         .WithTopCard(CardColor.Red, CardType.DrawTwo)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 2);
    }

    [Test]
    public void ShouldDrawWild()
    {
        GameScenario.Create($"{_rule.Name} rule draws draw four")
         .WithTopCard(CardColor.Black, CardType.WildFour)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 4);
    }

    [Test]
    public void ShouldHandleActivatedCards()
    {
        GameScenario.Create($"{_rule.Name} rule handles activated events")
         .WithTopCard(CardColor.Black, CardType.WildFour, true)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 1);
    }

    [Test]
    public void ShouldActivateEvent()
    {
        GameScenario.Create($"{_rule.Name} rule activated event")
         .WithTopCard(CardColor.Red, CardType.DrawTwo)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectTopCard(CardColor.Red, CardType.DrawTwo, true);
    }

    [Test]
    public void ShouldCheckActivePlayer()
    {
        GameScenario.Create($"{_rule.Name} rule accepts only active player")
         .WithTopCard(CardColor.Red, CardType.DrawTwo)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.As(1).DrawCard())
         .ExpectNoEvents();
    }

    [Test]
    public void ShouldSwitchPlayer()
    {
        GameScenario.Create($"{_rule.Name} rule switches player")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithPlayers([0, 1])
         .WithActivePlayer(0)
         .WithRule(_rule)
         .Trigger(TestClient.DrawCard())
         .ExpectActivePlayer(1);
    }

    [Test]
    public void ShouldDisplayCurrentDrawAmount()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create($"{_rule.Name} rule updates current draw amount")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == null);
    }

}