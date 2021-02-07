using App.Components.Contracts.Contracts;
using App.Components.CoinmarketcapApiClient.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace App.Components.CoinmarketcapApiClient.DependencyInjection
{
    public static class CoinmarketcapApiProviderServiceInjectionExtension
    {
        public static void InjectCoinmarketcapAPIProviderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CoinmarketcapApiOptions>(configuration.GetSection("CoinmarketcapAPI"));
            var withcaching = configuration.GetValue<bool>("CoinmarketcapAPI:EnableCaching");
            if(withcaching)
               services.AddSingleton<IExchangeRatesProvider, CoinmarketcapAPIProviderWithCache>();
            else
                services.AddSingleton<IExchangeRatesProvider, CoinmarketcapAPIProvider>();
            string serviceurl=configuration.GetValue<string>("CoinmarketcapAPI:ServiceBaseUrl");

            services.AddHttpClient("CoinmarketcapAPI", c =>
            {
                c.BaseAddress = new Uri(serviceurl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

        }
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(4,retryAttempt => TimeSpan.FromSeconds(retryAttempt*2));
        }

    }
}
