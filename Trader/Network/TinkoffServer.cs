using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SciChart.Examples.ExternalDependencies.Data;
using System.Threading.Tasks;
using Grpc.Core;
using Tinkoff.InvestApi.V1;
using Google.Protobuf.WellKnownTypes;
using Trader.GUI;
using Trader.Entities;

namespace Trader.Network
{
    public enum ServerMode
    {
        FullAccess, ReadOnly, Test, None
    }
    public class TinkoffServer : IServer
    {
        private static Utils.Config IO = new Utils.Config("config");

        #region Properties
        public static string ServerUrl
        {
            get => IO.GetVal("TinkoffServer", "Url", "");
            set => IO.SetVal("TinkoffServer", "Url", value);
        }
        public static string FullAccessToken
        {
            get => IO.GetVal("TinkoffServer", "FullAccessToken", "");
            set => IO.SetVal("TinkoffServer", "FullAccessToken", value);
        }
        public static string ReadOnlyToken
        {
            get => IO.GetVal("TinkoffServer", "ReadOnlyToken", "");
            set => IO.SetVal("TinkoffServer", "ReadOnlyToken", value);
        }
        public static string TestModeToken
        {
            get => IO.GetVal("TinkoffServer", "TestToken", "");
            set => IO.SetVal("TinkoffServer", "TestToken", value);
        }

        public string Name { get => "Tinkoff"; }
        public ServerMode Mode
        {
            get => IO.GetVal("TinkoffServer", "Type", ServerMode.Test);
            set => IO.SetVal("TinkoffServer", "Type", value);
        }
        public bool IsTest
        {
            get
            {
                if (Mode == ServerMode.Test) return true;
                return false;
            }
        }
        #endregion

        #region Clients
        private Metadata headers
        {
            get
            {
                string token;
                if (Mode == ServerMode.FullAccess) token = FullAccessToken;
                else if (Mode == ServerMode.ReadOnly) token = ReadOnlyToken;
                else token = TestModeToken;
                return new Metadata
                {
                    { "Authorization", $"Bearer {token}" }
                };
            }
        }
        private static Channel channel
        {
            get
            {
                return new Channel(ServerUrl, ChannelCredentials.SecureSsl);
            }
        }
        private InstrumentsService.InstrumentsServiceClient instrumentsServiceClient;
        private Tinkoff.InvestApi.V1.MarketDataService.MarketDataServiceClient marketDataServiceClient;
        private MarketDataStreamService.MarketDataStreamServiceClient marketDataStreamServiceClient;
        private OperationsService.OperationsServiceClient operationsServiceClient;
        private OrdersService.OrdersServiceClient ordersServiceClient;
        private OrdersStreamService.OrdersStreamServiceClient ordersStreamServiceClient;
        private SandboxService.SandboxServiceClient sandboxServiceClient;
        private StopOrdersService.StopOrdersServiceClient stopOrdersServiceClient;
        private UsersService.UsersServiceClient usersServiceClient;
        #endregion

        #region Ордера
        private bool isOrdersListened;
        public async void StartOrderStreamListener()
        {
            try
            {
                isOrdersListened = true;
                while (await OrderStream.ResponseStream.MoveNext())
                {
                    if (isOrdersListened == false) return;
                    // check data
                    if (OrderStream.ResponseStream.Current.OrderTrades != null)
                    {
                        StreamOrderRequestedEvent?.Invoke(OrderStream.ResponseStream.Current.OrderTrades);
                    }
                    if (OrderStream.ResponseStream.Current.Ping != null)
                    {
                        StreamPingRequestedEvent?.Invoke(OrderStream.ResponseStream.Current.Ping);
                        Utils.Loger.Log("Ping:" + OrderStream.ResponseStream.Current.Ping.ToString());
                    }
                }
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:StartStreamListener->" + ex.Status.ToString());
                await Task.Delay(TimeSpan.FromSeconds(2));
                StartOrderStreamListener();
            }
        }

