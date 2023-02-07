namespace FluxoCaixa.Application.Commands.Credit
{
    using System;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Repositories;
    using FluxoCaixa.Domain.CashFlows;
    using FluxoCaixa.Domain.ValueObjects;

    public sealed class CreditUseCase : ICreditUseCase
    {
        private readonly ICashFlowReadOnlyRepository cashFlowReadOnlyRepository;
        private readonly ICashFlowWriteOnlyRepository cashFlowWriteOnlyRepository;

        public CreditUseCase(
            ICashFlowReadOnlyRepository cashFlowReadOnlyRepository,
            ICashFlowWriteOnlyRepository cashFlowWriteOnlyRepository)
        {
            this.cashFlowReadOnlyRepository = cashFlowReadOnlyRepository;
            this.cashFlowWriteOnlyRepository = cashFlowWriteOnlyRepository;
        }


        public async Task<CreditResult> Execute(Guid cashFlowId, Amount amount)
        {
            CashFlow cashFlow = await cashFlowReadOnlyRepository.Get(cashFlowId);
            if (cashFlow == null)
                throw new CashFlowNotFoundException($"The cashFlow {cashFlowId} does not exists.");

            cashFlow.Credit(amount);
            Credit credit = (Credit)cashFlow.GetLastEntry();

            await cashFlowWriteOnlyRepository.Update(
                cashFlow,
                credit);

            CreditResult result = new CreditResult(
                credit,
                cashFlow.GetCurrentBalance());
            return result;
        }
    }
}
