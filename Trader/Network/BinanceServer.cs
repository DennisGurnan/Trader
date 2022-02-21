using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tinkoff.InvestApi.V1;
using SciChart.Examples.ExternalDependencies.Data;
using Binance.Net;
using System.Windows.Threading;
using Binance.Net.Enums;
using Binance.Net.Objects;
using Binance.Net.Objects.Spot;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;
using Binance.Net.Objects.Spot.MarketStream;
using Binance.Net.Objects.Spot.UserStream;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Trader.Entities;
using Google.Protobuf.WellKnownTypes;

namespace Trader.Network
{
    public class BinanceServer:IServer
    {
        private static Utils.Config IO = new Utils.Config("config");

        public string Name { get => "Binance"; }
        public ServerMode Mode { get; set; }
        public bool IsTest { get => false; }

        public static string ApiKey
        {
            get => IO.GetVal("BinanceServer", "ApiKey", "");
            set => IO.SetVal("BinanceServer", "ApiKey", value);
        }

        public static string SecretKey
        {
            get => IO.GetVal("BinanceServer", "SecretKey", "");
            set => IO.SetVal("BinanceServer", "SecretKey", value);
        }

        public static string ServerUrl
        {
            get => IO.GetVal("BinanceServer", "ServerUrl", @"https://www.binance.com");
            set => IO.SetVal("BinanceServer", "ServerUrl", value);
        }

        public static string WebSocket
        {
            get => IO.GetVal("BinanceServer", "WebSocket", @"wss://stream.binance.com:9443/ws/");
            set => IO.SetVal("BinanceServer", "WebSocket", value);
        }

        private BinanceClient client;
        private BinanceSocketClient socketClient;


