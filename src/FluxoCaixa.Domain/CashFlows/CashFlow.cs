namespace FluxoCaixa.Domain.CashFlows
{
    using FluxoCaixa.Domain.ValueObjects;
    using System;
    using System.Collections.Generic;
    using System.Transactions;

    public sealed class CashFlow : IEntity, IAggregateRoot
    {
        public Guid Id { get; private set; }
        public int Year { get; private set; }
        public IReadOnlyCollection<IEntry> GetEntries()
        {
            IReadOnlyCollection<IEntry> readOnly = _entries.GetEntries();
            return readOnly;
        }

        private EntryCollection _entries;
        private ReportCollection _report;
        
        public CashFlow(int year)
        {
            Id = Guid.NewGuid();
            Year = year;
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

        public IReadOnlyCollection<IEntry> GetEntriesByDate()
        {
            IReadOnlyCollection<IEntry> readOnly = _report.GetEntries();
            return readOnly;
        }

        public static CashFlow Load(Guid id, int year, EntryCollection entries)
        {
            CashFlow cashFlow = new CashFlow(year);
            cashFlow.Id = id;         
            cashFlow._entries = entries;
            return cashFlow;
        }

        public static CashFlow Load(Guid id, int year, EntryCollection entries, ReportCollection report)
        {
            CashFlow cashFlow = new CashFlow(year);
            cashFlow.Id = id;
            cashFlow._entries = entries;
            cashFlow._report= report;
            return cashFlow;
        }
    }
}
