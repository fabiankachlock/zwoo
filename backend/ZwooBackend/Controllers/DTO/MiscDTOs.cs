using System.Text.Json.Serialization;

namespace ZwooBackend.Controllers.DTO;

public class ContactForm
{
    [JsonPropertyName("name")]
    public string Name { set; get; }

    [JsonPropertyName("email")]
    public string Email { set; get; }

    [JsonPropertyName("message")]
    public string Message { set; get; }

    [JsonPropertyName("captchaToken")]
    public string CaptchaToken { set; get; }

    [JsonPropertyName("site")]
    public string Site { set; get; }

    public ContactForm(string name, string email, string message, string captchaToken, string site)
    {
        Name = name;
        Email = email;
        Message = message;
        CaptchaToken = captchaToken;
        Site = site;
    }
}