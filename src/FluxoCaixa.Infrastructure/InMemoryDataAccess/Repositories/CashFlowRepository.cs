namespace FluxoCaixa.Infrastructure.InMemoryDataAccess.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Repositories;
    using FluxoCaixa.Domain.CashFlows;

    public class CashFlowRepository : ICashFlowReadOnlyRepository, ICashFlowWriteOnlyRepository
    {
        private readonly Context _context;

        public CashFlowRepository(Context context)
        {
            _context = context;
        }

        public async Task Add(CashFlow cashFlow, Credit credit)
        {
            _context.CashFlows.Add(cashFlow);
            await Task.CompletedTask;
        }

        public async Task Delete(CashFlow cashFlow)
        {
            CashFlow cashFlowOld = _context.CashFlows
                .Where(e => e.Id == cashFlow.Id)
                .SingleOrDefault();

            _context.CashFlows.Remove(cashFlowOld);

            await Task.CompletedTask;
        }

        public async Task<CashFlow> Get(Guid id)
        {
            CashFlow cashFlow = _context.CashFlows
                .Where(e => e.Id == id)
                .SingleOrDefault();

            return await Task.FromResult<CashFlow>(cashFlow);
        }

        public async Task Update(CashFlow cashFlow, Credit credit)
        {
            CashFlow cashFlowOld = _context.CashFlows
                .Where(e => e.Id == cashFlow.Id)
                .SingleOrDefault();

            cashFlowOld = cashFlow;
            await Task.CompletedTask;
        }

        public async Task Update(CashFlow cashFlow, Debit debit)
        {
            CashFlow cashFlowOld = _context.CashFlows
                .Where(e => e.Id == cashFlow.Id)
                .SingleOrDefault();

            cashFlowOld = cashFlow;
            await Task.CompletedTask;
        }
    }
}
