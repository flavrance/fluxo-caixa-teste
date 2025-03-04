using FluxoCaixa.Application.Core.DTOs;
using FluxoCaixa.Domain.Core.Interfaces.Repositories;
using FluxoCaixa.Application.Core.Interfaces.Services;
using FluxoCaixa.Domain.Core.Entities;
using FluxoCaixa.Domain.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxoCaixa.Application.Core.Services
{
    /// <summary>
    /// Implementação do serviço de relatórios
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly ICashFlowReadOnlyRepository _readRepository;
        private readonly ICashFlowWriteOnlyRepository _writeRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            ICashFlowReadOnlyRepository readRepository,
            ICashFlowWriteOnlyRepository writeRepository,
            ICacheService cacheService,
            ILogger<ReportService> logger)
        {
            _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
            _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ReportDTO>> GetReportsByDateAsync(DateTime date)
        {
            try
            {
                // Tentar obter do cache primeiro
                var cacheKey = $"reports_{date:yyyyMMdd}";
                var cachedReports = await _cacheService.GetAsync<IEnumerable<ReportDTO>>(cacheKey);
                
                if (cachedReports != null)
                {
                    _logger.LogInformation("Relatórios obtidos do cache para a data {Date}", date);
                    return cachedReports;
                }
                
                // Se não estiver em cache, buscar do repositório
                var reports = await _readRepository.GetReportsByDateAsync(date);
                var reportDtos = reports.Select(MapToReportDto).ToList();
                
                // Armazenar em cache por 24 horas
                await _cacheService.SetAsync(cacheKey, reportDtos, TimeSpan.FromHours(24));
                
                return reportDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatórios para a data {Date}", date);
                return Enumerable.Empty<ReportDTO>();
            }
        }

        public async Task<ConsolidatedReportDto> GenerateConsolidatedReportAsync(DateTime date)
        {
            try
            {
                // Tentar obter do cache primeiro
                var cacheKey = $"consolidated_report_{date:yyyyMMdd}";
                var cachedReport = await _cacheService.GetAsync<ConsolidatedReportDto>(cacheKey);
                
                if (cachedReport != null)
                {
                    _logger.LogInformation("Relatório consolidado obtido do cache para a data {Date}", date);
                    return cachedReport;
                }
                
                // Se não estiver em cache, gerar o relatório
                var cashFlows = await _readRepository.GetByDateAsync(date);
                
                var consolidatedReport = new ConsolidatedReportDto
                {
                    Date = date,
                    TotalCredits = 0,
                    TotalDebits = 0,
                    FinalBalance = 0,
                    Entries = new List<EntryDto>()
                };
                
                foreach (var cashFlow in cashFlows)
                {
                    var entries = cashFlow.Entries.ToList();
                    var credits = entries.Where(e => e is Credit).Cast<Credit>();
                    var debits = entries.Where(e => e is Debit).Cast<Debit>();
                    
                    consolidatedReport.TotalCredits += credits.Sum(c => c.Amount);
                    consolidatedReport.TotalDebits += debits.Sum(d => d.Amount);
                    
                    foreach (var entry in entries)
                    {
                        consolidatedReport.Entries.Add(new EntryDto
                        {
                            Id = entry.Id,
                            Amount = entry.Amount,
                            Description = entry.Description,
                            Date = entry.Date,
                            Type = entry is Credit ? "Credit" : "Debit"
                        });
                    }
                }
                
                consolidatedReport.FinalBalance = consolidatedReport.TotalCredits - consolidatedReport.TotalDebits;
                
                // Armazenar em cache por 24 horas
                await _cacheService.SetAsync(cacheKey, consolidatedReport, TimeSpan.FromHours(24));
                
                return consolidatedReport;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório consolidado para a data {Date}", date);
                return new ConsolidatedReportDto
                {
                    Date = date,
                    TotalCredits = 0,
                    TotalDebits = 0,
                    FinalBalance = 0,
                    Entries = new List<EntryDto>()
                };
            }
        }

        public async Task<ConsolidatedReportDto> GeneratePeriodReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    throw new ArgumentException("A data inicial deve ser menor ou igual à data final");
                }
                
                // Tentar obter do cache primeiro
                var cacheKey = $"period_report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
                var cachedReport = await _cacheService.GetAsync<ConsolidatedReportDto>(cacheKey);
                
                if (cachedReport != null)
                {
                    _logger.LogInformation("Relatório de período obtido do cache para o período de {StartDate} a {EndDate}", startDate, endDate);
                    return cachedReport;
                }
                
                // Se não estiver em cache, gerar o relatório
                var consolidatedReport = new ConsolidatedReportDto
                {
                    Date = startDate,
                    TotalCredits = 0,
                    TotalDebits = 0,
                    FinalBalance = 0,
                    Entries = new List<EntryDto>()
                };
                
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dailyReport = await GenerateConsolidatedReportAsync(date);
                    
                    consolidatedReport.TotalCredits += dailyReport.TotalCredits;
                    consolidatedReport.TotalDebits += dailyReport.TotalDebits;
                    consolidatedReport.Entries.AddRange(dailyReport.Entries);
                }
                
                consolidatedReport.FinalBalance = consolidatedReport.TotalCredits - consolidatedReport.TotalDebits;
                
                // Armazenar em cache por 24 horas
                await _cacheService.SetAsync(cacheKey, consolidatedReport, TimeSpan.FromHours(24));
                
                return consolidatedReport;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de período de {StartDate} a {EndDate}", startDate, endDate);
                return new ConsolidatedReportDto
                {
                    Date = startDate,
                    TotalCredits = 0,
                    TotalDebits = 0,
                    FinalBalance = 0,
                    Entries = new List<EntryDto>()
                };
            }
        }

        public async Task ProcessDailyConsolidationAsync(DateTime date)
        {
            try
            {
                _logger.LogInformation("Iniciando processamento do consolidado diário para a data {Date}", date);
                
                // Gerar o relatório consolidado
                var consolidatedReport = await GenerateConsolidatedReportAsync(date);
                
                // Criar um relatório para cada fluxo de caixa da data
                var cashFlows = await _readRepository.GetByDateAsync(date);
                
                foreach (var cashFlow in cashFlows)
                {
                    var entries = cashFlow.Entries.Where(e => e.Date.Date == date.Date).ToList();
                    
                    if (entries.Any())
                    {
                        var report = Report.Create(
                            cashFlow.Id,
                            date,
                            cashFlow.Balance,
                            entries);
                            
                        await _writeRepository.AddReportAsync(report);
                        
                        _logger.LogInformation("Relatório criado para o fluxo de caixa {CashFlowId} na data {Date}", cashFlow.Id, date);
                    }
                }
                
                // Armazenar o consolidado em cache
                var cacheKey = $"daily_consolidation_{date:yyyyMMdd}";
                await _cacheService.SetAsync(cacheKey, consolidatedReport, TimeSpan.FromDays(30));
                
                _logger.LogInformation("Processamento do consolidado diário concluído para a data {Date}", date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar o consolidado diário para a data {Date}", date);
                throw;
            }
        }

        private ReportDTO MapToReportDto(Report report)
        {
            return new ReportDTO
            {
                Id = report.Id,
                CashFlowId = report.CashFlowId,
                Date = report.Date,
                Balance = report.Balance,
                Entries = report.Entries.Select(e => new EntryDto
                {
                    Id = e.Id,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.Date,
                    Type = e is Credit ? "Credit" : "Debit"
                }).ToList()
            };
        }
    }
} 