using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.CryptocurrencyExchangerAPI.Models
{
    public class CryptocurrencyProvidersOptions
    {
        public CurrencyexchangeProviderOptions MainCryptocurrencyProvider { set; get; }
        public CurrencyexchangeProviderOptions AdditionalFiatCurrencyProvider { set; get; }
    }
    public class CurrencyexchangeProviderOptions
    {
        public string Name { set; get; }
        public List<string> TargetedCurrencies { set; get; }
        public int RefreshCurrenyMapInSeconds { set; get; }
    }
}

