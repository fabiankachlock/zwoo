using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Settings;
using Zwoo.GameEngine.Game.Feedback;
using Zwoo.GameEngine.Lobby.Features;

namespace Zwoo.GameEngine.ZRP;

public static class Version
{
    public static readonly string CURRENT = "4.3.0";
}

/// <summary>
/// ZRPCode: 100 
/// </summary>
public record class PlayerJoinedNotification(long Id, string Username, int Wins, bool IsBot);

/// <summary>
/// ZRPCode: 101
/// </summary>
public record class SpectatorJoinedNotification(long Id, string Username);

/// <summary>
/// ZRPCode: 102
/// </summary>
public record class PlayerLeftNotification(long Id);

/// <summary>
/// ZRPCode: 103 
/// </summary>
public record class SpectatorLeftNotification(long Id);

/// <summary>
/// ZRPCode: 104
/// </summary>
public record class ChatMessageEvent(string Message);

/// <summary>
/// ZRPCode: 105
/// </summary>
public record class ChatMessageNotification(long Id, string Message);

/// <summary>
/// ZRPCode: 106
/// </summary>
public record class LeaveEvent(); // empty

/// <summary>
/// ZRPCode: 108
/// @link 109
/// </summary>
public record class GetLobbyEvent(); // empty

/// <see cref="GetLobbyNotification"/>
public record struct GetLobby_PlayerDTO(long Id, string Username, ZRPRole Role, ZRPPlayerState State, int Wins);

/// <summary>
/// ZRPCode: 109
/// </summary>
public record class GetLobbyNotification(GetLobby_PlayerDTO[] Players);

/// <summary>
/// ZRPCode: 110
/// </summary>
public record class SpectatorToPlayerEvent(); // empty

/// <summary>
/// ZRPCode: 111
/// </summary>
public record class PlayerToSpectatorEvent(long Id);

/// <summary>
/// ZRPCode: 112
/// </summary>
public record class PlayerToHostEvent(long Id);

/// <summary>
/// ZRPCode: 113
/// </summary>
public record class YouAreHostNotification(); // empty

/// <summary>
/// ZRPCode: 114
/// </summary>
public record class NewHostNotification(long Id);

/// <summary>
/// ZRPCode: 115
/// </summary>
public record class KickPlayerEvent(long Id);

/// <summary>
/// ZRPCode: 116
/// </summary>
public record class PlayerChangedRoleNotification(long Id, ZRPRole Role, int Wins);

/// <summary>
/// ZRPCode: 117
/// </summary>
public record class PlayerDisconnectedNotification(long Id);

/// <summary>
/// ZRPCode: 118
/// </summary>
public record class PlayerReconnectedNotification(long Id);

/// <summary>
/// ZRPCode: 198
/// </summary>
public record class KeepAliveEvent(); // empty

/// <summary>
/// ZRPCode: 199
/// @link 198
/// </summary>
public record class AckKeepAliveNotification(); // empty

/// <summary>
/// ZRPCode: 200
/// </summary>
public record class UpdateSettingEvent(string Setting, int Value);

/// <summary>
/// ZRPCode: 201
/// </summary>
public record class SettingChangedNotification(string Setting, int Value);

/// <summary>
/// ZRPCode: 202
/// </summary>
public record class GetSettingsEvent(); // empty

/// <see cref="AllSettingsNotification" />
public record struct AllSettings_SettingDTO(string Setting, int Value, Dictionary<string, string> Title, Dictionary<string, string> Description, GameSettingsType Type, bool IsReadonly, int? Min, int? Max);

/// <summary>
/// ZRPCode: 203
/// </summary>
public record class AllSettingsNotification(AllSettings_SettingDTO[] Settings);


/// <summary>
/// ZRPCode: 204
/// </summary>
public record class GetAllGameProfilesEvent(); // empty

/// <see cref="AllGameProfilesNotification" /> 
public record struct AllGameProfiles_ProfileDTO(string Id, string Name, GameProfileGroup Group);

/// <summary>
/// ZRPCode: 205
/// </summary>
public record class AllGameProfilesNotification(AllGameProfiles_ProfileDTO[] Profiles);

/// <summary>
/// ZRPCode: 206
/// </summary>
public record class SafeToGameProfileEvent(string Name);

/// <summary>
/// ZRPCode: 207
/// </summary>
public record class UpdateGameProfileEvent(string Id);

/// <summary>
/// ZRPCode: 208
/// </summary>
public record class ApplyGameProfileEvent(string Id);

/// <summary>
/// ZRPCode: 209
/// </summary>
public record class DeleteGameProfileEvent(string Id);

/// <summary>
/// ZRPCode: 210
/// </summary>
public record class StartGameEvent(); // empty

/// <see cref="CreateBotEvent" />
/// <see cref="UpdateBotEvent" />
/// <see cref="AllBots_BotDTO" />
public record class BotConfigDTO(int Type);

/// <summary>
/// ZRPCode: 230
/// </summary>
public record class CreateBotEvent(string Username, BotConfigDTO Config);

/// <summary>
/// ZRPCode: 231
/// </summary>
public record class BotJoinedNotification(long Id, string Username, int Wins);