        async public Task PostOrder(string figi, Int64 quantity, decimal price, OrderDirection direction, OrderType orderType, TOrders orders, string id = "")
        {
            try
            {
                PostOrderRequest request = new PostOrderRequest()
                {
                    Figi = figi,
                    AccountId = AccountsControl.Instance.CurrentAccountId,
                    Quantity = quantity,
                    Price = Utils.Convertor.DecToQuotation(price),
                    Direction = direction,
                    OrderType = orderType,
                    OrderId = id
                };
                PostOrderResponse response;
                if (Mode == ServerMode.Test)
                {
                    response = await sandboxServiceClient.PostSandboxOrderAsync(request, headers);
                }
                else
                {
                    response = await ordersServiceClient.PostOrderAsync(request, headers);
                }
                TOrder order = orders.SingleOrDefault(s => s.Id == response.OrderId);
                if (order == null)
                {
                    order = new TOrder();
                    order.TnkUpdate(response);
                    orders.Add(order);
                }
                else
                {
                    order.TnkUpdate(response);
                }
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:PostOrder(" + figi + ")->" + ex.Status.ToString());
            }
        }

        async public Task CancelOrder(string id, TOrders orders)
        {
            TOrder order = orders.SingleOrDefault(s => s.Id == id);
            if (order == null) return;
            try
            {
                CancelOrderRequest request = new CancelOrderRequest()
                {
                    AccountId = AccountsControl.Instance.CurrentAccountId,
                    OrderId = order.Id
                };
                CancelOrderResponse response;
                if (Mode == ServerMode.Test)
                {
                    response = await sandboxServiceClient.CancelSandboxOrderAsync(request, headers);
                }
                else
                {
                    response = await ordersServiceClient.CancelOrderAsync(request, headers);
                }
                order.Status = OrderExecutionReportStatus.ExecutionReportStatusCancelled;
                order.CanseledTime = response.Time.ToDateTime();
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:PostOrder(" + id + ")->" + ex.Status.ToString());
            }
        }

        async public Task GetOrders(TOrders orders)
        {
            try
            {
                GetOrdersRequest request = new GetOrdersRequest()
                {
                    AccountId = AccountsControl.Instance.CurrentAccountId
                };
                GetOrdersResponse response;
                if (Mode == ServerMode.Test)
                {
                    response = await sandboxServiceClient.GetSandboxOrdersAsync(request, headers);
                }
                else
                {
                    response = await ordersServiceClient.GetOrdersAsync(request, headers);
                }
                foreach (OrderState order in response.Orders)
                {
                    TOrder o = orders.SingleOrDefault(s => s.Id == order.OrderId);
                    if (o == null)
                    {
                        o = new TOrder();
                        orders.Add(o);
                    }
                    o.TnkUpdate(order);
                    o.Stages = new List<TOrderStage>();
                    if (order.Stages.Count > 0)
                    {
                        foreach (OrderStage stage in order.Stages)
                        {
                            TOrderStage s = new TOrderStage()
                            {
                                Price = Utils.Convertor.MoneyToDec(stage.Price),
                                Quantity = stage.Quantity
                            };
                            o.Stages.Add(s);
                        }
                    }
                }
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:GetOrders()->" + ex.Status.ToString());
            }
        }

        async public Task GetOrderState(Entities.TOrder order)
        {
            try
            {
                GetOrderStateRequest request = new GetOrderStateRequest()
                {
                    AccountId = AccountsControl.Instance.CurrentAccountId,
                    OrderId = order.Id
                };
                OrderState state;
                if(Mode == ServerMode.Test)
                {
                    state = await sandboxServiceClient.GetSandboxOrderStateAsync(request, headers);
                }
                else
                {
                    state = await ordersServiceClient.GetOrderStateAsync(request, headers);
                }
                order.TnkUpdate(state);
            }catch(RpcException ex)
            {
                Utils.Loger.Error("Server:GetOrderState()->" + ex.Status.ToString());
            }
        }
        #endregion

