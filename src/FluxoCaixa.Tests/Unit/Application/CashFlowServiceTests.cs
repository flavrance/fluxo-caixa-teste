using Bogus;
using FluentAssertions;
using FluxoCaixa.Application.Core.DTOs;
using FluxoCaixa.Application.Core.Interfaces.Repositories;
using FluxoCaixa.Application.Core.Interfaces.Services;
using FluxoCaixa.Application.Core.Services;
using FluxoCaixa.Domain.Core.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FluxoCaixa.Tests.Unit.Application
{
    public class CashFlowServiceTests
    {
        private readonly Mock<ICashFlowReadOnlyRepository> _mockReadRepo;
        private readonly Mock<ICashFlowWriteOnlyRepository> _mockWriteRepo;
        private readonly Mock<ILogger<CashFlowService>> _mockLogger;
        private readonly ICashFlowService _service;
        private readonly Faker _faker;

        public CashFlowServiceTests()
        {
            _mockReadRepo = new Mock<ICashFlowReadOnlyRepository>();
            _mockWriteRepo = new Mock<ICashFlowWriteOnlyRepository>();
            _mockLogger = new Mock<ILogger<CashFlowService>>();
            _service = new CashFlowService(_mockReadRepo.Object, _mockWriteRepo.Object, _mockLogger.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCashFlows()
        {
            // Arrange
            var cashFlows = new List<CashFlow>
            {
                new CashFlow(_faker.Commerce.ProductName(), DateTime.Now.AddDays(-1)),
                new CashFlow(_faker.Commerce.ProductName(), DateTime.Now),
                new CashFlow(_faker.Commerce.ProductName(), DateTime.Now.AddDays(1))
            };

            _mockReadRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(cashFlows);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Select(cf => cf.Name).Should().BeEquivalentTo(cashFlows.Select(cf => cf.Name));
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnCashFlow()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = _faker.Commerce.ProductName();
            var cashFlow = new CashFlow(name, DateTime.Now) { Id = id };

            _mockReadRepo.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(cashFlow);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Name.Should().Be(name);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockReadRepo.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync((CashFlow)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateCashFlowAndReturnId()
        {
            // Arrange
            var dto = new CashFlowCreateDto
            {
                Name = _faker.Commerce.ProductName(),
                Date = DateTime.Now
            };

            _mockWriteRepo.Setup(repo => repo.AddAsync(It.IsAny<CashFlow>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeEmpty();
            _mockWriteRepo.Verify(repo => repo.AddAsync(It.Is<CashFlow>(cf => 
                cf.Name == dto.Name && 
                cf.Date.Date == dto.Date.Date)), 
                Times.Once);
        }

        [Fact]
        public async Task AddCreditAsync_WithValidData_ShouldAddCreditAndReturnTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var amount = _faker.Random.Decimal(1, 1000);
            var description = _faker.Lorem.Sentence();
            var cashFlow = new CashFlow(_faker.Commerce.ProductName(), DateTime.Now) { Id = id };

            _mockReadRepo.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(cashFlow);
            _mockWriteRepo.Setup(repo => repo.UpdateAsync(It.IsAny<CashFlow>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddCreditAsync(id, amount, description);

            // Assert
            result.Should().BeTrue();
            _mockWriteRepo.Verify(repo => repo.UpdateAsync(It.Is<CashFlow>(cf => 
                cf.Id == id && 
                cf.Entries.Any(e => e is Credit && e.Amount == amount && e.Description == description))), 
                Times.Once);
        }

        [Fact]
        public async Task AddDebitAsync_WithValidData_ShouldAddDebitAndReturnTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var initialCredit = 1000m;
            var debitAmount = _faker.Random.Decimal(1, 500);
            var description = _faker.Lorem.Sentence();
            
            var cashFlow = new CashFlow(_faker.Commerce.ProductName(), DateTime.Now) { Id = id };
            cashFlow.AddCredit(initialCredit, "Initial Credit");

            _mockReadRepo.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(cashFlow);
            _mockWriteRepo.Setup(repo => repo.UpdateAsync(It.IsAny<CashFlow>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddDebitAsync(id, debitAmount, description);

            // Assert
            result.Should().BeTrue();
            _mockWriteRepo.Verify(repo => repo.UpdateAsync(It.Is<CashFlow>(cf => 
                cf.Id == id && 
                cf.Entries.Any(e => e is Debit && e.Amount == debitAmount && e.Description == description))), 
                Times.Once);
        }
    }
} 