using App.Components.Contracts.Contracts;
using App.Components.Contracts.Models;
using App.Components.ExchangeratesApiClient.Config;
using App.Components.ExchangeratesApiClient.Model;
using App.Components.Utilities.APIClient;
using App.Components.Utilities.CustomException;
using App.Components.Utilities.Logger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ILogger<ExchangeratesAPIProvider> _logger;
        
        protected HashSet<string> supportedCurrencies;

        public string ServiceProviderName => "exchangeratesapi.io";

        public ExchangeratesAPIProvider(IOptions<ExchangeratesApiOptions> options,IHttpClientFactory httpClientFactory, ILogger<ExchangeratesAPIProvider> logger)
        {
            _config = options.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            LoadSupportedCurrencies().Wait();

        }
        public virtual async Task<ExchangeRatesList> GetExchangeRatesList(string BaseCurrencySymbol, params string[] TargetedCurrencies)
        {
            // validate the input parameters
            BaseCurrencySymbol = BaseCurrencySymbol.ToUpper();
            TargetedCurrencies = TargetedCurrencies.Select(e => e.ToUpper()).ToArray();
            if (!supportedCurrencies.Contains(BaseCurrencySymbol))
                throw new InvalidRequestException($"{BaseCurrencySymbol} is Unsupported currency");
            if (TargetedCurrencies == null || TargetedCurrencies.Length == 0)
                TargetedCurrencies = _config.DefaultTargetedCurrencies.ToArray();
            else
            {
                var unsupportedCurrencies = TargetedCurrencies.Except(supportedCurrencies);
                if (unsupportedCurrencies.Count() == TargetedCurrencies.Length)
                    throw new InvalidRequestException($"[{string.Join(",", unsupportedCurrencies)}] are Unsupported currencies");
                if (unsupportedCurrencies.Any())
                {
                    _logger.LogWarning("[{0}] are invalid or Unsupported currencies", string.Join(",", unsupportedCurrencies));
                    TargetedCurrencies = TargetedCurrencies.Except(unsupportedCurrencies).ToArray();
                }
            }
           
            // calling the currency exchange provider
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["base"] = BaseCurrencySymbol;
            query["symbols"] = string.Join(",", TargetedCurrencies);
            var exchangeratesAPIResponse = await SendRequestAsync(query.ToString());
            return new ExchangeRatesList()
            {
                BaseCurrencySymbol = exchangeratesAPIResponse.BaseCurrency,
                CurrenciesRates = exchangeratesAPIResponse.Rates
            };

        }

        public async Task<ICollection<string>> LoadSupportedCurrencies()
        {
            try
            {
                // call currency exchange provider to load all the available currencies 
                var exchangeratesAPIResponse = await SendRequestAsync(string.Empty);
                if (exchangeratesAPIResponse?.Rates?.Count > 0)
                {
                    supportedCurrencies =new HashSet<string>(exchangeratesAPIResponse.Rates.Keys.Select(e => e.ToUpper()));
                    if (!supportedCurrencies.Contains(exchangeratesAPIResponse.BaseCurrency.ToUpper()))
                        supportedCurrencies.Add(exchangeratesAPIResponse.BaseCurrency.ToUpper());
                }
            }
            catch (Exception ex)
            {
                // if the request fails, the exception should be logged and the supported currencies should be loaded from the config
                _logger.LogErrorDetails(ex);
                if (_config.SupportedCurrencies?.Count > 0)
                {
                    supportedCurrencies =new HashSet<string>(_config.SupportedCurrencies.Select(e=>e.ToUpper()));
                    return supportedCurrencies;
                }
                throw new Exception("SupportedCurrencies Attribute is missing in the Config");
            }
            return supportedCurrencies;
        }
        private async Task<ExchangeratesAPIResponse> SendRequestAsync(string query)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/{_config.ExchangeRateEndpoint}?{query}");
            var httpclient = _httpClientFactory.CreateClient(ServiceProviderName);
            var response = await httpclient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ExchangeratesAPIResponse>(responseString);
            throw new RestAPIException(ServiceProviderName, response.StatusCode, responseString);
        }
    }
}
