using Microsoft.AspNetCore.Builder;
using Quartz;
using Quartz.Simpl;

namespace Zwoo.Backend.Shared.Services;

public static class QuarzExtensions
{
    public static void AddZwooScheduler(this WebApplicationBuilder builder, Action<IServiceCollectionQuartzConfigurator>? configurator = null)
    {
        builder.Services.AddQuartz(q =>
        {
            q.SchedulerId = "zwoo-scheduler";
            q.UseThreadPool<DefaultThreadPool>(c =>
            {
                c.MaxConcurrency = 1;
            });
            q.UseMicrosoftDependencyInjectionJobFactory();

            if (configurator != null)
            {
                configurator(q);
            }
        });


        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}