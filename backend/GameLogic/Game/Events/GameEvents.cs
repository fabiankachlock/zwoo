using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Events;

public enum GameEventType
{
    StartTurn = 301,
    EndTurn = 302,
    GetCard = 306,
    RemoveCard = 307,
    StateUpdate = 308,
    GetPlayerDecission = 316,
    PlayerWon = 399
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

    // StartTurnEvent
    public struct StartTurnEvent
    {
        public readonly long Player;

        public StartTurnEvent(long player)
        {
            Player = player;
        }
    }

    public static GameEvent<StartTurnEvent> StartTurn(long player)
    {
        return new GameEvent<StartTurnEvent>(GameEventType.StartTurn, new StartTurnEvent(player));
    }

    //EndTurnEvent
    public struct EndTurnEvent
    {
        public readonly long Player;

        public EndTurnEvent(long player)
        {
            Player = player;
        }
    }

    public static GameEvent<EndTurnEvent> EndTurn(long player)
    {
        return new GameEvent<EndTurnEvent>(GameEventType.EndTurn, new EndTurnEvent(player));
    }

    // GetCardEvent
    public struct GetCardEvent
    {
        public readonly long Player;
        public readonly Card Card;

        public GetCardEvent(long player, Card card)
        {
            Player = player;
            Card = card;
        }
    }

    public static GameEvent<GetCardEvent> SendCard(long player, Card card)
    {
        return new GameEvent<GetCardEvent>(GameEventType.GetCard, new GetCardEvent(player, card));
    }

    // RemoveCardEvent
    public struct RemoveCardEvent
    {
        public readonly long Player;
        public readonly List<Card> Cards;

        public RemoveCardEvent(long player, List<Card> cards)
        {
            Player = player;
            Cards = cards;
        }
    }

    public static GameEvent<RemoveCardEvent> RemoveCard(long player, List<Card> cards)
    {
        return new GameEvent<RemoveCardEvent>(GameEventType.RemoveCard, new RemoveCardEvent(player, cards));
    }


    // StateUpdateEvent
    public struct StateUpdateEvent
    {
        public readonly Card TopCard;
        public readonly long ActivePlayer;
        public readonly int ActivePlayerCardAmount;
        public readonly long LastPlayer;
        public readonly int LastPlayerCardAmount;

        public StateUpdateEvent(Card topCard, long activePlayer, int activePlayerCardAmount, long lastPlayer, int lastPlayerCardAmount)
        {
            TopCard = topCard;
            ActivePlayer = activePlayer;
            ActivePlayerCardAmount = activePlayerCardAmount;
            LastPlayer = lastPlayer;
            LastPlayerCardAmount = lastPlayerCardAmount;
        }
    }

    public static GameEvent<StateUpdateEvent> CreateStateUpdate(Card topCard, long activePlayer, int activePlayerCardAmount, long lastPlayer, int lastPlayerCardAmount)
    {
        return new GameEvent<StateUpdateEvent>(GameEventType.StateUpdate, new StateUpdateEvent(topCard, activePlayer, activePlayerCardAmount, lastPlayer, lastPlayerCardAmount));
    }

    // PlayerDecissionEvent
    public struct PlayerDecissionEvent
    {
        public readonly long Player;
        public readonly int Decission;

        public PlayerDecissionEvent(long player, int decission)
        {
            Player = player;
            Decission = decission;
        }
    }

    public static GameEvent<PlayerDecissionEvent> GetPlayerDecission(long player, int decission)
    {
        return new GameEvent<PlayerDecissionEvent>(GameEventType.GetPlayerDecission, new PlayerDecissionEvent(player, decission));
    }

    // PlayerWonEvent
    public struct PlayerWonEvent
    {
        public readonly long Winner;
        public readonly Dictionary<long, int> Scores;

        public PlayerWonEvent(long winner, Dictionary<long, int> scores)
        {
            Winner = winner;
            Scores = scores;
        }
    }

    public static GameEvent<PlayerWonEvent> PlayerWon(long player, Dictionary<long, int> scores)
    {
        return new GameEvent<PlayerWonEvent>(GameEventType.PlayerWon, new PlayerWonEvent(player, scores));
    }

}
