using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Events;

namespace Zwoo.GameEngine.Tests.Game.Rules;

public class BaseCardRuleTests
{
    private BaseCardRule rule = new();

    [Test]
    public void ShouldBeTriggered()
    {
        GameScenario.Create("triggers base card rule")
         .WithTopCard(CardColor.Red, CardType.Two)
         .ShouldTriggerRule(rule, TestClient.PlaceCard(CardColor.Red, CardType.Zero))
         .ShouldTriggerRule(rule, TestClient.PlaceCard(CardColor.Green, CardType.Five))
         .ShouldNotTriggerRule(rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(rule, TestClient.RequestEndTurn());
    }

    [Test]
    public void ShouldPlaceCardIfAllowed()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create("base card rule places card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(new StackCard(card, false));
    }

    [Test]
    public void ShouldNotPlaceCardIfDisallowed()
    {
        Card card = new Card(CardColor.Blue, CardType.Three);
        GameScenario.Create("base card rule rejects card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(CardColor.Red, CardType.Two);
    }

    [Test]
    public void ShouldCheckActivePlayer()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create("base card rule accepts only active player")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck(card)
         .Trigger(TestClient.As(1).PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldCheckPlayerHasCard()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create("base card rule checks player has card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck([])
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldRemoveCardFromDeck()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create("base card rule removes card from deck")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck([card, card])
         .Trigger(TestClient.PlaceCard(card))
         .ExpectPlayerDeck(TestClient.ID, [card]);
    }

    [Test]
    public void ShouldSwitchPlayer()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create("base card rule switches player")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithPlayersAndCards(new Dictionary<long, List<Card>> { { 0, [card] }, { 1, [] } })
         .WithActivePlayer(0)
         .WithRule(rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectActivePlayer(1);
    }

    [Test]
    public void ShouldDisplayCurrentDrawAmount()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        GameScenario.Create("base card rule updates current draw amount")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithDeck([card])
         .WithRule(rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectStateLike(s => s.Ui.CurrentDrawAmount == null);
    }

    [Test]
    public void ShouldNotAllowNoDrawCard()
    {
        Card card = new Card(CardColor.Red, CardType.DrawTwo);
        GameScenario.Create("base card rule does not allow on draw")
         .WithTopCard(CardColor.Red, CardType.DrawTwo)
         .WithDeck([card])
         .WithRule(rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectError();
    }

    [Test]
    public void ShouldAllowOnDrawOnceActivated()
    {
        Card card = new Card(CardColor.Red, CardType.Two);
        GameScenario.Create("base card rule allows on activated draw")
         .WithTopCard(CardColor.Red, CardType.DrawTwo, true)
         .WithDeck([card])
         .WithRule(rule)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(CardColor.Red, CardType.Two);
    }

}