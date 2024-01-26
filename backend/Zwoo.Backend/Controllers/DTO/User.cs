using System.Text.Json.Serialization;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Controllers.DTO;

public static class UserDaoExtensions
{
    public static User ToDTO(this UserDao dao)
    {
        return new User()
        {
            Email = dao.Email,
            Username = dao.Username,
            Wins = dao.Wins
        };
    }
}

public class User
{
    public User() { }

    [JsonPropertyName("username")]
    public string Username { set; get; } = "";

    [JsonPropertyName("email")]
    public string Email { set; get; } = "";


    [JsonPropertyName("wins")]
    public uint Wins { set; get; }
}

public class UserSettings
{
    public UserSettings() { }

    [JsonPropertyName("settings")]
    public string Settings { set; get; } = "";
}