        #region Операции
        async public Task GetOperations(Entities.TOperations operations, DateTime lastTimeUpdate)
        {
            try
            {
                Timestamp from = Timestamp.FromDateTime(lastTimeUpdate.ToUniversalTime());
                if (AccountsControl.Instance.CurrentAccount.OpenedDate > lastTimeUpdate)
                {
                    from = Timestamp.FromDateTime(AccountsControl.Instance.CurrentAccount.OpenedDate.ToUniversalTime());
                }
                OperationsRequest request = new OperationsRequest()
                {
                    AccountId = AccountsControl.Instance.CurrentAccountId,
                    From = from,
                    To = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                    State = OperationState.Unspecified
                };
                OperationsResponse response;
                if (IsTest)
                {
                    response = await sandboxServiceClient.GetSandboxOperationsAsync(request, headers);
                }
                else
                {
                    response = await operationsServiceClient.GetOperationsAsync(request, headers);
                }
                if (response.Operations.Count == 0) return;
                foreach (Operation o in response.Operations)
                {
                    TOperation op = operations.SingleOrDefault(s => s.Id == o.Id);
                    if(op == null)
                    {
                        op = new TOperation();
                        operations.Add(op);
                    }
                    op.TnkUpdate(o);
                }
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:GetOperations()->" + ex.Status.ToString());
            }
        }
        #endregion

        #region Портфель
        public async Task GetPortfolio(string accountId, TPositions portfolio)
        {
            try
            {
                PortfolioRequest request = new PortfolioRequest()
                {
                    AccountId = accountId
                };
                PortfolioResponse response;
                if (Mode == ServerMode.Test)
                {
                    response = await sandboxServiceClient.GetSandboxPortfolioAsync(request, headers);
                }
                else
                {
                    response = await operationsServiceClient.GetPortfolioAsync(request, headers);
                }
                portfolio.TotalAmountShares = Utils.Convertor.MoneyToDec(response.TotalAmountShares);
                portfolio.TotalAmountBonds = Utils.Convertor.MoneyToDec(response.TotalAmountBonds);
                portfolio.TotalAmountEtf = Utils.Convertor.MoneyToDec(response.TotalAmountEtf);
                portfolio.TotalAmountCurrencies = Utils.Convertor.MoneyToDec(response.TotalAmountCurrencies);
                portfolio.TotalAmountFutures = Utils.Convertor.MoneyToDec(response.TotalAmountFutures);
                portfolio.ExpectedYield = Utils.Convertor.QuotationToDec(response.ExpectedYield);
                if (response.Positions.Count > 0)
                {
                    portfolio.Clear();
                    foreach(PortfolioPosition p in response.Positions)
                    {
                        portfolio.Add(new Entities.TPosition(p.Figi)
                        {
                            Quantity = Utils.Convertor.QuotationToDec(p.Quantity),
                            AveragePositionPrice = Utils.Convertor.MoneyToDec(p.AveragePositionPrice),
                            ExpectedYield = Utils.Convertor.QuotationToDec(p.ExpectedYield),
                        });
                    }
                }
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:GetPortfolio(" + accountId + ")->" + ex.Status.ToString());
            }
        }

        public async Task GetBalance(string accountId, TPositions portfolio)
        {
            if (Mode == ServerMode.Test) return;
            try
            {
                WithdrawLimitsRequest request = new WithdrawLimitsRequest() { AccountId = accountId };
                WithdrawLimitsResponse response = await operationsServiceClient.GetWithdrawLimitsAsync(request, headers);
                ObservableCollection<TMoney> money = new ObservableCollection<TMoney>();
                foreach (MoneyValue m in response.Money) money.Add(new TMoney(Utils.Convertor.MoneyToDec(m), m.Currency));
                portfolio.Money = money;
                ObservableCollection<TMoney> Blocked = new ObservableCollection<TMoney>();
                foreach (MoneyValue m in response.Blocked) Blocked.Add(new TMoney(Utils.Convertor.MoneyToDec(m), m.Currency));
                portfolio.Blocked = Blocked;
                ObservableCollection<TMoney> BlockedG = new ObservableCollection<TMoney>();
                foreach (MoneyValue m in response.BlockedGuarantee) BlockedG.Add(new TMoney(Utils.Convertor.MoneyToDec(m), m.Currency));
                portfolio.BlockedGuarantee = BlockedG;
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:GetBalance("+ accountId + ")->" + ex.Status.ToString());

            }
        }
        #endregion

