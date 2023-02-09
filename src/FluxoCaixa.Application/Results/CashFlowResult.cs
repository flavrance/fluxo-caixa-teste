namespace FluxoCaixa.Application.Results
{
    using FluxoCaixa.Domain.CashFlows;
    using System;
    using System.Collections.Generic;

    public sealed class CashFlowResult
    {
        public Guid CashFlowId { get; }
        public int Year { get; }
        public double CurrentBalance { get; }
        public List<EntryResult> Entries { get; }

        public List<EntryResult> Report { get; }

        public CashFlowResult(
            Guid cashFlowId,
            int year,
            double currentBalance,
            List<EntryResult> entries,
            List<EntryResult> report)
        {
            CashFlowId = cashFlowId;
            Year = year;
            CurrentBalance = currentBalance;
            Entries = Entries;
            Report = report;
        }

        public CashFlowResult(CashFlow cashFlow)
        {
            CashFlowId = cashFlow.Id;
            Year = cashFlow.Year;
            CurrentBalance = cashFlow.GetCurrentBalance();

            List<EntryResult> entryResults = new List<EntryResult>();
            foreach (IEntry entry in cashFlow.GetEntries())
            {
                EntryResult entryResult = new EntryResult(
                    entry.Description, entry.Amount, entry.EntryDate);
                entryResults.Add(entryResult);
            }

            Entries = entryResults;

            List<EntryResult> reportResults = new List<EntryResult>();
            foreach (IEntry entry in cashFlow.GetEntriesByDate())
            {
                EntryResult entryResult = new EntryResult(
                    entry.Description, entry.Amount, entry.EntryDate);
                reportResults.Add(entryResult);
            }

            Report = reportResults;
        }
    }
}
