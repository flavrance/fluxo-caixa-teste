namespace FluxoCaixa.Application.Commands.Register
{
    using System.Threading.Tasks;

    public interface IRegisterUseCase
    {
        Task<RegisterResult> Execute(int year, double initialAmount);
    }
}
