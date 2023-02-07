namespace FluxoCaixa.Application.Commands.Credit
{
    using FluxoCaixa.Domain.ValueObjects;
    using System;
    using System.Threading.Tasks;

    public interface ICreditUseCase
    {
        Task<CreditResult> Execute(Guid cashFlowId, Amount amount);
    }
}
