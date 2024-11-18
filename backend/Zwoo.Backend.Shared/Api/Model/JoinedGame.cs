using Zwoo.Api.ZRP;

namespace Zwoo.Backend.Shared.Api.Model;

public class JoinedGame
{
    public long GameId { set; get; }

    public bool IsRunning { set; get; }

    public ZRPRole Role { set; get; }

    public long OwnId { set; get; }
}