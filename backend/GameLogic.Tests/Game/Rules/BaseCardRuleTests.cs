using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Game.Rules;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Events;

namespace ZwooGameLogic.Tests.Game.Rules;

public class BaseCardRuleTests
{
    private BaseCardRule rule = new();

    [Test]
    public void ShouldBeTriggered()
    {
        var scene = GameScenario.Create("triggers base card rule")
         .WithTopCard(CardColor.Red, CardType.Two)
         .ShouldTriggerRule(rule, TestClient.PlaceCard(CardColor.Red, CardType.Zero))
         .ShouldTriggerRule(rule, TestClient.PlaceCard(CardColor.Green, CardType.Five))
         .ShouldNotTriggerRule(rule, TestClient.DrawCard())
         .ShouldNotTriggerRule(rule, TestClient.PlayerDecision(PlayerDecision.SelectColor, 0))
         .ShouldNotTriggerRule(rule, TestClient.RequestEndTurn());
    }

    [Test]
    public void ShouldPlaceCardIfAllow()
    {
        Card card = new Card(CardColor.Red, CardType.Three);
        var scene = GameScenario.Create("base card rule places card")
         .WithTopCard(CardColor.Red, CardType.Two)
         .WithRule(rule)
         .WithDeck(card)
         .Trigger(TestClient.PlaceCard(card))
         .ExpectTopCard(new StackCard(card, false));
    }
}