namespace FluxoCaixa.Domain.CashFlows
{
    using FluxoCaixa.Domain.ValueObjects;
    using System;
    using System.Collections.Generic;
    using System.Transactions;

    public sealed class CashFlow : IEntity, IAggregateRoot
    {
        public Guid Id { get; private set; }
        
        public IReadOnlyCollection<IEntry> GetEntries()
        {
            IReadOnlyCollection<IEntry> readOnly = _entries.GetEntries();
            return readOnly;
        }

        private EntryCollection _entries;

        public CashFlow()
        {
            Id = Guid.NewGuid();
            _entries = new EntryCollection();            
        }
        public void Credit(Amount amount)
        {
            Credit credit = new Credit(Id, amount);
            _entries.Add(credit);
        }

        public void Debit(Amount amount){           

            Debit debit = new Debit(Id, amount);
            _entries.Add(debit);
        }

        public Amount GetCurrentBalance()
        {
            Amount totalAmount = _entries.GetCurrentBalance();
            return totalAmount;
        }

        public IEntry GetLastEntry()
        {
            IEntry entry = _entries.GetLastEntry();
            return entry;
        }        

        public static CashFlow Load(Guid id, EntryCollection entries)
        {
            CashFlow account = new CashFlow();
            account.Id = id;            
            account._entries = entries;
            return account;
        }
    }
}
