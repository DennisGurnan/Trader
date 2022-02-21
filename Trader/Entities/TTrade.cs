using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi.V1;

namespace Trader.Entities
{
    public class TTrade
    {
        public OrderDirection BuySell { get; set; }
        public double DealPrice { get; set; }
        public TInstrument Instrument { get; set; }
        public double Quantity { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}
