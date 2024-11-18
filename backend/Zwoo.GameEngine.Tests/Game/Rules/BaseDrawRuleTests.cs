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
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .ShouldTriggerRule(_rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Red, GameCardType.Zero))
         .ShouldNotTriggerRule(_rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(_rule, TestClient.RequestEndTurn());
    }

    [Test]
    public void ShouldDrawCard()
    {
        GameScenario.Create($"{_rule.Name} draws card")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 1);
    }

    [Test]
    public void ShouldDrawDrawTwo()
    {
        GameScenario.Create($"{_rule.Name} rule draws draw two")
         .WithTopCard(GameCardColor.Red, GameCardType.DrawTwo)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 2);
    }

    [Test]
    public void ShouldDrawWild()
    {
        GameScenario.Create($"{_rule.Name} rule draws draw four")
         .WithTopCard(GameCardColor.Black, GameCardType.WildFour)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 4);
    }

    [Test]
    public void ShouldHandleActivatedCards()
    {
        GameScenario.Create($"{_rule.Name} rule handles activated events")
         .WithTopCard(GameCardColor.Black, GameCardType.WildFour, true)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 1);
    }

    [Test]
    public void ShouldActivateEvent()
    {
        GameScenario.Create($"{_rule.Name} rule activated event")
         .WithTopCard(GameCardColor.Red, GameCardType.DrawTwo)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectTopCard(GameCardColor.Red, GameCardType.DrawTwo, true);
    }

    [Test]
    public void ShouldCheckActivePlayer()
    {
        GameScenario.Create($"{_rule.Name} rule accepts only active player")
         .WithTopCard(GameCardColor.Red, GameCardType.DrawTwo)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.As(1).DrawCard())
         .ExpectNoEvents();
    }

    [Test]
    public void ShouldSwitchPlayer()
    {
        GameScenario.Create($"{_rule.Name} rule switches player")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithPlayers([0, 1])
         .WithActivePlayer(0)
         .WithRule(_rule)
         .Trigger(TestClient.DrawCard())
         .ExpectActivePlayer(1);
    }

    [Test]
    public void ShouldDisplayCurrentDrawAmount()
    {
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Three);
        GameScenario.Create($"{_rule.Name} rule updates current draw amount")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == null);
    }

}