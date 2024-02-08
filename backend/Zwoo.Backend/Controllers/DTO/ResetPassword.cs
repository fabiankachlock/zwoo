namespace Zwoo.Backend.Controllers.DTO;

public class ResetPassword
{
    public string Code { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string CaptchaToken { get; set; } = string.Empty;

}