using System.Text.Json.Serialization;

namespace Zwoo.Backend.Controllers.DTO;

public class MessageDTO
{
    public MessageDTO() { }

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}