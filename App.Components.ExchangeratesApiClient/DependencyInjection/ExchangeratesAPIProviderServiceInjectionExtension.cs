using App.Components.Contracts.Contracts;
using App.Components.ExchangeratesApiClient.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace App.Components.ExchangeratesApiClient.DependencyInjection
{
    public static class ExchangeratesAPIProviderServiceInjectionExtension
    {
        public static void InjectExchangeratesAPIProviderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ExchangeratesApiOptions>(configuration.GetSection("exchangeratesapi.io"));
            var withcaching = configuration.GetValue<bool>("exchangeratesapi.io:EnableCaching");
            if(withcaching)
               services.AddSingleton<IExchangeRatesProvider, ExchangeratesAPIProviderWithCache>();
            else
                services.AddSingleton<IExchangeRatesProvider, ExchangeratesAPIProvider>();
            string serviceurl=configuration.GetValue<string>("exchangeratesapi.io:ServiceBaseUrl");

            services.AddHttpClient("exchangeratesapi.io", c =>
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
