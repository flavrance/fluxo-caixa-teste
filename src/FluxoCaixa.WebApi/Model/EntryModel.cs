namespace FluxoCaixa.WebApi.Model
{
    using System;
    public sealed class EntryModel
    {
        public double Amount { get; }
        public string Description { get; }
        public DateTime EntryDate { get; }
        public EntryModel(double amount, string description, DateTime entryDate)
        {
            Amount = amount;
            Description = description;
            EntryDate = entryDate;
        }
    }
}
