using System;
using System.Collections.Generic;
using System.Text;

namespace App.Components.Contracts.Models
{
    public class ExchangeRatesList
    {
        public ExchangeRatesList()
        {
            CurrenciesRates = new Dictionary<string, decimal>();
        }
        public string BaseCurrencySymbol { set; get; }
        public Dictionary<string,decimal> CurrenciesRates { set; get; }
    }
}
