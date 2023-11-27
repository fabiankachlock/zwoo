namespace Zwoo.Backend.Shared.Api.Model;

public class ContactForm
{
    public string Name { set; get; } = string.Empty;

    public string Email { set; get; } = string.Empty;

    public string Message { set; get; } = string.Empty;

    public string CaptchaToken { set; get; } = string.Empty;

    public string Site { set; get; } = string.Empty;
}