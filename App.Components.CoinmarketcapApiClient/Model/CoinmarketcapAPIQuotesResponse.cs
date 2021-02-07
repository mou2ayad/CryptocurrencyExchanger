using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.Components.CoinmarketcapApiClient.Model
{
    public class CoinmarketcapAPIQuotesResponse
    {
        [JsonProperty("status")]
        public CoinmarketcapResponseStatus Status { set; get; }
        [JsonProperty("data")]
        public Dictionary<string, CoinmarketcapResponseQuotesDataCryptoCurrency> Data { set; get; }
    }
   
    public class CoinmarketcapResponseQuotesDataCryptoCurrency
    {
        [JsonProperty("id")]
        public int CryptoCurrencyId { set; get; }

        [JsonProperty("name")]
        public string CryptoCurrencyName { set; get; }

        [JsonProperty("symbol")]
        public string CryptoCurrencySymbol { set; get; }

        [JsonProperty("slug")]
        public string CryptoCurrencySlug { set; get; }

        [JsonProperty("is_active")]
        public int IsActive { set; get; }

        [JsonProperty("quote")]
        public Dictionary<string, CoinmarketcapResponseDataCryptoCurrencyQuoteCurrencyRate> Quote { set; get; }

    }

    public class CoinmarketcapResponseDataCryptoCurrencyQuoteCurrencyRate
    {
        [JsonProperty("price")]
        public decimal Rate { set; get; }

    }


}
