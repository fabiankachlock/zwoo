using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Events;

public readonly record struct SendCardDTO(
    long Player,
    List<Card> Cards
);

public readonly record struct RemoveCardDTO(
    long Player,
    Card Card
);

public readonly record struct StateUpdateDTO(
    Card PileTop,
    long ActivePlayer,
    Dictionary<long, int> CardAmounts,
    int? CurrentDrawAmount
);

public readonly record struct PlayerDecisionDTO(
    long Player,
    PlayerDecision Decision
);

public readonly record struct PlayerWonDTO(long Winner,
    Dictionary<long, int> Scores);

public readonly record struct ErrorDto(
    long? Player,
    GameError Error,
    string Message
);

public interface IGameEventManager
{
    void StopGame();

    void StartTurn(long player);

    void EndTurn(long player);

    void SendCard(SendCardDTO data);

    void RemoveCard(RemoveCardDTO data);

    void StateUpdate(StateUpdateDTO data);

    void GetPlayerDecision(PlayerDecisionDTO data);

    void PlayerWon(PlayerWonDTO data, GameMeta gameMeta);

    void Error(ErrorDto data);
}
