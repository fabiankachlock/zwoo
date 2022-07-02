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

public struct ClientEvent<T>
{
    public readonly ClientEventType Type;
    public readonly T Payload;

    private ClientEvent(ClientEventType type, T payload)
    {
        Type = type;
        Payload = payload;
    }

    public struct RequestEndTurnEvent
    {
        public readonly long Player;

        public RequestEndTurnEvent(long player)
        {
            Player = player;
        }
    }

    public static ClientEvent<RequestEndTurnEvent> RequestEndTurn(long player)
    {
        return new ClientEvent<RequestEndTurnEvent>(ClientEventType.DrawCard, new RequestEndTurnEvent(player));
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

    public static ClientEvent<PlaceCardEvent> PlaceCard(long player, Card card)
    {
        return new ClientEvent<PlaceCardEvent>(ClientEventType.PlaceCard, new PlaceCardEvent(player, card));
    }

    public struct DrawCardEvent
    {
        public readonly long Player;

        public DrawCardEvent(long player)
        {
            Player = player;
        }
    }

    public static ClientEvent<DrawCardEvent> DrawCard(long player)
    {
        return new ClientEvent<DrawCardEvent>(ClientEventType.DrawCard, new DrawCardEvent(player));
    }

    public struct PlayerDecissionEvent
    {
        public readonly long Player;
        public readonly byte Decission;

        public PlayerDecissionEvent(long player, byte decission)
        {
            Player = player;
            Decission = decission;
        }
    }

    public static ClientEvent<PlayerDecissionEvent> PlayerDecission(long player, byte decission)
    {
        return new ClientEvent<PlayerDecissionEvent>(ClientEventType.SendPlayerDecission, new PlayerDecissionEvent(player, decission));
    }

}
