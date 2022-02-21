using System;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using Trader.Entities;


namespace Trader.GUI
{
    public partial class PortfolioControl : UserControl
    {
        public static PortfolioControl Instance;

        public TPositions Positions { get; set; }

        public PortfolioControl()
        {
            InitializeComponent();
            Positions = new TPositions();
            if (Instance == null) Instance = this;
            this.DataContext = this;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, EventArgs args)
        {
            OrdersControl.Instance.Orders.ChangedEvent += Update;
        }

        public void OnAccountChanged()
        {
            Positions.FillFromServer(AccountsControl.Instance.CurrentAccountId);
        }

        public void Clear()
        {
            Positions.Clear();
        }

        public void Update()
        {
            Positions.FillFromServer(AccountsControl.Instance.CurrentAccountId);
        }

        private void portfolioPositions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (portfolioPositions.SelectedItem == null) return;
            TPosition pos = portfolioPositions.SelectedItem as TPosition;
            InstrumentsControl.Instance.CurrentInstrumentFigi = pos.Instrument.Figi;
        }
    }
}
