using System.Text.Json.Serialization;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Controllers.DTO;

public static class ChangelogDaoExtensions
{
    public static Changelog ToDTO(this ChangelogDao dao)
    {
        return new Changelog()
        {
            ChangelogText = dao.ChangelogText,
            ChangelogVersion = dao.ChangelogVersion
        };
    }
}

public class Changelog
{
    public Changelog() { }

    [JsonPropertyName("version")]
    public string ChangelogVersion { set; get; } = "";

    [JsonPropertyName("changelog")]
    public string ChangelogText { set; get; } = "";

    [JsonPropertyName("timestamp")]
    public ulong Timestamp { set; get; } = 0;
}

public class VersionHistory
{
    public VersionHistory() { }

    [JsonPropertyName("versions")]
    public List<string> Versions { set; get; } = new();
}