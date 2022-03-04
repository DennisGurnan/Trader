using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tinkoff.InvestApi.V1;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using Google.Protobuf.WellKnownTypes;
using Trader.Network;
using Newtonsoft.Json;
using SciChart.Examples.ExternalDependencies.Data;

namespace Trader.Entities
{
    public class TCandles : List<TCandle>
    {
        private TCandleFactory factory;
        private TCandleInterval interval;
        private Utils.Config config;

        public delegate void TCandlesHandler();
        public event TCandlesHandler ChangedEvent;
        public event TCandlesHandler UpdateEvent;
        public int HiSteps {
            get => config.GetVal(factory.Figi + "_" + interval.ToString(), "HiSteps", 20);
            set { config.SetVal(factory.Figi + "_" + interval.ToString(), "HiSteps", value); config.Save(); OnChanged(); }
        }
        public int LoSteps
        {
            get => config.GetVal(factory.Figi + "_" + interval.ToString(), "LoSteps", 200);
            set { config.SetVal(factory.Figi + "_" + interval.ToString(), "LoSteps", value); config.Save(); OnChanged(); }
        }
        public int RsiPeriod
        {
            get => config.GetVal(factory.Figi + "_" + interval.ToString(), "RsiPeriod", 16);
            set { config.SetVal(factory.Figi + "_" + interval.ToString(), "RsiPeriod", value); config.Save(); OnChanged(); }
        }
        public int MacdSlow
        {
            get => config.GetVal(factory.Figi + "_" + interval.ToString(), "MacdSlow", 12);
            set { config.SetVal(factory.Figi + "_" + interval.ToString(), "MacdSlow", value); config.Save(); OnChanged(); }
        }
        public int MacdFast
        {
            get => config.GetVal(factory.Figi + "_" + interval.ToString(), "MacdFast", 26);
            set { config.SetVal(factory.Figi + "_" + interval.ToString(), "MacdFast", value); config.Save(); OnChanged(); }
        }
        public int MacdSignal
        {
            get => config.GetVal(factory.Figi + "_" + interval.ToString(), "MacdSignal", 9);
            set { config.SetVal(factory.Figi + "_" + interval.ToString(), "MacdSignal", value); config.Save(); OnChanged(); }
        }

        public OhlcDataSeries<DateTime, double> CandleData = new OhlcDataSeries<DateTime, double>() { SeriesName = "OHLC" };
        public XyDataSeries<DateTime, double> LowLineData = new XyDataSeries<DateTime, double>() { SeriesName = "Low Line" };
        public XyDataSeries<DateTime, double> HighLineData = new XyDataSeries<DateTime, double>() { SeriesName = "High Line" };
        public XyDataSeries<DateTime, double> VolumeData = new XyDataSeries<DateTime, double>() { SeriesName = "Volume" };
        public XyDataSeries<DateTime, double> RsiData = new XyDataSeries<DateTime, double>() { SeriesName = "RSI" };
        public XyDataSeries<DateTime, double> HistogramData = new XyDataSeries<DateTime, double>() { SeriesName = "Histogram" };
        public XyyDataSeries<DateTime, double> MacdData = new XyyDataSeries<DateTime, double>() { SeriesName = "MACD" };
        public TCandles(TCandleFactory f, TCandleInterval i) : base()
        {
            factory = f;
            interval = i;
            factory.ChangedEvent += OnChanged;
            factory.UpdateEvent += OnUpdated;
            config = new Utils.Config("data", "SensorsSettings");
        }

