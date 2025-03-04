using System;
using System.Collections.Generic;

namespace FluxoCaixa.Application.Core.DTOs
{
    /// <summary>
    /// DTO para fluxo de caixa
    /// </summary>
    public class CashFlowDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Balance { get; set; }
        public List<EntryDto> Entries { get; set; } = new List<EntryDto>();
    }

    public class CashFlowCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class CashFlowUpdateDto
    {
        public string Name { get; set; } = string.Empty;
    }
} 