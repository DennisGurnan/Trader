using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using SciChart.Examples.ExternalDependencies.Data;
using Trader.ViewModels;
using Trader.ViewModels.Chart.Annotations;
using Trader.Entities;

namespace Trader.GUI
{
    /// <summary>
    /// Логика взаимодействия для RobotControl.xaml
    /// </summary>
    public partial class RobotControl : UserControl, INotifyPropertyChanged
    {
        public enum StateMachine
        {
            FindIn, FindOut, Unknown
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static RobotControl Instance;
        #region Входные параметры
        // Эмулятор
        private DateTime _StartDate;
        public DateTime StartDate
        {
            get => _StartDate;
            set
            {
                string[] str = StartTime.Split(':');
                value = value.AddHours(-value.Hour + int.Parse(str[0]));
                value = value.AddMinutes(-value.Minute + int.Parse(str[1]));
                _StartDate = value;
                CurrentDate = value;
                RaisePropertyChangedEvent("StartDate");
            }
        }
        private DateTime _CurrentDate;
        public DateTime CurrentDate
        {
            get => _CurrentDate;
            set
            {
                _CurrentDate = value;
                RaisePropertyChangedEvent("currentFigi");
                RaisePropertyChangedEvent("CurrentDate");
                RaisePropertyChangedEvent("currentRsi");
                RaisePropertyChangedEvent("currentVolume");
                RaisePropertyChangedEvent("currentHiVal");
                RaisePropertyChangedEvent("currentLowVal");
                if (cursor != null)
                {
                    cursor.X1 = value;
                    cursor.Y1 = currentLowVal;
                }
                if (ChartControl.Instance.Candles != null)
                {
                    DateTime d = ChartControl.Instance.Candles.CurrentCandles.CandleData.XValues.Last(x => x <= CurrentDate);
                    currentDateIndex = ChartControl.Instance.Candles.CurrentCandles.CandleData.XValues.IndexOf(d);
                }
                else currentDateIndex = - 1;

            }
        }
        public string StartTime
        {
            get
            {
                return StartDate.Hour.ToString() + ":" + StartDate.Minute.ToString();
            }
            set
            {
                try
                {
                    string[] str = value.Split(':');
                    if (str.Length != 2) return;
                    DateTime newDate = _StartDate.AddHours(-StartDate.Hour + int.Parse(str[0]));
                    _StartDate = newDate.AddMinutes(-newDate.Minute + int.Parse(str[1]));
                    StartDate = newDate.AddMinutes(-newDate.Minute + int.Parse(str[1]));
                    RaisePropertyChangedEvent("StartTime");
                    RaisePropertyChangedEvent("StartDate");
                }
                catch(Exception ex)
                {

                }
            }
        }
        // Индикаторы
        public string currentFigi
        {
            get
            {
                if (ChartControl.Instance.Candles != null)
                    return ChartControl.Instance.Candles.Figi;
                else return "";
            }
        }
        public int currentDateIndex;

        public double currentRsi
        {
            get
            {
                if (ChartControl.Instance.Candles != null)
                {
                    if (currentDateIndex < 0) return 0;
                    if (!ChartControl.Instance.Candles.CurrentCandles.RsiData.HasValues) return 0;
                    return ChartControl.Instance.Candles.CurrentCandles.RsiData.YValues[currentDateIndex];
                }
                else return 0;
            }
        }
        public double currentVolume
        {
            get => ((ChartControl.Instance.Candles == null) || (currentDateIndex < 0)) ? 0 : ChartControl.Instance.Candles.CurrentCandles.VolumeData.YValues[currentDateIndex];
        }
        public MacdPoint currentMacdPoint
        {
            get
            {
                if (ChartControl.Instance.Candles != null)
                {
                    if (currentDateIndex < 0) return new MacdPoint() { Signal = 0, Divergence = 0, Macd = 0 };
                    return new MacdPoint()
                    {
                        Macd = ChartControl.Instance.Candles.CurrentCandles.MacdData.YValues[currentDateIndex],
                        Signal = ChartControl.Instance.Candles.CurrentCandles.MacdData.Y1Values[currentDateIndex],
                        Divergence = ChartControl.Instance.Candles.CurrentCandles.HistogramData.YValues[currentDateIndex]
                    };
                }
                else return new MacdPoint() { Signal = 0, Divergence = 0, Macd = 0 };
            }
        }
        public double currentHiVal
        {
            get => ((ChartControl.Instance.Candles == null) || (currentDateIndex < 0)) ? 0 : ChartControl.Instance.Candles.CurrentCandles.HighLineData.YValues[currentDateIndex];
        }
        public double currentLowVal
        {
            get => ((ChartControl.Instance.Candles == null)||(currentDateIndex < 0))?0:ChartControl.Instance.Candles.CurrentCandles.LowLineData.YValues[currentDateIndex];

        }
        public double currentPrice
        {
            get
            {
                int ind = currentDateIndex;
                return (ind >= 0)?(ChartControl.Instance.Candles.CurrentCandles.CandleData.OpenValues[ind]
                    + ChartControl.Instance.Candles.CurrentCandles.CandleData.CloseValues[ind]) / 2:0;
            }
        }
        // Операнды
        private double _Money;
        public double Money { get => _Money; set { _Money = value; RaisePropertyChangedEvent("Money"); RaisePropertyChangedEvent("Profit"); } }
        private double _Cripto;
        public double Cripto { get => _Cripto; set { _Cripto = value; RaisePropertyChangedEvent("Cripto") ; } }
        public double Profit { get => Money - 5000; }
        private StateMachine _State;
        public StateMachine State { get => _State; set { _State = value; RaisePropertyChangedEvent("State"); } }
        #endregion

