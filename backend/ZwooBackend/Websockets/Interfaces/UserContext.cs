using ZwooBackend.Games;
using ZwooBackend.ZRP;

namespace ZwooBackend.Websockets.Interfaces;

public class UserContext
{
    public readonly long Id;
    public readonly string UserName;
    public readonly ZRPRole Role;
    public readonly long GameId;
    public readonly GameRecord GameRecord;

    public UserContext(long id, string userName, ZRPRole role, long gameId, GameRecord gameRecord)
    {
        Id = id;
        UserName = userName;
        Role = role;
        GameId = gameId;
        GameRecord = gameRecord;
    }
}
