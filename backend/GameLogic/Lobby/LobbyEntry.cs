using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Lobby;

public class LobbyEntry : IPlayer
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string PublicId { get; set; }
    public ZRPRole Role { get; set; }
    public ZRPPlayerState State { get; set; }

    public LobbyEntry(long id, string publicId, string username, ZRPRole role, ZRPPlayerState state)
    {
        Id = id;
        PublicId = publicId;
        Username = username;
        Role = role;
        State = state;
    }
}