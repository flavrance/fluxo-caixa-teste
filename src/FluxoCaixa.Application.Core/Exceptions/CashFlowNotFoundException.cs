namespace FluxoCaixa.Application.Core.Exceptions
{
    /// <summary>
    /// Exceção lançada quando um fluxo de caixa não é encontrado
    /// </summary>
    public sealed class CashFlowNotFoundException : ApplicationException
    {
        public CashFlowNotFoundException(string message) : base(message)
        {
        }
    }
} 