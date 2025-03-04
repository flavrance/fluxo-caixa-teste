using System;

namespace FluxoCaixa.Application.Core.DTOs
{
    /// <summary>
    /// DTO para entradas de fluxo de caixa
    /// </summary>
    public class EntryDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}