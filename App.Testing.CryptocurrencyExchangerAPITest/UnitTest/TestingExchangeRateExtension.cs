using App.Components.Contracts.Models;
using App.CryptocurrencyExchangerAPI.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace App.Testing.CryptocurrencyExchangerAPITest
{
    public class TestingExchangeRateExtension
    {
        [Fact]
        public void TestAppendingNULLExchangeRate()
        {
            //Arrange
            var firstRateList = new Dictionary<string, decimal>
            {
                { "BRL", (decimal)39471.159 },
                { "EUR", (decimal)8708.442 },
                { "USD", (decimal)9558.551 }
            };
            var mainExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "BTC", CurrenciesRates = new Dictionary<string, decimal>(firstRateList) };            

            //Act
            mainExchageRate.Append(null, false);

            //Assert
            mainExchageRate.Should().NotBeNull();
            mainExchageRate.BaseCurrencySymbol.Should().Be("BTC");
            mainExchageRate.CurrenciesRates.Should().HaveCount(3).And.ContainKeys(firstRateList.Keys); ;
            mainExchageRate.CurrenciesRates["BRL"].Should().Be((decimal)(39471.159));
            mainExchageRate.CurrenciesRates["USD"].Should().Be((decimal)(9558.551));

        }
        [Fact]
        public void TestAppendingEmptyExchangeRate()
        {
            //Arrange
            var firstRateList = new Dictionary<string, decimal>
            {
                { "BRL", (decimal)39471.159 },
                { "EUR", (decimal)8708.442 },
                { "USD", (decimal)9558.551 }
            };
            var mainExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "BTC", CurrenciesRates = new Dictionary<string, decimal>(firstRateList) };
            var SecondExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "EUR" };

            //Act
            mainExchageRate.Append(SecondExchageRate);


            //Assert
            mainExchageRate.Should().NotBeNull();
            mainExchageRate.BaseCurrencySymbol.Should().Be("BTC");
            mainExchageRate.CurrenciesRates.Should().HaveCount(3).And.ContainKeys(firstRateList.Keys); ;
            mainExchageRate.CurrenciesRates["BRL"].Should().Be((decimal)(39471.159));
            mainExchageRate.CurrenciesRates["USD"].Should().Be((decimal)(9558.551));

        }
        [Fact]
        public void TestAppendingExchangeRateWithBaseCurrencyNotInMainListCurrenciesRates()
        {
            //Arrange
            var firstRateList = new Dictionary<string, decimal>
            {
                { "BRL", (decimal)39471.159 },
                { "EUR", (decimal)8708.442 },
                { "USD", (decimal)9558.551 }
            };
            var mainExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "BTC", CurrenciesRates = new Dictionary<string, decimal>(firstRateList) };

            //Act
            var SecondRateList = new Dictionary<string, decimal>
            {
                { "CAD", (decimal)1.5344 },
                { "HKD", (decimal)9.29 },
                { "AUD", (decimal)1.5761 },
                { "USD", (decimal)1.1983 }
            };
            var SecondExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "MYR", CurrenciesRates = new Dictionary<string, decimal>(SecondRateList) };

            //Assert
            Action act =()=>  mainExchageRate.Append(SecondExchageRate);
            act.Should().Throw<Exception>().Where(e=>e.Message.StartsWith("The ExchangeRatesList can't be appended"));
            
        }
        [Fact]
        public void TestAppendingExchangeRate_WithEmptyAppendedList()
        {
            //Arrange
            var firstRateList = new Dictionary<string, decimal>
            {
                { "BRL", (decimal)39471.159 },
                { "EUR", (decimal)8708.442 },
                { "USD", (decimal)9558.551 }
            };
            var mainExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "BTC", CurrenciesRates = new Dictionary<string, decimal>(firstRateList) };
            var SecondExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "EUR" };

            //Act
            mainExchageRate.Append(SecondExchageRate);

            //Assert
            mainExchageRate.Should().NotBeNull();
            mainExchageRate.BaseCurrencySymbol.Should().Be("BTC");
            mainExchageRate.CurrenciesRates.Should().HaveCount(3).And.ContainKeys(firstRateList.Keys); ;
            mainExchageRate.CurrenciesRates["BRL"].Should().Be((decimal)(39471.159));
            mainExchageRate.CurrenciesRates["USD"].Should().Be((decimal)(9558.551));

        }
        [Fact]
        public void TestAppendingExchangeRate_WithoutOverwrite()
        {
            //Arrange
            var firstRateList = new Dictionary<string, decimal>
            {
                { "BRL", (decimal)39471.159 },
                { "EUR", (decimal)8708.442 },
                { "USD", (decimal)9558.551 }
            };
            var mainExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "BTC", CurrenciesRates =new Dictionary<string, decimal>(firstRateList) };

            var SecondRateList = new Dictionary<string, decimal>
            {
                { "CAD", (decimal)1.5344 },
                { "HKD", (decimal)9.29 },
                { "AUD", (decimal)1.5761 },
                { "USD", (decimal)1.1983 }
            };
            var SecondExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "EUR", CurrenciesRates = new Dictionary<string, decimal>(SecondRateList) };

            //Act
            mainExchageRate.Append(SecondExchageRate, false);

            //Assert
            mainExchageRate.Should().NotBeNull();
            mainExchageRate.BaseCurrencySymbol.Should().Be("BTC");
            mainExchageRate.CurrenciesRates.Should().HaveCount(6).And.ContainKeys(SecondRateList.Keys);
            mainExchageRate.CurrenciesRates["CAD"].Should().Be((decimal)(1.5344 * 8708.442));
            mainExchageRate.CurrenciesRates["USD"].Should().Be((decimal)(9558.551));

        }
        [Fact]
        public void TestAppendingExchangeRate_WithOverwrite()
        {
            //Arrange
            var firstRateList = new Dictionary<string, decimal>
            {
                { "BRL", (decimal)39471.159 },
                { "EUR", (decimal)8708.442 },
                { "USD", (decimal)9558.551 }
            };
            var mainExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "BTC", CurrenciesRates = new Dictionary<string, decimal>(firstRateList) };

            var SecondRateList = new Dictionary<string, decimal>
            {
                { "CAD", (decimal)1.5344 },
                { "HKD", (decimal)9.29 },
                { "AUD", (decimal)1.5761 },
                { "USD", (decimal)1.3 }
            };
            var SecondExchageRate = new ExchangeRatesList() { BaseCurrencySymbol = "EUR", CurrenciesRates = new Dictionary<string, decimal>(SecondRateList) };

            //Act
            mainExchageRate.Append(SecondExchageRate, true);

            //Assert
            mainExchageRate.Should().NotBeNull();
            mainExchageRate.BaseCurrencySymbol.Should().Be("BTC");
            mainExchageRate.CurrenciesRates.Should().HaveCount(6).And.ContainKeys(SecondRateList.Keys);
            mainExchageRate.CurrenciesRates["CAD"].Should().Be((decimal)(1.5344 * 8708.442));
            mainExchageRate.CurrenciesRates["USD"].Should().Be((decimal)(1.3 * 8708.442));

        }

    }
}
