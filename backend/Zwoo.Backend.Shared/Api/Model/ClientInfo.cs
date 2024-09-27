
namespace Zwoo.Backend.Shared.Api.Model;

public class ClientInfo
{
    public required string Version { get; set; }
    public required string Hash { get; set; }
    public required string ZRPVersion { get; set; }
    public required string Mode { get; set; }
}