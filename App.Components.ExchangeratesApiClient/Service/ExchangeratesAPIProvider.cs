using App.Components.Contracts.Contracts;
using App.Components.Contracts.Models;
using App.Components.ExchangeratesApiClient.Config;
using App.Components.ExchangeratesApiClient.Model;
using App.Components.Utilities.APIClient;
using App.Components.Utilities.CustomException;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace App.Components.ExchangeratesApiClient
{
    public class ExchangeratesAPIProvider : IExchangeRatesProvider
    {
        protected readonly ExchangeratesApiOptions _config;
        private readonly IHttpClientFactory _httpClientFactory;
        
        protected List<string> supportedCurrencies;

        public string ServiceProviderName => "exchangeratesapi.io";

        public ExchangeratesAPIProvider(IOptions<ExchangeratesApiOptions> options,IHttpClientFactory httpClientFactory)
        {
            _config = options.Value;
            _httpClientFactory = httpClientFactory;
            LoadSupportedCurrencies();

        }
        public virtual async Task<ExchangeRatesList> GetExchangeRatesList(string BaseCurrencySymbol, params string[] TargetedCurrencies)
        {
            if (!supportedCurrencies.Contains(BaseCurrencySymbol))
                throw new InvalidRequestException($"{BaseCurrencySymbol} is Unsupported currency");
            if (TargetedCurrencies == null || TargetedCurrencies.Length == 0)
                TargetedCurrencies = supportedCurrencies.ToArray();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["base"] = BaseCurrencySymbol;
            query["symbols"] = string.Join(",", TargetedCurrencies);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/{_config.ExchangeRateEndpoint}?{query}");
            var httpclient = _httpClientFactory.CreateClient(ServiceProviderName);
            var response = await httpclient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var exchangeratesAPIResponse = JsonConvert.DeserializeObject<ExchangeratesAPIResponse>(responseString);
                return new ExchangeRatesList()
                {
                    BaseCurrencySymbol = exchangeratesAPIResponse.BaseCurrency,
                    CurrenciesRates = exchangeratesAPIResponse.Rates
                };
            }
            throw new RestAPIException(ServiceProviderName, response.StatusCode, responseString);
        }

        public Task<List<string>> LoadSupportedCurrencies()
        {
            return Task.Run(() =>
            {
                if(_config.SupportedCurrencies?.Count > 0)
                {
                    supportedCurrencies = _config.SupportedCurrencies ;
                    return supportedCurrencies;
                }
                throw new Exception("SupportedCurrencies Attribute is missing in the Config");
            });
        }
    }
}
