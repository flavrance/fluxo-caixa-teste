namespace FluxoCaixa.WebApi.UseCases.Register
{
    public sealed class RegisterRequest
    {
        public int Year { get; set; }
        public double InitialAmount { get; set; }
    }
}
