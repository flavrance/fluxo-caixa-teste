namespace FluxoCaixa.Domain.Tests
{
    using Xunit;
    using FluxoCaixa.Domain.ValueObjects;
    using FluxoCaixa.Domain.CashFlows;
    using System;

    public class CashFlowTests
    {
        [Fact]
        public void New_CashFlow_Should_Have_100_Credit_After_Credit()
        {
            //
            // Arrange
            
            Amount amount = new Amount(100.0);
            CashFlow sut = new CashFlow(2023);

            //
            // Act
            sut.Credit(amount);

            //
            // Assert
            Credit credit = (Credit)sut.GetLastEntry();
            
            Assert.Equal(100, credit.Amount);
            Assert.Equal("Credit", credit.Description);
            Assert.True(credit.CashFlowId != Guid.Empty);
        }

        [Fact]
        public void New_CashFlow_With_1000_Balance_Should_Have_900_Credit_After_Debit()
        {
            //
            // Arrange
            CashFlow sut = new CashFlow(2023);
            sut.Credit(1000.0);

            //
            // Act
            sut.Debit(100);

            //
            // Assert
            Assert.Equal(900, sut.GetCurrentBalance());
        }
     

        [Fact]
        public void CashFlow_With_Three_Entries_Should_Be_Consistent()
        {
            //
            // Arrange
            CashFlow sut = new CashFlow(2023);
            sut.Credit(200);
            sut.Debit(100);
            sut.Credit(50);

            //
            // Act and Assert

            var entries = sut.GetEntries();

            Assert.Equal(3, entries.Count); 
        }

        [Fact]
        public void CashFlow_Should_Be_Loaded()
        {
            //
            // Arrange
            EntryCollection entries = new EntryCollection();
            entries.Add(new Debit(Guid.Empty, 100));

            //
            // Act
            CashFlow cashFlow = CashFlow.Load(
                Guid.Empty,                
                0,
                entries);

            //
            // Assert
            Assert.Single(cashFlow.GetEntries());
            Assert.Equal(Guid.Empty, cashFlow.Id);            
        }
    }
}
