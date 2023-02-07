namespace FluxoCaixa.Domain.CashFlows
{
    using FluxoCaixa.Domain.ValueObjects;
    using System;

    public sealed class Debit : IEntity, IEntry
    {
        public Guid Id { get; private set; }
        public Guid CashFlowId { get; private set; }
        public Amount Amount { get; private set; }
        public string Description
        {
            get { return "Debit"; }
        }
        public DateTime EntryDate { get; private set; }

        private Debit() { }

        public static Debit Load(Guid id, Guid cashFlowId, Amount amount, DateTime entryDate)
        {
            Debit debit = new Debit();
            debit.Id = id;
            debit.CashFlowId = cashFlowId;
            debit.Amount = amount;
            debit.EntryDate = entryDate;
            return debit;
        }

        public Debit(Guid cashFlowId, Amount amount)
        {
            Id = Guid.NewGuid();
            CashFlowId = cashFlowId;
            Amount = amount;
            EntryDate = DateTime.UtcNow;
        }
    }
}
