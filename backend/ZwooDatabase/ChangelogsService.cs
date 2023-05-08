using MongoDB.Driver;
using ZwooDatabase.Dao;

namespace ZwooDatabase;

/// <summary>
/// a service for executing changelog related database operations
/// </summary>
public interface IChangelogService
{
    /// <summary>
    /// retrieve a changelog for a certain version
    /// </summary>
    /// <param name="version"></param>
    public ChangelogDao? GetChangelog(string version);

    /// <summary>
    /// return public  accessible changelogs
    /// </summary>
    public List<ChangelogDao> GetChangelogs();
}

public class ChangelogService : IChangelogService
{
    private readonly IDatabase _db;

    public ChangelogService(IDatabase db)
    {
        _db = db;
    }

    public ChangelogDao? GetChangelog(string version) => _db.Changelogs.AsQueryable().FirstOrDefault(c => c.ChangelogVersion == version && c.Public);

    public List<ChangelogDao> GetChangelogs() => _db.Changelogs.AsQueryable().Where(x => x.Public).ToList();
}