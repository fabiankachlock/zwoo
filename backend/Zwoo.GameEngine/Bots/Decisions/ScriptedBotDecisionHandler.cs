using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Logging;
using Microsoft.ClearScript.V8;
using Zwoo.GameEngine.Bots.State;
using Zwoo.GameEngine.Game.Cards;
using Microsoft.ClearScript;
using Zwoo.GameEngine.Bots.JS;
using Zwoo.GameEngine.Game.Feedback;
using Zwoo.GameEngine.Lobby.Features;
using Zwoo.GameEngine.Game.Settings;

namespace Zwoo.GameEngine.Bots.Decisions;


public class ScriptedBotDecisionManager : IBotDecisionHandler
{

    public event IBotDecisionHandler.EventHandler OnEvent = delegate { };
    private V8ScriptEngine _engine;
    private ILogger _logger;
    private Random _rand { get; set; } = new();
    private ScriptObject _bot;

    public ScriptedBotDecisionManager(string script, ILogger logger)
    {
        var flags = V8ScriptEngineFlags.EnableDynamicModuleImports | V8ScriptEngineFlags.EnableTaskPromiseConversion;
        _logger = logger;
        _engine = new V8ScriptEngine(flags);

        var delegateWrapper = new DelegateWrapper(TriggerEvent, logger);
        _engine.DocumentSettings.Loader = new StaticModuleLoader(new Dictionary<string, string>
        {
            { ScriptConstants.BotScriptIdentifier, script },
            { "@zwoo/bots-builder/globals", ScriptConstants.InjectGlobals }
        });

        _engine.AddRestrictedHostObject("_logger", _logger);
        _engine.AddRestrictedHostObject("_rand", _rand);
        _engine.AddHostObject("_triggerEvent", delegateWrapper);
        _engine.AddHostObject("_helper", new ScriptHelper());
        _engine.AddHostType(typeof(IBotDecisionHandler.EventHandler));
        _engine.AddHostTypes(typeof(Card), typeof(CardColor), typeof(CardType));
        _engine.AddHostTypes(typeof(List<>), typeof(Dictionary<,>));
        _engine.AddHostTypes(typeof(ZRPCode), typeof(ZRPPlayerState), typeof(ZRPRole), typeof(UIFeedbackKind), typeof(UIFeedbackType), typeof(GameProfileGroup), typeof(GameSettingsType));

        _engine.AddHostTypes(
            typeof(PlayerJoinedNotification),
            typeof(SpectatorJoinedNotification),
            typeof(PlayerLeftNotification),
            typeof(SpectatorLeftNotification),
            typeof(ChatMessageEvent),
            typeof(ChatMessageNotification),
            typeof(LeaveEvent),
            typeof(GetLobbyEvent),
            typeof(GetLobby_PlayerDTO),
            typeof(GetLobbyNotification),
            typeof(SpectatorToPlayerEvent),
            typeof(PlayerToSpectatorEvent),
            typeof(PlayerToHostEvent),
            typeof(YouAreHostNotification),
            typeof(NewHostNotification),
            typeof(KickPlayerEvent),
            typeof(PlayerChangedRoleNotification),
            typeof(PlayerDisconnectedNotification),
            typeof(PlayerReconnectedNotification),
            typeof(KeepAliveEvent),
            typeof(AckKeepAliveNotification),
            typeof(UpdateSettingEvent),
            typeof(SettingChangedNotification),
            typeof(GetSettingsEvent),
            typeof(AllSettings_SettingDTO),
            typeof(AllSettingsNotification),
            typeof(GetAllGameProfilesEvent),
            typeof(AllGameProfiles_ProfileDTO),
            typeof(AllGameProfilesNotification),
            typeof(SafeToGameProfileEvent),
            typeof(UpdateGameProfileEvent),
            typeof(ApplyGameProfileEvent),
            typeof(DeleteGameProfileEvent),
            typeof(StartGameEvent),
            typeof(BotConfigDTO),
            typeof(CreateBotEvent),
            typeof(BotJoinedNotification),
            typeof(BotLeftNotification),
            typeof(UpdateBotEvent),
            typeof(DeleteBotEvent),
            typeof(GetBotsEvent),
            typeof(AllBots_BotDTO),
            typeof(AllBotsNotification),
            typeof(GameStartedNotification),
            typeof(StartTurnNotification),
            typeof(EndTurnNotification),
            typeof(RequestEndTurnEvent),
            typeof(PlaceCardEvent),
            typeof(DrawCardEvent),
            typeof(SendCard_CardDTO),
            typeof(SendCardsNotification),
            typeof(RemoveCard_CardDTO),
            typeof(RemoveCardNotification),
            typeof(StateUpdate_PileTopDTO),
            typeof(StateUpdate_FeedbackDTO),
            typeof(StateUpdateNotification),
            typeof(GetDeckEvent),
            typeof(SendDeck_CardDTO),
            typeof(SendDeckNotification),
            typeof(GetPlayerStateEvent),
            typeof(SendPlayerState_PlayerDTO),
            typeof(SendPlayerStateNotification),
            typeof(GetPileTopEvent),
            typeof(SendPileTopNotification),
            typeof(GetPlayerDecisionNotification),
            typeof(PlayerDecisionEvent),
            typeof(PlayerWon_PlayerSummaryDTO),
            typeof(PlayerWonNotification),
            typeof(Error),
            typeof(AccessDeniedError),
            typeof(LobbyFullError),
            typeof(BotNameExistsError),
            typeof(EmptyPileError),
            typeof(PlaceCardError)
        );


        dynamic currentBot = ((Task<object>)_engine.Evaluate(ScriptConstants.SetupScript)).Result;
        _bot = (ScriptObject)currentBot;
    }

    public void AggregateNotification(BotZRPNotification<object> message)
    {
        try
        {
            // use InvokeMethod to avoid trimming warnings 
            _bot.InvokeMethod("AggregateNotification", message);
        }
        catch (Exception ex)
        {
            _logger.Error("error in bot: " + ex);
        }
    }

    private void TriggerEvent(ZRPCode code, object payload)
    {
        OnEvent.Invoke(code, payload);
    }

    public void Reset()
    {
        // use InvokeMethod to avoid trimming warnings 
        _bot.InvokeMethod("Reset");
    }

    public void Dispose()
    {
        _engine.Dispose();
    }
}