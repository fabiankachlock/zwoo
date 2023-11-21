using ZwooGameLogic.Lobby;
using ZwooGameLogic.Bots;

namespace ZwooGameLogic.ZRP;

public class UserContext : IPlayer
{
    public long RealId { get; }
    public long LobbyId { get; }
    public string Username { get; }
    public ZRPRole Role { get; }
    public readonly long GameId;
    public readonly ZwooRoom Room;

    /// <summary>
    /// THIS IS UNUSED
    /// </summary>
    public ZRPPlayerState State { get; }

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
