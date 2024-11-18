using System.Text.Json.Serialization;
using Zwoo.Backend.Services.GameProfiles;

namespace Zwoo.Backend.Controllers.DTO;

public class CreateGameProfile
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("settings")]
    public Dictionary<string, int> Settings { get; set; }
    
    public CreateGameProfile(string name, Dictionary<string, int> settings)
    {
        Name = name;
        Settings = settings;
    }
}

public class GameProfileResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("settings")]
    public Dictionary<string, int> Settings { get; set; }
    
    [JsonPropertyName("type")]
    public GameProfileGroup Type { get; set; }
    
    public GameProfileResponse(string id, string name, GameProfileGroup type, Dictionary<string, int> settings)
    {
        Id = id;
        Name = name;
        Type = type;
        Settings = settings;
    }
}

public class GameProfileListResponse
{
    [JsonPropertyName("profiles")]
    public List<GameProfileResponse> Profiles { get; set; }

    public GameProfileListResponse(List<GameProfileResponse> profiles)
    {
        Profiles = profiles;
    }
}

