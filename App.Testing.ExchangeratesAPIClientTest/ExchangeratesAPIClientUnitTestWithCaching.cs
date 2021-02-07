using System;
using Xunit;
using App.Components.Contracts.Contracts;
using FluentAssertions;
using App.Components.Utilities.CustomException;
using System.Linq;
using System.Collections.Generic;
using App.Components.Utilities.APIClient;
using Microsoft.Extensions.Caching.Memory;

namespace App.Testing.ExchangeratesAPIClientTest
{
    public class ExchangeratesAPIClientUnitTestWithCaching
    {
        private readonly string appsettingName = "appsettingsWithCache.json";
        private ServiceProvider serviceProvider => ServiceProviderFactory.Get(appsettingName);

        [Fact]
        public void TestLoadConfigurationWithCache()
        {
            // Arrange 

            // Act 
            var config = serviceProvider.GetExchangeratesAPIConfiguration().Value;

            // Assert  
            config.Should().NotBeNull();
            config.ServiceBaseUrl.Should().NotBeNullOrEmpty();
            config.ExchangeRateEndpoint.Should().NotBeNullOrEmpty();
            config.SupportedCurrencies.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(0);
            config.EnableCaching.Should().BeTrue();
            config.ExpiredAfterInMinutes.Should().BeGreaterThan(0);
        }
        [Fact]
        public void TestGetExchangeRatesListWithCache_UnsupportedBaseCurrency()
        {
            // Arrange 
            string BaseCurrencySymbol = "SYP";

            // Act 
            Action act =  () => serviceProvider.GetExchangeratesAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol).Wait();

            // Assert  
            act.Should().Throw<InvalidRequestException>()
                .WithMessage($"{BaseCurrencySymbol} is Unsupported currency");
        }
        [Fact]
        public void TestGetExchangeRatesListWithCache_WithTargetedCurrencies()
        {
            // Arrange 
            string BaseCurrencySymbol = "USD";
            string[] targetedCurencies = { "EUR", "GBP" };

            // Act 
            var results = serviceProvider.GetExchangeratesAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

            // Assert  
            // check if the baseCurrencyIn the response equal the input BaseCurrencySymbol
            results.BaseCurrencySymbol.ToUpper().Should().Be(BaseCurrencySymbol);

            // check if all response contains rates
            results.CurrenciesRates.Should().HaveCount(targetedCurencies.Length);

            // check if all the targeted Currencies are in the response
            targetedCurencies.Except(results.CurrenciesRates.Keys.Select(e=>e.ToUpper())).Any().Should().BeFalse();

            // check if all the targeted Currencies are in the response
            results.CurrenciesRates.Values.Any(e => e == 0).Should().BeFalse();

        }
        [Fact]
        public void TestGetExchangeRatesListWithCache_WithoutTargetedCurrencies()
        {
            // Arrange 
            string BaseCurrencySymbol = "USD";
            var config = serviceProvider.GetExchangeratesAPIConfiguration().Value;
            List<string> targetedCurencies =config.DefaultTargetedCurrencies;

            // Act 
            var results = serviceProvider.GetExchangeratesAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol).Result;

            // Assert  
            // check if the baseCurrencyIn the response equal the input BaseCurrencySymbol
            results.BaseCurrencySymbol.ToUpper().Should().Be(BaseCurrencySymbol);

            // check if all response contains rates
            results.CurrenciesRates.Should().HaveCount(targetedCurencies.Count);

            // check if all the targeted Currencies are in the response
            targetedCurencies.Except(results.CurrenciesRates.Keys.Select(e => e.ToUpper())).Any().Should().BeFalse();

            // check if all the targeted Currencies are in the response
            results.CurrenciesRates.Values.Any(e => e == 0).Should().BeFalse();

        }
        
        [Fact]
        public void TestGetExchangeRatesList_Caching()
        {
            // Arrange 
            string BaseCurrencySymbol = "USD";
            string[] targetedCurencies = { "EUR", "GBP" };
            var cache = serviceProvider.GetService<IMemoryCache>();
            // create a cache key for one of the currencies 
            string key = $"exchangeratesapi.io_usd_eur";
            //removing the key from the cache in case it is there
            cache.Remove(key);
            decimal cacheValue;
            
            // Assert  
            // ensure that we don't have the key in the cache by ren
            cache.TryGetValue(key, out cacheValue).Should().BeFalse();

            // Act 
            // get the currency from the provider
            var results = serviceProvider.GetExchangeratesAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

            // Assert  
            // ensure that the value is saved in the cache
            cache.TryGetValue(key, out cacheValue).Should().BeTrue();
            results.CurrenciesRates.Should().ContainKeys("EUR", "GBP");
            results.CurrenciesRates["EUR"].Should().Be(cacheValue);



            //Arrange
            // set another value in the cache for the same key Manually
            cache.Set(key, (decimal)10,DateTimeOffset.Now.AddHours(2));
            decimal actualEurValue = results.CurrenciesRates["EUR"];

            // Act 
            // get the currency from the provider through the Proxy cache class
            results = serviceProvider.GetExchangeratesAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

            // Assert  
            // ensure that the value comes from the cache not from Provider
            cache.TryGetValue(key, out cacheValue).Should().BeTrue();
            results.CurrenciesRates.Should().ContainKeys("EUR", "GBP");
            results.CurrenciesRates["EUR"].Should().Be(cacheValue);
            results.CurrenciesRates["EUR"].Should().NotBe(actualEurValue);
            cacheValue.Should().Be(10);

        }

    }
}
