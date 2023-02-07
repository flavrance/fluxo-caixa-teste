namespace FluxoCaixa.WebApi.Model
{
    using System;
    using System.Collections.Generic;

    public sealed class CashFlowDetailsModel
    {
        public Guid CashFlowId { get; }
        public double CurrentBalance { get; }
        public List<EntryModel> Entries { get; }

        public CashFlowDetailsModel(Guid cashFlowId, double currentBalance, List<EntryModel> entries)
        {
            CashFlowId = cashFlowId;
            CurrentBalance = currentBalance;
            Entries = entries;
        }
    }
}
