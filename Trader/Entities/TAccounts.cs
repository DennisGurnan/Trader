using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader.Network;
using Trader.MVVM;
using System.ComponentModel;

namespace Trader.Entities
{
    public class TAccounts:ObservableCollection<TAccount>
    {
        public delegate void MyHandler();
        public event MyHandler ChangedEvent;

        public string CurrentAccountId { get; set; }

        public TAccount CurrentAccount
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentAccountId)) return null;
                return this.Single(s => s.Id == CurrentAccountId);
            }
        }

        private Network.IServer Server;

        public TAccounts() : base()
        {
            ServersManager.Instance.ServerChangedEvent += OnServerChanged;
        }

        private void OnServerChanged()
        {
            if(Server != null)
            {
                Server.AccountsChangedEvent -= OnChanged;
            }
            Server = ServersManager.Instance.CurrentServer;
            if(Server != null)
            {
                Server.AccountsChangedEvent += OnChanged;
            }
            FillFromServer();
        }
        public async void FillFromServer()
        {
            await Server.GetAccounts(this);
            ChangedEvent?.Invoke();
        }
        private void OnChanged()
        {
            FillFromServer();
        }

        #region Tinkoff functions
        public void AddTestAccount()
        {
            Server.AddTestAccount();
        }
        public void CloseCurrentAccount()
        {
            Server.CloseTestAccount(CurrentAccountId);
            CurrentAccountId = null;
        }
        public void CloseTestAccount(string accountId)
        {
            Server.CloseTestAccount(accountId);
        }
        public void AddMoneyToTestAccount(decimal money, string curency, string accountId)
        {
            Server.AddMoneyToTestAccount(money, curency, accountId);
        }
        #endregion
    }
}
