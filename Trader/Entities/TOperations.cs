using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Trader.Network;
using System.IO;
using Newtonsoft.Json;
using Tinkoff.InvestApi.V1;

namespace Trader.Entities
{
    public class TOperations : ObservableCollection<TOperation>, INotifyPropertyChanged
    {
        public delegate void TOperationsHandler();
        public event TOperationsHandler ChangedEvent;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DateTime LastTimeUpdate = DateTime.MinValue;

        async public void UpdateFromServer(string accountId)
        {
            if (!string.IsNullOrEmpty(accountId))
            {
                await ServersManager.Instance.GetOperations(this, LastTimeUpdate);
                LastTimeUpdate = DateTime.Now;
                ChangedEvent?.Invoke();
            }
        }

        public List<string> OperationsList
        {
            get
            {
                List<string> o = new List<string>();
                List<OperationType> t = this.GroupBy(x => x.OperationType).Select(x => x.Key).ToList();
                o.Add("");
                foreach (OperationType a in t) o.Add(a.ToString());
                return o;
            }
        }
        public List<string> InstrumentsList
        {
            get => this.GroupBy(x => x.InstrumentName).Select(x => x.Key).ToList();
        }
        public List<string> TypesList
        {
            get
            {
                List<string> o = new List<string>();
                List<string> t = this.GroupBy(x => x.Type).Select(x => x.Key).ToList();
                o.Add("");
                o.AddRange(t);
                return o;
            }
        }

        private string _TypeFilter;
        public string TypeFilter
        {
            get => _TypeFilter;
            set
            {
                _TypeFilter = value;
                RaisePropertyChangedEvent("OperationsView");
            }
        }
        private string _InstrumentsFilter;
        public string InstrumentsFilter
        {
            get => _InstrumentsFilter;
            set
            {
                _InstrumentsFilter = value;
                RaisePropertyChangedEvent("OperationsView");
            }
        }
        private string _OperationFilter;
        public string OperationFilter
        {
            get => _OperationFilter;
            set
            {
                _OperationFilter = value;
                RaisePropertyChangedEvent("OperationsView");
            }
        }

        public List<TOperation> OperationsView
        {
            get
            {
                return this.Where(x => (
                (string.IsNullOrEmpty(InstrumentsFilter) || (InstrumentsFilter == x.InstrumentName))
                && (string.IsNullOrEmpty(OperationFilter) || (OperationFilter == x.Type))
                && (string.IsNullOrEmpty(TypeFilter) || (TypeFilter == x.OperationType.ToString()))
                )).ToList();
            }
        }


        public TOperations() : base()
        {
            ChangedEvent += OnChanged;
        }

        public void OnChanged()
        {
            RaisePropertyChangedEvent("OperationsView");
            RaisePropertyChangedEvent("OperationsList");
            RaisePropertyChangedEvent("InstrumentsList");
            RaisePropertyChangedEvent("TypesList");
        }

        #region Загрузка - сохранение
        public void Save()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\data\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += ServersManager.Instance.CurrentServer.Name+GUI.AccountsControl.Instance.CurrentAccountId+ "-operations.json";
            List<TOperation> inst = new List<TOperation>();
            foreach (TOperation t in this) inst.Add(t);
            string json = JsonConvert.SerializeObject(inst, Formatting.Indented);
            File.WriteAllText(path, json);
            Utils.Config conf = new Utils.Config("operations");
            conf.SetVal(ServersManager.Instance.CurrentServer.Name, "A"+ GUI.AccountsControl.Instance.CurrentAccountId, LastTimeUpdate.ToString());
            conf.Save();
        }

        async public Task Load()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\data\\" + ServersManager.Instance.CurrentServer.Name+GUI.AccountsControl.Instance.CurrentAccountId;
            path += "-operations.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                List<TOperation> inst = JsonConvert.DeserializeObject<List<TOperation>>(json);
                foreach (TOperation t in inst) Add(t);
                Utils.Config conf = new Utils.Config("operations");
                LastTimeUpdate = DateTime.Parse(conf.GetVal(ServersManager.Instance.CurrentServer.Name, "A"+GUI.AccountsControl.Instance.CurrentAccountId, LastTimeUpdate.ToString()));
                if (ChangedEvent != null) ChangedEvent.Invoke();
            }
            else
            {
                UpdateFromServer(GUI.AccountsControl.Instance.CurrentAccountId);
            }
        }
        #endregion
    }
}
