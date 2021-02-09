using App.Components.Contracts.Models;
using System;


namespace App.CryptocurrencyExchangerAPI.Extensions
{
    public static class ExchangeRatesListExtension
    {
        public static void Append(this ExchangeRatesList mainRate, ExchangeRatesList AppendedExchangeRate,bool WithOverwrite=false)
        {
            if (AppendedExchangeRate == null)
                return;
            decimal AppendedBaseCurrencyRate;
            if(!mainRate.CurrenciesRates.TryGetValue(AppendedExchangeRate.BaseCurrencySymbol,out AppendedBaseCurrencyRate))            
                throw new Exception("The ExchangeRatesList can't be appended because its BaseCurrencySymbol is doesn't exists in the main ExchangeRateList's CurrenciesRates");
            
            foreach(var rate in AppendedExchangeRate.CurrenciesRates)
            {
                var newRate = rate.Value * AppendedBaseCurrencyRate;
                if (!mainRate.CurrenciesRates.TryAdd(rate.Key, newRate) && WithOverwrite)
                    mainRate.CurrenciesRates[rate.Key] = newRate;
            }
            
            
        }

    }
}
