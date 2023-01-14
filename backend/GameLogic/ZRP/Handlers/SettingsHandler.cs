using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Notifications;

namespace ZwooGameLogic.ZRP.Handlers;

public class SettingsHandler : IEventHandler
{
    private INotificationAdapter _webSocketManager;

    public SettingsHandler(INotificationAdapter websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, IIncomingZRPMessage message)
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


    private void GetSettings(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            AllSettingsNotification payload = new AllSettingsNotification(context.Game.Settings.GetSettings().Select(setting => new AllSettings_SettingDTO(SettingsKeyMapper.ToString(setting.Key), setting.Value)).ToArray());
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SendAllSettings, payload);
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void UpdateSettings(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            UpdateSettingEvent payload = message.DecodePayload<UpdateSettingEvent>();

            if (context.Role != ZRPRole.Host)
            {
                _webSocketManager.SendPlayer(context.Id, ZRPCode.AccessDeniedError, new Error((int)ZRPCode.AccessDeniedError, "you are not the host"));
                return;
            }

            if (context.Game.Settings.Set(payload.Setting, payload.Value))
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SettingChanged, new SettingChangedNotification(payload.Setting, payload.Value));
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
