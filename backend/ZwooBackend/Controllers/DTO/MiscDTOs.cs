using System.Text.Json.Serialization;

namespace ZwooBackend.Controllers.DTO;

public class ContactForm
{

    [JsonPropertyName("sender")]
    public string Sender { set; get; }

    [JsonPropertyName("message")]
    public string Message { set; get; }

    public ContactForm(string sender, string message)
    {
        Sender = sender;
        Message = message;
    }
}