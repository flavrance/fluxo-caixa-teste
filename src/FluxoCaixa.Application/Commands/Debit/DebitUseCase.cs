namespace FluxoCaixa.Application.Commands.Debit
{
    using System;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Repositories;
    using FluxoCaixa.Domain.CashFlows;
    using FluxoCaixa.Domain.ValueObjects;

    public sealed class DebitUseCase : IDebitUseCase
    {
        private readonly ICashFlowReadOnlyRepository cashFlowtReadOnlyRepository;
        private readonly ICashFlowWriteOnlyRepository cashFlowWriteOnlyRepository;

        public DebitUseCase(
            ICashFlowReadOnlyRepository cashFlowtReadOnlyRepository,
            ICashFlowWriteOnlyRepository cashFlowWriteOnlyRepository)
        {
            this.cashFlowtReadOnlyRepository = cashFlowtReadOnlyRepository;
            this.cashFlowWriteOnlyRepository = cashFlowWriteOnlyRepository;
        }

        public async Task<DebitResult> Execute(Guid cashFlowId, Amount amount)
        {
            CashFlow cashFlow = await cashFlowtReadOnlyRepository.Get(cashFlowId);
            if (cashFlow == null)
                throw new CashFlowNotFoundException($"The cashFlow {cashFlowId} does not exists.");

            cashFlow.Debit(amount);
            Debit debit = (Debit)cashFlow.GetLastEntry();

            await cashFlowWriteOnlyRepository.Update(cashFlow, debit);

            DebitResult result = new DebitResult(
                debit,
                cashFlow.GetCurrentBalance()
            );

            return result;
        }
    }
}
