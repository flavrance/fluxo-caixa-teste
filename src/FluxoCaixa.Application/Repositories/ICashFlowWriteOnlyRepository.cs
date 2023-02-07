namespace FluxoCaixa.Application.Repositories
{
    using FluxoCaixa.Domain.CashFlows;
    using System.Threading.Tasks;

    public interface ICashFlowWriteOnlyRepository
    {
        Task Add(CashFlow cashFlow, Credit credit);
        Task Update(CashFlow cashFlow, Credit credit);
        Task Update(CashFlow cashFlow, Debit debit);
        Task Delete(CashFlow cashFlow);
    }
}