        #region Аккаунты
        public event IServer.AccountsChangedHandler AccountsChangedEvent;
        public async Task GetAccounts(Entities.TAccounts accounts)
        {
            try
            {
                GetAccountsRequest request = new GetAccountsRequest();
                GetAccountsResponse response;
                if (Mode == ServerMode.Test)
                    response = await sandboxServiceClient.GetSandboxAccountsAsync(request, headers);
                else
                    response = await usersServiceClient.GetAccountsAsync(request, headers);
                accounts.Clear();
                if (response.Accounts.Count > 0)
                {
                    foreach (Account account in response.Accounts)
                    {
                        Entities.TAccount my = new Entities.TAccount();
                        accounts.Add(my);
                        my.TnkUpdate(account);
                    }
                }
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:GetAccounts(TAccounts)->" + ex.Status.ToString());
            }
        }

        async public void AddTestAccount()
        {
            try
            {
                OpenSandboxAccountRequest request = new OpenSandboxAccountRequest();
                OpenSandboxAccountResponse response = await sandboxServiceClient.OpenSandboxAccountAsync(request, headers);
                AccountsChangedEvent?.Invoke();
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:AddTestAccount()->" + ex.Status.ToString());
            }
        }

        async public void CloseTestAccount(string accountId)
        {
            try
            {
                CloseSandboxAccountRequest request = new CloseSandboxAccountRequest()
                {
                    AccountId = accountId
                };
                CloseSandboxAccountResponse response = await sandboxServiceClient.CloseSandboxAccountAsync(request, headers);
                AccountsChangedEvent?.Invoke();
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:AddTestAccount("+accountId+")->" + ex.Status.ToString());
            }
        }

        async public void AddMoneyToTestAccount(decimal money, string curency, string accountId)
        {
            try
            {
                SandboxPayInRequest request = new SandboxPayInRequest()
                {
                    AccountId = accountId,
                    Amount = Utils.Convertor.DecToMoney(money, curency)
                };
                SandboxPayInResponse response = await sandboxServiceClient.SandboxPayInAsync(request, headers);
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:AddMoneyToTestAccount(" + accountId + ")->" + ex.Status.ToString());
            }
        }
        #endregion

        #region Инструменты
        public async Task FillBonds(TInstruments inst)
        {
            try
            {
                InstrumentsRequest request = new InstrumentsRequest()
                {
                    InstrumentStatus = InstrumentStatus.All
                };
                BondsResponse br = await instrumentsServiceClient.BondsAsync(request, headers);
                foreach(Bond i in br.Instruments)
                {
                    TInstrument mi = inst.GetByFigi(i.Figi);
                    if(mi == null)
                    {
                        mi = new TInstrument(i.Figi);
                        inst.Add(mi);
                    }
                    mi.TnkUpdate(i);
                }
                inst.OnChanged();
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:FillBods(" + ")->" + ex.Status.ToString());
            }
        }

        public async Task FillCurrencies(TInstruments inst)
        {
            try
            {
                InstrumentsRequest request = new InstrumentsRequest()
                {
                    InstrumentStatus = InstrumentStatus.All
                };
                CurrenciesResponse br = await instrumentsServiceClient.CurrenciesAsync(request, headers);
                foreach (Currency i in br.Instruments)
                {
                    TInstrument mi = inst.GetByFigi(i.Figi);
                    if (mi == null)
                    {
                        mi = new TInstrument(i.Figi);
                        inst.Add(mi);
                    }
                    mi.TnkUpdate(i);
                }
                inst.OnChanged();
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:FillCurrencies(" + ")->" + ex.Status.ToString());
            }
        }

        public async Task FillEtfs(TInstruments inst)
        {
            try
            {
                InstrumentsRequest request = new InstrumentsRequest()
                {
                    InstrumentStatus = InstrumentStatus.All
                };
                EtfsResponse br = await instrumentsServiceClient.EtfsAsync(request, headers);
                foreach (Etf i in br.Instruments)
                {
                    TInstrument mi = inst.GetByFigi(i.Figi);
                    if (mi == null)
                    {
                        mi = new TInstrument(i.Figi);
                        inst.Add(mi);
                    }
                    mi.TnkUpdate(i);
                }
                inst.OnChanged();
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:FillEtfs(" + ")->" + ex.Status.ToString());
            }
        }

