namespace FluxoCaixa.Application.Commands.Register
{
    using System.Threading.Tasks;    
    using FluxoCaixa.Application.Repositories;
    using FluxoCaixa.Domain.CashFlows;

    public sealed class RegisterUseCase : IRegisterUseCase    {
        
        private readonly ICashFlowWriteOnlyRepository cashFlowWriteOnlyRepository;

        public RegisterUseCase(            
            ICashFlowWriteOnlyRepository cashFlowWriteOnlyRepository)        {
            
            this.cashFlowWriteOnlyRepository = cashFlowWriteOnlyRepository;
        }

        public async Task<RegisterResult> Execute(int year, double initialAmount){
            

            CashFlow cashFlow = new CashFlow(year);
            cashFlow.Credit(initialAmount);
            Credit credit = (Credit)cashFlow.GetLastEntry();            

            
            await cashFlowWriteOnlyRepository.Add(cashFlow, credit);

            RegisterResult result = new RegisterResult(cashFlow);
            return result;
        }
    }
}