        #region Просматриваемые состояния
        private bool IsCalculated = false;
        public int EmulationTiming { get; set; }
        #endregion
        private DispatcherTimer timer = new DispatcherTimer();
        private StepAnnotationViewModel cursor;
        private void RobotTick(object sender, EventArgs args)
        {
            CurrentDate = CurrentDate.AddMinutes(1);
            switch (State)
            {
                case StateMachine.FindIn:
                    FindIn();
                    break;
                case StateMachine.FindOut:
                    FindOut();
                    break;
                case StateMachine.Unknown:
                    if (Cripto == 0) State = StateMachine.FindIn;
                    else State = StateMachine.FindOut;
                    break;
            }
            // Finish
            if (IsCalculated) timer.Start();
        }
        // Поиск точки входа
        private double lastLowVal;
        private double lastRSI;
        private double LowPersent(double x, double y)
        {
            double r = (x - y) / x * 100;
            return Math.Abs(r);
        }
        private void FindIn()
        {
            double cl = currentLowVal;
            double ch = currentHiVal;
            double cp = currentPrice;
            if (cp == ccp) return;
            ccp = cp;
            if (cl > ch) return;
            if(lastLowVal == 0)
            {
                lastLowVal = cl;
                return;
            }
            if(lastLowVal > cl)
            {
                lastLowVal = cl;
                return;
            }
            else
            {
                if((cp < cl)||(LowPersent(lastLowVal, cl) < 0.2))
                {
                    return;
                }
                // Покупка
                Cripto = Money / cp;
                Money = 0;
                TTrade trade = new TTrade()
                {
                    BuySell = Tinkoff.InvestApi.V1.OrderDirection.Buy,
                    DateTime = CurrentDate,
                    DealPrice = cp,
                    Instrument = InstrumentsControl.Instance.CurrentInstrument,
                    Quantity = Cripto,
                    TotalPrice = Cripto * cp
                };
                ChartControl.Instance.CreateTradeAnnotation(trade);
                State = StateMachine.FindOut;
                ccp = cp;
            }
        }
        private double ccp;
        private double lastCP;
        private void FindOut()
        {
            double ch = currentHiVal;
            double cl = currentLowVal;
            double cp = currentPrice;
            if (lastCP == cp) return;
            if ((ch > cl)&&(ccp<cp))
            {
                lastLowVal = cl;
                lastCP = cp;
                return;
            }
            if(lastLowVal < cl)
            {
                lastLowVal = cl;
                lastCP = cp;
                return;
            }
            if (((ccp > cp)&&(LowPersent(ccp,cp) < 0.5))||(lastCP < cp)||(LowPersent(cp, lastLowVal) < 0.2)) return;
            TTrade trade = new TTrade()
            {
                BuySell = Tinkoff.InvestApi.V1.OrderDirection.Sell,
                DateTime = CurrentDate,
                DealPrice = cp,
                Instrument = InstrumentsControl.Instance.CurrentInstrument,
                Quantity = Cripto,
                TotalPrice = Cripto * cp
            };
            Money = Cripto * cp;
            Cripto = 0;
            ChartControl.Instance.CreateTradeAnnotation(trade);
            State = StateMachine.FindIn;
        }

        public RobotControl()
        {
            InitializeComponent();
            if (Instance == null) Instance = this;
            DataContext = this;
            StartDate = DateTime.Now;
            timer.Tick += RobotTick;
        }

        #region Buttons
        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddMinutes(1);
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsCalculated)
            {
                IsCalculated = false;
                CalculateButton.Content = "Расчет";
                timer.Stop();
            }
            else
            {
                IsCalculated = true;
                CalculateButton.Content = "Пауза";
                timer.Interval = TimeSpan.FromMilliseconds(EmulationTiming);
                timer.Start();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            //ChartControl.Instance.ClearAnnotations();
            if(cursor == null)cursor = ChartControl.Instance.CreateStepAnnotation();
            Money = 5000;
            Cripto = 0;
            State = StateMachine.Unknown;
            CurrentDate = StartDate;
        }
        #endregion
    }
}
