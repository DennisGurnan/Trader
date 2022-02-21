using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Trader.MVVM;
using Binance.Net.Objects.Spot.UserStream;
using Binance.Net.Objects.Spot.SpotData;

namespace Trader.Entities
{
    public class TOrderStage
    {
        public decimal Price { get; set; }
        public Int64 Quantity { get; set; }
        public DateTime Time { get; set; }
    }

    public class TOrder : ObservableObject
    {
        private string _Id;
        private string _LastMessage;
        private string _Currency;
        private string _Figi;
        private Int64 _LotsRequested;
        private Int64 _LotsExecuted;
        private decimal _InitialOrderPrice;
        private decimal _ExecutedOrderPrice;
        private decimal _TotalOrderAmount;
        private decimal _AveragePositionPrice;
        private decimal _InitialCommission;
        private decimal _ExecutedCommission;
        private decimal _AciValue;
        private decimal _InitialSecurityPrice;
        private decimal _ServiceCommission;
        private OrderDirection _Direction;
        private OrderExecutionReportStatus _Status;
        private OrderType _Type;
        private DateTime _Date;
        private DateTime _CanseledTime;

        public string Id { get => _Id; set {_Id = value; RaisePropertyChangedEvent("Id"); } }
        public string LastMessage { get => _LastMessage; set { _LastMessage = value; RaisePropertyChangedEvent("LastMessage"); } }
        public string Currency { get => _Currency; set { _Currency = value; RaisePropertyChangedEvent("Currency"); } }
        public string Figi { get => _Figi; set { _Figi = value; RaisePropertyChangedEvent("Figi"); } }
        public Int64 LotsRequested { get => _LotsRequested; set { _LotsRequested = value; RaisePropertyChangedEvent("LotsRequested"); } }
        public Int64 LotsExecuted { get => _LotsExecuted; set { _LotsExecuted = value; RaisePropertyChangedEvent("LotsExecuted"); } }
        public decimal InitialOrderPrice { get => _InitialOrderPrice; set { _InitialOrderPrice = value; RaisePropertyChangedEvent("InitialOrderPrice"); } }
        public decimal ExecutedOrderPrice { get => _ExecutedOrderPrice; set { _ExecutedOrderPrice = value; RaisePropertyChangedEvent("ExecutedOrderPrice"); } }
        public decimal TotalOrderAmount { get => _TotalOrderAmount; set { _TotalOrderAmount = value; RaisePropertyChangedEvent("TotalOrderAmount"); } }
        public decimal AveragePositionPrice { get => _AveragePositionPrice; set { _AveragePositionPrice = value; RaisePropertyChangedEvent("AveragePositionPrice"); } }
        public decimal InitialCommission { get => _InitialCommission; set { _InitialCommission = value; RaisePropertyChangedEvent("InitialCommission"); } }
        public decimal ExecutedCommission { get => _ExecutedCommission; set { _ExecutedCommission = value; RaisePropertyChangedEvent("ExecutedCommission"); } }
        public decimal AciValue { get => _AciValue; set { _AciValue = value; RaisePropertyChangedEvent("AciValue"); } }
        public decimal InitialSecurityPrice { get => _InitialSecurityPrice; set { _InitialSecurityPrice = value; RaisePropertyChangedEvent("InitialSecurityPrice"); } }
        public decimal ServiceCommission { get => _ServiceCommission; set { _ServiceCommission = value; RaisePropertyChangedEvent("ServiceCommission"); } }
        public OrderDirection Direction { get => _Direction; set { _Direction = value; RaisePropertyChangedEvent("Direction"); } }
        public OrderExecutionReportStatus Status { get => _Status; set { _Status = value; RaisePropertyChangedEvent("Status"); } }
        public OrderType Type { get => _Type; set { _Type = value; RaisePropertyChangedEvent("Type"); } }
        public DateTime Date { get => _Date; set { _Date = value; RaisePropertyChangedEvent("Date"); } }
        public DateTime CanseledTime { get => _CanseledTime; set { _CanseledTime = value; RaisePropertyChangedEvent("CanseledTime"); } }
        private string _ListId;
        public string ListId { get => _ListId; set { _ListId = value; RaisePropertyChangedEvent("ListId"); } }
        public List<TOrderStage> Stages = new List<TOrderStage>();

