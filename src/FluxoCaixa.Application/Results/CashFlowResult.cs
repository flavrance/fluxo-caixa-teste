namespace FluxoCaixa.Application.Results
{
    using FluxoCaixa.Domain.CashFlows;
    using System;
    using System.Collections.Generic;

    public sealed class CashFlowResult
    {
        public Guid CashFlowId { get; }
        public double CurrentBalance { get; }
        public List<EntryResult> Entries { get; }

        public CashFlowResult(
            Guid cashFlowId,
            double currentBalance,
            List<EntryResult> entries)
        {
            CashFlowId = cashFlowId;
            CurrentBalance = currentBalance;
            Entries = Entries;
        }

        public CashFlowResult(CashFlow cashFlow)
        {
            CashFlowId = cashFlow.Id;
            CurrentBalance = cashFlow.GetCurrentBalance();

            List<EntryResult> entryResults = new List<EntryResult>();
            foreach (IEntry entry in cashFlow.GetEntries())
            {
                EntryResult entryResult = new EntryResult(
                    entry.Description, entry.Amount, entry.EntryDate);
                entryResults.Add(entryResult);
            }

            Entries = entryResults;
        }
    }
}
