using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Components.CoinmarketcapApiClient.Model
{
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
}
