using Zwoo.GameEngine;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.Backend.Shared.Api.Game;

public interface IGameEngineService
{
    public bool HasGame(long gameId);
    public ZwooRoom? GetGame(long id);
    public ZwooRoom CreateGame(string name, bool isPublic);
    public bool RemoveGame(long gameId);
    public void DistributeEvent(long gameId, IIncomingZRPMessage msg);
}