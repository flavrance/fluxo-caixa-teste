namespace FluxoCaixa.Infrastructure.MongoDataAccess.Entities
{
    using System;

    public class Debit
    {
        public Guid Id { get; set; }
        public Guid CashFlowId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
