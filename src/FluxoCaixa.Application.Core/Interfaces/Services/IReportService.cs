using FluxoCaixa.Application.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluxoCaixa.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Interface para serviço de relatórios
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Obtém relatórios por data
        /// </summary>
        Task<IEnumerable<ReportDTO>> GetReportsByDateAsync(DateTime date);
        
        /// <summary>
        /// Gera relatório consolidado para uma data específica
        /// </summary>
        Task<ConsolidatedReportDto> GenerateConsolidatedReportAsync(DateTime date);
        
        /// <summary>
        /// Gera relatório para um período específico
        /// </summary>
        Task<ConsolidatedReportDto> GeneratePeriodReportAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Processa o consolidado diário de forma assíncrona
        /// </summary>
        Task ProcessDailyConsolidationAsync(DateTime date);
    }
} 