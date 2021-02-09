
using App.Components.Utilities.RecurringJobs;
using App.Services.CryptocurrencyExchangerAPI.Models;
using App.Services.CryptocurrencyExchangerAPI.Services;
using Microsoft.Extensions.Options;


namespace App.Services.CryptocurrencyExchangerAPI.RecurringJobService
{
    public class RecurringJobsBuilder : IRecurringJobsBuilder
    {
        private readonly IRecurringJobService _recurringJobService;
        private readonly CryptocurrencyProvidersOptions _options;
        private readonly ICryptocurrencyExchangeProvider _cryptocurrencyExchangeProvider;
        private readonly IExchangeRateProviderRecuringJobAdapter _exchangeRateProviderRecuringJobAdapter;
        public RecurringJobsBuilder(IRecurringJobService recurringJobService, IOptions<CryptocurrencyProvidersOptions> Options, ICryptocurrencyExchangeProvider cryptocurrencyExchangeProvider, IExchangeRateProviderRecuringJobAdapter exchangeRateProviderRecuringJobAdapter)
        {
            _options = Options.Value;
            _cryptocurrencyExchangeProvider = cryptocurrencyExchangeProvider;
            _recurringJobService = recurringJobService;
            _exchangeRateProviderRecuringJobAdapter = exchangeRateProviderRecuringJobAdapter;

        }
        private void CreateReloadingCurrenciesMapJob(CurrencyexchangeProviderOptions CurrencyProviderOption)
        {

            if (CurrencyProviderOption != null && !string.IsNullOrEmpty(CurrencyProviderOption.RefreshCurrenyMapCronExpression))
            {                
                var currencyExchangeProvider = _cryptocurrencyExchangeProvider.GetProvider(CurrencyProviderOption.Name);
                _recurringJobService.AddOrUpdate($"ReloadingCurrenciesMap_{CurrencyProviderOption.Name}_Job",
                   () => _exchangeRateProviderRecuringJobAdapter.RunJob(CurrencyProviderOption.Name), CurrencyProviderOption.RefreshCurrenyMapCronExpression);
            }
        }
        public void Build()
        {
            // creat RecurringJob to LoadSupportedCurrencies from MainCryptocurrency Provider
            CreateReloadingCurrenciesMapJob(_options.MainCryptocurrencyProvider);

            // creat RecurringJob to LoadSupportedCurrencies from FiatCurrency Provider
            CreateReloadingCurrenciesMapJob(_options.AdditionalFiatCurrencyProvider);

          

                                  
        }       
    }
}
