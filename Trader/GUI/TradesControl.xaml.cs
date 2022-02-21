using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SciChart.Charting.Visuals.Axes.LabelProviders;
using SciChart.Charting.Model.DataSeries;
using Tinkoff.InvestApi.V1;
using Trader.Network;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Trader.GUI
{
    #region NumericLabelProviders
    public class TradesNumericLabelAsksProvider : NumericLabelProvider
    {
        public override string FormatLabel(IComparable dataValue)
        {
            // Note: Implement as you wish, converting Data-Value to string
            if (!string.IsNullOrEmpty(TradesControl.Instance.Figi))
            {
                int index = int.Parse(dataValue.ToString());
                TradesControl.GlassOrder order = null;
                if (index < TradesControl.Instance.Asks.Count)
                    order = TradesControl.Instance.Asks[index];
                if (order != null) return order.Price.ToString("0.########") + "/" + order.Quantity.ToString();
            }
            return "";
        }

        public override string FormatCursorLabel(IComparable dataValue)
        {
            if (!string.IsNullOrEmpty(TradesControl.Instance.Figi))
            {
                int index = int.Parse(dataValue.ToString());
                TradesControl.GlassOrder order = null;
                if (index < TradesControl.Instance.Asks.Count)
                    order = TradesControl.Instance.Asks[index];
                if (order != null) return order.Price.ToString("0.########");
            }
            return "";
        }
    }
    public class TradesNumericLabelBidsProvider : NumericLabelProvider
    {
        public override string FormatLabel(IComparable dataValue)
        {
            // Note: Implement as you wish, converting Data-Value to string
            if (!string.IsNullOrEmpty(TradesControl.Instance.Figi))
            {
                int index = int.Parse(dataValue.ToString());
                TradesControl.GlassOrder order = null;
                if (index < TradesControl.Instance.Bids.Count)
                    order = TradesControl.Instance.Bids[index];
                if (order != null) return order.Price.ToString("0.########") + "/" + order.Quantity.ToString();
            }
            return "";
        }

        public override string FormatCursorLabel(IComparable dataValue)
        {
            if (!string.IsNullOrEmpty(TradesControl.Instance.Figi))
            {
                int index = int.Parse(dataValue.ToString());
                TradesControl.GlassOrder order = null;
                if (index < TradesControl.Instance.Bids.Count)
                    order = TradesControl.Instance.Bids[index];
                if (order != null) return order.Price.ToString("0.########");
            }
            return "";
        }
    }
    #endregion

    public partial class TradesControl : UserControl
    {
        public static TradesControl Instance;

        public class GlassOrder
        {
            public double Price { get; set; }
            public double Quantity { get; set; }
        }
        private string _figi;
        public string Figi
        {
            get
            {
                return _figi;
            }
            set
            {
                if (!string.IsNullOrEmpty(_figi))
                {
                    UnSubscribe();
                    ClearCharts();
                }
                _figi = value;
                if (!string.IsNullOrEmpty(_figi))
                {
                    Subscribe();
                }
                else
                {
                    ClearCharts();
                }
            }
        }
        private int _depth;
        public int Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                if (!string.IsNullOrEmpty(_figi))
                {
                    ServersManager.Instance.SubscribeOrderBook(_figi, Depth, SubscriptionAction.Unsubscribe);
                }
                _depth = value;
                if (!string.IsNullOrEmpty(_figi))
                {
                    ServersManager.Instance.SubscribeOrderBook(_figi, Depth, SubscriptionAction.Subscribe);
                }
            }
        }

        private bool IsSubscribed = false;

        public SecurityTradingStatus TradingStatus = SecurityTradingStatus.Unspecified;

        public delegate void TradesControlClickHandler(string price);
        public event TradesControlClickHandler PriceSelectEvent;

        private readonly XyDataSeries<double, double> asksSeries;
        private readonly XyDataSeries<double, double> bidsSeries;
        public List<GlassOrder> Asks = new List<GlassOrder>();
        public List<GlassOrder> Bids = new List<GlassOrder>();

        public TradesControl()
        {
            InitializeComponent();
            if (Instance == null) Instance = this;
            asksSeries = new XyDataSeries<double, double> { SeriesName = "Продажа" };
            bidsSeries = new XyDataSeries<double, double> { SeriesName = "Покупка" };
            bidsColumnSeries.DataSeries = bidsSeries;
            bidsSeries.AcceptsUnsortedData = true;
            asksColumnSeries.DataSeries = asksSeries;
            asksSeries.AcceptsUnsortedData = true;
            ServersManager.Instance.StreamOrderBookRequestedEvent += OnOrderBook;
            ServersManager.Instance.StreamTradingStatusRequestedEvent += OnTradingStatusChanged;
            Depth = 10;
        }

        public void ClearCharts()
        {
            asksSeries.Clear();
            bidsSeries.Clear();
            Asks.Clear();
            Bids.Clear();
        }

        public void OnTradingStatusChanged(TradingStatus status)
        {
            TradingStatus = status.TradingStatus_;
            InstStatus.Content = TradingStatus switch
            {
                SecurityTradingStatus.Unspecified => "Торговый статус не определен",
                SecurityTradingStatus.NotAvailableForTrading => "Не доступен для торгов",
                SecurityTradingStatus.OpeningPeriod => "Период открытия торгов",
                SecurityTradingStatus.ClosingPeriod => "Период закрытия торгов",
                SecurityTradingStatus.BreakInTrading => "Перерыв в торговле",
                SecurityTradingStatus.NormalTrading => "Нормальная торговля",
                SecurityTradingStatus.ClosingAuction => "Аукцион закрытия",
                SecurityTradingStatus.DarkPoolAuction => "Аукцион крупных пакетов",
                SecurityTradingStatus.DiscreteAuction => "Дискретный аукцион",
                SecurityTradingStatus.OpeningAuctionPeriod => "Аукцион открытия",
                SecurityTradingStatus.TradingAtClosingAuctionPrice => "Период торгов по цине аукциона закрытия",
                SecurityTradingStatus.SessionAssigned => "Сессия принята",
                SecurityTradingStatus.SessionClose => "Сессия закрыта",
                SecurityTradingStatus.SessionOpen => "Сессия открыта",
                SecurityTradingStatus.DealerNormalTrading => "Нормальная торговля дилера",
                SecurityTradingStatus.DealerBreakInTrading => "Прерванная торговля дилера",
                SecurityTradingStatus.DealerNotAvailableForTrading => "Дилер не доступен для торгов",
                _ => "Не известно",
            };
        }

        public void OnOrderBook(OrderBook orderBook)
        {
            using (sciChartAsks.SuspendUpdates())
            {
                asksSeries.Clear();
                Asks.Clear();
                int i = 0;
                foreach (Order o in orderBook.Asks)
                {
                    GlassOrder order = new GlassOrder() { Price = (double)Utils.Convertor.QuotationToDec(o.Price), Quantity = o.Quantity };
                    Asks.Add(order);
                    asksSeries.Append(i, order.Quantity);
                    i++;
                }
            }
            this.Dispatcher.Invoke(() => sciChartAsks.ZoomExtents());
            using (sciChartBids.SuspendUpdates())
            {
                bidsSeries.Clear();
                Bids.Clear();
                int i = 0;
                foreach (Order o in orderBook.Bids)
                {
                    GlassOrder order = new GlassOrder() { Price = (double)Utils.Convertor.QuotationToDec(o.Price), Quantity = o.Quantity };
                    Bids.Add(order);
                    bidsSeries.Append(i, order.Quantity);
                    i++;
                }
            }
            this.Dispatcher.Invoke(() => sciChartBids.ZoomExtents());
        }

        public void Subscribe()
        {
            if (!IsSubscribed)
            {
                ServersManager.Instance.SubscribeOrderBook(_figi, Depth, SubscriptionAction.Subscribe);
                ServersManager.Instance.SubscribeInfo(_figi, SubscriptionAction.Subscribe);
                IsSubscribed = true;
            }
        }

        public void UnSubscribe()
        {
            if (IsSubscribed)
            {
                ServersManager.Instance.SubscribeOrderBook(_figi, Depth, SubscriptionAction.Subscribe);
                ServersManager.Instance.SubscribeInfo(_figi, SubscriptionAction.Subscribe);
                IsSubscribed = false;
            }
        }

        #region Ивенты
        private void SciChart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var hitTestPoint = e.GetPosition((sender as SciChart.Charting.Visuals.SciChartSurface).GridLinesPanel as UIElement);
            foreach (var renderableSeries in (sender as SciChart.Charting.Visuals.SciChartSurface).RenderableSeries)
            {
                var hitTestInfo = renderableSeries.HitTestProvider.HitTest(hitTestPoint, true);
                if (hitTestInfo.IsHit)
                {
                    var seriesInfo = renderableSeries.GetSeriesInfo(hitTestInfo);
                    PriceSelectEvent?.Invoke(seriesInfo.FormattedXValue);
                    GlassPrice.Text = seriesInfo.FormattedXValue;
                }
            }
        }

        private void GlassDepth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) Depth = int.Parse((e.AddedItems[0] as ComboBoxItem).Content.ToString());
        }

        private void BayMarketButton_Click(object sender, RoutedEventArgs e)
        {
            if(InstrumentsControl.Instance.CurrentInstrumentFigi != null)
                OrdersControl.Instance.BayMarket(InstrumentsControl.Instance.CurrentInstrumentFigi, long.Parse(GlassQuantity.Text), decimal.Parse(GlassPrice.Text));
        }

        private void SellMarketButton_Click(object sender, RoutedEventArgs e)
        {
            if (InstrumentsControl.Instance.CurrentInstrumentFigi != null)
                OrdersControl.Instance.SellMarket(InstrumentsControl.Instance.CurrentInstrumentFigi, long.Parse(GlassQuantity.Text), decimal.Parse(GlassPrice.Text));
        }

        private void BayLimitButton_Click(object sender, RoutedEventArgs e)
        {
            if (InstrumentsControl.Instance.CurrentInstrumentFigi != null)
                OrdersControl.Instance.BayLimit(InstrumentsControl.Instance.CurrentInstrumentFigi, long.Parse(GlassQuantity.Text), decimal.Parse(GlassPrice.Text));
        }

        private void SellLimitButton_Click(object sender, RoutedEventArgs e)
        {
            if (InstrumentsControl.Instance.CurrentInstrumentFigi != null)
                OrdersControl.Instance.SellLimit(InstrumentsControl.Instance.CurrentInstrumentFigi, long.Parse(GlassQuantity.Text), decimal.Parse(GlassPrice.Text));
        }

        private void ReturnLimitsButton_Click(object sender, RoutedEventArgs e)
        {
            if (InstrumentsControl.Instance.CurrentInstrumentFigi != null)
                OrdersControl.Instance.CancelOrders(InstrumentsControl.Instance.CurrentInstrumentFigi);
        }
        #endregion
    }
}
