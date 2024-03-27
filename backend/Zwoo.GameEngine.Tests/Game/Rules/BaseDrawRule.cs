using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Events;

namespace Zwoo.GameEngine.Tests.Game.Rules;

public class BaseDrawRuleTests
{
    private BaseDrawRule rule = new();

    [Test]
    public void ShouldBeTriggered()
    {
        GameScenario.Create("triggers base draw rule")
         .WithTopCard(CardColor.Red, CardType.Two)
         .ShouldTriggerRule(rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(rule, TestClient.PlaceCard(CardColor.Red, CardType.Zero))
         .ShouldNotTriggerRule(rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(rule, TestClient.RequestEndTurn());
    }

    [Test]
    public void ShouldDrawCard()
    {
        GameScenario.Create("base draw rule draws card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 1);
    }

    [Test]
    public void ShouldDrawDrawTwo()
    {
        GameScenario.Create("base draw rule draws draw two")
         .WithTopCard(CardColor.Red, CardType.DrawTwo)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 2);
    }

    [Test]
    public void ShouldDrawWild()
    {
        GameScenario.Create("base draw rule draws draw four")
         .WithTopCard(CardColor.Black, CardType.WildFour)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 4);
    }

    [Test]
    public void ShouldHandleActivatedCards()
    {
        GameScenario.Create("base draw rule handles activated events")
         .WithTopCard(CardColor.Black, CardType.WildFour, true)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.PlayerDecks[0].Count == 1);
    }

    [Test]
    public void ShouldActivateEvent()
    {
        GameScenario.Create("base draw rule activated event")
         .WithTopCard(CardColor.Red, CardType.DrawTwo)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.DrawCard())
         .ExpectTopCard(CardColor.Red, CardType.DrawTwo, true);
    }

    [Test]
    public void ShouldCheckActivePlayer()
    {
        GameScenario.Create("base draw rule accepts only active player")
         .WithTopCard(CardColor.Red, CardType.DrawTwo)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.As(1).DrawCard())
         .ExpectNoEvents();
    }

    [Test]
    public void ShouldSwitchPlayer()
    {
        GameScenario.Create("base draw rule switches player")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithPlayers([0, 1])
         .WithActivePlayer(0)
         .WithRule(rule)
         .Trigger(TestClient.DrawCard())
         .ExpectActivePlayer(1);
    }

    [Test]
    public void ShouldDisplayCurrentDrawAmount()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create("base draw rule updates current draw amount")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithDeck([card])
         .WithRule(rule)
         .Trigger(TestClient.DrawCard())
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == null);
    }

}