using App.Components.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.Components.Contracts.Contracts
{
    public interface IExchangeRatesProvider
    {
        string ServiceProviderName { get; }
        Task<List<string>> LoadSupportedCurrencies();
        Task<ExchangeRatesList> GetExchangeRatesList(string BaseCurrencySymbol,params string[] TargetedCurrencies);
        
    }
}
