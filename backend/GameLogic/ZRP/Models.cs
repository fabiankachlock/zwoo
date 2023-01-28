using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.ZRP;

/// <summary>
/// ZRPCode: 100 
/// </summary>
public readonly record struct PlayerJoinedNotification(string Id, string Username);

/// <summary>
/// ZRPCode: 101
/// </summary>
public readonly record struct SpectatorJoinedNotification(string Id, string Username);

/// <summary>
/// ZRPCode: 102
/// </summary>
public readonly record struct PlayerLeftNotification(string Id);

/// <summary>
/// ZRPCode: 103 
/// </summary>
public readonly record struct SpectatorLeftNotification(string Id);

/// <summary>
/// ZRPCode: 104
/// </summary>
public readonly record struct ChatMessageEvent(string Message);

/// <summary>
/// ZRPCode: 105
/// </summary>
public readonly record struct ChatMessageNotification(string Message, string Username, ZRPRole Role);

/// <summary>
/// ZRPCode: 106
/// </summary>
public readonly record struct LeaveEvent(); // empty

/// <summary>
/// ZRPCode: 108
/// @link 109
/// </summary>
public readonly record struct GetLobbyEvent(); // empty

/// <see cref="GetLobbyNotification"/>
public readonly record struct GetLobby_PlayerDTO(string Id, string Username, ZRPRole Role, ZRPPlayerState State);

/// <summary>
/// ZRPCode: 109
/// </summary>
public readonly record struct GetLobbyNotification(GetLobby_PlayerDTO[] Players);

/// <summary>
/// ZRPCode: 110
/// </summary>
public readonly record struct SpectatorToPlayerEvent(); // empty

/// <summary>
/// ZRPCode: 111
/// </summary>
public readonly record struct PlayerToSpectatorEvent(string Id);

/// <summary>
/// ZRPCode: 112
/// </summary>
public readonly record struct PlayerToHostEvent(string Id);

/// <summary>
/// ZRPCode: 113
/// </summary>
public readonly record struct YouAreHostNotification(); // empty

/// <summary>
/// ZRPCode: 114
/// </summary>
public readonly record struct NewHostNotification(string Id);

/// <summary>
/// ZRPCode: 115
/// </summary>
public readonly record struct KickPlayerEvent(string Id);

/// <summary>
/// ZRPCode: 116
/// </summary>
public readonly record struct PlayerChangedRoleNotification(string Id, ZRPRole Role);

/// <summary>
/// ZRPCode: 117
/// </summary>
public readonly record struct PlayerDisconnectedNotification(string Id);

/// <summary>
/// ZRPCode: 118
/// </summary>
public readonly record struct PlayerReconnectedNotification(string Id);

/// <summary>
/// ZRPCode: 198
/// </summary>
public readonly record struct KeepAliveEvent(); // empty

/// <summary>
/// ZRPCode: 199
/// @link 198
/// </summary>
public readonly record struct AckKeepAliveNotification(); // empty

/// <summary>
/// ZRPCode: 200
/// </summary>
public readonly record struct UpdateSettingEvent(string Setting, int Value);

/// <summary>
/// ZRPCode: 201
/// </summary>
public readonly record struct SettingChangedNotification(string Setting, int Value);

/// <summary>
/// ZRPCode: 202
/// </summary>
public readonly record struct GetSettingsEvent(); // empty

/// <see cref="AllSettingsNotification" />
public readonly record struct AllSettings_SettingDTO(string Setting, int Value);

/// <summary>
/// ZRPCode: 203
/// </summary>
public readonly record struct AllSettingsNotification(AllSettings_SettingDTO[] Settings);

/// <summary>
/// ZRPCode: 210
/// </summary>
public readonly record struct StartGameEvent(); // empty

/// <see cref="CreateBotEvent" />
/// <see cref="UpdateBotEvent" />
/// <see cref="AllBots_BotDTO" />
public readonly record struct BotConfigDTO(int Type);

/// <summary>
/// ZRPCode: 230
/// </summary>
public readonly record struct CreateBotEvent(string Username, BotConfigDTO Config);

/// <summary>
/// ZRPCode: 231
/// </summary>
public readonly record struct BotJoinedNotification(string Id, string Username);

/// <summary>
/// ZRPCode: 232
/// </summary>
public readonly record struct BotLeftNotification(string Id);

