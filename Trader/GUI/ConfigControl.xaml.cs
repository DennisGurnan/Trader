using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Trader.Network;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Trader.GUI
{
    /// <summary>
    /// Логика взаимодействия для ConfigControl.xaml
    /// </summary>
    public partial class ConfigControl : UserControl, INotifyPropertyChanged
    {
        public static ConfigControl Instance;
        public Entities.TCandleInterval selectedInterval;
        private Entities.TCandles currentCandles
        {
            get
            {
                if (ChartControl.Instance == null) return null;
                if (ChartControl.Instance.Candles == null) return null;
                switch (selectedInterval)
                {
                    case Entities.TCandleInterval._1min:
                        return ChartControl.Instance.Candles._1min;
                    case Entities.TCandleInterval._5min:
                        return ChartControl.Instance.Candles._5min;
                    case Entities.TCandleInterval._15min:
                        return ChartControl.Instance.Candles._15min;
                    case Entities.TCandleInterval._30min:
                        return ChartControl.Instance.Candles._30min;
                    case Entities.TCandleInterval._60min:
                        return ChartControl.Instance.Candles._60min;
                }
                return null;
            }
        }

        public Utils.Config IO = new Utils.Config("config");

        #region Chart
        public int MACDFast
        {
            get
            {
                if (currentCandles != null) return currentCandles.MacdFast;
                return 0;
            }
            set
            {
                if(currentCandles != null) currentCandles.MacdFast = value;
                OnPropertyChanged("MACDFast");
            }
        }
        public int MACDSlow
        {
            get
            {
                if (currentCandles != null) return currentCandles.MacdSlow;
                return 0;
            }
            set
            {
                if (currentCandles != null) currentCandles.MacdSlow = value;
                OnPropertyChanged("MACDSlow");
            }
        }
        public int MACDSignal
        {
            get
            {
                if (currentCandles != null) return currentCandles.MacdSignal;
                return 0;
            }
            set
            {
                if (currentCandles != null) currentCandles.MacdSignal = value;
                OnPropertyChanged("MACDSignal");
            }
        }
        public int RsiPeriod
        {
            get
            {
                if (currentCandles != null) return currentCandles.RsiPeriod;
                return 0;
            }
            set
            {
                if (currentCandles != null) currentCandles.RsiPeriod = value;
                OnPropertyChanged("RsiPeriod");
            }
        }
        public int LoLine
        {
            get
            {
                if (currentCandles != null) return currentCandles.LoSteps;
                return 0;
            }
            set
            {
                if (currentCandles != null) currentCandles.LoSteps = value;
                OnPropertyChanged("LoLine");
            }
        }
        public int HiLine
        {
            get
            {
                if (currentCandles != null) return currentCandles.HiSteps;
                return 0;
            }
            set
            {
                if (currentCandles != null) currentCandles.HiSteps = value;
                OnPropertyChanged("HiLine");
            }
        }
        #endregion

        #region Server
        private List<GroupBox> ServerCollection = new List<GroupBox>();

        private void HideServerPanels()
        {
            foreach (GroupBox g in ServerCollection) g.Visibility = Visibility.Collapsed;
        }

        private void ShowPanelByName(string name)
        {
            HideServerPanels();
            switch (name)
            {
                case "Tinkoff":
                    gTinkoff.Visibility = Visibility.Visible;
                    break;
                case "Binance":
                    gBinance.Visibility = Visibility.Visible;
                    break;
                case "TestServer":
                    gTestServer.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ServerApplyButton_Click(object sender, RoutedEventArgs e)
        {
            TinkoffServer.ServerUrl = SUrl.Text;
            TinkoffServer.FullAccessToken = FullAccess.Text;
            TinkoffServer.ReadOnlyToken = ReadOnly.Text;
            TinkoffServer.TestModeToken = TestMode.Text;
            if (rFullAccess.IsChecked.Value) ServersManager.Instance.GetServerByName("Tinkoff").Mode = ServerMode.FullAccess;
            else if (rReadOnly.IsChecked.Value) ServersManager.Instance.GetServerByName("Tinkoff").Mode = ServerMode.ReadOnly;
            else ServersManager.Instance.GetServerByName("Tinkoff").Mode = ServerMode.Test;
            BinanceServer.ServerUrl = ServerUrl.Text;
            BinanceServer.WebSocket = WebSocket.Text;
            BinanceServer.ApiKey = ApiKey.Text;
            BinanceServer.SecretKey = SecretKey.Text;
            IO.Save();
            ServersManager.Instance.SaveConfig();
            ServersManager.Instance.Reload();
            AccountsControl.Instance.accounts.FillFromServer();
        }

        private void ServerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                string name = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
                ServersManager.Instance.SelectServerByName(name);
                ShowPanelByName(name);
            }
        }
        #endregion

        public ConfigControl()
        {
            InitializeComponent();
            if (Instance == null) Instance = this;
            // Server
            SUrl.Text = TinkoffServer.ServerUrl;
            FullAccess.Text = TinkoffServer.FullAccessToken;
            ReadOnly.Text = TinkoffServer.ReadOnlyToken;
            TestMode.Text = TinkoffServer.TestModeToken;
            if (ServersManager.Instance.GetServerByName("Tinkoff").Mode == ServerMode.FullAccess) rFullAccess.IsChecked = true;
            else if (ServersManager.Instance.GetServerByName("Tinkoff").Mode == ServerMode.ReadOnly) rReadOnly.IsChecked = true;
            else rTestMode.IsChecked = true;
            ServerUrl.Text = BinanceServer.ServerUrl;
            WebSocket.Text = BinanceServer.WebSocket;
            ApiKey.Text = BinanceServer.ApiKey;
            SecretKey.Text = BinanceServer.SecretKey;
            ServerSelector.Items.Clear();
            foreach(IServer s in ServersManager.Instance)
            {
                ComboBoxItem i = new ComboBoxItem() { Content = s.Name };
                ServerSelector.Items.Add(i);
            }
            ServerCollection.Add(gTinkoff);
            ServerCollection.Add(gTestServer);
            ServerCollection.Add(gBinance);
            HideServerPanels();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void ScalerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem == null) return;
            switch(((sender as ComboBox).SelectedItem as ComboBoxItem).Content)
            {
                case "1 min":
                    selectedInterval = Entities.TCandleInterval._1min;
                    break;
                case "5 min":
                    selectedInterval = Entities.TCandleInterval._5min;
                    break;
                case "15 min":
                    selectedInterval = Entities.TCandleInterval._15min;
                    break;
                case "30 min":
                    selectedInterval = Entities.TCandleInterval._30min;
                    break;
                case "60 min":
                    selectedInterval = Entities.TCandleInterval._60min;
                    break;
            }
            OnPropertyChanged("MACDFast");
            OnPropertyChanged("MACDSlow");
            OnPropertyChanged("MACDSignal");
            OnPropertyChanged("RsiPeriod");
            OnPropertyChanged("LoLine");
            OnPropertyChanged("HiLine");
        }
    }
}
