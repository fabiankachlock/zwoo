using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.ZRP;

public readonly record struct PlayerJoinedDTO(
    string Username,
    int Wins
);

public readonly record struct SpectatorJoinedDTO(
    string Username,
    int Wins
);

public readonly record struct PlayerLeftDTO(string Username);

public readonly record struct LeaveGameDTO(); // empty

public readonly record struct SpectatorLeftDTO(string Username);

public readonly record struct PushMessageDTO(string Message);

public readonly record struct DistributeMessageDTO(
    string Message,
    string Username,
    ZRPRole Role
);

public readonly record struct GetAllPlayersDTO(); // empty

public readonly record struct ListPlayers_PlayerDTO(
    string Username,
    ZRPRole Role,
    int Wins
);
public readonly record struct ListPlayersDTO(ListPlayers_PlayerDTO[] Players);

public readonly record struct SpectatorWantsToPlayDTO(); // empty

public readonly record struct PlayerWantsToSpectateDTO(string Username);

public readonly record struct PromotePlayerToHostDTO(string Username);

public readonly record struct PromotedToHostDTO(); // empty

public readonly record struct HostChangedDTO(string Username);

public readonly record struct KickPlayerDTO(string Username);

public readonly record struct PlayerChangedRoleDTO(
    string Username,
    ZRPRole Role,
    int Wins
);

public readonly record struct PlayerDisconnectedDTO(string Username);

public readonly record struct PlayerReconnectedDTO(string Username);


public readonly record struct KeepAliveDTO(); // empty

public readonly record struct AckKeepAliveDTO(); // empty


public readonly record struct UpdateSettingDTO(
    string Setting,
    int Value
);

public readonly record struct ChangedSettingsDTO(
    string Setting,
    int Value
);

public readonly record struct GetAllSettingsDTO(); // empty

public readonly record struct AllSettings_SettingDTO(
    string Setting,
    int Value
);

public readonly record struct AllSettingsDTO(AllSettings_SettingDTO[] Settings);

public readonly record struct StartGameDTO(); // empty

public readonly record struct GameStartedDTO(); // empty

public readonly record struct StartTurnDTO(); // empty

public readonly record struct EndTurnDTO(); // empty

public readonly record struct RequestEndTurnDTO(); // empty

public readonly record struct PlaceCardDTO(
    int Type,
    int Symbol
);

public readonly record struct DrawCardDTO(); // empty

public readonly record struct SendCardDTO(
    CardColor Type,
    CardType Symbol
);

public readonly record struct RemoveCardDTO(
    CardColor Type,
    CardType Symbol
);

public readonly record struct StateUpdated_PileTopDTO(
    CardColor Type,
    CardType Symbol
);

public readonly record struct StateUpdatedDTO(
    StateUpdated_PileTopDTO PileTop,
    string ActivePlayer,
    int ActivePlayerCardAmount,
    string LastPlayer,
    int LastPlayerCardAmount
);

public readonly record struct GetHandDTO(); // empty

public readonly record struct SendHand_HandDTO(
    CardColor Type,
    CardType Symbol
);

public readonly record struct SendHandDTO(SendHand_HandDTO[] Hand);

public readonly record struct GetCardAmountDTO(); // empty

public readonly record struct SendCardAmount_PlayersDTO(
    string Username,
    int Cards,
    int Order,
    bool IsActivePlayer
);

public readonly record struct SendCardAmountDTO(SendCardAmount_PlayersDTO[] Players);

public readonly record struct GetPileTopDTO(); // empty

public readonly record struct SendPileTopDTO(
    CardColor Type,
    CardType Symbol
);

public readonly record struct GetPlayerDecisionDTO(int Type);

public readonly record struct ReceiveDecisionDTO(
    int Type,
    int Decision
);

public readonly record struct PlayerWon_SummaryDTO(
    string Username,
    int Position,
    int Score
);

public readonly record struct PlayerWonDTO(
    string Username,
    int Wins,
    PlayerWon_SummaryDTO[] Summary
);

public readonly record struct ErrorDTO(
    int Code,
    string Message
);

public readonly record struct AccessDeniedErrorDTO(
    int Code,
    string Message
);

public readonly record struct EndTurnErrorDTO(
    int Code,
    string Message
);

public readonly record struct PlaceCardErrorDTO(
    int Code,
    string Message
);