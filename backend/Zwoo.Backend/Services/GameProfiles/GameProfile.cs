namespace Zwoo.Backend.Services.GameProfiles;

public class GameProfile
{
    public string Id { get; set; }

    public string Name { get; set; }

    public Dictionary<string, int> Settings { get; set; }

    public GameProfile(string id, string name, GameProfileGroup system, Dictionary<string, int> settings)
    {
        Id = id;
        Name = name;
        Settings = settings;
    }
}

