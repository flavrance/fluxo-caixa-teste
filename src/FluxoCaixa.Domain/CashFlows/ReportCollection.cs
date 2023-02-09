using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FluxoCaixa.Domain.CashFlows
{
    public sealed class ReportCollection
    {
        private readonly IList<IEntry> _entries;

        public ReportCollection()
        {
            _entries = new List<IEntry>();
        }

        public IReadOnlyCollection<IEntry> GetEntries()
        {
            IReadOnlyCollection<IEntry> entries = new ReadOnlyCollection<IEntry>(_entries);
            return entries;
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
    }
}
