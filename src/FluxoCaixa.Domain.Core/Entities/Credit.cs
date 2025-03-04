using System;

namespace FluxoCaixa.Domain.Core.Entities
{
    /// <summary>
    /// Entidade que representa um crédito no fluxo de caixa
    /// </summary>
    public class Credit : IEntry
    {
        public Guid Id { get; private set; }
        public Guid CashFlowId { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime Date { get; private set; }

        // Construtor para criar um novo crédito
        public Credit(Guid cashFlowId, decimal amount, string description, DateTime date)
        {
            Id = Guid.NewGuid();
            CashFlowId = cashFlowId;
            Amount = amount > 0 ? amount : throw new ArgumentException("Valor deve ser maior que zero", nameof(amount));
            Description = description ?? string.Empty;
            Date = date;
        }

        // Construtor privado para EF Core
        private Credit() { }
    }
} 