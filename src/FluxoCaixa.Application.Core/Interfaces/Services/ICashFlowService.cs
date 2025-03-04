using FluxoCaixa.Application.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluxoCaixa.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Interface para o serviço de fluxo de caixa
    /// </summary>
    public interface ICashFlowService
    {
        Task<IEnumerable<CashFlowDto>> GetAllAsync();
        Task<CashFlowDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CashFlowCreateDto dto);
        Task<bool> UpdateAsync(Guid id, CashFlowUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> AddCreditAsync(Guid id, decimal amount, string description);
        Task<bool> AddDebitAsync(Guid id, decimal amount, string description);
    }
} 