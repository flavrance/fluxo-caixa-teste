namespace FluxoCaixa.Application
{
    using System;
    public class ApplicationException : Exception
    {
        public ApplicationException() : base() { }
        public ApplicationException(string businessMessage)
               : base(businessMessage)
        {
        }
    }
}
