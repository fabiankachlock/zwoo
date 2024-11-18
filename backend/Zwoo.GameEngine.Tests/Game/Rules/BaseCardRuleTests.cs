using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Events;

namespace Zwoo.GameEngine.Tests.Game.Rules;

[TestFixture(typeof(BaseCardRule))]
public class BaseCardRuleTests
{
    private BaseRule _rule;
    private Type _ruleImplementation;

    public BaseCardRuleTests(Type ruleImplementation)
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
         .ShouldTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Red, GameCardType.Zero))
         .ShouldTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Green, GameCardType.Five))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Green, GameCardType.Wild))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Green, GameCardType.WildFour))
         .ShouldNotTriggerRule(_rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(_rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(_rule, TestClient.RequestEndTurn());
    }

    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Red, GameCardType.Two)]
    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Red, GameCardType.Three)]
    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Green, GameCardType.Two)]
    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Red, GameCardType.DrawTwo)]
    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Red, GameCardType.Skip)]
    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Red, GameCardType.Reverse)]
    public void ShouldPlaceCardIfAllowed(GameCardColor topColor, GameCardType topType, GameCardColor placeColor, GameCardType placeType)
    {
        GameCard card = new GameCard(placeColor, placeType);
        GameScenario.Create($"{_rule.Name} places card")
         .WithTopCard(topColor, topType)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(new StackCard(card, false));
    }

    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Blue, GameCardType.DrawTwo)]
    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Red, GameCardType.Wild)]
    public void ShouldNotPlaceCardIfDisallowed(GameCardColor topColor, GameCardType topType, GameCardColor placeColor, GameCardType placeType)
    {
        GameCard card = new GameCard(placeColor, placeType);
        GameScenario.Create($"{_rule.Name} rejects card")
         .WithTopCard(topColor, topType)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectErrorOrNoOutput()
         .ExpectTopCard(topColor, topType);
    }

    [Test]
    public void ShouldCheckActivePlayer()
    {
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Three);
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
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Three);
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
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Three);
        GameScenario.Create($"{_rule.Name} removes card from deck")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithRule(_rule)
         .WithDeck([card, card])
         .Trigger(TestClient.PlaceCard(card))
         .ExpectPlayerDeck(TestClient.ID, [card]);
    }

    [Test]
    public void ShouldSwitchPlayer()
    {
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Three);
        GameScenario.Create($"{_rule.Name} switches player")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithPlayersAndCards(new Dictionary<long, List<GameCard>> { { 0, [card] }, { 1, [] } })
         .WithActivePlayer(0)
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectActivePlayer(1);
    }

    [TestCase(GameCardColor.Red, GameCardType.Three, null)]
    [TestCase(GameCardColor.Red, GameCardType.DrawTwo, 2)]
    [TestCase(GameCardColor.Red, GameCardType.WildFour, null)]
    public void ShouldDisplayCurrentDrawAmount(GameCardColor color, GameCardType type, int? expected)
    {
        GameCard card = new GameCard(color, type);
        GameScenario.Create($"{_rule.Name} updates current draw amount")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == expected);
    }

    [Test]
    public void ShouldNotAllowOnDrawCard()
    {
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.DrawTwo);
        GameScenario.Create($"{_rule.Name} does not allow on draw")
         .WithTopCard(GameCardColor.Red, GameCardType.DrawTwo)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldAllowOnDrawOnceActivated()
    {
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Two);
        GameScenario.Create($"{_rule.Name} allows on activated draw")
         .WithTopCard(GameCardColor.Red, GameCardType.DrawTwo, true)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(GameCardColor.Red, GameCardType.Two);
    }

}