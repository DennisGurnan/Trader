using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi.V1;
using Trader.Entities;
using SciChart.Examples.ExternalDependencies.Data;
using Binance.Net.Objects.Spot.UserStream;

namespace Trader.Network
{
    public class ServersManager:List<IServer>
    {
        public static ServersManager Instance { get; set; }

        public delegate void ServerManagerHandler();
        public event ServerManagerHandler ServerChangedEvent;
        public event ServerManagerHandler ServerChangingEvent;
        public event IServer.StreamOrdersHeader StreamOrderRequestedEvent;
        public event IServer.StreamCandleHeader StreamCandleRequestedEvent;
        public event IServer.StreamOrderBookHeader StreamOrderBookRequestedEvent;
        public event IServer.StreamTradingStatusHeader StreamTradingStatusRequestedEvent;
        public event IServer.StreamPingHeader StreamPingRequestedEvent;
        public event IServer.StreamTradeHeader StreamTradeRequestedEvent;
        public event IServer.AccountsChangedHandler AccountsChangedEvent;
        public event IServer.BinanceStreamOrderUpdateHeader BinanceStreamOrderUpdateEvent;
        public event IServer.BinanceStreamOrderListHeader BinanceStreamOrderListEvent;
        public event IServer.BinanceStreamPositionsUpdateHeader BinanceStreamPositionsUpdateEvent;
        public event IServer.BinanceStreamBalanceUpdateHeader BinanceStreamBalanceUpdateEvent;

        public ServerMode ServerMode
        {
            get => (CurrentServer == null) ? ServerMode.None:CurrentServer.Mode;
            set
            {
                if (CurrentServer != null) CurrentServer.Mode = value;
            }
        }

        private IServer _server;
        public IServer CurrentServer
        {
            get => _server;
            set
            {
                if(_server != null) ServerChangingEvent?.Invoke();
                _server = value;
                if (_server != null) Initialize();
                ServerChangedEvent?.Invoke();
            }
        }
        public ServersManager()
        {
            Add(new TinkoffServer());
            Add(new BinanceServer());
            Add(new TestServer());
            if (Instance == null) Instance = this;
        } 

        public void SelectServerByName(string name)
        {
            GUI.InstrumentsControl.Instance.Clear();
            GUI.PortfolioControl.Instance.Clear();
            GUI.OrdersControl.Instance.Clear();
            if (CurrentServer != null) DisconnectFromServerEvents();
            CurrentServer = Find(s => s.Name == name);
            if (CurrentServer != null) ConnectToServerEvents();

        }

        public IServer GetServerByName(string name)
        {
            return Find(s => s.Name == name);
        }

        private void ConnectToServerEvents()
        {
            if (CurrentServer != null)
            {
                CurrentServer.AccountsChangedEvent += OnAccountChanged;
                CurrentServer.StreamOrderRequestedEvent += OnStreamOrder;
                CurrentServer.StreamOrderBookRequestedEvent += OnStreamOrderBook;
                CurrentServer.StreamCandleRequestedEvent += OnStreamCandle;
                CurrentServer.StreamTradeRequestedEvent += OnStreamTrade;
                CurrentServer.StreamTradingStatusRequestedEvent += OnStreamTradingStatus;
                CurrentServer.StreamPingRequestedEvent += OnPing;
                CurrentServer.BinanceStreamOrderUpdateEvent += OnBinanceStreamOrderUpdate;
                CurrentServer.BinanceStreamOrderListEvent += OnBinanceStreamOrderListUpdate;
                CurrentServer.BinanceStreamPositionsUpdateEvent += OnBinanceStreamPositionsUpdate;
                CurrentServer.BinanceStreamBalanceUpdateEvent += OnBinanceStreamBalanceUpdate;
            }
        }

        private void DisconnectFromServerEvents()
        {
            if (CurrentServer != null)
            {
                CurrentServer.AccountsChangedEvent -= OnAccountChanged;
                CurrentServer.StreamOrderRequestedEvent -= OnStreamOrder;
                CurrentServer.StreamOrderBookRequestedEvent -= OnStreamOrderBook;
                CurrentServer.StreamCandleRequestedEvent -= OnStreamCandle;
                CurrentServer.StreamTradeRequestedEvent -= OnStreamTrade;
                CurrentServer.StreamTradingStatusRequestedEvent -= OnStreamTradingStatus;
                CurrentServer.StreamPingRequestedEvent -= OnPing;
                CurrentServer.BinanceStreamOrderUpdateEvent -= OnBinanceStreamOrderUpdate;
                CurrentServer.BinanceStreamOrderListEvent -= OnBinanceStreamOrderListUpdate;
                CurrentServer.BinanceStreamPositionsUpdateEvent -= OnBinanceStreamPositionsUpdate;
                CurrentServer.BinanceStreamBalanceUpdateEvent -= OnBinanceStreamBalanceUpdate;
            }
        }

        #region Events
        private void OnBinanceStreamOrderUpdate(BinanceStreamOrderUpdate orderUpdate)
        {
            BinanceStreamOrderUpdateEvent?.Invoke(orderUpdate);
        }

        private void OnBinanceStreamOrderListUpdate(BinanceStreamOrderList orderList)
        {
            BinanceStreamOrderListEvent?.Invoke(orderList);
        }

