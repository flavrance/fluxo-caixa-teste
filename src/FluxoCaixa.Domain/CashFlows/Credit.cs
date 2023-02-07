namespace FluxoCaixa.Domain.CashFlows
{
    using FluxoCaixa.Domain.ValueObjects;
    using System;

    public sealed class Credit : IEntity, IEntry
    {
        public Guid Id { get; private set; }
        public Guid CashFlowId { get; private set; }
        public Amount Amount { get; private set; }
        public string Description
        {
            get { return "Credit"; }
        }
        public DateTime EntryDate { get; private set; }

        private Credit() { }

        public static Credit Load(Guid id, Guid cashFlowId, Amount amount, DateTime entryDate)
        {
            Credit credit = new Credit();
            credit.Id = id;
            credit.CashFlowId = cashFlowId;
            credit.Amount = amount;
            credit.EntryDate = entryDate;
            return credit;
        }

        public Credit(Guid cashFlowId, Amount amount)
        {
            Id = Guid.NewGuid();
            CashFlowId = cashFlowId;
            Amount = amount;
            EntryDate = DateTime.UtcNow;
        }
    }
}
