using System;
using App.Components.Contracts.Contracts;
using App.Components.ExchangeratesApiClient.DependencyInjection;
using App.Components.ExchangeratesApiClient.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace App.Testing.ExchangeratesAPIClientTest
{
    public class ServiceProviderFactory
    {
        private static Dictionary<string, ServiceProvider> serviceProviders = new Dictionary<string, ServiceProvider>();
        private static readonly object locker = new object();

        public static ServiceProvider Get(string appsettingsFileName)
        {
            if (serviceProviders.ContainsKey(appsettingsFileName))
                return serviceProviders[appsettingsFileName];
            lock (locker)
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
            services.InjectExchangeratesAPIProviderService(configuration);
            return services.BuildServiceProvider();
        }

        public IExchangeRatesProvider GetExchangeratesAPIProviderService()
        {
            return _serviceProvider.GetService<IExchangeRatesProvider>();
        }
        public IOptions<ExchangeratesApiOptions> GetExchangeratesAPIConfiguration()
        {
            return _serviceProvider.GetService<IOptions<ExchangeratesApiOptions>>();
        }
    }
}