        #region Methods
        // System
        public void Initialize()
        {
            if (!string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(SecretKey))
            {
                BinanceClient.SetDefaultOptions(new BinanceClientOptions()
                {
                    ApiCredentials = new ApiCredentials(ApiKey, SecretKey)
                });
                BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
                {
                    ApiCredentials = new ApiCredentials(ApiKey, SecretKey)
                });
                client = new BinanceClient();
                socketClient = new BinanceSocketClient();
            }
        }
        public void Reload()
        {
            if (!string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(SecretKey))
            {
                BinanceClient.SetDefaultOptions(new BinanceClientOptions()
                {
                    ApiCredentials = new ApiCredentials(ApiKey, SecretKey)
                });
                BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
                {
                    ApiCredentials = new ApiCredentials(ApiKey, SecretKey)
                });
            }
        }
        public void Unload()
        {
            IO.Save();
            socketClient.UnsubscribeAllAsync();
        }
        #endregion
        #region Акаунты
        async public Task GetAccounts(TAccounts accounts)
        {
            TAccount account = new TAccount() { Id = "00000000", Name = "Binance Spot Account", Status = AccountStatus.Open };
            accounts.Clear();
            accounts.Add(account);
        }
        public void AddTestAccount()
        {

        }
        public void CloseTestAccount(string accountId)
        {

        }
        public void AddMoneyToTestAccount(decimal money, string curency, string accountId)
        {

        }
        #endregion
        #region портфель
        async public Task GetPortfolio(string accountId, TPositions portfolio)
        {
            var response = await client.General.GetAccountInfoAsync();
            portfolio.Clear();
            foreach(BinanceBalance b in response.Data.Balances)
            {
                if (b.Total > 0)
                {
                    TPosition pos = new TPosition(b.Asset)
                    {
                        Quantity = b.Total
                    };
                    portfolio.Add(pos);
                }
            }
        }
        async public Task GetBalance(string accountId, TPositions portfolio)
        {
        }
        #endregion
        #region Инструменты
        async public Task FillInstruments(TInstruments instruments, InstrumentType instrumentType)
        {

        }
        async public Task FillInstruments(TInstruments instruments)
        {
            var result = await client.Spot.System.GetExchangeInfoAsync();
            foreach(BinanceSymbol i in result.Data.Symbols)
            {
                TInstrument mi = instruments.GetByFigi(i.Name);
                if(mi == null)
                {
                    mi = new TInstrument(i.Name);
                    instruments.Add(mi);
                    if(instruments.GetByFigi(i.QuoteAsset) == null)
                    {
                        TInstrument qi = new TInstrument(i.QuoteAsset);
                        qi.BinUpdate(i.QuoteAsset);
                        instruments.Add(qi);
                    }
                    if (instruments.GetByFigi(i.BaseAsset) == null)
                    {
                        TInstrument qi = new TInstrument(i.BaseAsset);
                        qi.BinUpdate(i.BaseAsset);
                        instruments.Add(qi);
                    }
                }
                mi.BinUpdate(i);
            }
            CallResult subscribeResult = await socketClient.Spot.SubscribeToAllSymbolTickerUpdatesAsync(data =>
                {
                    foreach(var ud in data.Data)
                    {
                        var symbol = instruments.SingleOrDefault(p => p.Figi == ud.Symbol);
                        if (symbol != null) symbol.LastPrice = ud.LastPrice;
                    }
                    instruments.OnChanged();
            });
            if (!subscribeResult.Success) Utils.Loger.Error($"BinanceServer:FillInstruments()-> Failed to subscribe price updates: {subscribeResult.Error}");
            instruments.OnChanged();
        }
        async public Task GetLastPrices(TInstruments instruments, string[] figis = null)
        {
            var result = await client.Spot.Market.GetPricesAsync();
            foreach(BinancePrice price in result.Data)
            {
                TInstrument mi = instruments.GetByFigi(price.Symbol);
                if(mi == null)
                {
                    mi = new TInstrument(price.Symbol);
                }
                mi.LastPrice = price.Price;
                mi.LastPriceTime = (price.Timestamp == null)? DateTime.MinValue: price.Timestamp.Value;
            }
        }
        #endregion
        #region Свечи
        private KlineInterval ConvertInterval(CandleInterval i)
        {
            switch (i)
            {
                case CandleInterval._1Min:
                    return KlineInterval.OneMinute;
                case CandleInterval._5Min:
                    return KlineInterval.FiveMinutes;
                case CandleInterval._15Min:
                    return KlineInterval.FifteenMinutes;
                case CandleInterval.Hour:
                    return KlineInterval.OneHour;
                case CandleInterval.Day:
                    return KlineInterval.OneDay;
                default:
                    return KlineInterval.OneMinute;
            }
        }
        async public Task<List<TCandle>> GetCandles(string figi, DateTime b, DateTime e, CandleInterval ci)
        {
            var l = (e - b).TotalMinutes;
            WebCallResult<IEnumerable<IBinanceKline>> response;
            List<TCandle> candles = new List<TCandle>();
            if (l <= 500)
            {
                response = await client.Spot.Market.GetKlinesAsync(figi, ConvertInterval(ci), b, e);
                foreach (var i in response.Data)
                {
                    TCandle m = new TCandle(i, ci);
                    candles.Add(m);
                }
                return candles;
            }
            else
            {
                for(DateTime s = b; s < e; s = s.AddMinutes(500))
                {
                    response = await client.Spot.Market.GetKlinesAsync(figi, ConvertInterval(ci), s, s.AddMinutes(500));
                    foreach (var i in response.Data)
                    {
                        TCandle m = new TCandle(i, ci);
                        candles.Add(m);
                    }
                }
            }
            return candles;
        }
        async public void SubscribeCandle(string figi, SubscriptionInterval interval, SubscriptionAction action)
        {
            var response = await socketClient.Spot.SubscribeToKlineUpdatesAsync(figi, KlineInterval.OneMinute, (date => {
                TCandle candle = new TCandle(date.Data.Data, CandleInterval._1Min);
                StreamCandleRequestedEvent?.Invoke(candle);
            }));
        }
        #endregion
        #region Стакан
        public OrderBook OrderBook;
        async public void SubscribeOrderBook(string figi, int Depth, SubscriptionAction action)
        {
            var Response = await socketClient.Spot.SubscribeToOrderBookUpdatesAsync(figi,1000, (data => {
                OrderBook ob = new OrderBook();
                int c = 0;
                foreach(var i in data.Data.Asks)
                {
                    ob.Asks.Add(new Order { Price = Utils.Convertor.DecToQuotation(i.Price), Quantity = (long)i.Quantity });
                    c++;
                    if (c > Depth) break;
                }
                c = 0;
                foreach (var i in data.Data.Bids)
                {
                    ob.Bids.Add(new Order { Price = Utils.Convertor.DecToQuotation(i.Price), Quantity = (long)i.Quantity });
                    c++;
                    if (c > Depth) break;
                }
                OrderBook = ob.Clone();
                StreamOrderBookRequestedEvent?.Invoke(OrderBook);
            }));
        }
        #endregion
        #region Ордера
        async public void StartOrderStreamListener()
        {
            var startOkey = await client.Spot.UserStream.StartUserStreamAsync();
            if (!startOkey.Success)
            {
                Utils.Loger.Error($"Error starting user stream: {startOkey.Error.Message}");
                return;
            }
            var subOkey = await socketClient.Spot.SubscribeToUserDataUpdatesAsync(startOkey.Data, (data => {
                BinanceStreamOrderUpdateEvent?.Invoke(data.Data);
            }), (data => {
                BinanceStreamOrderListEvent?.Invoke(data.Data);
            }), (data => {
                BinanceStreamPositionsUpdateEvent?.Invoke(data.Data);
            }), (data => {
                BinanceStreamBalanceUpdateEvent?.Invoke(data.Data);
            }));
        }

        async public Task PostOrder(string figi, Int64 quantity, decimal price, OrderDirection direction, Tinkoff.InvestApi.V1.OrderType orderType, TOrders orders, string id = "")
        {

        }
        async public Task CancelOrder(string id, TOrders orders)
        {

        }
        async public Task GetOrders(TOrders orders)
        {
            if (GUI.InstrumentsControl.Instance.CurrentInstrument != null)
            {
                var response = await client.Spot.Order.GetOrdersAsync(GUI.InstrumentsControl.Instance.CurrentInstrumentFigi);
                orders.Clear();
                foreach (BinanceOrder o in response.Data)
                {
                    TOrder i = new TOrder();
                    i.BinUpdate(o);
                    orders.Add(i);
                }
            }
        }
        async public Task GetOrderState(TOrder order)
        {
            
        }
        #endregion
        #region Операции
        async public Task GetOperations(Entities.TOperations operations, DateTime lastTimeUpdate)
        {

        }
        #endregion
        #region Подписки
        async public void SubscribeTrade(string figi, SubscriptionAction action)
        {

        }
        async public void SubscribeInfo(string figi, SubscriptionAction action)
        {

        }
        #endregion

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
