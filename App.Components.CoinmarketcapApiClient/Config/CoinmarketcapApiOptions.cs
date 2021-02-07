using System;
using System.Collections.Generic;
using System.Text;

namespace App.Components.CoinmarketcapApiClient.Config
{
    public class CoinmarketcapApiOptions
    {
        public string ServiceBaseUrl { set; get; }
        public string Version { set; get; }
        public string QuotesEndpoint { set; get; }
        public string MapEndpoint { set; get; }
        public List<string> SupportedTargetedCurrencies { set; get; }
        public List<string> DefaultTargetedCurrencies { set; get; }
        public bool EnableCaching { set; get; }
        public int ExpiredAfterInMinutes { set; get; }
    }
}