        private void OnChanged()
        {
            TCandle f = factory.GetFirstCandle(interval);
            if (f == null) return;
            TCandle l = factory.GetLastCandle(interval);
            if (l == null) return;
            List<TCandle> lst = factory.GetCandlesLocal(factory.GetFirstCandle(interval).DateTime, factory.GetLastCandle(interval).DateTime, interval);
            if (lst == null) return;
            this.Clear();
            ResetRsi();
            CandleData.Clear();
            HighLineData.Clear();
            LowLineData.Clear();
            VolumeData.Clear();
            RsiData.Clear();
            HistogramData.Clear();
            MacdData.Clear();
            foreach (TCandle c in lst)
            {
                this.Add(c);
                RsiData.Append(c.DateTime, ComputeNextValue(c));
                CandleData.Append(c.DateTime, c.Open, c.High, c.Low, c.Close);
                VolumeData.Append(c.DateTime, c.Volume);
            }

            HighLineData.Append(CandleData.XValues, CandleData.CloseValues.MovingAverage(HiSteps));
            LowLineData.Append(CandleData.XValues, CandleData.CloseValues.MovingAverage(LoSteps));
            HistogramData.Append(CandleData.XValues, CandleData.CloseValues.Macd(MacdSlow, MacdFast, MacdSignal).Select(x => x.Divergence));
            MacdData.Append(CandleData.XValues, CandleData.CloseValues.Macd(MacdSlow, MacdFast, MacdSignal).Select(x => x.Macd),
                                                CandleData.CloseValues.Macd(MacdSlow, MacdFast, MacdSignal).Select(x => x.Signal));
            ChangedEvent?.Invoke();
        }

        private void OnUpdated()
        {
            TCandle c = factory.GetLastCandle(interval);
            if (c == null) return;
            if (this.Count > 0)
            {
                if (this[this.Count - 1].DateTime == c.DateTime)
                {
                    this[this.Count - 1].FromCandle(c);
                    CandleData.Update(c.DateTime, c.Open, c.High, c.Low, c.Close);
                    VolumeData.Update(c.DateTime, c.Volume);
                    return;
                }
            }
            Add(c);
            CandleData.Append(c.DateTime, c.Open, c.High, c.Low, c.Close);
            VolumeData.Append(c.DateTime, c.Volume);
            RsiData.Append(c.DateTime, ComputeNextValue(c));
            HistogramData.Clear();
            MacdData.Clear();
            HistogramData.Append(CandleData.XValues, CandleData.CloseValues.Macd(MacdSlow, MacdFast, MacdSignal).Select(x => x.Divergence));
            MacdData.Append(CandleData.XValues, CandleData.CloseValues.Macd(MacdSlow, MacdFast, MacdSignal).Select(x => x.Macd),
                                                CandleData.CloseValues.Macd(MacdSlow, MacdFast, MacdSignal).Select(x => x.Signal));
            UpdateEvent?.Invoke();
        }
        // Общет RSI
        #region Rsi
        private TCandle _previousInput;
        private int _index;
        private double _gain;
        private double _loss;
        private double _averageGain;
        private double _averageLoss;
        private double _totalGain;
        private double _totalLoss;
        private void ResetRsi()
        {
            _previousInput = null;
            _index = 0;
            _gain = 0;
            _loss = 0;
            _averageGain = 0;
            _averageLoss = 0;
        }
        public double ComputeNextValue(TCandle input)
        {
            // Formula: https://stackoverflow.com/questions/38481354/rsi-vs-wilders-rsi-calculation-problems?rq=1
            _index++;

            if (_previousInput != null)
            {
                var diff = input.Close - _previousInput.Close;
                _previousInput = input;

                if (_index <= RsiPeriod)
                {
                    if (diff >= 0)
                    {
                        _totalGain += diff;
                    }
                    else
                    {
                        _totalLoss -= diff;
                    }
                }

                if (_index < RsiPeriod)
                {
                    return 0;
                }
                else if (_index == RsiPeriod)
                {
                    _averageGain = _totalGain / RsiPeriod;
                    _averageLoss = _totalLoss / RsiPeriod;

                    double rs = _averageGain / _averageLoss;
                    return 100 - (100 / (1 + rs));
                }
                else // if (_index >= _period + 1)
                {
                    if (diff >= 0)
                    {
                        _averageGain = ((_averageGain * (RsiPeriod - 1)) + diff) / RsiPeriod;
                        _averageLoss = (_averageLoss * (RsiPeriod - 1)) / RsiPeriod;
                    }
                    else
                    {
                        _averageGain = (_averageGain * (RsiPeriod - 1)) / RsiPeriod;
                        _averageLoss = ((_averageLoss * (RsiPeriod - 1)) - diff) / RsiPeriod;
                    }

                    double rs = _averageGain / _averageLoss;
                    return 100 - (100 / (1 + rs));
                }
            }
            _previousInput = input;
            return 0;
        }
        #endregion
    }
}
