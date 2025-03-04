using System;

namespace FluxoCaixa.Application.Core.Exceptions
{
    /// <summary>
    /// Exceção base para erros de aplicação
    /// </summary>
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message)
        {
        }
    }
} 