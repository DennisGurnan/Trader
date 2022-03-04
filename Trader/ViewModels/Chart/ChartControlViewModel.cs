using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SciChart.Charting;
using System.Threading.Tasks;
using SciChart.Charting.Common.Helpers;
using SciChart.Charting.ViewportManagers;
using SciChart.Charting.Visuals.TradeChart;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Model.DataSeries;
using SciChart.Data.Model;
using SciChart.Examples.ExternalDependencies.Common;
using SciChart.Examples.ExternalDependencies.Data;
using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi.V1;
using Trader.Network;
using Trader.Entities;
using Trader.ViewModels.Chart.Annotations;

namespace Trader.ViewModels
{
    public class ChartControlViewModel : BaseViewModel
    {
        private string _figi;
        public string Figi
        {
            get
            {
                return _figi;
            }
            set
            {
                _figi = value;
                Candles.SetFigi(_figi, BeginDate, DateTime.Now, SelectedCandleInterval);
            }
        }
        public TCandleFactory Candles;
        private DateTime _beginDate;
        public DateTime BeginDate
        {
            get => _beginDate;
            set
            {
                _beginDate = value;
                if (!string.IsNullOrEmpty(_figi))
                {
                    //Candles.SelectCandles(_beginDate, DateTime.Now, SelectedCandleInterval);
                    int index = Candles.GetDataIndex(BeginDate);
                    XVisibleRange = new IndexRange(index, Candles.CurrentCandles.CandleData.Count - 1);
                }
            }
        }
        public PricePaneViewModel PricePanel;
        public ChartControlViewModel()
        {
            Candles = new TCandleFactory();
            ZoomModeCommand = new ActionCommand(SetZoomMode);
            PanModeCommand = new ActionCommand(SetPanMode);
            ZoomExtentsCommand = new ActionCommand(ZoomExtends);

            StrokeThicknesses = new[] { 1, 2, 3 };
            CandleIntervals = new[]
            {
                TCandleInterval._1min,
                TCandleInterval._5min,
                TCandleInterval._15min,
                TCandleInterval._30min,
                TCandleInterval._60min,
                TCandleInterval._day
            };
            SelectedCandleInterval = TCandleInterval._1min;
            SeriesStyles = new[] { "OHLC", "Candlestick", "Line", "Mountain" };
            SelectedStrokeThickness = 1;
            SelectedSeriesStyle = "Candlestick";
            _beginDate = DateTime.Now - TimeSpan.FromDays(1);
            _verticalChartGroupId = Guid.NewGuid().ToString();
            _viewportManager = new DefaultViewportManager();
            var closePaneCommand = new ActionCommand<IChildPane>(pane => ChartPaneViewModels.Remove((BaseChartPaneViewModel)pane));
            PricePanel = new PricePaneViewModel(this, Candles)
            {
                IsFirstChartPane = true,
                ViewportManager = _viewportManager
            };
            _chartPaneViewModels.Add(PricePanel);
            _chartPaneViewModels.Add(new MacdPaneViewModel(this, Candles)
            {
                Title = "MACD",
                ClosePaneCommand = closePaneCommand

            });
            _chartPaneViewModels.Add(new RsiPaneViewModel(this, Candles)
            {
                Title = "RSI",
                ClosePaneCommand = closePaneCommand
            });
            _chartPaneViewModels.Add(new VolumePaneViewModel(this, Candles)
            {
                Title = "Volume",
                ClosePaneCommand = closePaneCommand,
                IsLastChartPane = true
            });

            SetZoomMode();
        }

        public PricePaneViewModel GetPricePaneViewModel()
        {
            foreach (BaseChartPaneViewModel p in _chartPaneViewModels) if (p.GetType() == typeof(PricePaneViewModel)) return p as PricePaneViewModel;
            return null;
        }

