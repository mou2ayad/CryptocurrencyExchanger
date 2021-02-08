using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace App.Testing.CryptocurrencyExchangerAPITest.UnitTest
{
    public class TestingCryptocurrencyExchangeProviderOneProviderUnitTest
    {
        private readonly string appsettingName = "appsettingsOneProvider.json";
        private ServiceProvider serviceProvider => ServiceProviderFactory.Get(appsettingName);
        [Fact]
        public void TestLoadConfigurationOneProvider()
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
        }
        [Fact]
        public void TestRequestExchangeRateFromOneProvider()
        {
            // Arrange 
            var provider = serviceProvider.GetCryptocurrencyExchangeProvider();
            var config = serviceProvider.GetExchangeratesAPIConfiguration().Value;
            
            // Act 
            var results = provider.GetExchangeRateListAsync("BTC").Result;

            // Assert  
            results.BaseCurrencySymbol.Should().Be("BTC");
            results.CurrenciesRates.Should().NotBeNull().And.ContainKeys(config.MainCryptocurrencyProvider.TargetedCurrencies);
            results.CurrenciesRates.Where(e => e.Value == 0).Any().Should().BeFalse();
        }

    }
}
