using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Threading;
using Trader.Entities;

namespace Trader.GUI
{
    /// <summary>
    /// Логика взаимодействия для OperationsControl.xaml
    /// </summary>
    public partial class OperationsControl : UserControl
    {
        public static OperationsControl Instance;

        public TOperations Operations { get; set; }

        public TimeSpan UpdateInterval
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }

        private DispatcherTimer timer = new DispatcherTimer();

        public OperationsControl()
        {
            InitializeComponent();
            Operations = new TOperations();
            if (Instance == null) Instance = this;
            this.DataContext = this;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += OnTimerTick;
            timer.Start();
            Network.ServersManager.Instance.ServerChangingEvent += Save;
            AccountsControl.Instance.AccountChangeEvent += Download;
            MainWindow.Instance.Closing += OnUnloaded;
        }

        public void OnUnloaded(object sender, EventArgs args)
        {
            Save();
        }

        async public void Download()
        {
            Operations.Clear();
            if ((Network.ServersManager.Instance != null)
                && !string.IsNullOrEmpty(AccountsControl.Instance.CurrentAccountId))
                await Operations.Load();
        }

        public void Save()
        {
            if(!string.IsNullOrEmpty(AccountsControl.Instance.CurrentAccountId))
                Operations.Save();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(AccountsControl.Instance.CurrentAccountId))
            {
                Operations.UpdateFromServer(AccountsControl.Instance.CurrentAccountId);
            }
            timer.Start();
        }

        private void SelectorChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null) return;
            if (comboBox.SelectedItem == null) return;
            string item = comboBox.SelectedItem.ToString();
            switch (comboBox.Name)
            {
                case "OperationSelector":
                    Operations.OperationFilter = item;
                    break;
                case "InstrumentSelector":
                    Operations.InstrumentsFilter = item;
                    break;
                case "TypeSelector":
                    Operations.TypeFilter = item;
                    break;
            }
            operationsGrid.Items.Refresh();
        }

        public void Update()
        {
            Operations.UpdateFromServer(AccountsControl.Instance.CurrentAccountId);
        }

        public void Clear()
        {
            Operations.Clear();
        }
    }
}
