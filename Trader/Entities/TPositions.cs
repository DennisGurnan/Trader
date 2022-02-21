using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi.V1;
using Google.Protobuf.Collections;
using Grpc.Core;
using Trader.Network;
using Trader.MVVM;


namespace Trader.Entities
{
    public class TPositions : ObservableCollection<TPosition>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private decimal _TotalAmountShares;
        private decimal _TotalAmountBonds;
        private decimal _TotalAmountEtf;
        private decimal _TotalAmountCurrencies;
        private decimal _TotalAmountFutures;
        private decimal _ExpectedYield;

        public decimal TotalAmountShares
        {
            get => _TotalAmountShares;
            set
            {
                _TotalAmountShares = value;
                RaisePropertyChangedEvent("TotalAmountShares");
                RaisePropertyChangedEvent("AllPaysView");
                RaisePropertyChangedEvent("AllGoodsView");
                RaisePropertyChangedEvent("CurrentPayAll");
            }
        }
        public decimal TotalAmountBonds
        {
            get => _TotalAmountBonds;
            set
            {
                _TotalAmountBonds = value;
                RaisePropertyChangedEvent("TotalAmountBonds");
                RaisePropertyChangedEvent("AllPaysView");
                RaisePropertyChangedEvent("AllGoodsView");
                RaisePropertyChangedEvent("CurrentPayAll");
            }
        }
        public decimal TotalAmountEtf
        {
            get => _TotalAmountEtf;
            set
            {
                _TotalAmountEtf = value;
                RaisePropertyChangedEvent("TotalAmountEtf");
                RaisePropertyChangedEvent("AllPaysView");
                RaisePropertyChangedEvent("AllGoodsView");
                RaisePropertyChangedEvent("CurrentPayAll");
            }
        }
        public decimal TotalAmountCurrencies
        {
            get => _TotalAmountCurrencies;
            set
            {
                _TotalAmountCurrencies = value;
                RaisePropertyChangedEvent("TotalAmountCurrencies");
                RaisePropertyChangedEvent("AllPaysView");
                RaisePropertyChangedEvent("AllGoodsView");
                RaisePropertyChangedEvent("CurrentPayAll");
            }
        }
        public decimal TotalAmountFutures
        {
            get => _TotalAmountFutures;
            set
            {
                _TotalAmountFutures = value;
                RaisePropertyChangedEvent("TotalAmountFutures");
                RaisePropertyChangedEvent("AllPaysView");
                RaisePropertyChangedEvent("AllGoodsView");
                RaisePropertyChangedEvent("CurrentPayAll");
            }
        }
        public decimal ExpectedYield
        {
            get => _ExpectedYield;
            set
            {
                _ExpectedYield = value;
                RaisePropertyChangedEvent("ExpectedYield");
                RaisePropertyChangedEvent("AllPaysView");
                RaisePropertyChangedEvent("AllGoodsView");
                RaisePropertyChangedEvent("CurrentPayAll");
            }
        }
        public decimal AllPaysView
        {
            get => TotalAmountShares + TotalAmountBonds + TotalAmountEtf + TotalAmountCurrencies + TotalAmountFutures;
        }
        public decimal AllGoodsView
        {
            get
            {
                if (ExpectedYield != 0)
                {
                    return ((TotalAmountShares + TotalAmountBonds
                + TotalAmountEtf + TotalAmountCurrencies
                + TotalAmountFutures) / 100 * ExpectedYield);
                }
                return 0;
            }
        }
        public decimal CurrentPayAll
        {
            get
            {
                return (
                    (TotalAmountShares + TotalAmountBonds
                + TotalAmountEtf + TotalAmountCurrencies
                + TotalAmountFutures) -
                ((TotalAmountShares + TotalAmountBonds
                + TotalAmountEtf + TotalAmountCurrencies
                + TotalAmountFutures) / 100 * ExpectedYield)
                    );
            }
        }

        public ObservableCollection<TMoney> Money;
        public ObservableCollection<TMoney> Blocked;
        public ObservableCollection<TMoney> BlockedGuarantee;

        public string MoneyView
        {
            get
            {
                if (Money == null) return "";
                string o = "| ";
                foreach(TMoney i in Money)
                {
                    o += i.Price.ToString("N2") + i.Currency + " | ";
                }
                return o;
            }
        }
        public string BlockedView
        {
            get
            {
                if (Blocked == null) return "";
                string o = "| ";
                foreach (TMoney i in Blocked)
                {
                    o += i.Price.ToString("N2") + i.Currency + " | ";
                }
                return o;
            }
        }
        public string BlockedGuaranteeView
        {
            get
            {
                if (BlockedGuarantee == null) return "";
                string o = "| ";
                foreach (TMoney i in BlockedGuarantee)
                {
                    o += i.Price.ToString("N2") + i.Currency + " | ";
                }
                return o;
            }
        }

        async public void FillFromServer(string accountId)
        {
            if (!string.IsNullOrEmpty(accountId))
            {
                await ServersManager.Instance.GetPortfolio(accountId, this);
                await ServersManager.Instance.GetBalance(accountId, this);
                RaisePropertyChangedEvent("MoneyView");
                RaisePropertyChangedEvent("BlockedView");
                RaisePropertyChangedEvent("BlockedGuaranteeView");
            }
        }
    }
}
