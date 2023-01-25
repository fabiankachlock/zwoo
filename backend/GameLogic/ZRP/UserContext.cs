using ZwooGameLogic.Lobby;
using ZwooGameLogic.Bots;

namespace ZwooGameLogic.ZRP;

public class UserContext
{
    public readonly long Id;
    public readonly string PublicId;
    public readonly string UserName;
    public readonly ZRPRole Role;
    public readonly long GameId;
    public readonly ZwooRoom Room;

    public Game.Game Game
    {
        get => Room.Game;
    }

    public LobbyManager Lobby
    {
        get => Room.Lobby;
    }

    public BotManager BotManager
    {
        get => Room.BotManager;
    }

    public UserContext(long id, string publicId, string userName, ZRPRole role, long gameId, ZwooRoom room)
    {
        Id = id;
        PublicId = publicId;
        UserName = userName;
        Role = role;
        GameId = gameId;
        Room = room;
    }
}
