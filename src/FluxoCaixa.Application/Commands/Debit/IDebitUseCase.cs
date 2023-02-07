namespace FluxoCaixa.Application.Commands.Debit
{
    using FluxoCaixa.Domain.ValueObjects;
    using System;
    using System.Threading.Tasks;

    public interface IDebitUseCase
    {
        Task<DebitResult> Execute(Guid cashFlowId, Amount amount);
    }
}
