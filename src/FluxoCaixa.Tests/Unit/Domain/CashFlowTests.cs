using FluxoCaixa.Domain.Core.Entities;
using FluentAssertions;
using System;

namespace FluxoCaixa.Tests.Unit.Domain
{
    public class CashFlowTests
    {
        [Fact]
        public void CashFlow_WhenCreated_ShouldHaveEmptyEntries()
        {
            // Arrange & Act
            var cashFlow = new CashFlow("Test Cash Flow", DateTime.Now);

            // Assert
            cashFlow.Should().NotBeNull();
            cashFlow.Entries.Should().NotBeNull();
            cashFlow.Entries.Should().BeEmpty();
        }

        [Fact]
        public void CashFlow_WhenAddCredit_ShouldIncreaseBalance()
        {
            // Arrange
            var cashFlow = new CashFlow("Test Cash Flow", DateTime.Now);
            var creditAmount = 100.0m;
            var description = "Test Credit";

            // Act
            cashFlow.AddCredit(creditAmount, description);

            // Assert
            cashFlow.Balance.Should().Be(creditAmount);
            cashFlow.Entries.Should().HaveCount(1);
            cashFlow.Entries[0].Should().BeOfType<Credit>();
            cashFlow.Entries[0].Amount.Should().Be(creditAmount);
            cashFlow.Entries[0].Description.Should().Be(description);
        }

        [Fact]
        public void CashFlow_WhenAddDebit_ShouldDecreaseBalance()
        {
            // Arrange
            var cashFlow = new CashFlow("Test Cash Flow", DateTime.Now);
            var initialCredit = 200.0m;
            var debitAmount = 50.0m;
            var description = "Test Debit";

            // Act
            cashFlow.AddCredit(initialCredit, "Initial Credit");
            cashFlow.AddDebit(debitAmount, description);

            // Assert
            cashFlow.Balance.Should().Be(initialCredit - debitAmount);
            cashFlow.Entries.Should().HaveCount(2);
            cashFlow.Entries[1].Should().BeOfType<Debit>();
            cashFlow.Entries[1].Amount.Should().Be(debitAmount);
            cashFlow.Entries[1].Description.Should().Be(description);
        }

        [Fact]
        public void CashFlow_WhenAddDebitWithInsufficientFunds_ShouldThrowException()
        {
            // Arrange
            var cashFlow = new CashFlow("Test Cash Flow", DateTime.Now);
            var initialCredit = 50.0m;
            var debitAmount = 100.0m;

            // Act
            cashFlow.AddCredit(initialCredit, "Initial Credit");
            
            // Assert
            Action act = () => cashFlow.AddDebit(debitAmount, "Test Debit");
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*insufficient funds*");
        }
    }
}