/// <summary>
/// ZRPCode: 233
/// </summary>
public readonly record struct UpdateBotEvent(string Id, BotConfigDTO Config);

/// <summary>
/// ZRPCode: 235
/// </summary>
public readonly record struct DeleteBotEvent(string Id);

/// <summary>
/// ZRPCode: 236
/// </summary>
public readonly record struct GetBotsEvent();

/// <see cref="AllBotsNotification" />
public readonly record struct AllBots_BotDTO(string Id, string Username, BotConfigDTO Config);

/// <summary>
/// ZRPCode: 237
/// </summary>
public readonly record struct AllBotsNotification(AllBots_BotDTO[] Bots);

/// <summary>
/// ZRPCode: 300
/// </summary>
public readonly record struct GameStartedNotification(); // empty

/// <summary>
/// ZRPCode: 301
/// </summary>
public readonly record struct StartTurnNotification(); // empty

/// <summary>
/// ZRPCode: 302
/// </summary>
public readonly record struct EndTurnNotification(); // empty

/// <summary>
/// ZRPCode: 304
/// </summary>
public readonly record struct PlaceCardEvent(int Type, int Symbol);

/// <summary>
/// ZRPCode: 305
/// </summary>
public readonly record struct DrawCardEvent(); // empty

/// <see cref="SendCardsNotification" />
public readonly record struct SendCard_CardDTO(CardColor Type, CardType Symbol);

/// <summary>
/// ZRPCode: 306
/// </summary>
public readonly record struct SendCardsNotification(SendCard_CardDTO[] Cards);

/// <summary>
/// ZRPCode: 307
/// </summary>
public readonly record struct RemoveCardNotification(CardColor Type, CardType Symbol);


/// <see cref="StateUpdateNotification" />
public readonly record struct StateUpdate_PileTopDTO(CardColor Type, CardType Symbol);

/// <summary>
/// ZRPCode: 308
/// </summary>
public readonly record struct StateUpdateNotification(
    StateUpdate_PileTopDTO PileTop,
    string ActivePlayer,
    int ActivePlayerCardAmount,
    string LastPlayer,
    int LastPlayerCardAmount
);

/// <summary>
/// ZRPCode: 310
/// </summary>
public readonly record struct GetDeckEvent(); // empty

/// <see cref="SendDeckNotification" />
public readonly record struct SendDeck_CardDTO(CardColor Type, CardType Symbol);

/// <summary>
/// ZRPCode: 311
/// </summary>
public readonly record struct SendDeckNotification(SendDeck_CardDTO[] Hand);

/// <summary>
/// ZRPCode: 312
/// </summary>
public readonly record struct GetPlayerStateEvent(); // empty

/// <see cref="SendPlayerStateNotification" />
public readonly record struct SendPlayerState_PlayerDTO(
    string Id,
    string Username,
    int Cards,
    int Order,
    bool IsActivePlayer
);

/// <summary>
/// ZRPCode: 313
/// </summary>
public readonly record struct SendPlayerStateNotification(SendPlayerState_PlayerDTO[] Players);

/// <summary>
/// ZRPCode: 314
/// </summary>
public readonly record struct GetPileTopEvent(); // empty

/// <summary>
/// ZRPCode: 315
/// </summary>
public readonly record struct SendPileTopNotification(CardColor Type, CardType Symbol);

/// <summary>
/// ZRPCode: 316
/// </summary>
public readonly record struct GetPlayerDecisionNotification(int Type);

/// <summary>
/// ZRPCode: 317
/// </summary>
public readonly record struct PlayerDecisionEvent(int Type, int Decision);

/// <see cref="PlayerWonNotification" />
public readonly record struct PlayerWon_PlayerSummaryDTO(
    string Id,
    int Position,
    int Score
);

/// <summary>
/// ZRPCode: 399
/// </summary>
public readonly record struct PlayerWonNotification(
    string Id,
    PlayerWon_PlayerSummaryDTO[] Summary
);

/// <summary>
/// ZRPCode: 400
/// </summary>
public readonly record struct Error(int Code, string Message);

/// <summary>
/// ZRPCode: 420
/// </summary>
public readonly record struct AccessDeniedError(int Code, string Message);

/// <summary>
/// ZRPCode: 421
/// </summary>
public readonly record struct LobbyFullError(int Code, string Message);

/// <summary>
/// ZRPCode: 434
/// </summary>
public readonly record struct PlaceCardError(int Code, string Message);

