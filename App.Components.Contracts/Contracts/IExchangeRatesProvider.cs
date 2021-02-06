using App.Components.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.Components.Contracts.Contracts
{
    public interface IExchangeRatesProvider
    {
        Task LoadSupportedCurrencies();
        Task<ExchangeRatesList> GetExchangeRatesList(string BaseCurrencySymbol);
        
    }
}
