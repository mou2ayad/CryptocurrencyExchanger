using System;
using System.Collections.Generic;
using System.Text;

namespace App.Components.Contracts.Models
{
    public class ExchangeRatesList
    {
        public string BaseCurrencySymbol { set; get; }
        public List<ExchangeRate> CurrenciesRates { set; get; }
    }
}
