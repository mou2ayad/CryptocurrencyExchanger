using App.Components.Contracts.Contracts;
using App.Components.Contracts.Models;
using App.Components.CoinmarketcapApiClient.Config;
using App.Components.CoinmarketcapApiClient.Model;
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
using System.Linq;
using Microsoft.Extensions.Logging;

namespace App.Components.CoinmarketcapApiClient
{
    public class CoinmarketcapAPIProvider : IExchangeRatesProvider
    {
        protected readonly CoinmarketcapApiOptions _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CoinmarketcapAPIProvider> _logger;
        protected Dictionary<string,int> supportedCryptoCurrencies;

        public string ServiceProviderName => "CoinmarketcapAPI";

        public CoinmarketcapAPIProvider(IOptions<CoinmarketcapApiOptions> options,IHttpClientFactory httpClientFactory, ILogger<CoinmarketcapAPIProvider> logger)
        {
            _config = options.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            supportedCryptoCurrencies = new Dictionary<string, int>();
            LoadSupportedCurrencies();

        }
        public virtual async Task<ExchangeRatesList> GetExchangeRatesList(string BaseCurrencySymbol, params string[] TargetedCurrencies)
        {
            if (!supportedCryptoCurrencies.ContainsKey(BaseCurrencySymbol))
                throw new InvalidRequestException($"{BaseCurrencySymbol} is invalid or Unsupported Cryptocurrency");
            if (TargetedCurrencies == null || TargetedCurrencies.Length == 0)
                TargetedCurrencies = _config.DefaultTargetedCurrencies.ToArray();
            else
            {
                var unsupportedCurrencies = TargetedCurrencies.Except(_config.SupportedTargetedCurrencies);
                if (unsupportedCurrencies.Count() == TargetedCurrencies.Length)
                    throw new InvalidRequestException($"[{string.Join(",", unsupportedCurrencies)}] are Unsupported fiat currencies");
                if (unsupportedCurrencies.Any())
                {
                    _logger.LogWarning("[{0}] are invalid or Unsupported fiat currencies", string.Join(",", unsupportedCurrencies));
                    TargetedCurrencies = TargetedCurrencies.Except(unsupportedCurrencies).ToArray();
                }
            }
            string cryptoCurId = supportedCryptoCurrencies[BaseCurrencySymbol].ToString();
            var query = HttpUtility.ParseQueryString(String.Empty);
            query["id"] = cryptoCurId;
            query["convert"] = string.Join(",", TargetedCurrencies);
            query["aux"] = "is_active"; // adding this parameter to reduce the size of the response
            var request = new HttpRequestMessage(HttpMethod.Get, $"/{_config.Version}/{_config.QuotesEndpoint}?{query}");
            request.Headers.Add("X-CMC_PRO_API_KEY", "10e765f4-bcd3-4ede-b9a8-0c7c550181e7");
            var httpclient = _httpClientFactory.CreateClient(ServiceProviderName);
            var response = await httpclient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var CoinmarketcapAPIResponse = JsonConvert.DeserializeObject<CoinmarketcapAPIResponse>(responseString);
            if (response.IsSuccessStatusCode)
            {
                if (CoinmarketcapAPIResponse?.Status?.ErrorCode == 0
                    && CoinmarketcapAPIResponse.Status.ErrorMessage == null
                    && CoinmarketcapAPIResponse.Status.CreditCount > 0
                    && CoinmarketcapAPIResponse.Data?.Count > 0
                    && CoinmarketcapAPIResponse.Data.Count > 0
                    && CoinmarketcapAPIResponse.Data.ContainsKey(cryptoCurId)
                    )
                {
                    return new ExchangeRatesList()
                    {
                        BaseCurrencySymbol = CoinmarketcapAPIResponse.Data[cryptoCurId].CryptoCurrencySymbol,
                        CurrenciesRates = CoinmarketcapAPIResponse.Data[cryptoCurId].Quote.Where(e => TargetedCurrencies.Contains(e.Key)).ToDictionary(key => key.Key, val => val.Value.Rate)
                    };
                }

            }
            throw new RestAPIException(ServiceProviderName, response.StatusCode, CoinmarketcapAPIResponse.Status.ErrorMessage);
        }

        public Task<ICollection<string>> LoadSupportedCurrencies()
        {
            return Task.Run(() =>
            {
                supportedCryptoCurrencies = new Dictionary<string, int>();
                supportedCryptoCurrencies.Add("BTC", 1);
                return supportedCryptoCurrencies.Keys as ICollection<string>;
                throw new Exception("supportedCryptoCurrencies Attribute is missing in the Config");
            });
        }
    }
}
