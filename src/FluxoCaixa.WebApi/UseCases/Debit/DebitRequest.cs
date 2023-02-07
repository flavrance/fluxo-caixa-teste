namespace FluxoCaixa.WebApi.UseCases.Debit
{
    using System;
    public sealed class DebitRequest
    {
        public Guid CashFlowId { get; set; }
        public Double Amount { get; set; }
    }
}
