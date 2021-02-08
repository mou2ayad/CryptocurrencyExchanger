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
    public class CoinmarketcapAPIClientUnitTestWithoutCaching
    {
      
        private readonly string appsettingName = "appsettings.json";
        private ServiceProvider serviceProvider => ServiceProviderFactory.Get(appsettingName);
     
        [Fact]
        public void TestLoadSupportedCurrencies()
        {
            // Act 
            var resutls = serviceProvider.GetCoinmarketcapAPIProviderService().LoadSupportedCurrencies().Result;

            // Assert  
            resutls.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public void TestLoadConfiguration()
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
            config.EnableCaching.Should().BeFalse();
            config.APIKeyName.Should().NotBeNullOrEmpty();
            config.APIKeyValue.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public void TestGetExchangeRatesList_UnSupportedBaseCryptoCurrency()
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
        public void TestGetExchangeRatesList_WithTargetedCurrencies()
        {
            // Arrange 
            string BaseCurrencyCryptoSymbol = "BTC";
            string[] targetedCurencies = { "EUR", "USD" };

            // Act 
            var results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCurrencyCryptoSymbol, targetedCurencies).Result;

            // Assert  
            // check if the baseCurrencyIn the response equal the input BaseCurrencyCryptoSymbol
            results.BaseCurrencySymbol.ToUpper().Should().Be(BaseCurrencyCryptoSymbol);

            // check if all response contains rates
            results.CurrenciesRates.Should().HaveCount(targetedCurencies.Length);

            // check if all the targeted Currencies are in the response
            targetedCurencies.Except(results.CurrenciesRates.Keys.Select(e=>e.ToUpper())).Any().Should().BeFalse();

            // check if all the targeted Currencies are in the response
            results.CurrenciesRates.Values.Any(e => e == 0).Should().BeFalse();

        }
        [Fact]
        public void TestGetExchangeRatesList_TestingIfCurrencySymbolCaseSensitive()
        {
            // Arrange 
            string BaseCurrencySymbol = "btc";
            string[] targetedCurencies = { "eur", "usd" };

            // Act 
            var results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

            // Assert  
            // check if the baseCurrencyIn the response equal the input BaseCurrencySymbol
            results.BaseCurrencySymbol.ToUpper().Should().Be("BTC");

            // check if all response contains rates
            results.CurrenciesRates.Should().HaveCount(targetedCurencies.Length);

            // check if all the targeted Currencies are in the response
            targetedCurencies.Select(e => e.ToUpper()).Except(results.CurrenciesRates.Keys.Select(e => e.ToUpper())).Any().Should().BeFalse();

            // check if all the targeted Currencies are in the response
            results.CurrenciesRates.Values.Any(e => e == 0).Should().BeFalse();
        }
        [Fact]
        public void TestGetExchangeRatesList_WithoutTargetedCurrencies()
        {
            // Arrange 
            string BaseCurrencyCryptoSymbol = "BTC";
            var config = serviceProvider.GetCoinmarketcapAPIConfiguration().Value;
            List<string> targetedCurencies =config.DefaultTargetedCurrencies;

            // Act 
            var results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCurrencyCryptoSymbol).Result;

            // Assert  
            // check if the baseCurrencyIn the response equal the input BaseCurrencyCryptoSymbol
            results.BaseCurrencySymbol.ToUpper().Should().Be(BaseCurrencyCryptoSymbol);

            // check if all response contains rates
            results.CurrenciesRates.Should().HaveCount(targetedCurencies.Count);

            // check if all the targeted Currencies are in the response
            targetedCurencies.Except(results.CurrenciesRates.Keys.Select(e => e.ToUpper())).Any().Should().BeFalse();

            // check if all the targeted Currencies are in the response
            results.CurrenciesRates.Values.Any(e => e == 0).Should().BeFalse();

        }
        [Fact]
        public void TestGetExchangeRatesList_InvalidTargetedCurrencies()
        {
            // Arrange 
            string BaseCurrencySymbol = "BTC";
            string[] targetedCurencies = { "OOKS", "SYP" };

            // Act
            Action act = () => serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Wait();

            // Assert  
            act.Should().Throw<InvalidRequestException>()
                .Where(e => e.Message.StartsWith("[OOKS,SYP] are Unsupported fiat currencies"));
        }
        [Fact]
        public void TestGetExchangeRatesList_FewInvalidTargetedCurrencies()
        {
            // Arrange 
            string BaseCryptoCurrencySymbol = "BTC";
            // two invalid and one valid currencies
            string[] targetedCurencies = { "OOKS", "SYP","EUR" };

            // Act
            var results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCryptoCurrencySymbol, targetedCurencies).Result;

            // Assert  
            results.Should().NotBeNull();
            results.CurrenciesRates.Values.Should().HaveCount(1);
            results.CurrenciesRates.Should().ContainKey("EUR");
        }
        [Fact]
        public void TestGetExchangeRatesList_Without_Caching()
        {
            // Arrange 
            string BaseCryptoCurrencySymbol = "BTC";
            string[] targetedCurencies = { "EUR", "USD" };
            //var serviceProvider = new ServiceProvider(appsettingName);
            var cache = serviceProvider.GetService<IMemoryCache>();
            // create a cache key for one of the currencies 
            string key = $"coinmarketcapapi_btc_usd";
            //removing the key from the cache in case it is there
            cache.Remove(key);
            decimal cacheValue;

            // Assert  
            // ensure that we don't have the key in the cache by ren
            cache.TryGetValue(key, out cacheValue).Should().BeFalse();

            // Act 
            // get the currency from the provider
            var results = serviceProvider.GetCoinmarketcapAPIProviderService().GetExchangeRatesList(BaseCryptoCurrencySymbol, targetedCurencies).Result;

            // Assert  
            // ensure that the value is not saved in the cache
            cache.TryGetValue(key, out cacheValue).Should().BeFalse();
           
        }

    }
}
