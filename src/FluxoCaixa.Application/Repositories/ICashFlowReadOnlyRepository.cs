namespace FluxoCaixa.Application.Repositories
{
    using FluxoCaixa.Domain.CashFlows;
    using System;
    using System.Threading.Tasks;

    public interface ICashFlowReadOnlyRepository
    {
        Task<CashFlow> Get(Guid id);        
    }
}
