using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace App.Testing.CryptocurrencyExchangerAPITest.UnitTest
{
    public class TestingCryptocurrencyExchangeProviderFromTwoProviderUnitTest
    {
        private readonly string appsettingName = "appsettingsTwoProviders.json";
        private ServiceProvider ServiceProvider => ServiceProviderFactory.Get(appsettingName);
        [Fact]
        public void TestLoadConfigurationTwoProvider()
        {
            // Arrange 

            // Act 
            var config = ServiceProvider.GetExchangeratesAPIConfiguration().Value;

            // Assert  
            config.Should().NotBeNull();
            config.MainCryptocurrencyProvider.Should().NotBeNull();
            config.MainCryptocurrencyProvider.Name.Should().NotBeNullOrEmpty();
            config.MainCryptocurrencyProvider.TargetedCurrencies.Should().HaveCountGreaterThan(0);

            config.AdditionalFiatCurrencyProvider.Should().NotBeNull();
            config.AdditionalFiatCurrencyProvider.Name.Should().NotBeNullOrEmpty();
            config.AdditionalFiatCurrencyProvider.TargetedCurrencies.Should().HaveCountGreaterThan(0);
        }
        [Fact]
        public void TestRequestExchangeRateFromTwoProvider()
        {
            // Arrange 
            var provider = ServiceProvider.GetCryptocurrencyExchangeProvider();
            var config = ServiceProvider.GetExchangeratesAPIConfiguration().Value;
            
            // Act 
            var results = provider.GetExchangeRateListAsync("BTC").Result;

            // Assert  
            results.BaseCurrencySymbol.Should().Be("BTC");
            results.CurrenciesRates.Should().NotBeNull().And.ContainKeys(config.AdditionalFiatCurrencyProvider.TargetedCurrencies);
            results.CurrenciesRates.Where(e => e.Value == 0).Any().Should().BeFalse();
        }

    }
}