        public async Task FillFutures(TInstruments inst)
        {
            try
            {
                InstrumentsRequest request = new InstrumentsRequest()
                {
                    InstrumentStatus = InstrumentStatus.All
                };
                FuturesResponse br = await instrumentsServiceClient.FuturesAsync(request, headers);
                foreach (Future i in br.Instruments)
                {
                    TInstrument mi = inst.GetByFigi(i.Figi);
                    if (mi == null)
                    {
                        mi = new TInstrument(i.Figi);
                        inst.Add(mi);
                    }
                    mi.TnkUpdate(i);
                }
                inst.OnChanged();
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:FillFutures(" + ")->" + ex.Status.ToString());
            }
        }

        public async Task FillShares(TInstruments inst)
        {
            try
            {
                InstrumentsRequest request = new InstrumentsRequest()
                {
                    InstrumentStatus = InstrumentStatus.All
                };
                SharesResponse br = await instrumentsServiceClient.SharesAsync(request, headers);
                foreach (Share i in br.Instruments)
                {
                    TInstrument mi = inst.GetByFigi(i.Figi);
                    if (mi == null)
                    {
                        mi = new TInstrument(i.Figi);
                        inst.Add(mi);
                    }
                    mi.TnkUpdate(i);
                }
                inst.OnChanged();
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:FillShares(" + ")->" + ex.Status.ToString());
            }
        }

        public async Task GetLastPrices(TInstruments instruments, string[] figis = null)
        {
            try
            {
                GetLastPricesRequest request = new GetLastPricesRequest();
                if(figis != null)
                {
                    for(int i = 0; i < figis.Length; i++)
                    {
                        request.Figi.Add(figis[i]);
                    }
                }
                GetLastPricesResponse response = await marketDataServiceClient.GetLastPricesAsync(request, headers);
                foreach(LastPrice p in response.LastPrices)
                {
                    TInstrument inst = instruments.GetByFigi(p.Figi);
                    if (inst != null)
                    {
                        inst.LastPrice = Utils.Convertor.QuotationToDec(p.Price);
                        inst.LastPriceTime = (p.Time == null) ? DateTime.MinValue : p.Time.ToDateTime();
                    }
                }
            }catch(RpcException ex)
            {
                Utils.Loger.Error("Server:GetLastPrices(" + ")->" + ex.Status.ToString());
            }
        }

        public async Task FillInstruments(TInstruments instruments, InstrumentType type)
        {
            switch (type)
            {
                case InstrumentType.Bond:
                    await FillBonds(instruments);
                    instruments.OnChanged();
                    break;
                case InstrumentType.Curency:
                    await FillCurrencies(instruments);
                    instruments.OnChanged();
                    break;
                case InstrumentType.Etf:
                    await FillEtfs(instruments);
                    instruments.OnChanged();
                    break;
                case InstrumentType.Future:
                    await FillFutures(instruments);
                    instruments.OnChanged();
                    break;
                case InstrumentType.Share:
                    await FillShares(instruments);
                    instruments.OnChanged();
                    break;
                case InstrumentType.All:
                    await FillInstruments(instruments);
                    instruments.OnChanged();
                    break;
            }
        }

        public async Task FillInstruments(TInstruments instruments)
        {
            await FillBonds(instruments);
            await FillCurrencies(instruments);
            await FillEtfs(instruments);
            await FillFutures(instruments);
            await FillShares(instruments);
            instruments.OnChanged();
        }
        #endregion

        #region Свечи
        public async Task<List<TCandle>> GetCandles(string figi, DateTime b, DateTime e)
        {
            try
            {
                GetCandlesRequest request = new GetCandlesRequest()
                {
                    Figi = figi,
                    From = b.ToUniversalTime().ToTimestamp(),
                    To = e.ToUniversalTime().ToTimestamp(),
                    Interval = CandleInterval._1Min
                };
                GetCandlesResponse response = await marketDataServiceClient.GetCandlesAsync(request, headers);
                List<TCandle> result = new List<TCandle>();
                if (response.Candles.Count > 0)
                {
                    foreach (HistoricCandle c in response.Candles)
                    {
                        TCandle i = new TCandle(c, TCandleInterval._1min);
                        i.Interval = TCandleInterval._1min;
                        result.Add(i);
                    }
                }
                return result;
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:GetCandles(" + ")->" + ex.Status.ToString());
                return null;
            }
        }
        #endregion

