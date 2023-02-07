namespace FluxoCaixa.Domain.CashFlows
{
    using FluxoCaixa.Domain.ValueObjects;
    using System;

    public interface IEntry
    {
        Amount Amount { get; }
        string Description { get; }
        DateTime EntryDate { get; }
    }
}
