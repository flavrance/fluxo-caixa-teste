using FluxoCaixa.Domain.Core.Interfaces;
using FluxoCaixa.Domain.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace FluxoCaixa.Domain.Core.Entities
{
    /// <summary>
    /// Entidade que representa um relatório diário de fluxo de caixa
    /// </summary>
    public sealed class Report : IEntity
    {
        public Guid Id { get; private set; }
        public Guid CashFlowId { get; private set; }
        public DateTime Date { get; private set; }
        public Amount Balance { get; private set; }
        public IReadOnlyCollection<IEntry> Entries { get; private set; }

        private Report() { }

        public static Report Create(Guid cashFlowId, DateTime date, Amount balance, IReadOnlyCollection<IEntry> entries)
        {
            return new Report
            {
                Id = Guid.NewGuid(),
                CashFlowId = cashFlowId,
                Date = date,
                Balance = balance,
                Entries = entries
            };
        }
    }
} 