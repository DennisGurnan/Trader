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
using Trader.Network;
using Trader.GUI;

namespace Trader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public ServersManager serversManager;

        public MainWindow()
        {
            if (Instance == null) Instance = this;
            serversManager = new ServersManager();
            InitializeComponent();
            serversManager.ServerChangedEvent += AccountsControlObject.Download;
            InstrumentsControlObject.SelectInstrumentEvent += OnInstrumentSelected;
            AccountsControlObject.AccountChangeEvent += PortfolioControlObject.OnAccountChanged;
            AccountsControlObject.AccountChangeEvent += OperationsControlObject.Update;
            AccountsControlObject.AccountChangeEvent += OrdersControlObject.OnPortfolioSelected;
            InstrumentsControlObject.Instruments.ChangedEvent += PortfolioControlObject.Update;
            Closing += OnClosing;
            // Load window parameters
            this.Top = ConfigControlObject.IO.GetVal("Main", "Top", 100);
            this.Left = ConfigControlObject.IO.GetVal("Main", "Left", 100);
            this.Height = ConfigControlObject.IO.GetVal("Main", "Height", 100);
            this.Width = ConfigControlObject.IO.GetVal("Main", "Width", 200);
            mainRow1.Height = new GridLength(ConfigControlObject.IO.GetVal("Main", "Row1", 100), GridUnitType.Star);
            mainRow2.Height = new GridLength(ConfigControlObject.IO.GetVal("Main", "Row2", 100), GridUnitType.Star);
            mainCol1.Width = new GridLength(ConfigControlObject.IO.GetVal("Main", "Col1", 100), GridUnitType.Star);
            mainCol2.Width = new GridLength(ConfigControlObject.IO.GetVal("Main", "Col2", 100), GridUnitType.Star);

            serversManager.SelectServerByName("TestServer");
        }

        async public void ChangeBottomPanel(int index)
        {
            if (BottomPanel != null)
            {
                await Task.Delay(20);
                BottomPanel.SelectedIndex = index;
            }
        }

        private void OnInstrumentSelected()
        {
            if (InstrumentsControlObject.CurrentInstrument != null)
            {
                TradesControlObject.Figi = InstrumentsControlObject.CurrentInstrument.Figi;
                (chartControlObject.DataContext as ViewModels.ChartControlViewModel).Figi = InstrumentsControlObject.CurrentInstrument.Figi;
                OrdersControlObject.OnInstrumentSelected();
                Title = "Трейдер - " + InstrumentsControlObject.CurrentInstrument.Name;
            }
            else
            {
                TradesControlObject.Figi = null;
                (chartControlObject.DataContext as ViewModels.ChartControlViewModel).Figi = null;
                Title = "Трейдер";
            }
        }

        private void OnClosing(object sender, EventArgs e)
        {
            ConfigControlObject.IO.SetVal("Main", "Top", this.Top);
            ConfigControlObject.IO.SetVal("Main", "Left", this.Left);
            ConfigControlObject.IO.SetVal("Main", "Width", this.Width);
            ConfigControlObject.IO.SetVal("Main", "Height", this.Height);
            ConfigControlObject.IO.SetVal("Main", "Row1", (int)mainRow1.ActualHeight);
            ConfigControlObject.IO.SetVal("Main", "Row2", (int)mainRow2.ActualHeight);
            ConfigControlObject.IO.SetVal("Main", "Col1", (int)mainCol1.ActualWidth);
            ConfigControlObject.IO.SetVal("Main", "Col2", (int)mainCol2.ActualWidth);
            ConfigControlObject.IO.Save();
        }
    }
}
