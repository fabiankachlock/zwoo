using Zwoo.GameEngine.ZRP;
using Zwoo.Api.ZRP;

namespace Zwoo.GameEngine;

public interface IPlayer
{
    public long RealId { get; }
    public long LobbyId { get; }
    public string Username { get; }
    public ZRPRole Role { get; }
    public ZRPPlayerState State { get; }
}