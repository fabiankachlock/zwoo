using ZwooGameLogic.Lobby;
using ZwooGameLogic.Bots;

namespace ZwooGameLogic.ZRP;

public class UserContext
{
    public readonly long RealId;
    public readonly long LobbyId;
    public readonly string Username;
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

    public UserContext(long id, long lobbyId, string userName, ZRPRole role, long gameId, ZwooRoom room)
    {
        RealId = id;
        LobbyId = lobbyId;
        Username = userName;
        Role = role;
        GameId = gameId;
        Room = room;
    }
}
