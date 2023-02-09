using FluxoCaixa.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluxoCaixa.Domain.CashFlows
{
    public sealed class Report : IEntry
    {
        public Amount Amount { get; private set; }

        public string Description
        {
            get { return String.Empty; }
        }

        public DateTime EntryDate { get; private set; }

        public Report() { }

        public static Report Load(Amount amount, DateTime entryDate)
        {
            Report report = new Report();
            report.Amount = amount;
            report.EntryDate = entryDate;
            return report;
        }

        public Report(Amount amount)
        {            
            Amount = amount;
            EntryDate = DateTime.UtcNow;
        }
    }
}
