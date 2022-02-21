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
        public TCandles Candles;
        private DateTime _beginDate;
        public DateTime BeginDate
        {
            get => _beginDate;
            set
            {
                _beginDate = value;
                if (!string.IsNullOrEmpty(_figi))
                {
                    Candles.SelectCandles(_beginDate, DateTime.Now, SelectedCandleInterval);
                    int index = Candles.GetDataIndex(BeginDate);
                    XVisibleRange = new IndexRange(index, Candles.TimeData.Count - 1);
                }
            }
        }

        public ChartControlViewModel()
        {
            Candles = new TCandles();
            Candles.ChangedEvent += OnCandlesChanged;
            Candles.UpdateEvent += OnLastCandleUpdate;

            ZoomModeCommand = new ActionCommand(SetZoomMode);
            PanModeCommand = new ActionCommand(SetPanMode);
            ZoomExtentsCommand = new ActionCommand(ZoomExtends);

            StrokeThicknesses = new[] { 1, 2, 3 };
            CandleIntervals = new[]
            {
                CandleInterval._1Min,
                CandleInterval._5Min,
                CandleInterval._15Min,
                CandleInterval.Hour,
                CandleInterval.Day
            };
            SelectedCandleInterval = CandleInterval._1Min;
            SeriesStyles = new[] { "OHLC", "Candlestick", "Line", "Mountain" };
            SelectedStrokeThickness = 1;
            SelectedSeriesStyle = "Candlestick";
            _beginDate = DateTime.Now - TimeSpan.FromDays(1);
            _verticalChartGroupId = Guid.NewGuid().ToString();
            _viewportManager = new DefaultViewportManager();
            var closePaneCommand = new ActionCommand<IChildPane>(pane => ChartPaneViewModels.Remove((BaseChartPaneViewModel)pane));

            _chartPaneViewModels.Add(new PricePaneViewModel(this, Candles)
            {
                IsFirstChartPane = true,
                ViewportManager = _viewportManager
            });
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

        public MacdPaneViewModel GetMacdPaneViewModel()
        {
            foreach (BaseChartPaneViewModel p in _chartPaneViewModels) if (p.GetType() == typeof(MacdPaneViewModel)) return p as MacdPaneViewModel;
            return null;
        }

        private void OnLastCandleUpdate()
        {
            lock (this)
            {
                foreach (BaseChartPaneViewModel m in _chartPaneViewModels) m.Update();
            }
        }

        private void OnCandlesChanged()
        {
            lock (this)
            {
                if (Candles.Count > 0)
                {
                    foreach (BaseChartPaneViewModel b in _chartPaneViewModels) b.Reload();
                    //XVisibleRange = new IndexRange(0, Candles.TimeData.Count - 1);
                }
            }
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
        private CandleInterval _SelectedCandleInterval;
        public CandleInterval SelectedCandleInterval
        {
            get => _SelectedCandleInterval;
            set
            {
                _SelectedCandleInterval = value;
                OnPropertyChanged("SelectedCandleInterval");
                if (!string.IsNullOrEmpty(Figi))
                {
                    Candles.SelectCandles(_SelectedCandleInterval);
                }
            }
        }
        public IEnumerable<string> SeriesStyles { get; }
        public IEnumerable<CandleInterval> CandleIntervals { get; }
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
