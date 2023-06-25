using System.Text.Json.Serialization;

namespace ZwooBackend.Controllers.DTO;

public class ErrorDTO
{
    public ErrorDTO() { }

    [JsonPropertyName("code")]
    public int Code { get; set; } = 0;

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}