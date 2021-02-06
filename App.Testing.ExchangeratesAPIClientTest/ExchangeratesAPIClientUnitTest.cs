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
    public class ExchangeratesAPIClientUnitTest
    {
        private IExchangeRatesProvider _exchangeRatesProvider;
        private readonly IMemoryCache _memoryCache;
        public ExchangeratesAPIClientUnitTest()
        {
            _exchangeRatesProvider = ServiceProviderFactory.GetServiceProvider().GetExchangeratesAPIProviderService();

        }
        [Fact]
        public void TestLoadSupportedCurrencies()
        {
            // Act 
            var resutls = _exchangeRatesProvider.LoadSupportedCurrencies().Result;

            // Assert  
            resutls.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public void TestLoadConfiguration()
        {
            // Arrange 
            var serviceProvider = ServiceProviderFactory.GetServiceProvider();

            // Act 
            var config = serviceProvider.GetExchangeratesAPIConfiguration().Value;

            // Assert  
            config.Should().NotBeNull();
            config.ServiceBaseUrl.Should().NotBeNullOrEmpty();
            config.ExchangeRateEndpoint.Should().NotBeNullOrEmpty();
            config.SupportedCurrencies.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(0);
        }
        [Fact]
        public void TestGetExchangeRatesList_UnSupportedBaseCurrency()
        {
            // Arrange 
            string BaseCurrencySymbol = "MYR";

            // Act 
            Action act =  () =>  _exchangeRatesProvider.GetExchangeRatesList(BaseCurrencySymbol).Wait();

            // Assert  
            act.Should().Throw<InvalidRequestException>()
                .WithMessage($"{BaseCurrencySymbol} is Unsupported currency");
        }
        [Fact]
        public void TestGetExchangeRatesList_WithTargetedCurrencies()
        {
            // Arrange 
            string BaseCurrencySymbol = "USD";
            string[] targetedCurencies = { "EUR", "GBP" };

            // Act 
            var results = _exchangeRatesProvider.GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

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
        public void TestGetExchangeRatesList_WithoutTargetedCurrencies()
        {
            // Arrange 
            string BaseCurrencySymbol = "USD";
            var config = ServiceProviderFactory.GetServiceProvider().GetExchangeratesAPIConfiguration().Value;
            List<string> targetedCurencies =config.SupportedCurrencies;

            // Act 
            var results = _exchangeRatesProvider.GetExchangeRatesList(BaseCurrencySymbol).Result;

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
        public void TestGetExchangeRatesList_InvalidRequest()
        {
            // Arrange 
            string BaseCurrencySymbol = "USD";
            string[] targetedCurencies = { "OOKS", "SYP" };

            // Act
            Action act = () => _exchangeRatesProvider.GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Wait();

            // Assert  
            act.Should().Throw<RestAPIException>()
                .Where(e => e.Message.StartsWith("Request to [exchangeratesapi.io] faild with following response:"))
                .Where(e => e.ExceptionDetails.StatusCode == (int)System.Net.HttpStatusCode.BadRequest);
        }
        [Fact]
        public void TestGetExchangeRatesList_Caching()
        {
            // Arrange 
            string BaseCurrencySymbol = "USD";
            string[] targetedCurencies = { "EUR", "GBP" };
            var cache = ServiceProviderFactory.GetServiceProvider().GetService<IMemoryCache>();
            string key = $"exchangeratesapi.io_usd_eur";
            cache.Remove(key);
            decimal cacheValue;

            cache.TryGetValue(key, out cacheValue).Should().BeFalse();

            // Act 
            var results = _exchangeRatesProvider.GetExchangeRatesList(BaseCurrencySymbol, targetedCurencies).Result;

            // Assert  
            cache.TryGetValue(key, out cacheValue).Should().BeTrue();
            results.CurrenciesRates.Should().ContainKeys("EUR", "GBP");
            results.CurrenciesRates["EUR"].Should().Be(cacheValue);
           
        }

    }
}
