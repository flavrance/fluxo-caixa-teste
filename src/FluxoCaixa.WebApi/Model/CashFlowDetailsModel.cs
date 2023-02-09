namespace FluxoCaixa.WebApi.Model
{
    using System;
    using System.Collections.Generic;

    public sealed class CashFlowDetailsModel
    {
        public Guid CashFlowId { get; }

        public int Year { get; }
        public double CurrentBalance { get; }
        public List<EntryModel> Entries { get; }

        public CashFlowDetailsModel(Guid cashFlowId, int year, double currentBalance, List<EntryModel> entries)
        {
            CashFlowId = cashFlowId;
            Year = year;
            CurrentBalance = currentBalance;
            Entries = entries;
        }
    }
}
