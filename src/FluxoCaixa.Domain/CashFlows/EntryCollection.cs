namespace FluxoCaixa.Domain.CashFlows
{
    using FluxoCaixa.Domain.ValueObjects;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class EntryCollection
    {
        private readonly IList<IEntry> _entries;

        public EntryCollection()
        {
            _entries = new List<IEntry>();
        }

        public IReadOnlyCollection<IEntry> GetEntries()
        {
            IReadOnlyCollection<IEntry> entries = new ReadOnlyCollection<IEntry>(_entries);
            return entries;
        }

        public IEntry GetLastEntry()
        {
            IEntry entry = _entries[_entries.Count - 1];
            return entry;
        }

        public void Add(IEntry entry)
        {
            _entries.Add(entry);
        }

        public void Add(IEnumerable<IEntry> entries)
        {
            foreach (var entry in entries)
            {
                Add(entry);
            }
        }

        public Amount GetCurrentBalance()
        {
            Amount totalAmount = 0;

            foreach (IEntry item in _entries)
            {
                if (item is Debit)
                    totalAmount = totalAmount - item.Amount;

                if (item is Credit)
                    totalAmount = totalAmount + item.Amount;
            }

            return totalAmount;
        }
    }
}
