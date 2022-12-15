using ZwooBackend.ZRP;
using ZwooGameLogic.ZRP;

namespace ZwooBackend.Games;

// TODO: move to game logic
public class ZwooRoom { }


public struct GameEvent
{
    public int UserId;
    public int GameId;
    public ZRPMessage Payload;
}

public interface IGameLogicService
{
    public bool HasGame(long gameId);

    public long CreateGame(long gameId);

    public bool RemoveGame(long gameId);

    public ZwooRoom? GetGame(long gameId);

    public IEnumerable<ZwooRoom> ListAll();

    public void DistributeEvent(long gameId, IZRPMessage msg);

}

public class GameLogicService : IGameLogicService
{
    public long CreateGame(long gameId)
    {
        throw new NotImplementedException();
    }

    public void DistributeEvent(IZRPMessage msg)
    {
        throw new NotImplementedException();
    }

    public ZwooRoom? GetGame(long gameId)
    {
        throw new NotImplementedException();
    }

    public bool HasGame(long gameId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ZwooRoom> ListAll()
    {
        throw new NotImplementedException();
    }

    public bool RemoveGame(long gameId)
    {
        throw new NotImplementedException();
    }
}
