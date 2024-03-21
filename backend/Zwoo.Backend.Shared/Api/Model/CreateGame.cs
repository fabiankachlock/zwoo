namespace Zwoo.Backend.Shared.Api.Model;

public class CreateGame
{
    public string Name { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public bool UsePassword { get; set; }
}