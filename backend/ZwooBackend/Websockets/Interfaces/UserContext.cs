using ZwooBackend.ZRP;

namespace ZwooBackend.Websockets.Interfaces;

public readonly struct UserContext
{
    public readonly long Id;
    public readonly string UserName;
    public readonly ZRPRole Role;
    public readonly long GameId;

    public UserContext(long id, string userName, ZRPRole role, long gameId)
    {
        Id = id;
        UserName = userName;
        Role = role;
        GameId = gameId;
    }
}
