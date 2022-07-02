using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Events;

public enum GameEventType
{
    PlaceCard = 1,
    DrawCard = 2,
    GetPlayerDecission = 3
}

public struct GameEvent<T>
{
    public readonly GameEventType Type;
    public readonly T Payload;

    private GameEvent(GameEventType type, T payload)
    {
        Type = type;
        Payload = payload;
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

    public static GameEvent<PlaceCardEvent> PlaceCard(long player, Card card)
    {
        return new GameEvent<PlaceCardEvent>(GameEventType.PlaceCard, new PlaceCardEvent(player, card));
    }

    public struct DrawCardEvent
    {
        public readonly long Player;
        public readonly List<Card> Cards;

        public DrawCardEvent(long player, List<Card> cards)
        {
            Player = player;
            Cards = cards;
        }
    }

    public static GameEvent<DrawCardEvent> DrawCard(long player, List<Card> cards)
    {
        return new GameEvent<DrawCardEvent>(GameEventType.DrawCard, new DrawCardEvent(player, cards));
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

    public static GameEvent<PlayerDecissionEvent> PlayerDecission(long player, byte decission)
    {
        return new GameEvent<PlayerDecissionEvent>(GameEventType.GetPlayerDecission, new PlayerDecissionEvent(player, decission));
    }

}
