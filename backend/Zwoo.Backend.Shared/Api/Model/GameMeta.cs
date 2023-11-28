namespace Zwoo.Backend.Shared.Api.Model;

public class GameMeta
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int PlayerCount { get; set; }
}