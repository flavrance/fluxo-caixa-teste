namespace FluxoCaixa.Application
{
    public sealed class CashFlowNotFoundException : ApplicationException
    {        
        public CashFlowNotFoundException() : base() { }
        public CashFlowNotFoundException(string message)
            : base(message)
        { }
    }
}
