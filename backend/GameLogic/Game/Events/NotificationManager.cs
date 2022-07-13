﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Events;

public readonly record struct SendCardDTO(
    long Player,
    Card Card
);

public readonly record struct RemoveCardDTO(
    long Player,
    Card Card
);

public readonly record struct StateUpdateDTO(
    Card PileTop,
    long ActivePlayer,
    int ActivePlayerCardAmount,
    long LastPlayer,
    int LastPlayerCardAmount
);

public readonly record struct PlayerDecissionDTO(
    long Player,
    int Decission
);

public readonly record struct PlayerWonDTO(
    long Winner,
    Dictionary<long, int> Scores
);


public interface NotificationManager
{
    void StartGame();

    void StopGame();

    void StartTurn(long player);

    void EndTurn(long player);

    void SendCard(SendCardDTO data);

    void RemoveCard(RemoveCardDTO data);

    void StateUpdate(StateUpdateDTO data);

    void GetPlayerDecission(PlayerDecissionDTO data);

    void PlayerWon(PlayerWonDTO data);
}