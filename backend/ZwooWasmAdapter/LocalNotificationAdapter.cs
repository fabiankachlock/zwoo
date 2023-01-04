using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices.JavaScript;
using ZwooGameLogic;
using ZwooGameLogic.ZRP;

namespace ZwooWasm;

public partial class LocalNotificationAdapter : INotificationAdapter
{
    public static readonly LocalNotificationAdapter Instance = new LocalNotificationAdapter();

    #region Interface Implementation
    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        _messageHandler((int)code, payload!);
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
        _messageHandler((int)code, payload!);
        return Task.FromResult(true);
    }

    #endregion Interface Implementation

    #region Javascript Adaptation

    private Action<int, object> _messageHandler = (int code, object payload) => { };

    [JSExport]
    public static void OnMessage([JSMarshalAsAttribute<JSType.Function<JSType.Number, JSType.Any>>] Action<int, object> callback)
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