        #region Updates
        public void TnkUpdate(OrderState order)
        {
            Id = order.OrderId;
            Status = order.ExecutionReportStatus;
            LotsRequested = order.LotsRequested;
            LotsExecuted = order.LotsExecuted;
            InitialOrderPrice = Utils.Convertor.MoneyToDec(order.InitialOrderPrice);
            ExecutedOrderPrice = Utils.Convertor.MoneyToDec(order.ExecutedOrderPrice);
            TotalOrderAmount = Utils.Convertor.MoneyToDec(order.TotalOrderAmount);
            AveragePositionPrice = Utils.Convertor.MoneyToDec(order.AveragePositionPrice);
            InitialCommission = Utils.Convertor.MoneyToDec(order.InitialCommission);
            ExecutedCommission = Utils.Convertor.MoneyToDec(order.ExecutedCommission);
            Figi = order.Figi;
            Direction = order.Direction;
            InitialSecurityPrice = Utils.Convertor.MoneyToDec(order.InitialSecurityPrice);
            ServiceCommission = Utils.Convertor.MoneyToDec(order.ServiceCommission);
            Currency = order.Currency;
            Type = order.OrderType;
            Date = order.OrderDate.ToDateTime();
        }

        public void TnkUpdate(PostOrderResponse order)
        {
            Id = order.OrderId;
            Status = order.ExecutionReportStatus;
            LotsRequested = order.LotsRequested;
            LotsExecuted = order.LotsExecuted;
            InitialOrderPrice = Utils.Convertor.MoneyToDec(order.InitialOrderPrice);
            ExecutedOrderPrice = Utils.Convertor.MoneyToDec(order.ExecutedOrderPrice);
            TotalOrderAmount = Utils.Convertor.MoneyToDec(order.TotalOrderAmount);
            InitialCommission = Utils.Convertor.MoneyToDec(order.InitialCommission);
            ExecutedCommission = Utils.Convertor.MoneyToDec(order.ExecutedCommission);
            AciValue = Utils.Convertor.MoneyToDec(order.AciValue);
            Figi = order.Figi;
            Direction = order.Direction;
            InitialSecurityPrice = Utils.Convertor.MoneyToDec(order.InitialSecurityPrice);
            Type = order.OrderType;
            LastMessage = order.Message;
        }

        private OrderType ConvertType(Binance.Net.Enums.OrderType type)
        {
            switch (type)
            {
                case Binance.Net.Enums.OrderType.Limit:
                    return OrderType.Limit;
                case Binance.Net.Enums.OrderType.Market:
                    return OrderType.Market;
                default:
                    return OrderType.Unspecified;
            }
        }

        private OrderExecutionReportStatus ConvertStatus(Binance.Net.Enums.OrderStatus status)
        {
            switch (status)
            {
                case Binance.Net.Enums.OrderStatus.Canceled:
                    return OrderExecutionReportStatus.ExecutionReportStatusCancelled;
                case Binance.Net.Enums.OrderStatus.Filled:
                    return OrderExecutionReportStatus.ExecutionReportStatusFill;
                case Binance.Net.Enums.OrderStatus.New:
                    return OrderExecutionReportStatus.ExecutionReportStatusNew;
                case Binance.Net.Enums.OrderStatus.PartiallyFilled:
                    return OrderExecutionReportStatus.ExecutionReportStatusPartiallyfill;
                case Binance.Net.Enums.OrderStatus.Rejected:
                    return OrderExecutionReportStatus.ExecutionReportStatusRejected;
                default:
                    return OrderExecutionReportStatus.ExecutionReportStatusUnspecified;
            }
        }

        public void BinUpdate(BinanceStreamOrderUpdate update)
        {
            Figi = update.Symbol;
            LotsRequested = (long)update.Quantity;
            LotsExecuted = (long)update.QuantityFilled;
            ExecutedCommission = update.Commission;
            InitialOrderPrice = update.Price;
            ExecutedOrderPrice = update.LastPriceFilled;
            Type = ConvertType(update.Type);
            Direction = (update.Side == Binance.Net.Enums.OrderSide.Buy) ? OrderDirection.Buy : OrderDirection.Sell;
            Status = ConvertStatus(update.Status);
            Date = update.CreateTime;
            ListId = update.OrderListId.ToString();
        }
        public void BinUpdate(BinanceOrder o)
        {
            Figi = o.Symbol;
            LotsRequested = (long)o.Quantity;
            LotsExecuted = (long)o.QuantityFilled;
            InitialOrderPrice = o.Price;
            ExecutedOrderPrice = (o.AverageFillPrice == null)? 0:o.AverageFillPrice.Value;
            Status = ConvertStatus(o.Status);
            Type = ConvertType(o.Type);
            Direction = (o.Side == Binance.Net.Enums.OrderSide.Buy) ? OrderDirection.Buy : OrderDirection.Sell;
            Date = o.CreateTime;
        }
        #endregion
    }
}
