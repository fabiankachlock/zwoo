using Zwoo.Api.ZRP;

namespace Zwoo.GameEngine.Lobby;

public class LobbyEntry : IPlayer
{
    public long RealId { get; set; }
    public string Username { get; set; }
    public long LobbyId { get; set; }
    public ZRPRole Role { get; set; }
    public ZRPPlayerState State { get; set; }

    public LobbyEntry(long id, long lobbyId, string username, ZRPRole role, ZRPPlayerState state)
    {
        RealId = id;
        LobbyId = lobbyId;
        Username = username;
        Role = role;
        State = state;
    }
}