using Quartz;
using ZwooDatabase;

namespace ZwooBackend.Services;


public class DatabaseCleanupJob : IJob
{
    private IDatabase _db { get; set; }

    public DatabaseCleanupJob(IDatabase db)
    {
        _db = db;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _db.CleanDatabase();
        return Task.CompletedTask;
    }
}