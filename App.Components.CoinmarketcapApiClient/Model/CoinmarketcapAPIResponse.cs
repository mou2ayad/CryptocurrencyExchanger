using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.Components.CoinmarketcapApiClient.Model
{
    public class CoinmarketcapAPIResponse
    {
        [JsonProperty("status")]
        public CoinmarketcapResponseStatus Status { set; get; }
        [JsonProperty("data")]
        public Dictionary<string, CoinmarketcapResponseDataCryptoCurrency> Data { set; get; }
    }
    public class CoinmarketcapResponseStatus
    {
        [JsonProperty("error_code")]
        public int ErrorCode { set; get; }
        [JsonProperty("error_message")]
        public string ErrorMessage { set; get; }
        [JsonProperty("credit_count")]
        public int CreditCount { set; get; }
        [JsonProperty("notice")]
        public string Notice { set; get; }
       
    }
    //public class CoinmarketcapResponseData
    //{
    //    public Dictionary<string, CoinmarketcapResponseDataCryptoCurrency> CryptoCurrencyData { set; get; }

    //}
    public class CoinmarketcapResponseDataCryptoCurrency
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

        //[JsonProperty("percent_change_1h")]
        //public decimal Percent_Change_1h { set; get; }

        //[JsonProperty("percent_change_24h")]
        //public decimal Percent_Change_24h { set; get; }

        //[JsonProperty("percent_change_7d")]
        //public decimal Percent_Change_7d { set; get; }
    }


}
