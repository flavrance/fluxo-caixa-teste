namespace FluxoCaixa.WebApi.UseCases.Credit
{
    using System;

    internal sealed class Model
    {
        public double Amount { get; }
        public string Description { get; }
        public DateTime EntryDate { get; }
        public double UpdateBalance { get; }

        public Model(double amount, string description, DateTime entryDate, double updatedBalance)
        {
            Amount = amount;
            Description = description;
            EntryDate = entryDate;
            UpdateBalance = updatedBalance;
        }
    }
}
