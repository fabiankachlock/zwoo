using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Events;

public enum ClientEventType
{
    RequestEndTurn = 303,
    PlaceCard = 304,
    DrawCard = 305,
    SendPlayerDecission = 317
}

public struct ClientEvent
{
    public readonly ClientEventType Type;
    public readonly object Payload;

    private ClientEvent(ClientEventType type, object payload)
    {
        Type = type;
        Payload = payload;
    }

    public T CastPayload<T>()
    {
        return (T)Payload;
    }

    public struct RequestEndTurnEvent
    {
        public readonly long Player;

        public RequestEndTurnEvent(long player)
        {
            Player = player;
        }
    }

    public static ClientEvent RequestEndTurn(long player)
    {
        return new ClientEvent(ClientEventType.DrawCard, new RequestEndTurnEvent(player));
    }

    public struct PlaceCardEvent
    {
        public readonly long Player;
        public readonly Card Card;

        public PlaceCardEvent(long player, Card card)
        {
            Player = player;
            Card = card;
        }
    }

    public static ClientEvent PlaceCard(long player, Card card)
    {
        return new ClientEvent(ClientEventType.PlaceCard, new PlaceCardEvent(player, card));
    }

    public struct DrawCardEvent
    {
        public readonly long Player;

        public DrawCardEvent(long player)
        {
            Player = player;
        }
    }

    public static ClientEvent DrawCard(long player)
    {
        return new ClientEvent(ClientEventType.DrawCard, new DrawCardEvent(player));
    }

    public struct PlayerDecissionEvent
    {
        public readonly long Player;
        public readonly PlayerDecission Decission;
        public readonly int Value;

        public PlayerDecissionEvent(long player, PlayerDecission decission, int value)
        {
            Player = player;
            Decission = decission;
            Value = value;
        }
    }

    public static ClientEvent PlayerDecission(long player, PlayerDecission decission, int value)
    {
        return new ClientEvent(ClientEventType.SendPlayerDecission, new PlayerDecissionEvent(player, decission, value));
    }

}
