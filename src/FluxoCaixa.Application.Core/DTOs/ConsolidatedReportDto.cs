using System;
using System.Collections.Generic;

namespace FluxoCaixa.Application.Core.DTOs
{
    /// <summary>
    /// DTO para relatório consolidado de fluxo de caixa
    /// </summary>
    public class ConsolidatedReportDto
    {
        /// <summary>
        /// Data do relatório
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Total de créditos
        /// </summary>
        public decimal TotalCredits { get; set; }

        /// <summary>
        /// Total de débitos
        /// </summary>
        public decimal TotalDebits { get; set; }

        /// <summary>
        /// Saldo final
        /// </summary>
        public decimal FinalBalance { get; set; }

        /// <summary>
        /// Lista de entradas (créditos e débitos)
        /// </summary>
        public List<EntryDto> Entries { get; set; } = new List<EntryDto>();
    }
} 