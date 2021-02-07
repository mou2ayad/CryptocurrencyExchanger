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
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        
    }

    public class CryptocurrencyComparer : IEqualityComparer<CoinmarketcapMapResponseData>
    {
        public bool Equals(CoinmarketcapMapResponseData x, CoinmarketcapMapResponseData y)
        {
            //First check if both object reference are equal then return true
            if (object.ReferenceEquals(x, y))
                return true;
            
            //If either one of the object refernce is null, return false
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;
            // compare only symbol
            return x.symbol == y.symbol;
        }
        public int GetHashCode(CoinmarketcapMapResponseData obj)
        {
            //If obj is null then return 0
            if (obj == null)
                return 0;

            //Get the symbol HashCode Value
            //Check for null refernece exception
            int symbolHashCode = obj.symbol == null ? 0 : obj.symbol.GetHashCode();
            return symbolHashCode;
        }
    }


}
