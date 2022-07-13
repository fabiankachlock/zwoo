﻿using System;
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

public struct GameEvent
{
    public readonly GameEventType Type;
    public readonly object Payload;

    private GameEvent(GameEventType type, object payload)
    {
        Type = type;
        Payload = payload;
    }

    public T CastPayload<T>()
    {
        return (T)Payload;
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

    public static GameEvent StartTurn(long player)
    {
        return new GameEvent(GameEventType.StartTurn, new StartTurnEvent(player));
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

    public static GameEvent EndTurn(long player)
    {
        return new GameEvent(GameEventType.EndTurn, new EndTurnEvent(player));
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

    public static GameEvent SendCard(long player, Card card)
    {
        return new GameEvent(GameEventType.GetCard, new GetCardEvent(player, card));
    }

    // RemoveCardEvent
    public struct RemoveCardEvent
    {
        public readonly long Player;
        public readonly Card Card;

        public RemoveCardEvent(long player, Card card)
        {
            Player = player;
            Card = card;
        }
    }

    public static GameEvent RemoveCard(long player, Card card)
    {
        return new GameEvent(GameEventType.RemoveCard, new RemoveCardEvent(player, card));
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

    public static GameEvent CreateStateUpdate(Card topCard, long activePlayer, int activePlayerCardAmount, long lastPlayer, int lastPlayerCardAmount)
    {
        return new GameEvent(GameEventType.StateUpdate, new StateUpdateEvent(topCard, activePlayer, activePlayerCardAmount, lastPlayer, lastPlayerCardAmount));
    }

    // PlayerDecissionEvent
    public struct PlayerDecissionEvent
    {
        public readonly long Player;
        public readonly PlayerDecission Decission;

        public PlayerDecissionEvent(long player, PlayerDecission decission)
        {
            Player = player;
            Decission = decission;
        }
    }

    public static GameEvent GetPlayerDecission(long player, PlayerDecission decission)
    {
        return new GameEvent(GameEventType.GetPlayerDecission, new PlayerDecissionEvent(player, decission));
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

    public static GameEvent PlayerWon(long player, Dictionary<long, int> scores)
    {
        return new GameEvent(GameEventType.PlayerWon, new PlayerWonEvent(player, scores));
    }

}