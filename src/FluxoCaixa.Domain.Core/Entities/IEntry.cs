using System;

namespace FluxoCaixa.Domain.Core.Entities
{
    /// <summary>
    /// Interface para entradas de fluxo de caixa (créditos e débitos)
    /// </summary>
    public interface IEntry
    {
        Guid Id { get; }
        Guid CashFlowId { get; }
        decimal Amount { get; }
        string Description { get; }
        DateTime Date { get; }
    }
} 