        #region Подписки
        private AsyncDuplexStreamingCall<MarketDataRequest, MarketDataResponse> stream;
        private AsyncServerStreamingCall<TradesStreamResponse> OrderStream;

        private bool _StreamLocked = false;
        private MarketDataRequest marketStreamRequest = new MarketDataRequest() { };
        // Подписка на стакан
        public async void SubscribeOrderBook(string figi, int Depth, SubscriptionAction action)
        {
            try
            {
                while (_StreamLocked) await Task.Delay(50);
                _StreamLocked = true;
                marketStreamRequest.SubscribeOrderBookRequest = new SubscribeOrderBookRequest()
                {
                    SubscriptionAction = action,
                    Instruments = { new OrderBookInstrument() { Figi = figi, Depth = Depth } }
                };
                await stream.RequestStream.WriteAsync(marketStreamRequest);
                _StreamLocked = false;
            }
            catch(RpcException ex)
            {
                Utils.Loger.Error("Server:SubscribeOrderBook->" + ex.Status.ToString());
                _StreamLocked = false;
            }
        }

        // Подписка на свечу
        public async void SubscribeCandle(string figi, SubscriptionAction action)
        {
            try
            {
                while (_StreamLocked) await Task.Delay(50);
                _StreamLocked = true;
                marketStreamRequest.SubscribeCandlesRequest = new SubscribeCandlesRequest()
                {
                    SubscriptionAction = action,
                    Instruments = { new CandleInstrument() { Figi = figi, Interval = SubscriptionInterval.OneMinute } }
                };
                await stream.RequestStream.WriteAsync(marketStreamRequest);
                _StreamLocked = false;
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:SubscribeOrderBook->" + ex.Status.ToString());
                _StreamLocked = false;
            }
        }

        // Подписка на обезличенные сделки
        public async void SubscribeTrade(string figi, SubscriptionAction action)
        {
            try
            {
                while (_StreamLocked) await Task.Delay(50);
                _StreamLocked = true;
                await stream.RequestStream.WriteAsync(
                    new MarketDataRequest()
                    {
                        SubscribeTradesRequest = new SubscribeTradesRequest()
                        {
                            SubscriptionAction = action,
                            Instruments = { new TradeInstrument() { Figi = figi } }
                        }
                    });
                _StreamLocked = false;
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:SubscribeOrderBook->" + ex.Status.ToString());
                _StreamLocked = false;
            }
        }

        // Подписка на торговые статусы
        public async void SubscribeInfo(string figi, SubscriptionAction action)
        {
            try
            {
                while (_StreamLocked) await Task.Delay(50);
                _StreamLocked = true;
                await stream.RequestStream.WriteAsync(
                    new MarketDataRequest()
                    {
                        SubscribeInfoRequest = new SubscribeInfoRequest()
                        {
                            SubscriptionAction = action,
                            Instruments = { new InfoInstrument() { Figi = figi } }
                        }
                    });
                _StreamLocked = false;
            }
            catch (RpcException ex)
            {
                Utils.Loger.Error("Server:SubscribeOrderBook->" + ex.Status.ToString());
                _StreamLocked = false;
            }
        }
        #endregion

        #region Stream Listener
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

        public SubscribeCandlesResponse SubscribeCandlesStatus;
        public SubscribeOrderBookResponse SubscribeOrderBookStatus;
        public SubscribeTradesResponse SubscribeTradesStatus;
        public SubscribeInfoResponse SubscribeInfoStatus;

