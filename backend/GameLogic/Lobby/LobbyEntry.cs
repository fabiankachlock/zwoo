using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Lobby;

public class LobbyEntry : IPlayer
{
    public long Id { get; set; }
    public string Username { get; set; }
    public int LobbyId { get; set; }
    public ZRPRole Role { get; set; }
    public ZRPPlayerState State { get; set; }

    public LobbyEntry(long id, int lobbyId, string username, ZRPRole role, ZRPPlayerState state)
    {
        Id = id;
        LobbyId = lobbyId;
        Username = username;
        Role = role;
        State = state;
    }
}