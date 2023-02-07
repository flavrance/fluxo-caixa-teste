namespace FluxoCaixa.Application.Commands.Register
{
    using FluxoCaixa.Application.Results;
    using FluxoCaixa.Domain.CashFlows;    
    using System.Collections.Generic;

    public sealed class RegisterResult
    {
        
        public CashFlowResult CashFlow { get; }

        public RegisterResult(CashFlow cashFlow)
        {
            List<EntryResult> entryResults = new List<EntryResult>();

            foreach (IEntry entry in cashFlow.GetEntries())
            {
                entryResults.Add(
                    new EntryResult(
                        entry.Description,
                        entry.Amount,
                        entry.EntryDate));
            }

            CashFlow = new CashFlowResult(cashFlow.Id, cashFlow.GetCurrentBalance(), entryResults);

            List<CashFlowResult> cashFlowResults = new List<CashFlowResult>();
            cashFlowResults.Add(CashFlow);            
        }
    }
}
