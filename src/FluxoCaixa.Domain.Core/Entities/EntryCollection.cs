using FluxoCaixa.Domain.Core.ValueObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FluxoCaixa.Domain.Core.Entities
{
    /// <summary>
    /// Coleção de entradas de fluxo de caixa
    /// </summary>
    public sealed class EntryCollection
    {
        private readonly IList<IEntry> _entries;

        public EntryCollection()
        {
            _entries = new List<IEntry>();
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

        public IReadOnlyCollection<IEntry> GetEntries()
        {
            IReadOnlyCollection<IEntry> readOnly = new ReadOnlyCollection<IEntry>(_entries);
            return readOnly;
        }

        public Amount GetCurrentBalance()
        {
            Amount totalAmount = 0;

            foreach (IEntry entry in _entries)
            {
                if (entry is Credit)
                {
                    totalAmount += entry.Amount;
                }

                if (entry is Debit)
                {
                    totalAmount -= entry.Amount;
                }
            }

            return totalAmount;
        }

        public IEntry GetLastEntry()
        {
            return _entries.LastOrDefault();
        }
    }
} 