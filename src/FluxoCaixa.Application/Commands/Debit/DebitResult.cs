namespace FluxoCaixa.Application.Commands.Debit
{
    using FluxoCaixa.Application.Results;
    using FluxoCaixa.Domain.CashFlows;
    using FluxoCaixa.Domain.ValueObjects;

    public sealed class DebitResult
    {
        public EntryResult Entry { get; }
        public double UpdatedBalance { get; }

        public DebitResult(Debit entry, Amount updatedBalance)
        {
            Entry = new EntryResult(
                entry.Description,
                entry.Amount,
                entry.EntryDate);

            UpdatedBalance = updatedBalance;
        }
    }
}
