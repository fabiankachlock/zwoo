using System.Threading.Tasks;
using ZwooGameLogic;
using ZwooGameLogic.ZRP;

namespace ZwooWasm;

public class WasmNotificationDistributer : INotificationAdapter
{
    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> DisconnectGame(long gameId)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> DisconnectPlayer(long playerId)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload)
    {
        throw new System.NotImplementedException();
    }
}