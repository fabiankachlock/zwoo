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
        _messageHandler((int)code, JsonSerializer.Serialize(payload, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
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
            _messageHandler((int)code, JsonSerializer.Serialize(payload, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
        return Task.FromResult(true);
    }

    #endregion Interface Implementation

    #region Javascript Adaptation

    private Action<int, string> _messageHandler = (int code, string payload) => { };

    [JSExport]
    public static void OnMessage([JSMarshalAsAttribute<JSType.Function<JSType.Number, JSType.String>>] Action<int, string> callback)
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