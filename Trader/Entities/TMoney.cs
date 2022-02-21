using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader.MVVM;

namespace Trader.Entities
{
    public class TMoney : ObservableObject
    {
        private decimal _Price;
        public decimal Price { get => _Price; set { _Price = value; RaisePropertyChangedEvent("Price"); } }
        private string _Currency;
        public string Currency { get => _Currency; set { _Currency = value; RaisePropertyChangedEvent("Currency"); } }
        public TMoney(decimal price, string currency)
        {
            _Price = price;
            _Currency = currency;
        }
    }
}
