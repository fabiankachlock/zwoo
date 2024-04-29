using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Tests.Framework;

public class EventCreator
{
    private long _playerId = 0;

    public long ID => _playerId;

    public EventCreator(long? id)
    {
        _playerId = id ?? 0;
    }

    public ClientEvent PlaceCard(CardColor c, CardType t)
    {
        return ClientEvent.PlaceCard(_playerId, new Card(c, t));
    }

    public ClientEvent PlaceCard(Card c)
    {
        return ClientEvent.PlaceCard(_playerId, c);
    }

    public ClientEvent DrawCard()
    {
        return ClientEvent.DrawCard(_playerId);
    }

    public ClientEvent PlayerDecision(PlayerDecision d, int v)
    {
        return ClientEvent.PlayerDecision(_playerId, d, v);
    }

    public ClientEvent RequestEndTurn()
    {
        return ClientEvent.RequestEndTurn(_playerId);
    }
}

public static class TestClient
{
    private static EventCreator _instance = new EventCreator(0);

    public static EventCreator As(long playerId)
    {
        return new EventCreator(playerId);
    }

    public static long ID => _instance.ID;

    public static ClientEvent PlaceCard(CardColor c, CardType t)
    {
        return _instance.PlaceCard(new Card(c, t));
    }

    public static ClientEvent PlaceCard(Card c)
    {
        return _instance.PlaceCard(c);
    }

    public static ClientEvent DrawCard()
    {
        return _instance.DrawCard();
    }

    public static ClientEvent PlayerDecision(PlayerDecision d, int v)
    {
        return _instance.PlayerDecision(d, v);
    }

    public static ClientEvent RequestEndTurn()
    {
        return _instance.RequestEndTurn();
    }
}