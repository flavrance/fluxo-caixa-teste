using System;
using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Application.Core.DTOs
{
    /// <summary>
    /// DTO para operações de débito
    /// </summary>
    public class DebitDTO
    {
        public Guid Id { get; set; }
        public Guid CashFlowId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
    }

    /// <summary>
    /// DTO para débitos
    /// </summary>
    public class DebitDto
    {
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string Description { get; set; } = string.Empty;
    }
}