        #region Interfaces
        private ObservableCollection<IRenderableSeriesViewModel> _seriesViewModels;
        public ObservableCollection<IRenderableSeriesViewModel> SeriesViewModels
        {
            get => _seriesViewModels;
            set
            {
                _seriesViewModels = value;
                OnPropertyChanged("SeriesViewModels");
            }
        }
        private IndexRange _xAxisVisibleRange;
        private ObservableCollection<BaseChartPaneViewModel> _chartPaneViewModels = new ObservableCollection<BaseChartPaneViewModel>();
        private readonly ICommand _closePaneCommand;
        private bool _isPanEnabled;
        private bool _isZoomEnabled;
        private string _verticalChartGroupId;
        private readonly IViewportManager _viewportManager;
        private int _selectedStrokeThickness;
        public int SelectedStrokeThickness
        {
            get => _selectedStrokeThickness;
            set
            {
                _selectedStrokeThickness = value;
                foreach (BaseChartPaneViewModel m in _chartPaneViewModels)
                {
                    m.SelectedStrokeThickness = value;
                }
                OnPropertyChanged("SelectedStrokeThickness");
            }
        }
        private string _selectedSeriesStyle;
        public string SelectedSeriesStyle
        {
            get => _selectedSeriesStyle;
            set
            {
                _selectedSeriesStyle = value;
                OnPropertyChanged("SelectedSeriesStyle");

                if (_selectedSeriesStyle == "OHLC")
                {
                    _chartPaneViewModels[0].ChartSeriesViewModels[0] = new OhlcRenderableSeriesViewModel
                    {
                        DataSeries = _chartPaneViewModels[0].ChartSeriesViewModels[0].DataSeries,
                        StyleKey = "BaseRenderableSeriesStyle"
                    };
                }
                else if (_selectedSeriesStyle == "Candlestick")
                {
                    if (_chartPaneViewModels.Count > 0)
                    {
                        _chartPaneViewModels[0].ChartSeriesViewModels[0] = new CandlestickRenderableSeriesViewModel
                        {
                            DataSeries = _chartPaneViewModels[0].ChartSeriesViewModels[0].DataSeries,
                            StyleKey = "BaseRenderableSeriesStyle"
                        };
                    }
                }
                else if (_selectedSeriesStyle == "Line")
                {
                    _chartPaneViewModels[0].ChartSeriesViewModels[0] = new LineRenderableSeriesViewModel
                    {
                        DataSeries = _chartPaneViewModels[0].ChartSeriesViewModels[0].DataSeries,
                        StyleKey = "BaseRenderableSeriesStyle"
                    };
                }
                else if (_selectedSeriesStyle == "Mountain")
                {
                    _chartPaneViewModels[0].ChartSeriesViewModels[0] = new MountainRenderableSeriesViewModel
                    {
                        DataSeries = _chartPaneViewModels[0].ChartSeriesViewModels[0].DataSeries,
                        StyleKey = "BaseRenderableSeriesStyle"
                    };
                }

                OnPropertyChanged("SeriesViewModels");
            }
        }
        private TCandleInterval _SelectedCandleInterval;
        public TCandleInterval SelectedCandleInterval
        {
            get => _SelectedCandleInterval;
            set
            {
                _SelectedCandleInterval = value;
                OnPropertyChanged("SelectedCandleInterval");
                if (!string.IsNullOrEmpty(Figi))
                {
                    Candles.CurrentCandleInterval = _SelectedCandleInterval;
                    foreach (BaseChartPaneViewModel m in _chartPaneViewModels) m.Refresh();
                }
            }
        }
        public IEnumerable<string> SeriesStyles { get; }
        public IEnumerable<TCandleInterval> CandleIntervals { get; }
        public IEnumerable<int> StrokeThicknesses { get; }

        private void ZoomExtends()
        {
            _viewportManager.AnimateZoomExtents(TimeSpan.FromMilliseconds(500));
        }

        public IEnumerable<string> AllThemes { get { return ThemeManager.AllThemes; } }

        public ICommand ZoomModeCommand { get; private set; }

        public ICommand PanModeCommand { get; private set; }

        public ICommand ZoomExtentsCommand { get; private set; }

        public string VerticalChartGroupId
        {
            get { return _verticalChartGroupId; }
            set
            {
                if (_verticalChartGroupId == value) return;
                _verticalChartGroupId = value;
                OnPropertyChanged("VerticalChartGroupId");
            }
        }

        public IndexRange XVisibleRange
        {
            get { return _xAxisVisibleRange; }
            set
            {
                if (Equals(_xAxisVisibleRange, value)) return;

                _xAxisVisibleRange = value;
                OnPropertyChanged("XVisibleRange");
            }
        }

        public ObservableCollection<BaseChartPaneViewModel> ChartPaneViewModels
        {
            get { return _chartPaneViewModels; }
            set
            {
                if (_chartPaneViewModels == value) return;

                _chartPaneViewModels = value;
                OnPropertyChanged("ChartPaneViewModels");
            }
        }

        public bool IsPanEnabled
        {
            get { return _isPanEnabled; }
            set
            {
                _isPanEnabled = value;
                OnPropertyChanged("IsPanEnabled");
            }
        }

        public bool IsZoomEnabled
        {
            get { return _isZoomEnabled; }
            set
            {
                _isZoomEnabled = value;
                OnPropertyChanged("IsZoomEnabled");
            }
        }

        private void SetPanMode()
        {
            IsPanEnabled = true;
            IsZoomEnabled = false;
        }

        private void SetZoomMode()
        {
            IsPanEnabled = false;
            IsZoomEnabled = true;
        }
        #endregion
    }
}
