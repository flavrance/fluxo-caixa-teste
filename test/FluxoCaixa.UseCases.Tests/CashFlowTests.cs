namespace FluxoCaixa.UseCases.Tests
{
    using Xunit;    
    using NSubstitute;
    using System;
    using FluxoCaixa.Application.Commands.Register;
    using FluxoCaixa.Application.Commands.Credit;
    using FluxoCaixa.Domain.CashFlows;
    using FluxoCaixa.Application.Repositories;

    public class CashFlowTests
    {
        public ICashFlowReadOnlyRepository cashFlowReadOnlyRepository;
        public ICashFlowWriteOnlyRepository cashFlowWriteOnlyRepository;        

        public CashFlowTests()
        {
            cashFlowReadOnlyRepository = Substitute.For<ICashFlowReadOnlyRepository>();
            cashFlowWriteOnlyRepository = Substitute.For<ICashFlowWriteOnlyRepository>();
            
        }       


        [Theory]
        [InlineData("c725315a-1de6-4bf7-aecf-3af8f0083681", 100)]
        public async void Credit_Valid_Amount(string cashFlowId, double amount)
        {
            var cashFlow = new CashFlow();
            

            cashFlowReadOnlyRepository
                .Get(Guid.Parse(cashFlowId))
                .Returns(cashFlow);

            var creditUseCase = new CreditUseCase(
                cashFlowReadOnlyRepository,
                cashFlowWriteOnlyRepository
            );

            CreditResult result = await creditUseCase.Execute(
                Guid.Parse(cashFlowId),
                amount);

            Assert.Equal(amount, result.Entry.Amount);
        }
        
    }
}
