using ZwooGameLogic.Lobby;

namespace ZwooGameLogic;

public class UserContext
{
    public readonly long Id;
    public readonly string UserName;
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

    public UserContext(long id, string userName, int wins, long gameId, ZwooRoom room)
    {
        Id = id;
        UserName = userName;
        Wins = wins;
        GameId = gameId;
        Room = room;
    }
}
