using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tinkoff.InvestApi.V1;
using Google.Protobuf.WellKnownTypes;
using Trader.Network;
using Newtonsoft.Json;
using SciChart.Examples.ExternalDependencies.Data;

namespace Trader.Entities
{
    // Набор свечей с одинаковым интервалом
    public class TCandles : List<TCandle>
    {
        public string Figi; // Текущий Figi инструмента
        public TCandle LastCandle; // Последняя свеча
        public DateTime LastBegin; // Время первой запрошенной свечи
        public DateTime LastEnd; // Время последней запрошенной свечи
        public DateTime LoadedBegin; // Время первой загруженной 1-мин свечи
        public DateTime LoadedEnd; // Время последней загруженной 1-мин свечи

        public CandleInterval CurrentCandleInterval; // Текущий интервал свечей

        // Динамические массивы последней обработки свечей включая данные датчиков
        public List<DateTime> TimeData = new List<DateTime>();
        public List<double> OpenData = new List<double>();
        public List<double> CloseData = new List<double>();
        public List<double> HighData = new List<double>();
        public List<double> LowData = new List<double>();
        public List<double> VolumeData = new List<double>();
        public List<double> RsiData = new List<double>();
        public List<double> HiLineData = new List<double>();
        public List<double> LoLineData = new List<double>();
        public List<MacdPoint> MacdData = new List<MacdPoint>();

        public delegate void TCandlesHandler();
        public event TCandlesHandler ChangedEvent;
        public event TCandlesHandler UpdateEvent;

        public bool IsSubscribed = false; // Включена подписка на свечи

        // Смена инсрумента
        public void SetFigi(string figi, DateTime b, DateTime e, CandleInterval ci = CandleInterval._1Min)
        {
            if (Figi == figi) return;
            if (!string.IsNullOrEmpty(Figi))
            {
                Save();
                Unsubscribe();
            }
            Clear();
            Figi = figi;
            if (!string.IsNullOrEmpty(Figi))
            {
                LastBegin = Interval(b, CandleInterval.Day);
                LastEnd = Interval(e, CandleInterval.Day);
                LoadedBegin = DateTime.MinValue;
                LoadedEnd = DateTime.MinValue;
                CurrentCandleInterval = ci;
                SelectCandles();
            }
        }

