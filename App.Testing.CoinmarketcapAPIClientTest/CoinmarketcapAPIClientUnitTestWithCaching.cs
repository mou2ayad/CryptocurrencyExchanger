using System;
using Xunit;
using App.Components.Contracts.Contracts;
using FluentAssertions;
using App.Components.Utilities.CustomException;
using System.Linq;
using System.Collections.Generic;
using App.Components.Utilities.APIClient;
using Microsoft.Extensions.Caching.Memory;

namespace App.Testing.CoinmarketcapAPIClientTest
{
    public class CoinmarketcapAPIClientUnitTestWithCaching
    {
        private readonly string appsettingName = "appsettingsWithCache.json";
        private ServiceProvider serviceProvider => ServiceProviderFactory.Get(appsettingName);
        [Fact]
        public void TestLoadConfigurationWithCache()
        {
            // Arrange 

            // Act 
            var config = serviceProvider.GetCoinmarketcapAPIConfiguration().Value;

            // Assert  
            config.Should().NotBeNull();
            config.ServiceBaseUrl.Should().NotBeNullOrEmpty();
            config.MapEndpoint.Should().NotBeNullOrEmpty();
            config.QuotesEndpoint.Should().NotBeNullOrEmpty();
            config.SupportedTargetedCurrencies.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(0);
            config.DefaultTargetedCurrencies.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(0);
            config.EnableCaching.Should().BeTrue();
            config.ExpiredAfterInMinutes.Should().BeGreaterThan(0);
            config.APIKeyName.Should().NotBeNullOrEmpty();
            config.APIKeyValue.Should().NotBeNullOrEmpty();


        }
        [Fact]
        public void TestGetExchangeRatesListWithCache_UnsupportedBaseCryptoCurrency()
        {
            // Arrange 
            string BaseCryptoCurrencySymbol = "SYP";

            // Act 
            Action act =  () => serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCryptoCurrencySymbol).Wait();

            // Assert  
            act.Should().Throw<InvalidRequestException>()
                .WithMessage($"{BaseCryptoCurrencySymbol} is invalid or Unsupported Cryptocurrency");
        }
        [Fact]
        public void TestGetExchangeRatesListWithCache_WithTargetedCurrencies()
        {
            // Arrange 
            string BaseCurrencySymbol = "BTC";
            string[] targetedCurencies = { "EUR", "USD" };

            // Act 
            var results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

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
            string BaseCurrencySymbol = "BTC";
            var config = serviceProvider.GetCoinmarketcapAPIConfiguration().Value;
            List<string> targetedCurencies =config.DefaultTargetedCurrencies;

            // Act 
            var results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol).Result;

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
            string BaseCurrencySymbol = "BTC";
            string[] targetedCurencies = { "EUR", "USD" };
            var cache = serviceProvider.GetService<IMemoryCache>();
            // create a cache key for one of the currencies 
            string key = $"coinmarketcapapi_btc_eur";
            //removing the key from the cache in case it is there
            cache.Remove(key);
            decimal cacheValue;
            
            // Assert  
            // ensure that we don't have the key in the cache by ren
            cache.TryGetValue(key, out cacheValue).Should().BeFalse();

            // Act 
            // get the currency from the provider
            var results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

            // Assert  
            // ensure that the value is saved in the cache
            cache.TryGetValue(key, out cacheValue).Should().BeTrue();
            results.CurrenciesRates.Should().ContainKeys("EUR", "USD");
            results.CurrenciesRates["EUR"].Should().Be(cacheValue);


            //Arrange
            // set another value in the cache for the same key Manually
            cache.Set(key, (decimal)10,DateTimeOffset.Now.AddHours(2));
            decimal actualEurValue = results.CurrenciesRates["EUR"];

            // Act 
            // get the currency from the provider through the Proxy cache class
            results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

            // Assert  
            // ensure that the value comes from the cache not from Provider
            cache.TryGetValue(key, out cacheValue).Should().BeTrue();
            results.CurrenciesRates.Should().ContainKeys("EUR", "USD");
            results.CurrenciesRates["EUR"].Should().Be(cacheValue);
            results.CurrenciesRates["EUR"].Should().NotBe(actualEurValue);
            cacheValue.Should().Be(10);

        }

    }
}
