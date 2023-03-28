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

    [JsonPropertyName("acceptedTerms")]
    public bool AcceptedTerms { set; get; }

    [JsonPropertyName("acceptedTermsAt")]
    public long AcceptedTermsAt { set; get; }

    [JsonPropertyName("captchaScore")]
    public double CaptchaScore { set; get; }

    [JsonPropertyName("site")]
    public string Site { set; get; }

    public ContactForm(string name, string email, string message, bool acceptedTerms, long acceptedTermsAt, double captchaScore, string site)
    {
        Name = name;
        Email = email;
        Message = message;
        AcceptedTerms = acceptedTerms;
        AcceptedTermsAt = acceptedTermsAt;
        CaptchaScore = captchaScore;
        Site = site;
    }
}