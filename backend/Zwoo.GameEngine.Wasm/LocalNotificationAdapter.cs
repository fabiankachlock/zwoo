using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Runtime.InteropServices.JavaScript;
using Zwoo.GameEngine.Notifications;
using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Logging;
using ZwooWasm.Logging;

namespace ZwooWasm;

[SupportedOSPlatform("browser")]
public partial class LocalNotificationAdapter : INotificationAdapter
{
    public static readonly LocalNotificationAdapter Instance = new LocalNotificationAdapter();

    private ILogger _logger = WasmLoggerFactory.Instance.CreateLogger("LocalNotificationAdapter");


    #region Interface Implementation
    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        _messageHandler(ZRPEncoder.Encode(code, payload));
        return Task.FromResult(true);
    }

    public Task<bool> DisconnectGame(long gameId)
    {
        _disconnectHandler();
        return Task.FromResult(true);
    }

    public Task<bool> DisconnectPlayer(long playerId)
    {
        _disconnectHandler();
        return Task.FromResult(true);
    }

    public Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload)
    {
        _logger.Debug($"Sending message to player {playerId} {code} ");
        if (playerId == Constants.LocalUser)
        {
            try
            {
                _messageHandler(ZRPEncoder.Encode(code, payload));
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
        return Task.FromResult(true);
    }

    #endregion Interface Implementation

    #region Javascript Adaptation

    private Action<string> _messageHandler = (string msg) => { };

    [JSExport]
    public static void OnMessage([JSMarshalAsAttribute<JSType.Function<JSType.String>>] Action<string> callback)
    {
        Instance._messageHandler = callback;
    }

    private Action _disconnectHandler = () => { };

    [JSExport]
    public static void OnDisconnect([JSMarshalAsAttribute<JSType.Function>] Action callback)
    {
        Instance._disconnectHandler = callback;
    }

    #endregion Javascript Adaptation

}