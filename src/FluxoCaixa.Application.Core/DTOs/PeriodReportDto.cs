using System;
using System.Collections.Generic;

namespace FluxoCaixa.Application.Core.DTOs
{
    /// <summary>
    /// DTO para relatório de fluxo de caixa por período
    /// </summary>
    public class PeriodReportDto
    {
        /// <summary>
        /// Data inicial do período
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Data final do período
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Total de créditos no período
        /// </summary>
        public decimal TotalCredits { get; set; }

        /// <summary>
        /// Total de débitos no período
        /// </summary>
        public decimal TotalDebits { get; set; }

        /// <summary>
        /// Saldo inicial (antes do período)
        /// </summary>
        public decimal InitialBalance { get; set; }

        /// <summary>
        /// Saldo final (após o período)
        /// </summary>
        public decimal FinalBalance { get; set; }

        /// <summary>
        /// Relatórios diários dentro do período
        /// </summary>
        public List<DailyReportDto> DailyReports { get; set; } = new List<DailyReportDto>();
    }

    /// <summary>
    /// DTO para relatório diário dentro de um período
    /// </summary>
    public class DailyReportDto
    {
        /// <summary>
        /// Data do relatório diário
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Total de créditos no dia
        /// </summary>
        public decimal DailyCredits { get; set; }

        /// <summary>
        /// Total de débitos no dia
        /// </summary>
        public decimal DailyDebits { get; set; }

        /// <summary>
        /// Saldo do dia
        /// </summary>
        public decimal DailyBalance { get; set; }
    }
} 