        // Настройки MACD
        public int MacdSlow = 12;
        public int MacdFast = 26;
        public int MacdSignal = 9;
        public int RsiPeriod = 16;
        public int LoValue = 50;
        public int HiValue = 200;
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
        #region CandleDataStorage
        // Загружат массивы данных последним диапазоном свечей
        public void SelectCandles(DateTime b, DateTime e, CandleInterval ci)
        {
            LastBegin = b;
            LastEnd = e;
            CurrentCandleInterval = ci;
            SelectCandles();
        }
        public void SelectCandles(CandleInterval ci)
        {
            CurrentCandleInterval = ci;
            SelectCandles();
        }
        private bool isInSelect = false;
        async public void SelectCandles()
        {
            if (isInSelect) return;
            isInSelect = true;
            TimeData.Clear(); OpenData.Clear(); CloseData.Clear(); HighData.Clear(); LowData.Clear(); VolumeData.Clear();
            RsiData.Clear();
            if (((Interval(LastBegin, CandleInterval.Day) < Interval(LoadedBegin, CandleInterval.Day)) && this.Count>0) || (LoadedBegin == DateTime.MinValue))
            {
                await Download();
            }
            List<TCandle> c = GetCandlesLocal(LastBegin, LastEnd, CurrentCandleInterval);
            c.Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            ResetRsi();
            foreach (TCandle i in c)
            {
                TimeData.Add(i.DateTime);
                OpenData.Add(i.Open);
                CloseData.Add(i.Close);
                HighData.Add(i.High);
                LowData.Add(i.Low);
                VolumeData.Add(i.Volume);
                RsiData.Add(ComputeNextValue(i));
            }
            MacdData = CloseData.Macd(MacdSlow, MacdFast, MacdSignal).ToList();
            HiLineData = CloseData.MovingAverage(HiValue).ToList();
            LoLineData = CloseData.MovingAverage(LoValue).ToList();
            isInSelect = false;
            ChangedEvent?.Invoke();
        }
        public bool IsCandlesPresent(DateTime day)
        {
            return !(Find(x => (x.DateTime.Day == day.Day) && (x.DateTime.Month == day.Month) && (x.DateTime.Year == day.Year)) == null);
        }
        // Возвращает последнюю свечу данного или текущего интервала
        public TCandle GetLastCandle(CandleInterval ci)
        {
            if (ci == CandleInterval.Unspecified) ci = CandleInterval._1Min;
            TCandle lastCandle = FindLast(x => x.DateTime < DateTime.Now);
            if (ci == CandleInterval._1Min) return lastCandle;
            if (ci == CandleInterval.Hour) return ConvertToCandle(lastCandle.DateTime - TimeSpan.FromMinutes(lastCandle.DateTime.Minute), ci);
            if (ci == CandleInterval.Day)
                return ConvertToCandle(lastCandle.DateTime - TimeSpan.FromHours(lastCandle.DateTime.Hour) - TimeSpan.FromMinutes(lastCandle.DateTime.Minute), ci);
            int interval = CandleIntervalToMinutes(ci);
            if (lastCandle.DateTime.Minute % interval == 0) return ConvertToCandle(lastCandle.DateTime, ci);
            DateTime date = lastCandle.DateTime;
            date.AddMinutes(-(lastCandle.DateTime.Minute % interval));
            return ConvertToCandle(date, ci);
        }
        public TCandle GetLastCandle()
        {
            return GetLastCandle(CurrentCandleInterval);
        }
        // Возвращает первую свечу из данного или текущего интервала
        public TCandle GetFirstCandle(CandleInterval ci)
        {
            return Find(x => x.Interval == ci);
        }
        public TCandle GetFirstCandle()
        {
            return Find(x => x.Interval == CurrentCandleInterval);
        }
        // Возвращает индекс в массиве данных по дате равной или ниже данной даты
        public int GetDataIndex(DateTime date)
        {
            DateTime d = TimeData.Find(x => x <= date);
            return TimeData.IndexOf(d);
        }
        #endregion
        // Взять свечи с b по e с интервалом ci
        private List<TCandle> GetCandlesLocal(DateTime b, DateTime e, CandleInterval ci)
        {
            if (ci == CandleInterval.Unspecified) ci = CandleInterval._1Min;
            List<TCandle> result = new List<TCandle>();
            for (DateTime i = b; i <= e; i += TimeSpan.FromDays(1))
            {
                result.AddRange(GetCandlesOfDay(i, ci));
            }
            return result;
        }
        #region Functions
        // переводит CandleInterval в количество минут
        private int CandleIntervalToMinutes(CandleInterval ci)
        {
            switch (ci)
            {
                case CandleInterval._5Min:
                    return 5;
                case CandleInterval._15Min:
                    return 15;
                case CandleInterval.Hour:
                    return 60;
                case CandleInterval.Day:
                    return 1440;
                default:
                    return 1;
            }
        }
        // Конвертирует 1-минутные свечи начиная с b в свечу с интервалом ci
        private TCandle ConvertToCandle(DateTime b, CandleInterval ci)
        {
            TCandle result = new TCandle();
            DateTime candleTime = b;
            int steps = CandleIntervalToMinutes(ci);
            for(int  i = 0; i < steps; i++)
            {
                TCandle nextCandle = Find(x => x.DateTime == b);
                if(nextCandle == null)
                {
                    // Неполная свеча
                    result.Interval = ci;
                    result.DateTime = candleTime;
                    if (i > 0) return result;
                    return null;
                }
                if (i == 0)
                {
                    result.High = nextCandle.High;
                    result.Low = nextCandle.Low;
                    result.Open = nextCandle.Open;
                    result.Close = nextCandle.Close;
                    result.Volume = nextCandle.Volume;
                }
                else
                {
                    result.High = Math.Max(result.High, nextCandle.High);
                    result.Low = Math.Min(result.Low, nextCandle.Low);
                    result.Close = nextCandle.Close;
                    result.Volume += nextCandle.Volume;
                }
                b = b.AddMinutes(1);
            }
            result.Interval = ci;
            result.DateTime = candleTime;
            return result;
        }
        // Возвращает свечи  дня с заданным интервалом
        private List<TCandle> GetCandlesOfDay(DateTime b, CandleInterval ci)
        {
            bool save = false;
            if (ci == CandleInterval._1Min) return FindAll(x => (x.DateTime >= b && x.DateTime <= (b + TimeSpan.FromDays(1))) && x.Interval == ci);
            int step = CandleIntervalToMinutes(ci);
            List<TCandle> result = new List<TCandle>();
            for(DateTime s = b; s < (b + TimeSpan.FromDays(1)); s = s.AddMinutes(step))
            {
                TCandle c = Find(x => (x.DateTime == s) && (x.Interval == ci));
                if (c != null)
                    result.Add(c);
                else
                {
                    c = ConvertToCandle(s, ci);
                    if (c != null)
                    {
                        Add(c);
                        result.Add(c);
                        save = true;
                    }
                }
            }
            if (save) Save();
            return result;
        }
        // Округляем до интервала
        private DateTime Interval(DateTime t, CandleInterval i)
        {
            return new DateTime(t.Ticks - (t.Ticks % (TimeSpan.TicksPerMinute * CandleIntervalToMinutes(i))));
        }
        #endregion
        #region Server
        // загружает с сервера день 1-минутных свечей
        private async Task FillDayCandles(DateTime day)
        {
            List<TCandle> cndls = new List<TCandle>();
            try
            {
                cndls = await ServersManager.Instance.GetCandles(Figi, day, day + TimeSpan.FromDays(1), CandleInterval._1Min);
            }
            catch (Exception ex)
            {
                return;
            }
            Load();
            cndls.ForEach(x => x.DateTime = Interval(x.DateTime, CandleInterval._1Min));
            for(int i = 0; i < 1440; i++)
            {
                if (day >= DateTime.Now) return;
                TCandle c = cndls.Find(x => x.DateTime == day);
                if(c != null)
                {
                    c.DateTime = Interval(c.DateTime, CandleInterval._1Min);
                    if(FindLast(x=>x.DateTime == c.DateTime) == null) Add(c);
                }
                day += TimeSpan.FromMinutes(1);
            }
            Save();
        }
        // Загружает 1-минутные свечи
        public async Task Download()
        {
            DateTime b = Interval(LastBegin.ToUniversalTime(), CandleInterval.Day);
            DateTime e = Interval(LastEnd.ToUniversalTime(), CandleInterval.Day);
            if (Load() && (this.Count > 0) && (this[0].DateTime == LastBegin) && (this.Last().DateTime == LastEnd)) return;
            for (DateTime i = b; i <= e; i = i.AddDays(1))
            {
                DateTime ie = i + TimeSpan.FromDays(1);
                if ((Find(f => (f.DateTime >= i) && (f.DateTime <= ie)) == null)
                    || (i == Interval(DateTime.Now, CandleInterval.Day)))
                {
                    await FillDayCandles(i);
                }
            }
            Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            if (this.Count > 0)
            {
                LoadedEnd = this.Last().DateTime;
                LoadedBegin = this[0].DateTime;
            }
            SelectCandles();
            Subscribe();
            Save();
        }

