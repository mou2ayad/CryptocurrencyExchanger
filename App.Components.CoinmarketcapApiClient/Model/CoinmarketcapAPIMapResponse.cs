using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.Components.CoinmarketcapApiClient.Model
{
    public class CoinmarketcapAPIMapResponse
    {
        [JsonProperty("status")]
        public CoinmarketcapResponseStatus Status { set; get; }
        [JsonProperty("data")]
        public List<CoinmarketcapMapResponseData> Data { set; get; }
    }

    public class CoinmarketcapMapResponseData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        
    }

    public class CryptocurrencyComparer : IEqualityComparer<CoinmarketcapMapResponseData>
    {
        public bool Equals(CoinmarketcapMapResponseData x, CoinmarketcapMapResponseData y)
        {
            //First check if both object reference are equal then return true
            if (ReferenceEquals(x, y))
                return true;
            
            //If either one of the object refernce is null, return false
            if (x is null || y is null)
                return false;
            // compare only symbol
            return x.Symbol == y.Symbol;
        }
        public int GetHashCode(CoinmarketcapMapResponseData obj)
        {
            //If obj is null then return 0
            if (obj == null)
                return 0;

            //Get the symbol HashCode Value
            //Check for null refernece exception
            int symbolHashCode = obj.Symbol == null ? 0 : obj.Symbol.GetHashCode();
            return symbolHashCode;
        }
    }


}
