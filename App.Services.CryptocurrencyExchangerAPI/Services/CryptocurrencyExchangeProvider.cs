using App.Components.Contracts.Contracts;
using App.Components.Contracts.Models;
using App.CryptocurrencyExchangerAPI.Extensions;
using App.Services.CryptocurrencyExchangerAPI.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.CryptocurrencyExchangerAPI.Services
{
    public class CryptocurrencyExchangeProvider : ICryptocurrencyExchangeProvider
    {
        private readonly CryptocurrencyProvidersOptions _options;
        private readonly IEnumerable<IExchangeRatesProvider> _exchangeRatesProviders;
        public CryptocurrencyExchangeProvider(IOptions<CryptocurrencyProvidersOptions> Options, IEnumerable<IExchangeRatesProvider> exchangeRatesProviders)
        {
            _options = Options.Value;
            _exchangeRatesProviders = exchangeRatesProviders;
        }
        public async Task<ExchangeRatesList> GetExchangeRateListAsync(string BaseCryptocurrencySymbol)
            => await (_options.AdditionalFiatCurrencyProvider == null 
            ? GetExchangeRateListSingleProviderAsync(BaseCryptocurrencySymbol) 
            : GetExchangeRateListFromMultipleProvidersAsync(BaseCryptocurrencySymbol));
        
        private async Task<ExchangeRatesList> GetExchangeRateListFromMultipleProvidersAsync(string BaseCryptocurrencySymbol)
        {

            var mainProvider = GetProvider(_options.MainCryptocurrencyProvider.Name);
            var mainRequest = mainProvider.GetExchangeRatesList(BaseCryptocurrencySymbol, _options.MainCryptocurrencyProvider.TargetedCurrencies.ToArray());
            var secondProvider = GetProvider(_options.AdditionalFiatCurrencyProvider.Name);
            var secondRequest = secondProvider.GetExchangeRatesList(_options.MainCryptocurrencyProvider.TargetedCurrencies.First(), _options.AdditionalFiatCurrencyProvider.TargetedCurrencies.ToArray());
            var results = await Task.WhenAll(mainRequest, secondRequest);
            results[0].Append(results[1]);
            return results[0];

        }
        private async Task<ExchangeRatesList> GetExchangeRateListSingleProviderAsync(string BaseCryptocurrencySymbol)
            => await GetProvider(_options.MainCryptocurrencyProvider.Name)
            .GetExchangeRatesList(BaseCryptocurrencySymbol, _options.MainCryptocurrencyProvider.TargetedCurrencies.ToArray());
            

        public IExchangeRatesProvider GetProvider(string ProviderName)
            => _exchangeRatesProviders.FirstOrDefault(e => e.ServiceProviderName == ProviderName);
        
    }
}
