using Quartz;
using ZwooDatabase;

namespace ZwooBackend.Services;


public class DatabaseCleanupJob : IJob
{
    private readonly IUserService _users;

    public DatabaseCleanupJob(IUserService users)
    {
        _users = users;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _users.CleanUpUsers();
        return Task.CompletedTask;
    }
}