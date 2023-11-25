using Microsoft.AspNetCore.Builder;
using Quartz;
using Quartz.Simpl;

namespace Zwoo.Backend.Shared.Services;

public static class QuarzExtensions
{
    public static void AddZwooScheduler(this WebApplicationBuilder builder)
    {
        builder.Services.AddQuartz(q =>
        {
            q.SchedulerId = "zwoo-scheduler";
            q.UseThreadPool<DefaultThreadPool>(c =>
            {
                c.MaxConcurrency = 1;
            });
            q.UseMicrosoftDependencyInjectionJobFactory();
            // q.ScheduleJob<DatabaseCleanupJob>(t => t
            //     .WithIdentity("cleanup", "db")
            //     .WithCronSchedule("0 1 1 1/1 * ? *"));
        });

        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        // builder.Services.AddTransient<DatabaseCleanupJob>();
    }
}