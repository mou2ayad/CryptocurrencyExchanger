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
        private ServiceProvider serviceProvider => ServiceProviderFactory.Get(appsettingName);
        [Fact]
        public void TestLoadConfigurationTwoProvider()
        {
            // Arrange 

            // Act 
            var config = serviceProvider.GetExchangeratesAPIConfiguration().Value;

            // Assert  
            config.Should().NotBeNull();
            config.MainCryptocurrencyProvider.Should().NotBeNull();
            config.MainCryptocurrencyProvider.Name.Should().NotBeNullOrEmpty();
            config.MainCryptocurrencyProvider.RefreshCurrenyMapInSeconds.Should().BeGreaterThan(0);
            config.MainCryptocurrencyProvider.TargetedCurrencies.Should().HaveCountGreaterThan(0);

            config.AdditionalFiatCurrencyProvider.Should().NotBeNull();
            config.AdditionalFiatCurrencyProvider.Name.Should().NotBeNullOrEmpty();
            config.AdditionalFiatCurrencyProvider.RefreshCurrenyMapInSeconds.Should().BeGreaterThan(0);
            config.AdditionalFiatCurrencyProvider.TargetedCurrencies.Should().HaveCountGreaterThan(0);
        }
        [Fact]
        public void TestRequestExchangeRateFromTwoProvider()
        {
            // Arrange 
            var provider = serviceProvider.GetCryptocurrencyExchangeProvider();
            var config = serviceProvider.GetExchangeratesAPIConfiguration().Value;
            
            // Act 
            var results = provider.GetExchangeRateListAsync("BTC").Result;

            // Assert  
            results.BaseCurrencySymbol.Should().Be("BTC");
            results.CurrenciesRates.Should().NotBeNull().And.ContainKeys(config.AdditionalFiatCurrencyProvider.TargetedCurrencies);
            results.CurrenciesRates.Where(e => e.Value == 0).Any().Should().BeFalse();
        }

    }
}
