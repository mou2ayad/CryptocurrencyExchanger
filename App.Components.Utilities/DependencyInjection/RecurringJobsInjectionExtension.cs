using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;

using App.Components.Utilities.RecurringJobs;

namespace App.Components.Utilities.DependencyInjection
{
    public static class RecurringJobsInjectionExtension
    {
        public static void InjectRecurringJobsService(this IServiceCollection services)
        {
            
            services.AddSingleton<IRecurringJobService, RecurringJobService>();

            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());

            services.AddHangfireServer();
        }
    }
}
