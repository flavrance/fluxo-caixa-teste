namespace FluxoCaixa.Application.Results
{
    using System;

    public sealed class EntryResult
    {
        public string Description { get; }
        public double Amount { get; }
        public DateTime EntryDate { get; }

        public EntryResult(
            string description,
            double amount,
            DateTime entryDate)
        {
            Description = description;
            Amount = amount;
            EntryDate = entryDate;
        }
    }
}
