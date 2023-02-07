namespace FluxoCaixa.Domain.Tests
{
    using Xunit;
    using FluxoCaixa.Domain.CashFlows;
    using System;

    public class DebitTests
    {
        [Fact]
        public void Debit_Should_Be_Loaded()
        {
            Debit debit = Debit.Load(
                Guid.Empty,
                Guid.Empty,
                100,
                DateTime.Today);

            Assert.Equal(Guid.Empty, debit.Id);
            Assert.Equal(Guid.Empty, debit.CashFlowId);
            Assert.Equal(100, debit.Amount);
            Assert.Equal(DateTime.Today, debit.EntryDate);
            Assert.Equal("Debit", debit.Description);
        }
    }
}
