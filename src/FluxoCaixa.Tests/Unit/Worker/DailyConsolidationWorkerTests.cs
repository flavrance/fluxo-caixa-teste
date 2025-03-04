using FluxoCaixa.Application.Core.Interfaces.Services;
using FluxoCaixa.Infrastructure.Data.Messaging;
using FluxoCaixa.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluxoCaixa.Tests.Unit.Worker
{
    public class DailyConsolidationWorkerTests
    {
        private readonly Mock<IReportService> _mockReportService;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ILogger<DailyConsolidationWorker>> _mockLogger;

        public DailyConsolidationWorkerTests()
        {
            _mockReportService = new Mock<IReportService>();
            _mockMessageService = new Mock<IMessageService>();
            _mockLogger = new Mock<ILogger<DailyConsolidationWorker>>();
        }

        [Fact]
        public async Task ProcessConsolidationMessage_ShouldCallProcessDailyConsolidationAsync()
        {
            // Arrange
            var worker = new DailyConsolidationWorker(
                _mockReportService.Object,
                _mockMessageService.Object,
                _mockLogger.Object);

            var message = new ConsolidationMessage
            {
                Date = DateTime.Today,
                RetryCount = 0
            };

            _mockReportService.Setup(s => s.ProcessDailyConsolidationAsync(It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            // Act - Use reflection to call the private method
            var method = typeof(DailyConsolidationWorker).GetMethod("ProcessConsolidationMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            await (Task)method.Invoke(worker, new object[] { message });

            // Assert
            _mockReportService.Verify(s => s.ProcessDailyConsolidationAsync(message.Date), Times.Once);
        }

        [Fact]
        public async Task ProcessConsolidationMessage_ShouldPublishForReprocessing_WhenExceptionOccurs()
        {
            // Arrange
            var worker = new DailyConsolidationWorker(
                _mockReportService.Object,
                _mockMessageService.Object,
                _mockLogger.Object);

            var message = new ConsolidationMessage
            {
                Date = DateTime.Today,
                RetryCount = 0
            };

            _mockReportService.Setup(s => s.ProcessDailyConsolidationAsync(It.IsAny<DateTime>()))
                .ThrowsAsync(new Exception("Test exception"));

            _mockMessageService.Setup(s => s.PublishMessage(It.IsAny<string>(), It.IsAny<ConsolidationMessage>()));

            // Act - Use reflection to call the private method
            var method = typeof(DailyConsolidationWorker).GetMethod("ProcessConsolidationMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            await (Task)method.Invoke(worker, new object[] { message });

            // Assert
            _mockReportService.Verify(s => s.ProcessDailyConsolidationAsync(message.Date), Times.Once);
            _mockMessageService.Verify(s => s.PublishMessage(It.IsAny<string>(), It.Is<ConsolidationMessage>(m => 
                m.Date == message.Date && m.RetryCount == message.RetryCount + 1)), Times.Once);
        }

        [Fact]
        public async Task ProcessConsolidationMessage_ShouldNotPublishForReprocessing_WhenMaxRetriesReached()
        {
            // Arrange
            var worker = new DailyConsolidationWorker(
                _mockReportService.Object,
                _mockMessageService.Object,
                _mockLogger.Object);

            var message = new ConsolidationMessage
            {
                Date = DateTime.Today,
                RetryCount = 3 // Max retries
            };

            _mockReportService.Setup(s => s.ProcessDailyConsolidationAsync(It.IsAny<DateTime>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act - Use reflection to call the private method
            var method = typeof(DailyConsolidationWorker).GetMethod("ProcessConsolidationMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            await (Task)method.Invoke(worker, new object[] { message });

            // Assert
            _mockReportService.Verify(s => s.ProcessDailyConsolidationAsync(message.Date), Times.Once);
            _mockMessageService.Verify(s => s.PublishMessage(It.IsAny<string>(), It.IsAny<ConsolidationMessage>()), Times.Never);
        }
    }
} 