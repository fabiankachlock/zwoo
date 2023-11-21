using MongoDB.Driver;
using Zwoo.Database.Dao;

namespace Zwoo.Database;

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

    /// <summary>
    /// update a changelog entity
    /// </summary>
    /// <param name="data">the new changelog</param>
    public void UpdateChangelog(ChangelogDao data);
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

    public void UpdateChangelog(ChangelogDao changelog)
    {
        _db.Changelogs.ReplaceOne(x => x.Id == changelog.Id, changelog);
    }
}