using System.Net.WebSockets;
using ZwooGameLogic.ZRP;

namespace ZwooGameLogic;

public interface INotificationAdapter
{

    public Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload);

    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload);

    public Task<bool> DisconnectPlayer(long playerId);

    public Task<bool> DisconnectGame(long gameId);
}