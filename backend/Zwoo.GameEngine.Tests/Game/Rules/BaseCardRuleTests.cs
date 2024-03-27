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