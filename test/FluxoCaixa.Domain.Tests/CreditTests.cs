namespace FluxoCaixa.Domain.Tests
{
    using Xunit;
    using FluxoCaixa.Domain.CashFlows;
    using System;

    public class CreditTests
    {
        [Fact]
        public void Credit_Should_Be_Loaded()
        {
            Credit credit = Credit.Load(
                Guid.Empty,
                Guid.Empty,
                100,
                DateTime.Today);

            Assert.Equal(Guid.Empty, credit.Id);
            Assert.Equal(Guid.Empty, credit.CashFlowId);
            Assert.Equal(100, credit.Amount);
            Assert.Equal(DateTime.Today, credit.EntryDate);
            Assert.Equal("Credit", credit.Description);
        }
    }
}
