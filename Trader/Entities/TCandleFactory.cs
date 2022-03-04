using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tinkoff.InvestApi.V1;
using Trader.Network;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Trader.Entities
{
    // Набор свечей с одинаковым интервалом
    public class TCandleFactory : List<TCandle>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Figi; // Текущий Figi инструмента
        public TCandle LastCandle; // Последняя свеча
        public DateTime LastBegin; // Время первой запрошенной свечи
        public DateTime LastEnd; // Время последней запрошенной свечи
        public DateTime LoadedBegin; // Время первой загруженной 1-мин свечи
        public DateTime LoadedEnd; // Время последней загруженной 1-мин свечи

        public TCandleInterval _CurrentCandleInterval;// Текущий интервал свечей
        public TCandleInterval CurrentCandleInterval
        {
            get => _CurrentCandleInterval;
            set
            {
                _CurrentCandleInterval = value;
                RaisePropertyChangedEvent("CurrentCandleInterval");
                RaisePropertyChangedEvent("CurrentCandles");
            }
        } 

        // Динамические массивы последней обработки свечей включая данные датчиков
        public TCandles _1min;
        public TCandles _5min;
        public TCandles _15min;
        public TCandles _30min;
        public TCandles _60min;

        public TCandles CurrentCandles
        {
            get
            {
                switch (CurrentCandleInterval)
                {
                    case TCandleInterval._1min:
                        return _1min;
                    case TCandleInterval._5min:
                        return _5min;
                    case TCandleInterval._15min:
                        return _15min;
                    case TCandleInterval._30min:
                        return _30min;
                    case TCandleInterval._60min:
                        return _60min;
                    default:
                        return _1min;
                }
            }
        }

        public delegate void TCandlesHandler();
        public event TCandlesHandler ChangedEvent;
        public event TCandlesHandler UpdateEvent;

        public bool IsSubscribed = false; // Включена подписка на свечи

        public TCandleFactory()
        {
            _1min = new TCandles(this, TCandleInterval._1min);
            _5min = new TCandles(this, TCandleInterval._5min);
            _15min = new TCandles(this, TCandleInterval._15min);
            _30min = new TCandles(this, TCandleInterval._30min);
            _60min = new TCandles(this, TCandleInterval._60min);
            CurrentCandleInterval = TCandleInterval._1min;
        }

        // Смена инсрумента
        public void SetFigi(string figi, DateTime b, DateTime e, TCandleInterval ci = TCandleInterval._1min)
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
                LastBegin = Interval(b, TCandleInterval._day);
                LastEnd = Interval(e, TCandleInterval._day);
                LoadedBegin = DateTime.MinValue;
                LoadedEnd = DateTime.MinValue;
                CurrentCandleInterval = ci;
                Download();
            }
        }

        #region CandleDataStorage
        public bool IsCandlesPresent(DateTime day)
        {
            return !(Find(x => (x.DateTime.Day == day.Day) && (x.DateTime.Month == day.Month) && (x.DateTime.Year == day.Year)) == null);
        }
        // Возвращает последнюю свечу данного или текущего интервала
        public TCandle GetLastCandle(TCandleInterval ci)
        {
            //if (ci == TCandleInterval.Unspecified) ci = TCandleInterval._1min;
            TCandle lastCandle = FindLast(x => x.DateTime < DateTime.Now);
            if (ci == TCandleInterval._1min) return lastCandle;
            if (ci == TCandleInterval._60min) return ConvertToCandle(lastCandle.DateTime - TimeSpan.FromMinutes(lastCandle.DateTime.Minute), ci);
            if (ci == TCandleInterval._day)
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
        public TCandle GetFirstCandle(TCandleInterval ci)
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
            return CurrentCandles.VolumeData.FindIndex(date);
        }
        #endregion
        // Взять свечи с b по e с интервалом ci
        public List<TCandle> GetCandlesLocal(DateTime b, DateTime e, TCandleInterval ci = TCandleInterval._1min)
        {
            //if (ci == CandleInterval.Unspecified) ci = CandleInterval._1Min;
            List<TCandle> result = new List<TCandle>();
            for (DateTime i = b; i <= e; i += TimeSpan.FromDays(1))
            {
                result.AddRange(GetCandlesOfDay(i, ci));
            }
            return result;
        }
        #region Functions
        // переводит CandleInterval в количество минут
        private int CandleIntervalToMinutes(TCandleInterval ci)
        {
            switch (ci)
            {
                case TCandleInterval._5min:
                    return 5;
                case TCandleInterval._15min:
                    return 15;
                case TCandleInterval._30min:
                    return 30;
                case TCandleInterval._60min:
                    return 60;
                case TCandleInterval._day:
                    return 1440;
                default:
                    return 1;
            }
        }
        // Конвертирует 1-минутные свечи начиная с b в свечу с интервалом ci
        private TCandle ConvertToCandle(DateTime b, TCandleInterval ci)
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
        private List<TCandle> GetCandlesOfDay(DateTime b, TCandleInterval ci)
        {
            bool save = false;
            if (ci == TCandleInterval._1min) return FindAll(x => (x.DateTime >= b && x.DateTime <= (b + TimeSpan.FromDays(1))) && x.Interval == ci);
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
        private DateTime Interval(DateTime t, TCandleInterval i)
        {
            return new DateTime(t.Ticks - (t.Ticks % (TimeSpan.TicksPerMinute * CandleIntervalToMinutes(i))));
        }
        #endregion
        #region Server
        // загружает с сервера день 1-минутных свечей
        private async Task FillDayCandles(DateTime day)
        {
            List<TCandle> cndls;
            try
            {
                cndls = await ServersManager.Instance.GetCandles(Figi, day, day + TimeSpan.FromDays(1));
            }
            catch (Exception ex)
            {
                return;
            }
            cndls.ForEach(x => x.DateTime = Interval(x.DateTime, TCandleInterval._1min));
            for(DateTime i = day; i < day.AddMinutes(1440); i = i.AddMinutes(1))
            {
                if (i >= DateTime.Now) return;
                TCandle c = cndls.Find(x => x.DateTime == i);
                if(c != null)
                {
                    if(FindLast(x=>x.DateTime == c.DateTime) == null) Add(c);
                }
            }
            DateTime finish = day.AddMinutes(1440);
            for(DateTime i = day; finish.CompareTo(i) > 0; i = i.AddMinutes(5))
            {
                if (Find(x => (x.DateTime == i) && (x.Interval == TCandleInterval._5min)) == null)
                {
                    Add(ConvertToCandle(i, TCandleInterval._5min));
                }
            }
            for(DateTime i = day; finish.CompareTo(i) > 0; i = i.AddMinutes(15))
            {
                if(Find(x => (x.DateTime == i)&&(x.Interval == TCandleInterval._15min)) == null)
                {
                    Add(ConvertToCandle(i, TCandleInterval._15min));
                }
            }
            for (DateTime i = day; finish.CompareTo(i) > 0; i = i.AddMinutes(30))
            {
                if (Find(x => (x.DateTime == i) && (x.Interval == TCandleInterval._30min)) == null)
                {
                    Add(ConvertToCandle(i, TCandleInterval._30min));
                }
            }
            for (DateTime i = day; finish.CompareTo(i) > 0; i = i.AddMinutes(60))
            {
                if(Find(x=>(x.DateTime == i)&&(x.Interval == TCandleInterval._60min)) == null)
                {
                    Add(ConvertToCandle(i, TCandleInterval._60min));
                }
            }
            Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            Save();
        }
        // Загружает 1-минутные свечи
        public async Task Download()
        {
            DateTime b = Interval(LastBegin.ToUniversalTime(), TCandleInterval._day);
            DateTime e = Interval(LastEnd.ToUniversalTime(), TCandleInterval._day);
            if (Load() && (this.Count > 0) && (this[0].DateTime == LastBegin) && (this.Last().DateTime == LastEnd)) return;
            for (DateTime i = b; i <= e; i = i.AddDays(1))
            {
                DateTime ie = i + TimeSpan.FromDays(1);
                if ((Find(f => (f.DateTime >= i) && (f.DateTime <= ie)) == null)
                    || (i == Interval(DateTime.Now, TCandleInterval._day)))
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
            Subscribe();
            Save();
            ChangedEvent?.Invoke();
        }

        public async Task Download(DateTime beginDate, DateTime endDate)
        {
            DateTime b = Interval(beginDate.ToUniversalTime(), TCandleInterval._day);
            DateTime e = Interval(endDate.ToUniversalTime(), TCandleInterval._day);
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
            Save();
            ChangedEvent?.Invoke();
        }

        public void Subscribe()
        {
            if (!IsSubscribed)
            {
                ServersManager.Instance.StreamCandleRequestedEvent += StreamCandleRequestedEvent;
                ServersManager.Instance.SubscribeCandle(Figi, SubscriptionAction.Subscribe);
                IsSubscribed = true;
            }
        }
        public void Unsubscribe()
        {
            if (IsSubscribed)
            {
                ServersManager.Instance.StreamCandleRequestedEvent -= StreamCandleRequestedEvent;
                ServersManager.Instance.SubscribeCandle(Figi, SubscriptionAction.Unsubscribe);
                IsSubscribed = false;
            }
        }
        private void StreamCandleRequestedEvent(TCandle c)
        {
            c.DateTime = Interval(c.DateTime, TCandleInterval._1min);
            TCandle candle = FindLast(x => x.DateTime == c.DateTime);
            if (candle == null)
            {
                candle = c;
                Add(candle);
                LoadedEnd = c.DateTime;
            }
            else
            {
                candle.FromCandle(c);
            }
            if(CurrentCandleInterval != TCandleInterval._1min)
            {
                if (c.DateTime.Minute % CandleIntervalToMinutes(c.Interval) != 0)
                {
                    candle = FindLast(x => x.DateTime == Interval(c.DateTime, CurrentCandleInterval));
                    candle.FromCandle(GetLastCandle());
                }
            }
            Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            Save();
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
