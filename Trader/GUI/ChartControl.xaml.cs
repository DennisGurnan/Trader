using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Trader.ViewModels;
using SciChart.Examples.ExternalDependencies.Data;
using Trader.Entities;
using System.ComponentModel;
using Trader.ViewModels.Chart.Annotations;


namespace Trader.GUI
{
    public partial class ChartControl : UserControl, INotifyPropertyChanged
    {
        public static ChartControl Instance;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ChartControlViewModel control;
        public string Figi
        {
            get
            {
                if (control != null) return control.Figi;
                return null;
            }
            set
            {
                if (control != null) control.Figi = value;
            }
        }
        public TCandleFactory Candles
        {
            get
            {
                if (control == null) return null;
                return control.Candles;
            }
        }
        // Macd
        public int MacdSlow
        {
            get
            {
                if (Candles != null) return Candles.CurrentCandles.MacdSlow;
                return 0;
            }
            set
            {
                if (Candles != null) { Candles.CurrentCandles.MacdSlow = value; }
            }
        }
        public int MacdFast
        {
            get
            {
                if (Candles != null) return Candles.CurrentCandles.MacdFast;
                return 0;
            }
            set
            {
                if (Candles != null) { Candles.CurrentCandles.MacdFast = value; }
            }
        }
        public int MacdSignal
        {
            get
            {
                if (Candles != null) return Candles.CurrentCandles.MacdSignal;
                return 0;
            }
            set
            {
                if (control != null) { Candles.CurrentCandles.MacdSignal = value; }
            }
        }
        public MacdPoint LastMacdPoint
        {
            get
            {
                if (Candles != null) return new MacdPoint()
                {
                    Macd = Candles.CurrentCandles.MacdData.YValues.Last(),
                    Signal = Candles.CurrentCandles.MacdData.Y1Values.Last(),
                    Divergence = Candles.CurrentCandles.HistogramData.YValues.Last()
                };
                return new MacdPoint();
            }
        }
        // RSI
        public int RsiPeriod
        {
            get
            {
                if (Candles != null) return Candles.CurrentCandles.RsiPeriod;
                return 0;
            }
            set
            {
                if (control != null) { Candles.CurrentCandles.RsiPeriod = value; }
            }
        }
        // Hi/Lo
        public int LoValue
        {
            get
            {
                if (Candles != null) return Candles.CurrentCandles.LoSteps;
                return 0;
            }
            set
            {
                if (control != null) { Candles.CurrentCandles.LoSteps = value; }
            }
        }
        public int HiValue
        {
            get
            {
                if (Candles != null) return Candles.CurrentCandles.HiSteps;
                return 0;
            }
            set
            {
                if (control != null) { Candles.CurrentCandles.HiSteps = value; }
            }
        }

        public DateTime BeginDate
        {
            get
            {
                if (control != null) return control.BeginDate;
                return DateTime.MinValue;
            }
            set
            {
                if (control != null) control.BeginDate = value;
                RaisePropertyChangedEvent("BeginDate");
            }
        }

        #region Annotations
        public void CreateTradeAnnotation(TTrade trade)
        {
            control.PricePanel.CreateTradeAnnotation(trade);
        }
        public void CreateNewsAnnotation(TNewsEvent newsEvent)
        {
            control.PricePanel.CreateNewsAnnotation(newsEvent);
        }
        public StepAnnotationViewModel CreateStepAnnotation()
        {
            return control.PricePanel.CreateStepAnnotation();
        }
        public void ClearAnnotations()
        {
            control.PricePanel.ClearAnnotations();
        }
        #endregion

        public ChartControl()
        {
            InitializeComponent();
            if (Instance == null) Instance = this;
            Loaded += OnLoaded;
            DownloadBeginDate.SelectedDate = DateTime.Now;
            DownloadEndDate.SelectedDate = DateTime.Now;
        }

        private void OnLoaded(object sender, EventArgs args)
        {
            control = DataContext as ChartControlViewModel;
        }

        async private void DownloadHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if((DownloadBeginDate.SelectedDate != null) && (DownloadEndDate.SelectedDate != null)&&(DownloadBeginDate.SelectedDate <= DownloadEndDate.SelectedDate))
                await Candles.Download(DownloadBeginDate.SelectedDate.Value, DownloadEndDate.SelectedDate.Value);
        }
    }
}
