using System;

namespace FluxoCaixa.Domain.Core.Exceptions
{
    /// <summary>
    /// Exceção base para erros de domínio
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
} 