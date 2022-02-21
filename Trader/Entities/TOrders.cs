using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Trader.Network;
using Tinkoff.InvestApi.V1;
using Grpc.Core;
using Binance.Net.Objects.Spot.MarketStream;
using Binance.Net.Objects.Spot.UserStream;


namespace Trader.Entities
{
    public class TOrders: ObservableCollection<TOrder>, INotifyPropertyChanged
    {
        public delegate void TOrdersHandler();
        public event TOrdersHandler ChangedEvent;

        public TOrders() : base()
        {
            ServersManager.Instance.StreamOrderRequestedEvent += OnServerStreamResponse;
            ServersManager.Instance.BinanceStreamOrderUpdateEvent += OnOrderUpdateEvent;
            ServersManager.Instance.BinanceStreamOrderListEvent += OnOrderListUpdateEvent;
        }

        private void OnOrderUpdateEvent(BinanceStreamOrderUpdate message)
        {
            TOrder order = this.ToList().Find(x => x.Id == message.OrderId.ToString());
            if(order == null)
            {
                order = new TOrder();
                Add(order);
            }
            order.BinUpdate(message);
            ChangedEvent?.Invoke();
        }

        private void OnOrderListUpdateEvent(BinanceStreamOrderList list)
        {
        }

        private string GenerateId(string figi, long quantity, decimal price, OrderDirection dir, OrderType type)
        {
            return GUI.InstrumentsControl.Instance.Instruments.Single(s=>s.Figi == figi).Ticker + quantity.ToString("N2") + price.ToString("N2") + dir.ToString() + type.ToString() + DateTime.Now.Minute;
        }

        async public void SellLimit(string figi, long quantity, decimal price)
        {
            await ServersManager.Instance.PostOrder(figi, quantity, price, OrderDirection.Sell, OrderType.Limit, this,
                GenerateId(figi, quantity, price, OrderDirection.Sell, OrderType.Limit));
            ChangedEvent?.Invoke();
        }

        async public void SellMarket(string figi, long quantity, decimal price)
        {
            await ServersManager.Instance.PostOrder(figi, quantity, price, OrderDirection.Sell, OrderType.Market, this,
                GenerateId(figi, quantity, price, OrderDirection.Sell, OrderType.Market));
            ChangedEvent?.Invoke();
        }

        async public void BayLimit(string figi, Int64 quantity, decimal price)
        {
            await ServersManager.Instance.PostOrder(figi, quantity, price, OrderDirection.Buy, OrderType.Limit, this,
                GenerateId(figi, quantity, price, OrderDirection.Buy, OrderType.Limit));
            ChangedEvent?.Invoke();
        }

        async public void BayMarket(string figi, Int64 quantity, decimal price)
        {
            await ServersManager.Instance.PostOrder(figi, quantity, price, OrderDirection.Buy, OrderType.Market, this,
                GenerateId(figi, quantity, price, OrderDirection.Buy, OrderType.Market));
            ChangedEvent?.Invoke();
        }

        async public void Cancel(string id)
        {
            await ServersManager.Instance.CancelOrder(id, this);
            ChangedEvent?.Invoke();
        }

        async public void CancelByFigi(string figi)
        {
            List<TOrder> o = this.Where(s => s.Figi == figi).ToList();
            if(o.Count != 0)
            {
                foreach(TOrder i in o)
                {
                    if((i.Status != OrderExecutionReportStatus.ExecutionReportStatusCancelled)||
                        (i.Status != OrderExecutionReportStatus.ExecutionReportStatusRejected))
                    {
                        Cancel(i.Id);
                        await Task.Delay(30);
                    }
                }
            }
        }

        async public void FillFromServer()
        {
            await ServersManager.Instance.GetOrders(this);
            ChangedEvent?.Invoke();
        }

        async private void OnServerStreamResponse(OrderTrades trades)
        {
            TOrder order = this.SingleOrDefault(s => s.Id == trades.OrderId);
            foreach(OrderTrade t in trades.Trades)
            {
                TOrderStage s = new TOrderStage()
                {
                    Price = Utils.Convertor.QuotationToDec(t.Price),
                    Quantity = t.Quantity,
                    Time = t.DateTime.ToDateTime()
                };
                order.Stages.Add(s);
            }
            await ServersManager.Instance.GetOrderState(order);
            ChangedEvent?.Invoke();
        }
    }
}
