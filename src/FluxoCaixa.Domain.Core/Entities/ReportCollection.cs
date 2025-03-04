using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FluxoCaixa.Domain.Core.Entities
{
    /// <summary>
    /// Coleção de relatórios de fluxo de caixa
    /// </summary>
    public sealed class ReportCollection
    {
        private readonly IList<Report> _reports;

        public ReportCollection()
        {
            _reports = new List<Report>();
        }

        public void Add(Report report)
        {
            _reports.Add(report);
        }

        public void Add(IEnumerable<Report> reports)
        {
            foreach (var report in reports)
            {
                Add(report);
            }
        }

        public IReadOnlyCollection<Report> GetReports()
        {
            IReadOnlyCollection<Report> readOnly = new ReadOnlyCollection<Report>(_reports);
            return readOnly;
        }

        public IReadOnlyCollection<IEntry> GetEntries()
        {
            var entries = _reports.SelectMany(r => r.Entries).ToList();
            return new ReadOnlyCollection<IEntry>(entries);
        }
    }
} 