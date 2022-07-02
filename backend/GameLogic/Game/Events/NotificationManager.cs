using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Events;

public readonly record struct StateUpdateDTO(
    Card PileTop,
    long ActivePlayer,
    int ActivePlayerCardAmount,
    long LastPlayer,
    int LastPlayerCardAmount
);

public readonly record struct PlayerDecissionDTO(
    long Player,
    byte Decission
);

public readonly record struct DrawCardsDTO(
    long Player,
    List<Card> Cards
);

public interface NotificationManager
{
    void StartGame();

    void StopGame();

    void StateUpdate(StateUpdateDTO data);

    void SendGetDecission(PlayerDecissionDTO data);

    void SendDrawCards(DrawCardsDTO data);

    void SendStartTurn(long player);

    void SendEndTurn(long player);
}
