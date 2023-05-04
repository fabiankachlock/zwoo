using ZwooDatabase.Dao;

namespace ZwooDatabase;

/// <summary>
/// a service for executing changelog related database operations
/// </summary>
public interface IChangelogService
{
    public ChangelogDao? GetChangelog(string version);
    public ChangelogDao[] GetChangelogs();
}