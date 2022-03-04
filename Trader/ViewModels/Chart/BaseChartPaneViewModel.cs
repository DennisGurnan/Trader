using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.ViewportManagers;
using SciChart.Charting.Visuals.TradeChart;
using SciChart.Examples.ExternalDependencies.Common;
using SciChart.Charting.Visuals.Annotations;
using Trader.Entities;
using Trader.ViewModels.Chart.Annotations;
using Tinkoff.InvestApi.V1;

namespace Trader.ViewModels
{
    public abstract class BaseChartPaneViewModel : BaseViewModel, IChildPane
    {
        public IEnumerable<int> StrokeThicknesses { get; }
        private int _selectedStrokeThickness = 1;
        public int SelectedStrokeThickness
        {
            get => _selectedStrokeThickness;
            set
            {
                _selectedStrokeThickness = value;
                OnPropertyChanged("SelectedStrokeThickness");
            }
        }
        private readonly ChartControlViewModel _parentViewModel;
        private readonly ObservableCollection<IRenderableSeriesViewModel> _chartSeriesViewModels;
        public SciStockChart PART_ChartPaneView;
        private string _title;
        private string _yAxisTextFormatting;
        private string _xAxisTextFormatting;
        private bool _isFirstChartPane;
        private bool _isLastChartPane;
        private double _height = double.NaN;
        public TCandleFactory Candles;

        #region Annotations
        private ObservableCollection<IAnnotationViewModel> _annotations;
        public ObservableCollection<IAnnotationViewModel> TradeAnnotations
        {
            get { return _annotations; }
            set
            {
                _annotations = value;
                OnPropertyChanged("TradeAnnotations");
            }
        }
        public void CreateTradeAnnotation(TTrade trade)
        {
            var annotation = trade.BuySell == OrderDirection.Buy ? new BuyMarkerAnnotationViewModel() : (IBuySellAnnotationViewModel)new SellMarkerAnnotationViewModel();
            annotation.TradeData = trade;
            annotation.X1 = trade.DateTime;
            annotation.Y1 = trade.DealPrice;
            TradeAnnotations.Add(annotation);
        }
        public void CreateNewsAnnotation(TNewsEvent newsEvent)
        {
            var annotation = new NewsBulletAnnotationViewModel();
            annotation.NewsData = newsEvent;
            annotation.X1 = newsEvent.DateTime;
            annotation.Y1 = 0.99;
            annotation.CoordinateMode = AnnotationCoordinateMode.RelativeY;
            TradeAnnotations.Add(annotation);
        }
        public StepAnnotationViewModel CreateStepAnnotation()
        {
            var annotation = new StepAnnotationViewModel();
            TradeAnnotations.Add(annotation);
            return annotation;
        }
        public void ClearAnnotations()
        {
            if (TradeAnnotations.Count > 0)
            {
                TradeAnnotations.Clear();
            }
        }
        #endregion

        protected BaseChartPaneViewModel(ChartControlViewModel parentViewModel, TCandleFactory candles)
        {
            _chartSeriesViewModels = new ObservableCollection<IRenderableSeriesViewModel>();
            _parentViewModel = parentViewModel;
            ViewportManager = new DefaultViewportManager();
            Candles = candles;
            TradeAnnotations = new ObservableCollection<IAnnotationViewModel>();
        }

        virtual public void Refresh() { }

        public ChartControlViewModel ParentViewModel
        {
            get { return _parentViewModel; }
        }

        public ObservableCollection<IRenderableSeriesViewModel> ChartSeriesViewModels
        {
            get { return _chartSeriesViewModels; }
        }

        public IViewportManager ViewportManager { get; set; }

        public string YAxisTextFormatting
        {
            get { return _yAxisTextFormatting; }
            set
            {
                if (_yAxisTextFormatting == value)
                    return;

                _yAxisTextFormatting = value;
                OnPropertyChanged("YAxisTextFormatting");
            }
        }

        public string XAxisTextFormatting
        {
            get => _xAxisTextFormatting;
            set
            {
                if (_xAxisTextFormatting == value) return;
                _xAxisTextFormatting = value;
                OnPropertyChanged("XAxisTextFormatting");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) return;

                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool IsFirstChartPane
        {
            get { return _isFirstChartPane; }
            set
            {
                if (_isFirstChartPane == value) return;
                _isFirstChartPane = value;
                OnPropertyChanged("IsFirstChartPane");
            }
        }

        public bool IsLastChartPane
        {
            get { return _isLastChartPane; }
            set
            {
                if (_isLastChartPane == value) return;
                _isLastChartPane = value;
                OnPropertyChanged("IsLastChartPane");
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (Math.Abs(_height - value) < double.Epsilon) return;
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        public void ZoomExtents()
        {
        }

        public ICommand ClosePaneCommand
        {
            get; set;
        }

        public ICommand OpenPaneCommand { get; set; }
    }
}
