using System;
using App.Components.Contracts.Contracts;
using App.Components.CoinmarketcapApiClient.DependencyInjection;
using App.Components.CoinmarketcapApiClient.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace App.Testing.CoinmarketcapAPIClientTest
{
    public  class ServiceProviderFactory
    {
        private static Dictionary<string, ServiceProvider> serviceProviders = new Dictionary<string, ServiceProvider>();
        private static readonly object locker = new object();

        public static ServiceProvider Get(string appsettingsFileName)
        {
            if (serviceProviders.ContainsKey(appsettingsFileName))
                return serviceProviders[appsettingsFileName];
            lock(locker)
            {
                if (serviceProviders.ContainsKey(appsettingsFileName))
                    return serviceProviders[appsettingsFileName];

                serviceProviders.Add(appsettingsFileName, new ServiceProvider(appsettingsFileName));
                return serviceProviders[appsettingsFileName];
            }
        }
    }
    public class ServiceProvider 
    {
        private IConfiguration configuration { get; set; }
        private IServiceProvider _serviceProvider { set; get; }
        public  ServiceProvider(string appsettingsFileName)
        {
            var config = new ConfigurationBuilder()
              .AddJsonFile($"Settings/{appsettingsFileName}")
              .Build();

            configuration = config;
            _serviceProvider = GetServiceProvider();

        }
        public T GetService<T>() =>
            _serviceProvider.GetService<T>();
      
        private IServiceProvider GetServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddMemoryCache();
            services.AddOptions();
            services.InjectCoinmarketcapAPIProviderService(configuration);
            return services.BuildServiceProvider();
        }

        public IExchangeRatesProvider GetCoinmarketcapAPIProviderService()
        {
            return _serviceProvider.GetService<IExchangeRatesProvider>();
        }
        public IOptions<CoinmarketcapApiOptions> GetCoinmarketcapAPIConfiguration()
        {
            return _serviceProvider.GetService<IOptions<CoinmarketcapApiOptions>>();
        }

      
    }
}
