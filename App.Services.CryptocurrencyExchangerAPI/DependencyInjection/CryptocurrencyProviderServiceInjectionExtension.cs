using App.Components.CoinmarketcapApiClient.DependencyInjection;
using App.Components.Contracts.Contracts;
using App.Components.ExchangeratesApiClient.DependencyInjection;
using App.Services.CryptocurrencyExchangerAPI.Models;
using App.Services.CryptocurrencyExchangerAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.CryptocurrencyExchangerAPI.DependencyInjection
{
    public static class CryptocurrencyProviderServiceInjectionExtension
    {
        public static void InjectCryptocurrencyProviderService(this IServiceCollection services, IConfiguration configuration)
        {           
            services.Configure<CryptocurrencyProvidersOptions>(configuration.GetSection("CryptocurrencyProviders"));
            string CryptocurrenciesproviderName = configuration.GetValue<string>("CryptocurrencyProviders:MainCryptocurrencyProvider:Name");
            InjectExchangeRatesProviderTypeByName(services, configuration, CryptocurrenciesproviderName);
            string AdditionalFiatCurrencyProviderName = configuration.GetValue<string>("CryptocurrencyProviders:AdditionalFiatCurrencyProvider:Name");
            if (!string.IsNullOrEmpty(AdditionalFiatCurrencyProviderName))
                InjectExchangeRatesProviderTypeByName(services, configuration, AdditionalFiatCurrencyProviderName);
            services.AddSingleton<ICryptocurrencyExchangeProvider, CryptocurrencyExchangeProvider>();

        }
        private static void InjectExchangeRatesProviderTypeByName(IServiceCollection services, IConfiguration configuration,string CryptocurrenciesproviderName)
        {
            switch (CryptocurrenciesproviderName.ToLower())
            {
                case "coinmarketcapapi":
                    services.InjectCoinmarketcapAPIProviderService(configuration);
                    break;
                case "exchangeratesapi.io":
                    services.InjectExchangeratesAPIProviderService(configuration);
                    break;

                default:
                    throw new Exception($"There in no CurrencyExchangeProvider with name '{CryptocurrenciesproviderName}'");
            }
        }

    }
}
