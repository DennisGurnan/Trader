using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi.V1;
using Google.Protobuf.WellKnownTypes;
using Trader.Entities;
using Binance.Net.Objects.Spot.MarketStream;
using Binance.Net.Objects.Spot.UserStream;

namespace Trader.Network
{
    public interface IServer
    {
        public string Name { get; }
        public ServerMode Mode { get; set; }
        public bool IsTest { get; }

        #region Methods
        // System
        public void Initialize();
        public void Reload();
        public void Unload();
        // Accounts
        public Task GetAccounts(TAccounts accounts);
        public void AddTestAccount();
        public void CloseTestAccount(string accountId);
        public void AddMoneyToTestAccount(decimal money, string curency, string accountId);
        // Portfolio
        public Task GetPortfolio(string accountId, TPositions portfolio);
        public Task GetBalance(string accountId, TPositions portfolio);
        // Instruments
        public Task FillInstruments(TInstruments instruments, InstrumentType instrumentType);
        public Task FillInstruments(TInstruments instruments);
        public Task GetLastPrices(TInstruments instruments, string[] figis = null);
        // Candles
        public Task<List<TCandle>> GetCandles(string figi, DateTime b, DateTime e, CandleInterval ci);
        public void SubscribeCandle(string figi, SubscriptionInterval interval, SubscriptionAction action);
        // OrderBook
        public void SubscribeOrderBook(string figi, int Depth, SubscriptionAction action);
        // Orders
        public void StartOrderStreamListener();
        public Task PostOrder(string figi, Int64 quantity, decimal price, OrderDirection direction, OrderType orderType, TOrders orders, string id = "");
        public Task CancelOrder(string id, TOrders orders);
        public Task GetOrders(TOrders orders);
        public Task GetOrderState(TOrder order);
        // Операции
        public Task GetOperations(TOperations operations, DateTime lastTimeUpdate);
        // Инфа
        public void SubscribeTrade(string figi, SubscriptionAction action);
        public void SubscribeInfo(string figi, SubscriptionAction action);
        #endregion

        #region Events
        public delegate void AccountsChangedHandler();
        public event AccountsChangedHandler AccountsChangedEvent;
        public delegate void StreamOrderBookHeader(OrderBook orderBook);
        public event StreamOrderBookHeader StreamOrderBookRequestedEvent;
        public delegate void StreamCandleHeader(TCandle candle);
        public event StreamCandleHeader StreamCandleRequestedEvent;
        public delegate void StreamTradeHeader(Trade trade);
        public event StreamTradeHeader StreamTradeRequestedEvent;
        public delegate void StreamTradingStatusHeader(TradingStatus tradingStatus);
        public event StreamTradingStatusHeader StreamTradingStatusRequestedEvent;
        public delegate void StreamPingHeader(Ping ping);
        public event StreamPingHeader StreamPingRequestedEvent;
        public delegate void StreamOrdersHeader(OrderTrades trades);
        public event StreamOrdersHeader StreamOrderRequestedEvent;
        public delegate void BinanceStreamOrderUpdateHeader(BinanceStreamOrderUpdate orderUpdate);
        public event BinanceStreamOrderUpdateHeader BinanceStreamOrderUpdateEvent;
        public delegate void BinanceStreamOrderListHeader(BinanceStreamOrderList orderList);
        public event BinanceStreamOrderListHeader BinanceStreamOrderListEvent;
        public delegate void BinanceStreamPositionsUpdateHeader(BinanceStreamPositionsUpdate PositionsUpdate);
        public event BinanceStreamPositionsUpdateHeader BinanceStreamPositionsUpdateEvent;
        public delegate void BinanceStreamBalanceUpdateHeader(BinanceStreamBalanceUpdate balanceUpdate);
        public event BinanceStreamBalanceUpdateHeader BinanceStreamBalanceUpdateEvent;
        #endregion
    }
}
