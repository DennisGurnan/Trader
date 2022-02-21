using System;
using Tinkoff.InvestApi.V1;
using Trader.MVVM;

namespace Trader.Entities
{
    public class TAccount:ObservableObject
    {
        private string _name;
        public string Name { get => _name; set { _name = value; RaisePropertyChangedEvent("Name"); } } // Наименование аккаунта (в тесте всегда "")
        private string _id;
        public string Id { get => _id; set { _id = value; RaisePropertyChangedEvent("Id"); } } // Id аккаунта
        private AccountType _Type;
        public AccountType Type { get => _Type; set { RaisePropertyChangedEvent("Type"); } } // Тип аккаунта всегда Tinkoff
        private AccountStatus _Status;
        public AccountStatus Status { get => _Status; set { RaisePropertyChangedEvent("Status"); } } // Статус аккаунта
        private DateTime _OpenedDate;
        public DateTime OpenedDate { get => _OpenedDate; set { RaisePropertyChangedEvent("OpenedDate"); } } // Дата открытия
        private DateTime _ClosedDate;
        public DateTime ClosedDate { get => _ClosedDate; set { RaisePropertyChangedEvent("ClosedDate"); } } // Дата закрытия

        // Обновление данных из объекта Tinkoff
        public void TnkUpdate(Account account)
        {
            Name = account.Name;
            Id = account.Id;
            Type = account.Type;
            Status = account.Status;
            OpenedDate = (account.OpenedDate == null)? DateTime.MinValue : account.OpenedDate.ToDateTime();
            ClosedDate = (account.ClosedDate == null) ? DateTime.MinValue : account.ClosedDate.ToDateTime();
        }
    }
}
