using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Rules;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Game;

namespace ZwooGameLogic.Tests.Framework;

public static class TestClient
{
    public static ClientEvent PlaceCard(CardColor c, CardType t)
    {
        return ClientEvent.PlaceCard(0, new Card(c, t));
    }

    public static ClientEvent PlaceCard(Card c)
    {
        return ClientEvent.PlaceCard(0, c);
    }

    public static ClientEvent DrawCard()
    {
        return ClientEvent.DrawCard(0);
    }

    public static ClientEvent PlayerDecision(PlayerDecision d, int v)
    {
        return ClientEvent.PlayerDecision(0, d, v);
    }

    public static ClientEvent RequestEndTurn()
    {
        return ClientEvent.RequestEndTurn(0);
    }
}