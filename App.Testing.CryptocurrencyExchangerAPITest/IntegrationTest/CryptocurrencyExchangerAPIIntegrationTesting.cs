using App.Components.Contracts.Models;
using App.Components.Utilities.ErrorHandling;
using App.Components.Utilities.JWT_Auth;
using CryptocurrencyExchanger;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace App.Testing.CryptocurrencyExchangerAPITest.IntegrationTest
{
    public class CryptocurrencyExchangerControllerIntegrationTesting : IntegrationTest
    {
        
        public CryptocurrencyExchangerControllerIntegrationTesting():base()
        {
            
        }
        

        [Fact]
        public async Task Testing_Quotes_With_Valid_InputAsync()
        {
            //Arrange
            string currencyInput = "BTC";
            
            await Authentication("knab","knab2021");
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/cryptocurrency/Quotes/{currencyInput}");

            //Act
            var response = await httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var exchangeRatesList = JsonConvert.DeserializeObject<ExchangeRatesList>(responseString);

            //Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            exchangeRatesList.Should().NotBeNull();
            exchangeRatesList.BaseCurrencySymbol.Should().Be("BTC");
            exchangeRatesList.CurrenciesRates.Should().HaveCountGreaterThan(0);
            exchangeRatesList.CurrenciesRates.Should().ContainKey("USD");

        }

        [Fact]
        public async Task Testing_Quotes_With_Invalid_InputAsync()
        {
            //Arrange
            string currencyInput = "SYB";
            await Authentication("knab", "knab2021");
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/cryptocurrency/Quotes/{currencyInput}");

            //Act
            var response = await httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var ErrorMessage = JsonConvert.DeserializeObject<HttpExceptionDetails>(responseString);

            //Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            ErrorMessage.StatusCode.Should().Be(400);
            ErrorMessage.ErrorMessage.Should().Be("SYB is invalid or Unsupported Cryptocurrency");           

        }
    }
}
