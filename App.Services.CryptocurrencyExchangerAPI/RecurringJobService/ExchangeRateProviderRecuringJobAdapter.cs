using App.Components.Contracts.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace App.Services.CryptocurrencyExchangerAPI.RecurringJobService
{
    public class ExchangeRateProviderRecuringJobAdapter : IExchangeRateProviderRecuringJobAdapter
    {
        private readonly IEnumerable<IExchangeRatesProvider> _exchangeRatesProviders;
        public ExchangeRateProviderRecuringJobAdapter(IEnumerable<IExchangeRatesProvider> exchangeRatesProviders)
        {
            _exchangeRatesProviders = exchangeRatesProviders;

        }
        public void RunJob(string ProviderName)
        {
            _exchangeRatesProviders.FirstOrDefault(e => e.ServiceProviderName == ProviderName)?.LoadSupportedCurrencies();
        }

    }
}
