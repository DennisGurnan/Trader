using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Trader.Entities;
using Tinkoff.InvestApi.V1;
using SciChart.Examples.ExternalDependencies.Data;
using SciChart.Examples.ExternalDependencies.Common;

namespace Trader.Network
{
    public class TestServer:IServer
    {
        public string Name { get => "TestServer"; }
        public ServerMode Mode { get => ServerMode.Test; set { } }
        public bool IsTest { get => true; }

        private static Utils.Config IO = new Utils.Config("config");
        public static TimeSpan StepTime
        {
            get => IO.GetVal("TestServer", "StepTime", TimeSpan.FromSeconds(5));
            set => IO.SetVal("TestServer", "StepTime", value);
        }

        private TInstrument testInstrument;
        private List<TCandle> _5MinPirces;
        private SciChart.Examples.ExternalDependencies.Data.MarketDataService MarketService;

        public void Initialize()
        {
            testInstrument = new TInstrument()
            {
                Name = "Test Instrument",
                Figi = "1234567890",
                Ticker = "TST",
                Currency = "rub"
            };
            MarketService = new SciChart.Examples.ExternalDependencies.Data.MarketDataService(new DateTime(2000, 08, 01, 12, 00, 00), 5, 20);
        }

        public void Reload()
        {

        }

        public void Unload()
        {
            IO.Save();
        }
        #region Accounts
        private class accountCash { public string id; public decimal cash; public string currency; }
        private class accountItem { public TAccount Account; public TPositions Portffolio; public accountCash Cash; }
        private List<accountItem> _accounts = new List<accountItem>();

        async public Task GetAccounts(TAccounts accounts)
        {
            accounts.Clear();
            foreach (accountItem a in _accounts) accounts.Add(a.Account);
        }
        public void AddTestAccount()
        {
            TAccount account = new TAccount()
            {
                Status = AccountStatus.Open,
                ClosedDate = DateTime.MinValue,
                OpenedDate = DateTime.Now,
                Name = "TestAccount_" + _accounts.Count.ToString(),
                Id = _accounts.Count.ToString()
            };
            _accounts.Add(new accountItem() { Account = account, Portffolio = new TPositions(), Cash = new accountCash() });
            AccountsChangedEvent?.Invoke();
        }
        public void CloseTestAccount(string accountId)
        {
            _accounts.Remove(_accounts.Find(s => s.Account.Id == accountId));
            AccountsChangedEvent?.Invoke();
        }
        public void AddMoneyToTestAccount(decimal money, string curency, string accountId)
        {
            accountItem i = _accounts.Find(s => s.Account.Id == accountId);
            i.Cash.cash += money;
            i.Cash.currency = curency;
            AccountsChangedEvent?.Invoke();
        }
        #endregion
        #region Portfolio
        private List<TPositions> portfolios = new List<TPositions>();
        async public Task GetPortfolio(string accountId, TPositions portfolio)
        {
            accountItem i = _accounts.Find(s => s.Account.Id == accountId);
            if(i != null) portfolio = i.Portffolio;
        }
        async public Task GetBalance(string accountId, TPositions portfolio)
        {
            accountItem i = _accounts.Find(s => s.Account.Id == accountId);
            if (i == null) return;
            portfolio.Money = new ObservableCollection<TMoney>() { new TMoney(i.Cash.cash, "rur")};
        }
        #endregion
        #region Instruments
        async public Task FillInstruments(TInstruments instruments, InstrumentType instrumentType)
        {
            TInstrument inst = instruments.GetByFigi(testInstrument.Figi);
            if (inst == null) instruments.Add(testInstrument);
        }
        async public Task FillInstruments(TInstruments instruments)
        {
            TInstrument inst = instruments.GetByFigi(testInstrument.Figi);
            if (inst == null) instruments.Add(testInstrument);
        }
        async public Task GetLastPrices(TInstruments instruments, string[] figis = null)
        {
            TCandle bar = _5MinPirces.Last();
            instruments.First().LastPrice = (decimal) (bar.High + bar.Low) / 2;
        }
        #endregion
        #region Candles
        private bool candlesSubscribed;
        async public Task<List<TCandle>> GetCandles(string figi, DateTime b, DateTime e, CandleInterval ci)
        {
            int num = (int) (e - b).TotalMinutes / 5;
            IEnumerable<PriceBar> s = MarketService.GetHistoricalData(num);
            _5MinPirces = new List<TCandle>();
            List<TCandle> c = new List<TCandle>();
            foreach (PriceBar p in s) { c.Add(new TCandle(p, ci)); _5MinPirces.Add(new TCandle(p, ci)); }
            return c;
        }
        public void SubscribeCandle(string figi, SubscriptionInterval interval, SubscriptionAction action)
        {
            if (action == SubscriptionAction.Subscribe) candlesSubscribed = true;
            else candlesSubscribed = false;
        }
        #endregion
        #region OrderBook
        private bool orderbookSuscribed;
        public void SubscribeOrderBook(string figi, int Depth, SubscriptionAction action)
        {
            if (action == SubscriptionAction.Subscribe) orderbookSuscribed = true;
            else orderbookSuscribed = false;
        }
        #endregion
        #region Orders
        public void StartOrderStreamListener()
        {

        }
        async public Task PostOrder(string figi, Int64 quantity, decimal price, OrderDirection direction, OrderType orderType, TOrders orders, string id = "")
        {

        }
        async public Task CancelOrder(string id, TOrders orders)
        {

        }
        async public Task GetOrders(TOrders orders)
        {

        }
        async public Task GetOrderState(TOrder order)
        {

        }
        #endregion
        #region Operations
        async public Task GetOperations(Entities.TOperations operations, DateTime lastTimeUpdate)
        {

        }
        #endregion
        #region Инфа
        public void SubscribeTrade(string figi, SubscriptionAction action)
        {

        }
        public void SubscribeInfo(string figi, SubscriptionAction action)
        {

        }
        #endregion

        async private void StreamLoop()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
        #region Events
        public event IServer.AccountsChangedHandler AccountsChangedEvent;
        public event IServer.StreamOrderBookHeader StreamOrderBookRequestedEvent;
        public event IServer.StreamCandleHeader StreamCandleRequestedEvent;
        public event IServer.StreamTradeHeader StreamTradeRequestedEvent;
        public event IServer.StreamTradingStatusHeader StreamTradingStatusRequestedEvent;
        public event IServer.StreamPingHeader StreamPingRequestedEvent;
        public event IServer.StreamOrdersHeader StreamOrderRequestedEvent;
        public event IServer.BinanceStreamOrderUpdateHeader BinanceStreamOrderUpdateEvent;
        public event IServer.BinanceStreamOrderListHeader BinanceStreamOrderListEvent;
        public event IServer.BinanceStreamPositionsUpdateHeader BinanceStreamPositionsUpdateEvent;
        public event IServer.BinanceStreamBalanceUpdateHeader BinanceStreamBalanceUpdateEvent;
        #endregion
    }
}
