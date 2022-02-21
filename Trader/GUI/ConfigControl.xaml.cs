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

        public Utils.Config IO = new Utils.Config("config");

        #region Chart
        public int MACDFast
        {
            get
            {
                return IO.GetVal("Chart", "MACDFastVal", 0);
            }
            set
            {
                IO.SetVal("Chart", "MACDFastVal", value);
                if(ChartControl.Instance != null) ChartControl.Instance.MacdFast = value;
                OnPropertyChanged("MACDFastValStr");
            }
        }
        public int MACDSlow
        {
            get
            {
                return IO.GetVal("Chart", "MACDSlowVal", 0);
            }
            set
            {
                IO.SetVal("Chart", "MACDSlowVal", value);
                if (ChartControl.Instance != null) ChartControl.Instance.MacdSlow = value;
                OnPropertyChanged("MACDSlowValStr");
            }
        }
        public int MACDSignal
        {
            get
            {
                return IO.GetVal("Chart", "MACDSignalVal", 0);
            }
            set
            {
                IO.SetVal("Chart", "MACDSignalVal", value);
                ChartControl.Instance.MacdSignal = value;
            }
        }
        public int RsiPeriod
        {
            get
            {
                return IO.GetVal("Chart", "RsiPeriod", 0);
            }
            set
            {
                IO.SetVal("Chart", "RsiPeriod", value);
                ChartControl.Instance.RsiPeriod = value;
            }
        }
        public int LoLine
        {
            get
            {
                return IO.GetVal("Chart", "LoLine", 0);
            }
            set
            {
                IO.SetVal("Chart", "LoLine", value);
                ChartControl.Instance.LoValue = value;
            }
        }
        public int HiLine
        {
            get
            {
                return IO.GetVal("Chart", "HiLine", 0);
            }
            set
            {
                IO.SetVal("Chart", "HiLine", value);
                ChartControl.Instance.HiValue = value;
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
    }
}
