using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi.V1;
using Google.Protobuf.WellKnownTypes;
using Trader.MVVM;

namespace Trader.Entities
{
    public class TOperation:ObservableObject
    {
        private string _Id;
        private string _ParentOperationId;
        private string _Currency;
        private string _Figi;
        private string _InstrumentType;
        private string _Type;
        private decimal _Payment;
        private decimal _Price;
        private Int64 _Quantity;
        private Int64 _QuantityRest;
        private DateTime _Date;
        private OperationState _State;
        private OperationType _OperationType;

        public string Id { get => _Id; set { _Id = value; RaisePropertyChangedEvent("Id") ; } }
        public string ParentOperationId { get => _ParentOperationId; set { _ParentOperationId = value; RaisePropertyChangedEvent("ParentOperationId"); } }
        public string Currency { get => _Currency; set { _Currency = value; RaisePropertyChangedEvent("Currency"); } }
        public string Figi { get => _Figi; set { _Figi = value; RaisePropertyChangedEvent("Figi"); } }
        public string InstrumentType { get => _InstrumentType; set { _InstrumentType = value; RaisePropertyChangedEvent("InstrumentType"); } }
        public string Type { get => _Type; set { _Type = value; RaisePropertyChangedEvent("Type"); } }
        public decimal Payment { get => _Payment; set { _Payment = value; RaisePropertyChangedEvent("Payment"); } }
        public decimal Price { get => _Price; set { _Price = value; RaisePropertyChangedEvent("Price"); } }
        public Int64 Quantity { get => _Quantity; set { _Quantity = value; RaisePropertyChangedEvent("Quantity"); } }
        public Int64 QuantityRest { get => _QuantityRest; set { _QuantityRest = value; RaisePropertyChangedEvent("QuantityRest"); } }
        public DateTime Date { get => _Date; set { _Date = value; RaisePropertyChangedEvent("Date"); } }
        public OperationState State { get => _State; set { _State = value; RaisePropertyChangedEvent("State"); } }
        public OperationType OperationType { get => _OperationType; set { _OperationType = value; RaisePropertyChangedEvent("OperationType"); } }

        public string InstrumentName
        {
            get
            {
                TInstrument i = GUI.InstrumentsControl.Instance.Instruments.GetByFigi(Figi);
                if (i != null) return i.Name;
                return "";
            }
        }

        public void TnkUpdate(Operation o)
        {
            Id = o.Id;
            ParentOperationId = o.ParentOperationId;
            Currency = o.Currency;
            Payment = Utils.Convertor.MoneyToDec(o.Payment);
            Price = Utils.Convertor.MoneyToDec(o.Price);
            State = o.State;
            Quantity = o.Quantity;
            QuantityRest = o.QuantityRest;
            Figi = o.Figi;
            InstrumentType = o.InstrumentType;
            Date = (o.Date == null) ? DateTime.MinValue : o.Date.ToDateTime();
            Type = o.Type;
            OperationType = o.OperationType;
        }
    }
}
