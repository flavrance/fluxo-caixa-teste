using FluxoCaixa.Application.Core.DTOs;
using FluxoCaixa.Application.Core.Interfaces.Services;
using FluxoCaixa.Domain.Core.Entities;
using FluxoCaixa.Domain.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxoCaixa.Application.Core.Services
{
    /// <summary>
    /// Implementação do serviço de fluxo de caixa
    /// </summary>
    public class CashFlowService : ICashFlowService
    {
        private readonly ICashFlowReadOnlyRepository _readRepository;
        private readonly ICashFlowWriteOnlyRepository _writeRepository;
        private readonly ILogger<CashFlowService> _logger;

        public CashFlowService(
            ICashFlowReadOnlyRepository readRepository,
            ICashFlowWriteOnlyRepository writeRepository,
            ILogger<CashFlowService> logger)
        {
            _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
            _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
            _logger = logger;
        }

        public async Task<IEnumerable<CashFlowDto>> GetAllAsync()
        {
            try
            {
                var cashFlows = await _readRepository.GetAllAsync();
                return cashFlows.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cash flows");
                return Enumerable.Empty<CashFlowDto>();
            }
        }

        public async Task<CashFlowDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var cashFlow = await _readRepository.GetByIdAsync(id);
                return cashFlow != null ? MapToDto(cashFlow) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash flow with ID {Id}", id);
                return null;
            }
        }

        public async Task<Guid> CreateAsync(CashFlowCreateDto dto)
        {
            try
            {
                var cashFlow = new CashFlow(dto.Name, dto.Date);
                await _writeRepository.AddAsync(cashFlow);
                return cashFlow.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cash flow");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, CashFlowUpdateDto dto)
        {
            try
            {
                var cashFlow = await _readRepository.GetByIdAsync(id);
                if (cashFlow == null)
                {
                    _logger.LogWarning("Cash flow with ID {Id} not found", id);
                    return false;
                }

                cashFlow.UpdateName(dto.Name);
                return await _writeRepository.UpdateAsync(cashFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cash flow with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                return await _writeRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cash flow with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> AddCreditAsync(Guid id, decimal amount, string description)
        {
            try
            {
                var cashFlow = await _readRepository.GetByIdAsync(id);
                if (cashFlow == null)
                {
                    _logger.LogWarning("Cash flow with ID {Id} not found", id);
                    return false;
                }

                cashFlow.AddCredit(amount, description);
                return await _writeRepository.UpdateAsync(cashFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding credit to cash flow with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> AddDebitAsync(Guid id, decimal amount, string description)
        {
            try
            {
                var cashFlow = await _readRepository.GetByIdAsync(id);
                if (cashFlow == null)
                {
                    _logger.LogWarning("Cash flow with ID {Id} not found", id);
                    return false;
                }

                cashFlow.AddDebit(amount, description);
                return await _writeRepository.UpdateAsync(cashFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding debit to cash flow with ID {Id}", id);
                return false;
            }
        }

        private CashFlowDto MapToDto(CashFlow cashFlow)
        {
            return new CashFlowDto
            {
                Id = cashFlow.Id,
                Name = cashFlow.Name,
                Date = cashFlow.Date,
                Balance = cashFlow.Balance,
                Entries = cashFlow.Entries.Select(MapToEntryDto).ToList()
            };
        }

        private EntryDto MapToEntryDto(IEntry entry)
        {
            return new EntryDto
            {
                Id = entry.Id,
                Amount = entry.Amount,
                Description = entry.Description,
                Date = entry.Date,
                Type = entry is Credit ? "Credit" : "Debit"
            };
        }
    }
} 