        private bool isListened = false;
        public async void StartStreamListener()
        {
            try
            {
                isListened = true;
                while(await stream.ResponseStream.MoveNext())
                {
                    if (isListened == false) return;
                    // check data
                    if(stream.ResponseStream.Current.Orderbook != null)
                    {
                        StreamOrderBookRequestedEvent?.Invoke(stream.ResponseStream.Current.Orderbook);
                    }
                    if(stream.ResponseStream.Current.Candle != null)
                    {
                        StreamCandleRequestedEvent?.Invoke(new TCandle(stream.ResponseStream.Current.Candle, TCandleInterval._1min));
                    }
                    if(stream.ResponseStream.Current.Trade != null)
                    {
                        StreamTradeRequestedEvent?.Invoke(stream.ResponseStream.Current.Trade);
                    }
                    if (stream.ResponseStream.Current.TradingStatus != null)
                    {
                        StreamTradingStatusRequestedEvent?.Invoke(stream.ResponseStream.Current.TradingStatus);
                    }
                    if (stream.ResponseStream.Current.Ping != null)
                    {
                        StreamPingRequestedEvent?.Invoke(stream.ResponseStream.Current.Ping);
                        Utils.Loger.Log("Ping:" + stream.ResponseStream.Current.Ping.ToString());
                    }
                    // Check responses
                    if(stream.ResponseStream.Current.SubscribeCandlesResponse != null)
                    {
                        SubscribeCandlesStatus = stream.ResponseStream.Current.SubscribeCandlesResponse;
                        Utils.Loger.Log("SubscribeCandlesStatus:" + SubscribeCandlesStatus.CandlesSubscriptions.ToString());
                    }
                    if(stream.ResponseStream.Current.SubscribeOrderBookResponse != null)
                    {
                        SubscribeOrderBookStatus = stream.ResponseStream.Current.SubscribeOrderBookResponse;
                        Utils.Loger.Log("SubscribeOrderBookStatus:" + SubscribeOrderBookStatus.OrderBookSubscriptions.ToString());
                    }
                    if(stream.ResponseStream.Current.SubscribeTradesResponse != null)
                    {
                        SubscribeTradesStatus = stream.ResponseStream.Current.SubscribeTradesResponse;
                        Utils.Loger.Log("SubscribeTradesStatus:" + SubscribeTradesStatus.TradeSubscriptions.ToString());
                    }
                    if(stream.ResponseStream.Current.SubscribeInfoResponse != null)
                    {
                        SubscribeInfoStatus = stream.ResponseStream.Current.SubscribeInfoResponse;
                        Utils.Loger.Log("SubscribeInfoStatus:" + SubscribeInfoStatus.InfoSubscriptions.ToString());
                    }
                }
            }catch(RpcException ex)
            {
                Utils.Loger.Error("Server:StartStreamListener->" + ex.Status.ToString());
            }
        }
        #endregion

        #region Load/Save config
        public void Initialize()
        {
            instrumentsServiceClient = new InstrumentsService.InstrumentsServiceClient(channel);
            marketDataServiceClient = new Tinkoff.InvestApi.V1.MarketDataService.MarketDataServiceClient(channel);
            marketDataStreamServiceClient = new MarketDataStreamService.MarketDataStreamServiceClient(channel);
            operationsServiceClient = new OperationsService.OperationsServiceClient(channel);
            ordersServiceClient = new OrdersService.OrdersServiceClient(channel);
            ordersStreamServiceClient = new OrdersStreamService.OrdersStreamServiceClient(channel);
            sandboxServiceClient = new SandboxService.SandboxServiceClient(channel);
            stopOrdersServiceClient = new StopOrdersService.StopOrdersServiceClient(channel);
            usersServiceClient = new UsersService.UsersServiceClient(channel);
            stream = marketDataStreamServiceClient.MarketDataStream(headers);
            TradesStreamRequest request = new TradesStreamRequest();
            OrderStream = ordersStreamServiceClient.TradesStream(request, headers);
            StartStreamListener();
            StartOrderStreamListener();
        }

        public void Reload()
        {
            stream.Dispose();
            OrderStream.Dispose();
            isListened = false;
            isOrdersListened = false;
            stream = marketDataStreamServiceClient.MarketDataStream(headers);
            OrderStream = ordersStreamServiceClient.TradesStream(new TradesStreamRequest(), headers);
            StartStreamListener();
            StartOrderStreamListener();
            AccountsChangedEvent?.Invoke();
        }
        
        public void Unload()
        {
            IO.Save();
            isListened = false;
            isOrdersListened = false;
            stream.Dispose();
            OrderStream.Dispose();
        }
        #endregion
    }
}
