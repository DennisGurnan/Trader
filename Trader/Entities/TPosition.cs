using System.Linq;
using Trader.GUI;
using Trader.MVVM;

namespace Trader.Entities
{
    public class TPosition:ObservableObject
    {
        private decimal _Quantity;
        private decimal _AveragePositionPrice;
        private decimal _ExpectedYield;
        private decimal _Blocked;
        
        public decimal Quantity { get => _Quantity; set { _Quantity = value; RaisePropertyChangedEvent("Quantity"); } }
        public decimal AveragePositionPrice { get => _AveragePositionPrice; set { _AveragePositionPrice = value; RaisePropertyChangedEvent("AveragePositionPrice"); } }
        public decimal ExpectedYield { get => _ExpectedYield; set { _ExpectedYield = value; RaisePropertyChangedEvent("ExpectedYield"); } }
        public decimal Blocked { get => _Blocked; set { _Blocked = value; RaisePropertyChangedEvent("Blocked"); } }

        public string Ticker { get => Instrument.Ticker; }
        public string Name { get => Instrument.Name; }
        public string InstrumentType { get => Instrument.InstrumentType; }
        public decimal CurrentPrice { get => Instrument.LastPrice; }
        public string Currency { get => Instrument.Currency; }
        public decimal Cost { get => _Quantity * _AveragePositionPrice; }
        public decimal FullCost { get => _Quantity * Instrument.LastPrice; }
        public decimal ExpectedYieldPercent { get => ((_Quantity == 0)||(_AveragePositionPrice == 0))? 0 :_ExpectedYield / (_Quantity * _AveragePositionPrice) * 100; }

        public TInstrument Instrument;

        public TPosition(string figi)
        {
            Instrument = InstrumentsControl.Instance.Instruments.GetByFigi(figi);
            if (Instrument == null) Instrument = new TInstrument(figi);
        }
    }
}
