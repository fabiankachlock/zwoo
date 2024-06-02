namespace Zwoo.Backend.Shared.Authentication;

public class UserSession
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Wins { get; set; }
}

public class Login
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string CaptchaToken { get; set; } = string.Empty;
}