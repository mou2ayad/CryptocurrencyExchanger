using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptocurrencyExchanger
{

    public class Rootobject
    {
        public Status status { get; set; }
        public Data data { get; set; }
    }

    public class Status
    {
        public DateTime timestamp { get; set; }
        public int error_code { get; set; }
        public object error_message { get; set; }
        public int elapsed { get; set; }
        public int credit_count { get; set; }
        public object notice { get; set; }
    }

    public class Data
    {
        public _1 _1 { get; set; }
    }

    public class _1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string slug { get; set; }
        public int is_active { get; set; }
        public int is_market_cap_included_in_calc { get; set; }
        public DateTime last_updated { get; set; }
        public Quote quote { get; set; }
    }

    public class Quote
    {
        public USD USD { get; set; }
    }

    public class USD
    {
        public float price { get; set; }
        public float volume_24h { get; set; }
        public float percent_change_1h { get; set; }
        public float percent_change_24h { get; set; }
        public float percent_change_7d { get; set; }
        public float market_cap { get; set; }
        public DateTime last_updated { get; set; }
    }

}
