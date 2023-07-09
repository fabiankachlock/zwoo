using ZwooGameLogic.ZRP;

namespace ZwooGameLogic;

public interface IPlayer
{
    public long Id { get; }
    public int LobbyId { get; }
    public string Username { get; }
    public ZRPRole Role { get; }
    public ZRPPlayerState State { get; }
}