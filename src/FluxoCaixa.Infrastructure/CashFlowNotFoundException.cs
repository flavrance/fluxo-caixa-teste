namespace FluxoCaixa.Infrastructure
{
    public class CashFlowNotFoundException : InfrastructureException
    {
        internal CashFlowNotFoundException(string message)
            : base(message)
        { }
    }
}
