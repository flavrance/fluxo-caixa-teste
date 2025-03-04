using System;
using System.Collections.Generic;

namespace FluxoCaixa.Application.Core.DTOs
{
    /// <summary>
    /// DTO para relatórios de fluxo de caixa
    /// </summary>
    public class ReportDTO
    {
        public Guid Id { get; set; }
        public Guid CashFlowId { get; set; }
        public DateTime Date { get; set; }
        public decimal Balance { get; set; }
        public List<EntryDto> Entries { get; set; } = new List<EntryDto>();
    }
} 