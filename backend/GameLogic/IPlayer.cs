using ZwooGameLogic.ZRP;

namespace ZwooGameLogic;

public interface IPlayer
{
    public long Id { get; }
    public string PublicId { get; }
    public string Username { get; }
    public ZRPRole Role { get; }
    public ZRPPlayerState State { get; }
}