        public async Task Download(DateTime beginDate, DateTime endDate)
        {
            DateTime b = Interval(beginDate.ToUniversalTime(), CandleInterval.Day);
            DateTime e = Interval(endDate.ToUniversalTime(), CandleInterval.Day);
            if (b == e)
            {
                await FillDayCandles(b);
            }
            else
            {
                for (DateTime i = b; i <= e; i = i.AddDays(1))
                {
                    DateTime ie = i + TimeSpan.FromDays(1);
                    await FillDayCandles(i);
                }
            }
            Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            SelectCandles();
            Save();
        }

        public void Subscribe()
        {
            if (!IsSubscribed)
            {
                ServersManager.Instance.StreamCandleRequestedEvent += StreamCandleRequestedEvent;
                ServersManager.Instance.SubscribeCandle(Figi, SubscriptionInterval.OneMinute, SubscriptionAction.Subscribe);
                IsSubscribed = true;
            }
        }
        public void Unsubscribe()
        {
            if (IsSubscribed)
            {
                ServersManager.Instance.StreamCandleRequestedEvent -= StreamCandleRequestedEvent;
                ServersManager.Instance.SubscribeCandle(Figi, SubscriptionInterval.OneMinute, SubscriptionAction.Unsubscribe);
                IsSubscribed = false;
            }
        }
        private void StreamCandleRequestedEvent(TCandle c)
        {
            c.DateTime = Interval(c.DateTime, CandleInterval._1Min);
            TCandle candle = FindLast(x => x.DateTime == c.DateTime);
            bool isUpdate = false;
            if (candle == null)
            {
                candle = c;
                Add(candle);
                LoadedEnd = c.DateTime;
            }
            else
            {
                candle.FromCandle(c);
                isUpdate = true;
            }
            if((CurrentCandleInterval != CandleInterval._1Min) && (CurrentCandleInterval != CandleInterval.Unspecified))
            {
                if (c.DateTime.Minute % CandleIntervalToMinutes(c.Interval) != 0)
                {
                    candle = FindLast(x => x.DateTime == Interval(c.DateTime, CurrentCandleInterval));
                    candle.FromCandle(GetLastCandle());
                    isUpdate = true;
                }
            }
            if (isUpdate)
            {
                CloseData[CloseData.Count-1] = candle.Close;
                HighData[HighData.Count-1] = candle.High;
                LowData[LowData.Count-1] = candle.Low;
                VolumeData[VolumeData.Count-1] = candle.Volume;
            }
            else
            {
                TimeData.Add(candle.DateTime);
                OpenData.Add(candle.Open);
                CloseData.Add(candle.Close);
                HighData.Add(candle.High);
                LowData.Add(candle.Low);
                VolumeData.Add(candle.Volume);
                RsiData.Add(ComputeNextValue(candle));
            }
            MacdData = CloseData.Macd(MacdSlow, MacdFast, MacdSignal).ToList();
            HiLineData = CloseData.MovingAverage(HiValue).ToList();
            LoLineData = CloseData.MovingAverage(LoValue).ToList();
            Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            Save();
            //SelectCandles();
            UpdateEvent?.Invoke();
        }
        #endregion
        #region Load/Save
        private bool isSave = false;
        public void Save()
        {
            if (isSave) return;
            isSave = true;
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\data\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += ServersManager.Instance.CurrentServer.Name + "-" + Figi + "-candles.json";
            List<TCandle> inst = new List<TCandle>();
            foreach (TCandle t in this) inst.Add(t);
            inst.Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            if (inst.Count > 0)
            {
                string json = JsonConvert.SerializeObject(inst, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            isSave = false;
        }

        public bool Load()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\data\\" + ServersManager.Instance.CurrentServer.Name + "-" + Figi + "-candles.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                List<TCandle> inst = JsonConvert.DeserializeObject<List<TCandle>>(json);
                if (inst.Count == 0) return false;
                Clear();
                foreach (TCandle t in inst) Add(t);
                Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
                LoadedBegin = this[0].DateTime;
                LoadedEnd = this.Last().DateTime;
                return true;
            }
            return false;
        }
        #endregion
    }
}
