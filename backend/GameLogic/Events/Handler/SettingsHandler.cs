using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;


namespace ZwooGameLogic.Events.Handler;

public class SettingsHandler : IUserEventHandler
{
    private INotificationAdapter _webSocketManager;

    public SettingsHandler(INotificationAdapter websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, IIncomingEvent message)
    {
        if (message.Code == ZRPCode.GetAllSettings)
        {
            GetSettings(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.UpdateSetting)
        {
            UpdateSettings(context, message);
            return true;
        }
        return false;
    }


    private void GetSettings(UserContext context, IIncomingEvent message)
    {
        try
        {
            AllSettingsNotification payload = new AllSettingsNotification(context.Game.Settings.GetSettings().Select(s => new AllSettings_SettingDTO(s.Key, s.Value, s.Title, s.Description, s.Type, false, s.Min, s.Max)).ToArray());
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SendAllSettings, payload);
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void UpdateSettings(UserContext context, IIncomingEvent message)
    {
        try
        {
            UpdateSettingEvent payload = message.DecodePayload<UpdateSettingEvent>();

            if (context.Role != ZRPRole.Host)
            {
                _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.AccessDeniedError, new Error((int)ZRPCode.AccessDeniedError, "you are not the host"));
                return;
            }

            if (context.Game.Settings.Set(payload.Setting, payload.Value))
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SettingChanged, new SettingChangedNotification(payload.Setting, payload.Value));
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