        private void OnBinanceStreamPositionsUpdate(BinanceStreamPositionsUpdate positionsList)
        {
            BinanceStreamPositionsUpdateEvent?.Invoke(positionsList);
        }

        private void OnBinanceStreamBalanceUpdate(BinanceStreamBalanceUpdate balanceList)
        {
            BinanceStreamBalanceUpdateEvent?.Invoke(balanceList);
        }

        private void OnAccountChanged()
        {
            AccountsChangedEvent?.Invoke();
        }
        private void OnStreamOrder(OrderTrades trades)
        {
            StreamOrderRequestedEvent?.Invoke(trades);
        }
        private void OnStreamOrderBook(OrderBook orderbook)
        {
            StreamOrderBookRequestedEvent?.Invoke(orderbook);
        }
        private void OnStreamCandle(TCandle candle)
        {
            StreamCandleRequestedEvent?.Invoke(candle);
        }
        private void OnStreamTrade(Trade trade)
        {
            StreamTradeRequestedEvent?.Invoke(trade);
        }
        private void OnStreamTradingStatus(TradingStatus tradingStatus)
        {
            StreamTradingStatusRequestedEvent?.Invoke(tradingStatus);
        }
        private void OnPing(Ping ping)
        {
            StreamPingRequestedEvent?.Invoke(ping);
        }
        #endregion

        public void Initialize()
        {
            if (CurrentServer != null) CurrentServer.Initialize();
        }
        public void Reload()
        {
            if (CurrentServer != null) CurrentServer.Reload();
        }
        public void SaveConfig()
        {
            if (CurrentServer != null) CurrentServer.Unload();
        }

        // Accounts
        async public Task GetAccounts(TAccounts accounts)
        {
            if (CurrentServer != null) await CurrentServer.GetAccounts(accounts);
        }
        public void AddTestAccount()
        {
            if (CurrentServer != null) CurrentServer.AddTestAccount();
        }
        public void CloseTestAccount(string accountId)
        {
            if (CurrentServer != null) CurrentServer.CloseTestAccount(accountId);
        }
        public void AddMoneyToTestAccount(decimal money, string curency, string accountId)
        {
            if (CurrentServer != null) CurrentServer.AddMoneyToTestAccount(money, curency, accountId);
        }
        // Portfolio
        async public Task GetPortfolio(string accountId, TPositions portfolio)
        {
            if (CurrentServer != null) await CurrentServer.GetPortfolio(accountId, portfolio);
        }
        async public Task GetBalance(string accountId, TPositions portfolio)
        {
            if (CurrentServer != null) await CurrentServer.GetBalance(accountId, portfolio);
        }
        // Instruments
        async public Task FillInstruments(TInstruments instruments, InstrumentType instrumentType)
        {
            if (CurrentServer != null) await CurrentServer.FillInstruments(instruments, instrumentType);
        }
        async public Task FillInstruments(TInstruments instruments)
        {
            if (CurrentServer != null) await CurrentServer.FillInstruments(instruments);
        }
        async public Task GetLastPrices(TInstruments instruments, string[] figis = null)
        {
            if (CurrentServer != null) await CurrentServer.GetLastPrices(instruments, figis);
        }
        // Candles
        public async Task<List<TCandle>> GetCandles(string figi, DateTime b, DateTime e, CandleInterval ci)
        {
            return (CurrentServer == null)? null : await CurrentServer.GetCandles(figi, b, e, ci);
        }
        public void SubscribeCandle(string figi, SubscriptionInterval interval, SubscriptionAction action)
        {
            if (CurrentServer != null) CurrentServer.SubscribeCandle(figi, interval, action);
        }
        // OrderBook
        public void SubscribeOrderBook(string figi, int Depth, SubscriptionAction action)
        {
            if (CurrentServer != null) CurrentServer.SubscribeOrderBook(figi, Depth, action);
        }
        // Orders
        public void StartOrderStreamListener()
        {
            if (CurrentServer != null) CurrentServer.StartOrderStreamListener();
        }
        async public Task PostOrder(string figi, Int64 quantity, decimal price, OrderDirection direction, OrderType orderType, TOrders orders, string id = "")
        {
            if (CurrentServer != null) await CurrentServer.PostOrder(figi, quantity, price, direction, orderType, orders, id);
        }
        async public Task CancelOrder(string id, TOrders orders)
        {
            if (CurrentServer != null) await CurrentServer.CancelOrder(id, orders);
        }
        async public Task GetOrders(TOrders orders)
        {
            if(CurrentServer != null) await CurrentServer.GetOrders(orders);
        }
        async public Task GetOrderState(TOrder order)
        {
            if (CurrentServer != null) await CurrentServer.GetOrderState(order);
        }
        // операции
        async public Task GetOperations(TOperations operations, DateTime lastTimeUpdate)
        {
            if (CurrentServer != null) await CurrentServer.GetOperations(operations, lastTimeUpdate);
        }
        // Инфа
        public void SubscribeTrade(string figi, SubscriptionAction action)
        {
            if (CurrentServer != null) CurrentServer.SubscribeTrade(figi, action);
        }
        public void SubscribeInfo(string figi, SubscriptionAction action)
        {
            if (CurrentServer != null) CurrentServer.SubscribeInfo(figi, action);
        }

    }
}
