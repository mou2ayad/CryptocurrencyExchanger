using App.Components.Contracts.Models;
using App.Components.Utilities.ErrorHandling;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Threading.Tasks;
using Xunit;

namespace App.Testing.CryptocurrencyExchangerAPITest.IntegrationTest
{
    public class CryptocurrencyExchangerControllerIntegrationTesting 
    {             

        [Fact]
        public async Task Testing_Quotes_With_Valid_InputAsync()
        {
            //Arrange
            string currencyInput = "BTC";
            var httpClient = AppClientFactory.GetClient();
            await httpClient.AuthenticationAsync("knab", "knab2021");
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
            var httpClient = AppClientFactory.GetClient();
            await httpClient.AuthenticationAsync("knab", "knab2021");
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

        [Fact]
        public async Task Testing_Quotes_Without_Authentication()
        {
            //Arrange
            string currencyInput = "BTC";            
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/cryptocurrency/Quotes/{currencyInput}");
            var httpClient = AppClientFactory.GetClient();

            //Act
            var response = await httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var exchangeRatesList = JsonConvert.DeserializeObject<ExchangeRatesList>(responseString);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseString.Should().Contain("Unauthenticated  Access");
            response.IsSuccessStatusCode.Should().BeFalse();
            exchangeRatesList.Should().NotBeNull();
            exchangeRatesList.BaseCurrencySymbol.Should().BeNull();
            exchangeRatesList.CurrenciesRates.Should().BeEmpty();

        }
        [Fact]
        public async Task Testing_Quotes_With_invalid_Authentication_Token()
        {
            //Arrange
            string currencyInput = "BTC";
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/cryptocurrency/Quotes/{currencyInput}");
            var httpClient = AppClientFactory.GetClient();            
            // add invalid bearer token value
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJTQnFKdmwzZ3dSWTBnaERob0p");


            //Act

            var response = await httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var exchangeRatesList = JsonConvert.DeserializeObject<ExchangeRatesList>(responseString);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseString.Should().Contain("Unauthenticated  Access");
            response.IsSuccessStatusCode.Should().BeFalse();
            exchangeRatesList.Should().NotBeNull();
            exchangeRatesList.BaseCurrencySymbol.Should().BeNull();
            exchangeRatesList.CurrenciesRates.Should().BeEmpty();

        }

        [Fact]
        public async Task Testing_Quotes_With_Valid_Authentication_Token_But_Unauthorized_Access()
        {
            //Arrange
            string currencyInput = "BTC";
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/cryptocurrency/Quotes/{currencyInput}");
            var httpClient = AppClientFactory.GetClient();
            // add valid user user but has no permission to Quotes endpoint
            await httpClient.AuthenticationAsync("test", "test2021");

            //Act

            var response = await httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var exchangeRatesList = JsonConvert.DeserializeObject<ExchangeRatesList>(responseString);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseString.Should().Contain("Unauthorized Access");
            response.IsSuccessStatusCode.Should().BeFalse();
            exchangeRatesList.Should().NotBeNull();
            exchangeRatesList.BaseCurrencySymbol.Should().BeNull();
            exchangeRatesList.CurrenciesRates.Should().BeEmpty();

        }

    }
}
