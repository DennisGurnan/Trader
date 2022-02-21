using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Trader.Entities;

namespace Trader.GUI
{
    /// <summary>
    /// Логика взаимодействия для InstrumentsControl.xaml
    /// </summary>
    public partial class InstrumentsControl : UserControl
    {
        public static InstrumentsControl Instance;

        public delegate void InstrumentSelectHandler();
        public event InstrumentSelectHandler SelectInstrumentEvent;

        private TInstrument _currentInstrument;
        public TInstrument CurrentInstrument
        {
            get { return _currentInstrument; }
            set
            {
                _currentInstrument = value;
                if (value != null)
                {
                    ChartControl.Instance.Figi = _currentInstrument.Figi;
                    TradesControl.Instance.Figi = _currentInstrument.Figi;
                }
                else
                {
                    ChartControl.Instance.Figi = null;
                    TradesControl.Instance.Figi = null;
                }
                SelectInstrumentEvent?.Invoke();
            }
        }
        public string CurrentInstrumentFigi
        {
            get
            {
                if(_currentInstrument != null)return _currentInstrument.Figi;
                return null;
            }
            set
            {
                CurrentInstrument = Instruments.Single(s => s.Figi == value);
            }
        }
        public TInstruments Instruments { get; set; }

        public InstrumentsControl()
        {
            Instruments = new TInstruments();
            InitializeComponent();
            this.DataContext = this;
            if (Instance == null) Instance = this;
            InstrumentsType.SelectionChanged += OnTypeSelectionChanged;
            InstrumentsCurrency.SelectionChanged += OnTypeSelectionChanged;

            Network.ServersManager.Instance.ServerChangingEvent += Save;
            Network.ServersManager.Instance.ServerChangedEvent += Download;
            MainWindow.Instance.Closing += OnUnloaded;
        }

        public void Clear()
        {
            Instruments.Clear();
            CurrentInstrument = null;
        }

        public void OnUnloaded(object sender, EventArgs args)
        {
            Save();
        }

        public void Save()
        {
            Instruments.Save();
        }

        async public void Download()
        {
            Instruments.Clear();
            if(Network.ServersManager.Instance != null) await Instruments.Load();
        }

        public void Update()
        {
            Instruments.UpdateFromServer();
        }

        private void OnTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null) return;
            if (comboBox.SelectedItem == null) return;
            string item = comboBox.SelectedItem.ToString();
            switch (comboBox.Name)
            {
                case "InstrumentsCurrency":
                    Instruments.CurrencyFilter = item;
                    break;
                case "InstrumentsType":
                    Instruments.TypeFilter = item;
                    break;
            }
            InstrumentsGrid.Items.Refresh();
        }

        private void OnInstrumentsInstrumentDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (InstrumentsGrid.SelectedItem == null) return;
            TInstrument pos = (TInstrument)InstrumentsGrid.SelectedItem;
            CurrentInstrument = Instruments.Single(s => s.Figi == pos.Figi);
        }

        private void nameTicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            Instruments.TickerFilter = (sender as TextBox).Text;
        }

        private void nameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Instruments.NameFilter = (sender as TextBox).Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> figis = new List<string>();
            foreach(TInstrument item in InstrumentsGrid.Items) { figis.Add(item.Figi); }
            Instruments.GetPrices(figis.ToArray());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Instruments.UpdateFromServer();
        }
    }
}
