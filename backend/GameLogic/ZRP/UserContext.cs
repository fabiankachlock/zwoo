using ZwooGameLogic.Lobby;
using ZwooGameLogic.ZRP;

namespace ZwooGameLogic;

public class UserContext
{
    public readonly long Id;
    public readonly string UserName;
    public readonly ZRPRole Role;
    public readonly int Wins;
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

    public UserContext(long id, string userName, ZRPRole role, int wins, long gameId, ZwooRoom room)
    {
        Id = id;
        UserName = userName;
        Wins = wins;
        Role = role;
        GameId = gameId;
        Room = room;
    }
}
