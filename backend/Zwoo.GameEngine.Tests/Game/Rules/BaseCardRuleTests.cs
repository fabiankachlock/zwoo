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
         .WithTopCard(CardColor.Red, CardType.Two)
         .ShouldTriggerRule(_rule, TestClient.PlaceCard(CardColor.Red, CardType.Zero))
         .ShouldTriggerRule(_rule, TestClient.PlaceCard(CardColor.Green, CardType.Five))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(CardColor.Green, CardType.Wild))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(CardColor.Green, CardType.WildFour))
         .ShouldNotTriggerRule(_rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(_rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(_rule, TestClient.RequestEndTurn());
    }

    [TestCase(CardColor.Red, CardType.Two, CardColor.Red, CardType.Two)]
    [TestCase(CardColor.Red, CardType.Two, CardColor.Red, CardType.Three)]
    [TestCase(CardColor.Red, CardType.Two, CardColor.Green, CardType.Two)]
    [TestCase(CardColor.Red, CardType.Two, CardColor.Red, CardType.DrawTwo)]
    [TestCase(CardColor.Red, CardType.Two, CardColor.Red, CardType.Skip)]
    [TestCase(CardColor.Red, CardType.Two, CardColor.Red, CardType.Reverse)]
    public void ShouldPlaceCardIfAllowed(CardColor topColor, CardType topType, CardColor placeColor, CardType placeType)
    {
        Card card = new Card(placeColor, placeType);
        GameScenario.Create($"{_rule.Name} places card")
         .WithTopCard(topColor, topType)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(new StackCard(card, false));
    }

    [TestCase(CardColor.Red, CardType.Two, CardColor.Blue, CardType.DrawTwo)]
    [TestCase(CardColor.Red, CardType.Two, CardColor.Red, CardType.Wild)]
    public void ShouldNotPlaceCardIfDisallowed(CardColor topColor, CardType topType, CardColor placeColor, CardType placeType)
    {
        Card card = new Card(placeColor, placeType);
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
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create($"{_rule.Name} accepts only active player")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(_rule)
         .WithDeck(card)
         .Trigger(TestClient.As(1).PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldCheckPlayerHasCard()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create($"{_rule.Name} checks player has card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(_rule)
         .WithDeck([])
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldRemoveCardFromDeck()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create($"{_rule.Name} removes card from deck")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(_rule)
         .WithDeck([card, card])
         .Trigger(TestClient.PlaceCard(card))
         .ExpectPlayerDeck(TestClient.ID, [card]);
    }

    [Test]
    public void ShouldSwitchPlayer()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create($"{_rule.Name} switches player")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithPlayersAndCards(new Dictionary<long, List<Card>> { { 0, [card] }, { 1, [] } })
         .WithActivePlayer(0)
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectActivePlayer(1);
    }

    [TestCase(CardColor.Red, CardType.Three, null)]
    [TestCase(CardColor.Red, CardType.DrawTwo, 2)]
    [TestCase(CardColor.Red, CardType.WildFour, null)]
    public void ShouldDisplayCurrentDrawAmount(CardColor color, CardType type, int? expected)
    {
        Card card = new Card(color, type);
        GameScenario.Create($"{_rule.Name} updates current draw amount")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == expected);
    }

    [Test]
    public void ShouldNotAllowOnDrawCard()
    {
        Card card = new Card(CardColor.Red, CardType.DrawTwo);
        GameScenario.Create($"{_rule.Name} does not allow on draw")
         .WithTopCard(CardColor.Red, CardType.DrawTwo)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldAllowOnDrawOnceActivated()
    {
        Card card = new Card(CardColor.Red, CardType.Two);
        GameScenario.Create($"{_rule.Name} allows on activated draw")
         .WithTopCard(CardColor.Red, CardType.DrawTwo, true)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(CardColor.Red, CardType.Two);
    }

}