using System.Net.WebSockets;
using ZwooBackend.Websockets.Interfaces;
using ZwooBackend.ZRP;
using ZwooGameLogic.Game.Settings;

namespace ZwooBackend.Websockets.Handlers;

public class SettingsHandler : MessageHandler
{
    private SendableWebSocketManager _webSocketManager;

    public SettingsHandler(SendableWebSocketManager websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, ZRPMessage message)
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


    private void GetSettings(UserContext context, ZRPMessage message)
    {
        try
        {
            AllSettingsDTO payload = new AllSettingsDTO(context.GameRecord.Game.Settings.GetSettings().Select(setting => new AllSettings_SettingDTO(SettingsKeyMapper.ToString(setting.Key), setting.Value)).ToArray());
            _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.AllSettings, payload));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString())));
        }
    }

    private void UpdateSettings(UserContext context, ZRPMessage message)
    {
        try
        {
            UpdateSettingDTO payload = message.DecodePyload<UpdateSettingDTO>();

            if (context.Role != ZRPRole.Host)
            {
                _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.AccessDeniedError, new ErrorDTO((int)ZRPCode.AccessDeniedError, "you are not the host")));
                return;
            }

            if (context.GameRecord.Game.Settings.Set(payload.Setting, payload.Value))
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.ChangedSettings, new ChangedSettingsDTO(payload.Setting, payload.Value)));
            }
        }
        catch (Exception e) 
        { 
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString()))); 
        }
    }
}
