using FluxoCaixa.API;
using FluxoCaixa.Application.Core.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FluxoCaixa.Tests.Integration.API
{
    public class CashFlowControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CashFlowControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/cashflow");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreatedStatusCode()
        {
            // Arrange
            var cashFlowDto = new CashFlowCreateDto
            {
                Name = "Test Cash Flow",
                Date = DateTime.Now
            };

            var content = new StringContent(
                JsonSerializer.Serialize(cashFlowDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/cashflow", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var returnedCashFlow = await response.Content.ReadFromJsonAsync<CashFlowDto>();
            Assert.NotNull(returnedCashFlow);
            Assert.Equal(cashFlowDto.Name, returnedCashFlow.Name);
        }

        [Fact]
        public async Task AddCredit_WithValidData_ShouldReturnOkStatusCode()
        {
            // Arrange
            var cashFlowId = Guid.NewGuid(); // Assume this ID exists
            var creditDto = new CreditDto
            {
                Amount = 100.0m,
                Description = "Test Credit"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(creditDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync($"/api/cashflow/{cashFlowId}/credits", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddDebit_WithValidData_ShouldReturnOkStatusCode()
        {
            // Arrange
            var cashFlowId = Guid.NewGuid(); // Assume this ID exists and has sufficient funds
            var debitDto = new DebitDto
            {
                Amount = 50.0m,
                Description = "Test Debit"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(debitDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync($"/api/cashflow/{cashFlowId}/debits", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}