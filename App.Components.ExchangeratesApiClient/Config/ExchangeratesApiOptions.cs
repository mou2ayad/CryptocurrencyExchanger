using System;
using System.Collections.Generic;
using System.Text;

namespace App.Components.ExchangeratesApiClient.Config
{
    public class ExchangeratesApiOptions
    {
        public string ServiceBaseUrl { set; get; }
        public List<string> SupportedCurrencies { set; get; }
        public string ExchangeRateEndpoint { set; get; }
        public bool EnableCaching { set; get; }
        public int ExpiredAfterInMinutes { set; get; }

    }
}
