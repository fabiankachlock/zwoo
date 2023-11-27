namespace Zwoo.Backend.Controllers.DTO;

public class RequestResetPassword
{
    public string Email { get; set; } = string.Empty;
    public string CaptchaToken { get; set; } = string.Empty;

}