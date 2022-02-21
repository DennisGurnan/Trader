using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tinkoff.InvestApi.V1;
using Google.Protobuf.WellKnownTypes;

namespace Trader.ViewModels
{
    public class OrdersControlViewModelItem
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public Int64 LotsRequested { get; set; }
        public Int64 LotsExecuted { get; set; }
        public decimal InitialOrderPrice { get; set; }
        public decimal ExecutedOrderPrice { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public decimal AveragePositionPrice { get; set; }
        public decimal InitialCommission { get; set; }
        public decimal ExecutedCommission { get; set; }
        public decimal AciValue { get; set; }
        public string Figi { get; set; }
        public string Direction { get; set; }
        public decimal InitialSecurityPrice { get; set; }
        public List<Entities.TOrderStage> Stages { get; set; }
        public decimal ServiceCommission { get; set; }
        public string LastMessage { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public DateTime CanseledTime { get; set; }
    }

    public class OrdersControlViewModel:ObservableCollection<OrdersControlViewModelItem>
    {
        public Entities.TOrders orders = new Entities.TOrders();

        public OrdersControlViewModel() : base()
        {
            orders.ChangedEvent += Filter;
        }

        public void Filter()
        {
            Clear();
            foreach (Entities.TOrder op in orders)
            {
                string status;
                switch (op.Status)
                {
                    case OrderExecutionReportStatus.ExecutionReportStatusNew:
                        status = "Новый";
                        break;
                    case OrderExecutionReportStatus.ExecutionReportStatusCancelled:
                        status = "Отменен";
                        break;
                    case OrderExecutionReportStatus.ExecutionReportStatusFill:
                        status = "Исполнен";
                        break;
                    case OrderExecutionReportStatus.ExecutionReportStatusPartiallyfill:
                        status = "Исполнен частично";
                        break;
                    case OrderExecutionReportStatus.ExecutionReportStatusRejected:
                        status = "Отклонен";
                        break;
                    default:
                        status = "Не известен";
                        break;
                }
                string direction = "Не известно";
                if (op.Direction == OrderDirection.Sell) direction = "Продажа";
                if (op.Direction == OrderDirection.Buy) direction = "Покупка";
                string type = "Не известно";
                if (op.Type == OrderType.Limit) type = "Лимит";
                if (op.Type == OrderType.Market) type = "Рынок";

                OrdersControlViewModelItem i = new OrdersControlViewModelItem()
                {
                    Id = op.Id,
                    Status = status,
                    LotsExecuted = op.LotsExecuted,
                    LotsRequested = op.LotsRequested,
                    InitialOrderPrice = op.InitialOrderPrice,
                    ExecutedOrderPrice = op.ExecutedOrderPrice,
                    TotalOrderAmount = op.TotalOrderAmount,
                    AveragePositionPrice = op.AveragePositionPrice,
                    InitialCommission = op.InitialCommission,
                    ExecutedCommission = op.ExecutedCommission,
                    AciValue = op.AciValue,
                    Figi = op.Figi,
                    Direction = direction,
                    InitialSecurityPrice = op.InitialSecurityPrice,
                    Stages = op.Stages,
                    ServiceCommission = op.ServiceCommission,
                    LastMessage = op.LastMessage,
                    Currency = op.Currency,
                    Type = type,
                    Date = (op.Date == null) ? DateTime.MinValue : op.Date,
                    CanseledTime = (op.CanseledTime == null) ? DateTime.MinValue : op.CanseledTime
                };
                Add(i);
            }
        }
    }
}
