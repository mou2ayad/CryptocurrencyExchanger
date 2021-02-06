using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.Components.ExchangeratesApiClient.Model
{
    public class ExchangeratesAPIResponse
    {
        [JsonProperty("rates")]
        public Dictionary<string, decimal> Rates { set; get; }
        [JsonProperty("base")]
        public string BaseCurrency { set; get; }
    }
}
