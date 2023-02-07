namespace FluxoCaixa.Application.Queries
{
    using FluxoCaixa.Application.Results;
    using System;
    using System.Threading.Tasks;

    public interface ICashFlowsQueries
    {
        Task<CashFlowResult> GetCashFlow(Guid cashFlowId);
    }
}
