using App.Components.Contracts.Models;
using App.Components.ExchangeratesApiClient.Config;
using App.Components.Utilities.CustomException;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace App.Components.ExchangeratesApiClient
{
    public class ExchangeratesAPIProviderWithCache : ExchangeratesAPIProvider
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ExchangeratesAPIProvider> _logger;
       
        public ExchangeratesAPIProviderWithCache(IOptions<ExchangeratesApiOptions> options, IHttpClientFactory httpClientFactory,IMemoryCache memoryCache, ILogger<ExchangeratesAPIProvider> logger) : base(options, httpClientFactory, logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<ExchangeRatesList> GetExchangeRatesList(string BaseCurrencySymbol, params string[] TargetedCurrencies)
        {
            BaseCurrencySymbol = BaseCurrencySymbol.ToUpper();
            TargetedCurrencies = TargetedCurrencies.Select(e => e.ToUpper()).ToArray();
            if (!supportedCurrencies.Contains(BaseCurrencySymbol))
                throw new InvalidRequestException($"{BaseCurrencySymbol} is Unsupported currency");
            if (TargetedCurrencies == null || TargetedCurrencies.Length == 0)
                TargetedCurrencies = _config.DefaultTargetedCurrencies.ToArray();

            List<string> newTargets = new List<string>();
            ExchangeRatesList exchangeRatesList = new ExchangeRatesList() { BaseCurrencySymbol = BaseCurrencySymbol };
            foreach(var targetedcurrency in TargetedCurrencies)
            {
                decimal rate;
                if (_memoryCache.TryGetValue(GetCacheKey(BaseCurrencySymbol, targetedcurrency), out rate))
                    exchangeRatesList.CurrenciesRates.Add(targetedcurrency, rate);
                else
                    newTargets.Add(targetedcurrency);
            }
            if(newTargets.Count>0)
            {
                _logger.LogInformation($"Requesting ExchangeRates of {string.Join(",", newTargets)} from {ServiceProviderName} Provider");
                var rates=await base.GetExchangeRatesList(BaseCurrencySymbol, newTargets.ToArray());
                foreach (var rate in rates.CurrenciesRates)
                {
                    _memoryCache.Set(GetCacheKey(BaseCurrencySymbol, rate.Key), rate.Value, DateTimeOffset.Now.AddMinutes(_config.ExpiredAfterInMinutes));
                    exchangeRatesList.CurrenciesRates.Add(rate.Key, rate.Value);
                }

            }
            return exchangeRatesList;
        }
        private string GetCacheKey(string BaseCurrencySymbol, string targetedcurrency)
        => $"{ServiceProviderName}_{BaseCurrencySymbol}_{targetedcurrency}".ToLower();

    }
}
