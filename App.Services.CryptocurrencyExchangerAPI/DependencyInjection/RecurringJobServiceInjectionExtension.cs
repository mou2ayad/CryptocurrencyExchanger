
using App.Components.Utilities.DependencyInjection;
using App.Services.CryptocurrencyExchangerAPI.RecurringJobService;
using Microsoft.Extensions.DependencyInjection;

namespace App.Services.CryptocurrencyExchangerAPI.DependencyInjection
{
    public static class RecurringJobServiceInjectionExtension
    {
        public static void InjectRecurringJobService(this IServiceCollection services)
        {
            services.InjectRecurringJobsService();
            services.AddSingleton<IRecurringJobsBuilder, RecurringJobsBuilder>();
            services.AddTransient<IExchangeRateProviderRecuringJobAdapter,ExchangeRateProviderRecuringJobAdapter>();
        }
     

    }
}
