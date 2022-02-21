using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader.Network;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Trader.Entities
{
    [Serializable]
    public class TInstruments:ObservableCollection<TInstrument>, INotifyPropertyChanged
    {
        public delegate void TInstrumentsChanged();
        public event TInstrumentsChanged ChangedEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public DateTime LastTimeUpdate = DateTime.MinValue;
        public List<string> TypesList
        {
            get
            {
                List<string> m = new List<string>();
                m.Add("");
                m.AddRange(this.GroupBy(x => x.InstrumentType).Select(x => x.Key).ToList());
                return m;
            }
        }
        public List<string> CurrencyList
        {
            get
            {
                List<string> m = new List<string>();
                m.Add("");
                m.AddRange(this.GroupBy(x => x.Currency).Select(x => x.Key).ToList());
                return m;
            }
        }

        private string _TypeFilter;
        public string TypeFilter { get => _TypeFilter;
            set
            {
                _TypeFilter = value;
                RaisePropertyChangedEvent("InstrumentsView");
            }
        }
        private string _CurrencyFilter;
        public string CurrencyFilter { get => _CurrencyFilter;
            set
            {
                _CurrencyFilter = value;
                RaisePropertyChangedEvent("InstrumentsView");
            }
        }
        private string _NameFilter;
        public string NameFilter
        {
            get => _NameFilter;
            set
            {
                _NameFilter = value;
                RaisePropertyChangedEvent("InstrumentsView");
            }
        }
        private string _TickerFilter;
        public string TickerFilter
        {
            get => _TickerFilter;
            set
            {
                _TickerFilter = value;
                RaisePropertyChangedEvent("InstrumentsView");
            }
        }

        public TInstruments() : base()
        {
            this.ChangedEvent += OnChanged;
        }

        public List<TInstrument> InstrumentsView
        {
            get
            {
                List<TInstrument> i = this.Where(x =>(
                (string.IsNullOrEmpty(CurrencyFilter) || (CurrencyFilter == x.Currency))
                &&
                (string.IsNullOrEmpty(_TypeFilter) || (_TypeFilter == x.InstrumentType))
                &&
                (string.IsNullOrEmpty(TickerFilter) || Regex.IsMatch(x.Ticker, TickerFilter, RegexOptions.IgnoreCase))
                &&
                (string.IsNullOrEmpty(NameFilter) || Regex.IsMatch(x.Name, NameFilter, RegexOptions.IgnoreCase))
                ) ).ToList();
                return i;
            }
        }

        public TInstrument GetByFigi(string figi)
        {
            return this.SingleOrDefault(x => x.Figi == figi);
        }

        public void OnChanged()
        {
            RaisePropertyChangedEvent("InstrumentsView");
            RaisePropertyChangedEvent("TypesList");
            RaisePropertyChangedEvent("CurrencyList");
        }

        async public void GetPrices(string[] figis)
        {
            await ServersManager.Instance.GetLastPrices(this, figis);
        }

        async public void UpdateFromServer()
        {
            await ServersManager.Instance.FillInstruments(this);
            LastTimeUpdate = DateTime.Now;
        }

        #region Загрузка - сохранение
        public void Save()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\data\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += ServersManager.Instance.CurrentServer.Name + "-instruments.json";
            List<TInstrument> inst = new List<TInstrument>();
            foreach (TInstrument t in this) inst.Add(t);
            string json = JsonConvert.SerializeObject(inst, Formatting.Indented);
            File.WriteAllText(path, json);
            Utils.Config conf = new Utils.Config("instruments");
            conf.SetVal(ServersManager.Instance.CurrentServer.Name, "LastTimeUpdate", LastTimeUpdate.ToString());
            conf.Save();
        }

        async public Task Load()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\data\\" + ServersManager.Instance.CurrentServer.Name;
            path += "-instruments.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                List<TInstrument> inst = JsonConvert.DeserializeObject<List<TInstrument>>(json);
                foreach (TInstrument t in inst) Add(t);
                Utils.Config conf = new Utils.Config("instruments");
                LastTimeUpdate = DateTime.Parse(conf.GetVal(ServersManager.Instance.CurrentServer.Name, "LastTimeUpdate", LastTimeUpdate.ToString()));
                ChangedEvent?.Invoke();
            }
            else
            {
                UpdateFromServer();
            }
        }
        #endregion
    }
}
