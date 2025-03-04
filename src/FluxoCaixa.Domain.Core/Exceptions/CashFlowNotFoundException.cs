using System;

namespace FluxoCaixa.Domain.Core.Exceptions
{
    /// <summary>
    /// Exceção lançada quando um fluxo de caixa não é encontrado
    /// </summary>
    public class CashFlowNotFoundException : Exception
    {
        public CashFlowNotFoundException(string message) : base(message)
        {
        }
    }
}