/// <summary>
/// ZRPCode: 232
/// </summary>
public record class BotLeftNotification(long Id);

/// <summary>
/// ZRPCode: 233
/// </summary>
public record class UpdateBotEvent(long Id, BotConfigDTO Config);

/// <summary>
/// ZRPCode: 235
/// </summary>
public record class DeleteBotEvent(long Id);

/// <summary>
/// ZRPCode: 236
/// </summary>
public record class GetBotsEvent();

/// <see cref="AllBotsNotification" />
public record struct AllBots_BotDTO(long Id, string Username, BotConfigDTO Config, int Wins);

/// <summary>
/// ZRPCode: 237
/// </summary>
public record class AllBotsNotification(AllBots_BotDTO[] Bots);

/// <summary>
/// ZRPCode: 300
/// </summary>
public record class GameStartedNotification(SendDeck_CardDTO[] Hand, SendPlayerState_PlayerDTO[] Players, SendPileTopNotification Pile); // empty

/// <summary>
/// ZRPCode: 301
/// </summary>
public record class StartTurnNotification(); // empty

/// <summary>
/// ZRPCode: 302
/// </summary>
public record class EndTurnNotification(); // empty

/// <summary>
/// ZRPCode: 303
/// </summary>
public record class RequestEndTurnEvent(); // empty

/// <summary>
/// ZRPCode: 304
/// </summary>
public record class PlaceCardEvent(int Type, int Symbol);

/// <summary>
/// ZRPCode: 305
/// </summary>
public record class DrawCardEvent(); // empty

/// <see cref="SendCardsNotification" />
public record class SendCard_CardDTO(CardColor Type, CardType Symbol);

/// <summary>
/// ZRPCode: 306
/// </summary>
public record class SendCardsNotification(SendCard_CardDTO[] Cards);

/// <see cref="RemoveCardNotification" />
public record class RemoveCard_CardDTO(CardColor Type, CardType Symbol);

/// <summary>
/// ZRPCode: 307
/// </summary>
public record class RemoveCardNotification(RemoveCard_CardDTO[] Cards);


/// <see cref="StateUpdateNotification" />
public record class StateUpdate_PileTopDTO(CardColor Type, CardType Symbol);

/// <see cref="StateUpdateNotification" />
public record class StateUpdate_FeedbackDTO(UIFeedbackType Type, UIFeedbackKind Kind, Dictionary<string, long> Args);

/// <summary>
/// ZRPCode: 308
/// </summary>
public record class StateUpdateNotification(
    StateUpdate_PileTopDTO PileTop,
    long ActivePlayer,
    Dictionary<long, int> CardAmounts,
    List<StateUpdate_FeedbackDTO> Feedback,
    int? CurrentDrawAmount
);

/// <summary>
/// ZRPCode: 310
/// </summary>
public record class GetDeckEvent(); // empty

/// <see cref="SendDeckNotification" />
public record class SendDeck_CardDTO(CardColor Type, CardType Symbol);

/// <summary>
/// ZRPCode: 311
/// </summary>
public record class SendDeckNotification(SendDeck_CardDTO[] Hand);

/// <summary>
/// ZRPCode: 312
/// </summary>
public record class GetPlayerStateEvent(); // empty

/// <see cref="SendPlayerStateNotification" />
public record class SendPlayerState_PlayerDTO(
    long Id,
    string Username,
    int Cards,
    int Order,
    bool IsActivePlayer
);

/// <summary>
/// ZRPCode: 313
/// </summary>
public record class SendPlayerStateNotification(SendPlayerState_PlayerDTO[] Players);

/// <summary>
/// ZRPCode: 314
/// </summary>
public record class GetPileTopEvent(); // empty

/// <summary>
/// ZRPCode: 315
/// </summary>
public record class SendPileTopNotification(CardColor Type, CardType Symbol);

/// <summary>
/// ZRPCode: 316
/// </summary>
public record class GetPlayerDecisionNotification(int Type, List<string> Options);

/// <summary>
/// ZRPCode: 317
/// </summary>
public record class PlayerDecisionEvent(int Type, int Decision);

/// <see cref="PlayerWonNotification" />
public record class PlayerWon_PlayerSummaryDTO(
    long Id,
    int Position,
    int Score
);

/// <summary>
/// ZRPCode: 399
/// </summary>
public record class PlayerWonNotification(
    long Id,
    PlayerWon_PlayerSummaryDTO[] Summary
);

/// <summary>
/// ZRPCode: 400
/// </summary>
public record class Error(int Code, string Message);

/// <summary>
/// ZRPCode: 420
/// </summary>
public record class AccessDeniedError(int Code, string Message);

/// <summary>
/// ZRPCode: 421
/// </summary>
public record class LobbyFullError(int Code, string Message);

/// <summary>
/// ZRPCode: 425
/// </summary>
public record class BotNameExistsError(int Code, string Message);

/// <summary>
/// ZRPCode: 426
/// </summary>
public record class EmptyPileError(int Code, string Message);

/// <summary>
/// ZRPCode: 434
/// </summary>
public record class PlaceCardError(int Code, string Message);


