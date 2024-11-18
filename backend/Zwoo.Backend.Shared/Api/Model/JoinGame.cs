using Zwoo.Api.ZRP;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.Backend.Shared.Api.Model;

public class JoinGame
{
    public long GameId { get; set; }
    public string? Password { get; set; } = string.Empty;
    public ZRPRole Role { get; set; }
}