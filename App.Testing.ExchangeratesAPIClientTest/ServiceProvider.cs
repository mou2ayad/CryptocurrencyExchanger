using System;
using App.Components.Contracts.Contracts;
using App.Components.ExchangeratesApiClient.DependencyInjection;
using App.Components.ExchangeratesApiClient.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace App.Testing.ExchangeratesAPIClientTest
{
    internal static class ServiceProviderFactory
    {
        private static ServiceProvider serviceProvider;
        public static ServiceProvider GetServiceProvider()
        {
            if (serviceProvider == null)
                serviceProvider = ServiceProvider.GetInstance();
            return serviceProvider;
        }
    }
    internal class ServiceProvider
    {
        private IConfiguration configuration { get; set; }
        private IServiceProvider _serviceProvider { set; get; }
        private ServiceProvider()
        {
            var config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .Build();

            configuration = config;
            _serviceProvider = GetServiceProvider();

        }
        public T GetService<T>() =>
            _serviceProvider.GetService<T>();

        

        public static ServiceProvider GetInstance()
        {
            return new ServiceProvider();
        }
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
