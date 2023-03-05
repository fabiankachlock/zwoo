using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Runtime.InteropServices.JavaScript;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;

namespace ZwooWasm;

public partial class LocalNotificationAdapter : INotificationAdapter
{
    public static readonly LocalNotificationAdapter Instance = new LocalNotificationAdapter();

    #region Interface Implementation
    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        Console.WriteLine($"sending broadcast: {code}");
        _messageHandler(ZRPEncoder.Encode(code, payload));
        return Task.FromResult(true);
    }

    public Task<bool> DisconnectGame(long gameId)
    {
        Console.WriteLine($"disconnect");
        _disconnectHandler();
        return Task.FromResult(true);
    }

    public Task<bool> DisconnectPlayer(long playerId)
    {
        Console.WriteLine($"disconnect");
        _disconnectHandler();
        return Task.FromResult(true);
    }

    public Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload)
    {
        Console.WriteLine($"sending player {playerId} {code} {JsonSerializer.Serialize(payload)}");
        if (playerId == Constants.LocalUser)
        {
            _messageHandler(ZRPEncoder.Encode(code, payload));
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