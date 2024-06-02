namespace Zwoo.Backend.Controllers.DTO;

public class CreateAccount
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool AcceptedTerms { get; set; }
    public string Code { get; set; } = string.Empty;
    public string CaptchaToken { get; set; } = string.Empty;
}
