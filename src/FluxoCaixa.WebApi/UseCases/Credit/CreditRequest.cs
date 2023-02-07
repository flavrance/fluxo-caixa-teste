namespace FluxoCaixa.WebApi.UseCases.Credit
{
    using System;
    public sealed class CreditRequest
    {
        public Guid CashFlowId { get; set; }
        public Double Amount { get; set; }
    }
}
