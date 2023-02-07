namespace FluxoCaixa.Infrastructure.InMemoryDataAccess
{
    using FluxoCaixa.Domain.CashFlows;    
    using System.Collections.ObjectModel;

    public class Context
    {        
        public Collection<CashFlow> CashFlows { get; set; }

        public Context()
        {
            CashFlows = new Collection<CashFlow>();
        }
    }
}
