namespace FluxoCaixa.Application.Commands.Credit
{
    using FluxoCaixa.Application.Results;
    using FluxoCaixa.Domain.CashFlows;
    using FluxoCaixa.Domain.ValueObjects;

    public sealed class CreditResult
    {
        public EntryResult Entry { get; }
        public double UpdatedBalance { get; }

        public CreditResult(
            Credit credit,
            Amount updatedBalance)
        {
            Entry = new EntryResult(
                credit.Description,
                credit.Amount,
                credit.EntryDate);

            UpdatedBalance = updatedBalance;
        }
    }
}
