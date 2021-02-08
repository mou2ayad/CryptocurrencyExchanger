using App.Components.Contracts.Contracts;
using App.Components.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.CryptocurrencyExchangerAPI.Services
{   
    public interface ICryptocurrencyExchangeProvider
    {
        Task<ExchangeRatesList> GetExchangeRateListAsync(string BaseCryptocurrencySymbol);
       
    }
}
