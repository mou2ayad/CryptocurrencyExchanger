using System;
using System.Collections.Generic;
using System.Text;

namespace App.Components.Contracts.Models
{
    public class ExchangeRate
    {
        public string Symbol { set; get; }
        public decimal Rate { set; get; }
    }
}
