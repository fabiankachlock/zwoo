using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Events;

namespace Zwoo.GameEngine.Tests.Game.Rules;

[TestFixture(typeof(ReverseCardRule))]
public class ReverseRuleTest
{
    private BaseRule _rule;
    private Type _ruleImplementation;

    public ReverseRuleTest(Type ruleImplementation)
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
         .ShouldTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Red, GameCardType.Reverse))
         .ShouldTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Green, GameCardType.Reverse))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Green, GameCardType.Five))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Green, GameCardType.Skip))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Green, GameCardType.Wild))
         .ShouldNotTriggerRule(_rule, TestClient.PlaceCard(GameCardColor.Green, GameCardType.WildFour))
         .ShouldNotTriggerRule(_rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(_rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(_rule, TestClient.RequestEndTurn());
    }

    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Red, GameCardType.Reverse)]
    [TestCase(GameCardColor.Red, GameCardType.Reverse, GameCardColor.Blue, GameCardType.Reverse)]
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

    [TestCase(GameCardColor.Red, GameCardType.Two, GameCardColor.Blue, GameCardType.Reverse)]
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
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Reverse);
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
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Reverse);
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
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Reverse);
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
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Reverse);
        GameScenario.Create($"{_rule.Name} switches player")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithPlayersAndCards(new Dictionary<long, List<GameCard>> { { 0, [card] }, { 1, [] } })
         .WithActivePlayer(0)
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectActivePlayer(1);
    }

    public void ShouldDisplayCurrentDrawAmount()
    {
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Reverse);
        GameScenario.Create($"{_rule.Name} updates current draw amount")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == null);
    }

    [Test]
    public void ShouldNotAllowOnDrawCard()
    {
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Reverse);
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
        GameCard card = new GameCard(GameCardColor.Red, GameCardType.Reverse);
        GameScenario.Create($"{_rule.Name} allows on activated draw")
         .WithTopCard(GameCardColor.Red, GameCardType.DrawTwo, true)
         .WithDeck([card])
         .WithRule(_rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(GameCardColor.Red, GameCardType.Reverse);
    }

    [Test]
    public void ShouldReverse()
    {
        GameScenario.Create($"{_rule.Name} reverses direction")
         .WithTopCard(GameCardColor.Red, GameCardType.Two)
         .WithPlayersAndCards(new Dictionary<long, List<GameCard>>{
            {0, [new GameCard(GameCardColor.Red, GameCardType.Three), new GameCard(GameCardColor.Red, GameCardType.Four)]},
            {1, [new GameCard(GameCardColor.Red, GameCardType.Reverse)]},
            {2, [new GameCard(GameCardColor.Red, GameCardType.Reverse)]},
         })
         .WithActivePlayer(0)
         .WithRules(_rule, new BaseCardRule())
         .Trigger(TestClient.PlaceCard(GameCardColor.Red, GameCardType.Three))
         .ExpectActivePlayer(1)
         .Trigger(TestClient.As(1).PlaceCard(GameCardColor.Red, GameCardType.Reverse))
         .ExpectActivePlayer(0)
         .Trigger(TestClient.PlaceCard(GameCardColor.Red, GameCardType.Four))
         .ExpectActivePlayer(2)
         .Trigger(TestClient.As(2).PlaceCard(GameCardColor.Red, GameCardType.Reverse))
         .ExpectActivePlayer(0);

    }

}