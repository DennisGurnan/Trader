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
using Trader.Entities;

namespace Trader.GUI
{
    /// <summary>
    /// Логика взаимодействия для OrdersControl.xaml
    /// </summary>
    public partial class OrdersControl : UserControl
    {
        public static OrdersControl Instance;
        public TOrders Orders { get; set; }

        public OrdersControl()
        {
            InitializeComponent();
            Orders = new TOrders();
            if (Instance == null) Instance = this;
            DataContext = this;
        }

        public void OnPortfolioSelected()
        {
            Orders.FillFromServer();
        }

        public void OnInstrumentSelected()
        {
            if (!string.IsNullOrEmpty(AccountsControl.Instance.CurrentAccountId))
            {
                Orders.FillFromServer();
            }
        }

        public void Clear()
        {
            Orders.Clear();
        }

        #region Операции
        public void SellLimit(string figi, long quantity, decimal price)
        {
            Orders.SellLimit(figi, quantity, price);
        }

        public void SellMarket(string figi, long quantity, decimal price)
        {
            Orders.SellMarket(figi, quantity, price);
        }

        public void BayLimit(string figi, long quantity, decimal price)
        {
            Orders.BayLimit(figi, quantity, price);
        }

        public void BayMarket(string figi, long quantity, decimal price)
        {
            Orders.BayMarket(figi, quantity, price);
        }

        public void CancelOrder(string id)
        {
            Orders.Cancel(id);
        }

        public void CancelOrders(string figi)
        {
            Orders.CancelByFigi(figi);
        }
        #endregion
    }
}
