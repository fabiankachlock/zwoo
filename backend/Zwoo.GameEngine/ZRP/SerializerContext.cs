using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zwoo.GameEngine.ZRP;

[JsonSerializable(typeof(PlayerJoinedNotification))]
[JsonSerializable(typeof(SpectatorJoinedNotification))]
[JsonSerializable(typeof(PlayerLeftNotification))]
[JsonSerializable(typeof(SpectatorLeftNotification))]
[JsonSerializable(typeof(ChatMessageEvent))]
[JsonSerializable(typeof(ChatMessageNotification))]
[JsonSerializable(typeof(LeaveEvent))]
[JsonSerializable(typeof(GetLobbyEvent))]
[JsonSerializable(typeof(GetLobby_PlayerDTO))]
[JsonSerializable(typeof(GetLobbyNotification))]
[JsonSerializable(typeof(SpectatorToPlayerEvent))]
[JsonSerializable(typeof(PlayerToSpectatorEvent))]
[JsonSerializable(typeof(PlayerToHostEvent))]
[JsonSerializable(typeof(YouAreHostNotification))]
[JsonSerializable(typeof(NewHostNotification))]
[JsonSerializable(typeof(KickPlayerEvent))]
[JsonSerializable(typeof(PlayerChangedRoleNotification))]
[JsonSerializable(typeof(PlayerDisconnectedNotification))]
[JsonSerializable(typeof(PlayerReconnectedNotification))]
[JsonSerializable(typeof(KeepAliveEvent))]
[JsonSerializable(typeof(AckKeepAliveNotification))]
[JsonSerializable(typeof(UpdateSettingEvent))]
[JsonSerializable(typeof(SettingChangedNotification))]
[JsonSerializable(typeof(GetSettingsEvent))]
[JsonSerializable(typeof(AllSettings_SettingDTO))]
[JsonSerializable(typeof(AllSettingsNotification))]
[JsonSerializable(typeof(GetAllGameProfilesEvent))]
[JsonSerializable(typeof(AllGameProfiles_ProfileDTO))]
[JsonSerializable(typeof(AllGameProfilesNotification))]
[JsonSerializable(typeof(SafeToGameProfileEvent))]
[JsonSerializable(typeof(UpdateGameProfileEvent))]
[JsonSerializable(typeof(ApplyGameProfileEvent))]
[JsonSerializable(typeof(DeleteGameProfileEvent))]
[JsonSerializable(typeof(StartGameEvent))]
[JsonSerializable(typeof(BotConfigDTO))]
[JsonSerializable(typeof(CreateBotEvent))]
[JsonSerializable(typeof(BotJoinedNotification))]
[JsonSerializable(typeof(BotLeftNotification))]
[JsonSerializable(typeof(UpdateBotEvent))]
[JsonSerializable(typeof(DeleteBotEvent))]
[JsonSerializable(typeof(GetBotsEvent))]
[JsonSerializable(typeof(AllBots_BotDTO))]
[JsonSerializable(typeof(AllBotsNotification))]
[JsonSerializable(typeof(GameStartedNotification))]
[JsonSerializable(typeof(StartTurnNotification))]
[JsonSerializable(typeof(EndTurnNotification))]
[JsonSerializable(typeof(RequestEndTurnEvent))]
[JsonSerializable(typeof(PlaceCardEvent))]
[JsonSerializable(typeof(DrawCardEvent))]
[JsonSerializable(typeof(SendCard_CardDTO))]
[JsonSerializable(typeof(SendCardsNotification))]
[JsonSerializable(typeof(RemoveCard_CardDTO))]
[JsonSerializable(typeof(RemoveCardNotification))]
[JsonSerializable(typeof(StateUpdate_PileTopDTO))]
[JsonSerializable(typeof(StateUpdate_FeedbackDTO))]
[JsonSerializable(typeof(StateUpdateNotification))]
[JsonSerializable(typeof(GetDeckEvent))]
[JsonSerializable(typeof(SendDeck_CardDTO))]
[JsonSerializable(typeof(SendDeckNotification))]
[JsonSerializable(typeof(GetPlayerStateEvent))]
[JsonSerializable(typeof(SendPlayerState_PlayerDTO))]
[JsonSerializable(typeof(SendPlayerStateNotification))]
[JsonSerializable(typeof(GetPileTopEvent))]
[JsonSerializable(typeof(SendPileTopNotification))]
[JsonSerializable(typeof(GetPlayerDecisionNotification))]
[JsonSerializable(typeof(PlayerDecisionEvent))]
[JsonSerializable(typeof(PlayerWon_PlayerSummaryDTO))]
[JsonSerializable(typeof(PlayerWonNotification))]
[JsonSerializable(typeof(Error))]
[JsonSerializable(typeof(AccessDeniedError))]
[JsonSerializable(typeof(LobbyFullError))]
[JsonSerializable(typeof(BotNameExistsError))]
[JsonSerializable(typeof(EmptyPileError))]
[JsonSerializable(typeof(PlaceCardError))]
public partial class ZRPSerializerContext : JsonSerializerContext { }