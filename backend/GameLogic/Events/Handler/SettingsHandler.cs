using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;


namespace ZwooGameLogic.Events.Handler;

public class SettingsHandler : IUserEventHandler
{
    public Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> GetHandles()
    {
        return new Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>>() {
            { ZRPCode.GetAllSettings, GetSettings},
            { ZRPCode.UpdateSetting, UpdateSettings},
        };
    }

    private void GetSettings(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            AllSettingsNotification payload = new AllSettingsNotification(context.Game.Settings.GetSettings().Select(s => new AllSettings_SettingDTO(s.Key, s.Value, s.Title, s.Description, s.Type, false, s.Min, s.Max)).ToArray());
            websocketManager.BroadcastGame(context.GameId, ZRPCode.SendAllSettings, payload);
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void UpdateSettings(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            UpdateSettingEvent payload = message.DecodePayload<UpdateSettingEvent>();

            if (context.Role != ZRPRole.Host)
            {
                websocketManager.SendPlayer(context.LobbyId, ZRPCode.AccessDeniedError, new Error((int)ZRPCode.AccessDeniedError, "you are not the host"));
                return;
            }

            if (context.Game.Settings.Set(payload.Setting, payload.Value))
            {
                websocketManager.BroadcastGame(context.GameId, ZRPCode.SettingChanged, new SettingChangedNotification(payload.Setting, payload.Value));
            }
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
