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
    public class ReportServiceTests
    {
        private readonly Mock<ICashFlowReadOnlyRepository> _mockReadRepo;
        private readonly Mock<ICashFlowWriteOnlyRepository> _mockWriteRepo;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<ILogger<ReportService>> _mockLogger;
        private readonly IReportService _service;
        private readonly Faker _faker;

        public ReportServiceTests()
        {
            _mockReadRepo = new Mock<ICashFlowReadOnlyRepository>();
            _mockWriteRepo = new Mock<ICashFlowWriteOnlyRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _mockLogger = new Mock<ILogger<ReportService>>();
            _service = new ReportService(_mockReadRepo.Object, _mockWriteRepo.Object, _mockCacheService.Object, _mockLogger.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task GetReportsByDateAsync_ShouldReturnReportsFromCache_WhenCacheExists()
        {
            // Arrange
            var date = DateTime.Today;
            var cachedReports = new List<ReportDTO>
            {
                new ReportDTO
                {
                    Id = Guid.NewGuid(),
                    CashFlowId = Guid.NewGuid(),
                    Date = date,
                    Balance = 100,
                    Entries = new List<EntryDto>()
                }
            };

            _mockCacheService.Setup(cache => cache.GetAsync<IEnumerable<ReportDTO>>(It.IsAny<string>()))
                .ReturnsAsync(cachedReports);

            // Act
            var result = await _service.GetReportsByDateAsync(date);

            // Assert
            result.Should().BeEquivalentTo(cachedReports);
            _mockReadRepo.Verify(repo => repo.GetReportsByDateAsync(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task GetReportsByDateAsync_ShouldReturnReportsFromRepository_WhenCacheDoesNotExist()
        {
            // Arrange
            var date = DateTime.Today;
            var reports = new List<Report>
            {
                Report.Create(
                    Guid.NewGuid(),
                    date,
                    100,
                    new List<IEntry>())
            };

            _mockCacheService.Setup(cache => cache.GetAsync<IEnumerable<ReportDTO>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<ReportDTO>)null);

            _mockReadRepo.Setup(repo => repo.GetReportsByDateAsync(date))
                .ReturnsAsync(reports);

            // Act
            var result = await _service.GetReportsByDateAsync(date);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(reports.Count);
            _mockReadRepo.Verify(repo => repo.GetReportsByDateAsync(date), Times.Once);
            _mockCacheService.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<ReportDTO>>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task GenerateConsolidatedReportAsync_ShouldReturnReportFromCache_WhenCacheExists()
        {
            // Arrange
            var date = DateTime.Today;
            var cachedReport = new ConsolidatedReportDto
            {
                Date = date,
                TotalCredits = 100,
                TotalDebits = 50,
                FinalBalance = 50,
                Entries = new List<EntryDto>()
            };

            _mockCacheService.Setup(cache => cache.GetAsync<ConsolidatedReportDto>(It.IsAny<string>()))
                .ReturnsAsync(cachedReport);

            // Act
            var result = await _service.GenerateConsolidatedReportAsync(date);

            // Assert
            result.Should().BeEquivalentTo(cachedReport);
            _mockReadRepo.Verify(repo => repo.GetByDateAsync(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task GenerateConsolidatedReportAsync_ShouldGenerateReport_WhenCacheDoesNotExist()
        {
            // Arrange
            var date = DateTime.Today;
            var cashFlowId = Guid.NewGuid();
            
            var credit = new Credit(cashFlowId, 100, "Credit test", date);
            var debit = new Debit(cashFlowId, 50, "Debit test", date);
            
            var cashFlow = new CashFlow("Test", date);
            var entries = new List<IEntry> { credit, debit };
            
            // Use reflection to set the private field _entries
            var entriesField = cashFlow.GetType().GetField("_entries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            entriesField.SetValue(cashFlow, entries);
            
            var cashFlows = new List<CashFlow> { cashFlow };

            _mockCacheService.Setup(cache => cache.GetAsync<ConsolidatedReportDto>(It.IsAny<string>()))
                .ReturnsAsync((ConsolidatedReportDto)null);

            _mockReadRepo.Setup(repo => repo.GetByDateAsync(date))
                .ReturnsAsync(cashFlows);

            // Act
            var result = await _service.GenerateConsolidatedReportAsync(date);

            // Assert
            result.Should().NotBeNull();
            result.Date.Should().Be(date);
            result.TotalCredits.Should().Be(100);
            result.TotalDebits.Should().Be(50);
            result.FinalBalance.Should().Be(50);
            result.Entries.Should().HaveCount(2);
            
            _mockReadRepo.Verify(repo => repo.GetByDateAsync(date), Times.Once);
            _mockCacheService.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<ConsolidatedReportDto>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task ProcessDailyConsolidationAsync_ShouldProcessConsolidation()
        {
            // Arrange
            var date = DateTime.Today;
            var cashFlowId = Guid.NewGuid();
            
            var credit = new Credit(cashFlowId, 100, "Credit test", date);
            var debit = new Debit(cashFlowId, 50, "Debit test", date);
            
            var cashFlow = new CashFlow("Test", date);
            var entries = new List<IEntry> { credit, debit };
            
            // Use reflection to set the private field _entries
            var entriesField = cashFlow.GetType().GetField("_entries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            entriesField.SetValue(cashFlow, entries);
            
            var cashFlows = new List<CashFlow> { cashFlow };

            _mockReadRepo.Setup(repo => repo.GetByDateAsync(date))
                .ReturnsAsync(cashFlows);

            _mockWriteRepo.Setup(repo => repo.AddReportAsync(It.IsAny<Report>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.ProcessDailyConsolidationAsync(date);

            // Assert
            _mockReadRepo.Verify(repo => repo.GetByDateAsync(date), Times.AtLeastOnce);
            _mockWriteRepo.Verify(repo => repo.AddReportAsync(It.IsAny<Report>()), Times.Once);
            _mockCacheService.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<ConsolidatedReportDto>(), It.IsAny<TimeSpan>()), Times.AtLeastOnce);
        }
    }
} 