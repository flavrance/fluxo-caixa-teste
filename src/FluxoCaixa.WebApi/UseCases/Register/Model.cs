namespace FluxoCaixa.WebApi.UseCases.Register
{
    using FluxoCaixa.WebApi.Model;
    using System;
    using System.Collections.Generic;

    internal sealed class Model
    {        
        public List<CashFlowDetailsModel> CashFlows { get; set; }

        public Model(List<CashFlowDetailsModel> cashFlows)
        {            
            CashFlows = cashFlows;
        }
    }
}
