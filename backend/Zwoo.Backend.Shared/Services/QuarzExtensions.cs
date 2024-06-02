using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Simpl;

namespace Zwoo.Backend.Shared.Services;

public static class QuarzExtensions
{
    public static void AddZwooScheduler(this IServiceCollection services, Action<IServiceCollectionQuartzConfigurator>? configurator = null)
    {
        services.AddQuartz(q =>
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


        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}