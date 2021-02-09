using System;
using App.Components.ExchangeratesApiClient.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using App.Services.CryptocurrencyExchangerAPI.DependencyInjection;
using App.Services.CryptocurrencyExchangerAPI.Services;
using App.Services.CryptocurrencyExchangerAPI.Models;

namespace App.Testing.CryptocurrencyExchangerAPITest
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
        private IConfiguration Configuration { get; set; }
        private IServiceProvider _serviceProvider { set; get; }
        public  ServiceProvider(string appsettingsFileName)
        {
            var config = new ConfigurationBuilder()
              .AddJsonFile($"Settings/{appsettingsFileName}")
              .AddUserSecrets("05bf9c07-fc4d-41d9-b305-5bd50c1e211b")
              .Build();

            Configuration = config;
            _serviceProvider = GetServiceProvider();

        }
        public T GetService<T>() =>
            _serviceProvider.GetService<T>();
      
        private IServiceProvider GetServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddMemoryCache();
            services.AddOptions();
            services.InjectCryptocurrencyProviderService(Configuration);
            return services.BuildServiceProvider();
        }

        public ICryptocurrencyExchangeProvider GetCryptocurrencyExchangeProvider()        
           => _serviceProvider.GetService<ICryptocurrencyExchangeProvider>();
        
        public IOptions<CryptocurrencyProvidersOptions> GetExchangeratesAPIConfiguration()
            => _serviceProvider.GetService<IOptions<CryptocurrencyProvidersOptions>>();
        
    }
}
