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

namespace Trader.GUI
{
    /// <summary>
    /// Логика взаимодействия для AccountsControl.xaml
    /// </summary>
    public partial class AccountsControl : UserControl
    {
        public static AccountsControl Instance;
        public delegate void AccountsHandler();
        public event AccountsHandler AccountChangeEvent;

        public Entities.TAccounts accounts = new Entities.TAccounts();

        public Entities.TAccount CurrentAccount
        {
            get => accounts.CurrentAccount;
            set { if (value != null) accounts.CurrentAccountId = value.Id; }
        }

        public string CurrentAccountId
        {
            get => accounts.CurrentAccountId;
        }

        public AccountsControl()
        {
            InitializeComponent();
            if (Instance == null) Instance = this;
            accounts.CollectionChanged += OnCollectionChanged;
            accountsGrid.ItemsSource = accounts;
            DeleteAccountButton.IsEnabled = false;
            AddMoneyButton.IsEnabled = false;
        }

        public void Download()
        {
            accounts.FillFromServer();
        }

        private void OnCollectionChanged(object sender, EventArgs args)
        {
            if (ServersManager.Instance.ServerMode == ServerMode.Test)
            {
                ButtonsBar.Visibility = Visibility.Visible;
            }
            else
            {
                ButtonsBar.Visibility = Visibility.Collapsed;
            }
        }

        private void accountsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (accountsGrid.SelectedItem == null) return;
            accounts.CurrentAccountId = (accountsGrid.SelectedItem as Entities.TAccount).Id;
            if (AccountChangeEvent != null) AccountChangeEvent.Invoke();
            MainWindow.Instance.ChangeBottomPanel(1);
        }

        private void accountsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServersManager.Instance.ServerMode == ServerMode.Test)
            {
                if (accountsGrid.SelectedItem != null)
                {
                    DeleteAccountButton.IsEnabled = true;
                    AddMoneyButton.IsEnabled = true;
                }
                else
                {
                    DeleteAccountButton.IsEnabled = false;
                    AddMoneyButton.IsEnabled = false;
                }
            }
        }

        private void AddMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            if (accountsGrid.SelectedItem == null) return;
            int money = int.Parse(feeMany.Text);
            accounts.AddMoneyToTestAccount(
                money,
                Curency.Text,
                (accountsGrid.SelectedItem as Entities.TAccount).Id
                );
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            accounts.AddTestAccount();
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (accountsGrid.SelectedItem == null) return;
            accounts.CloseTestAccount((accountsGrid.SelectedItem as Entities.TAccount).Id);
        }
    }
}
