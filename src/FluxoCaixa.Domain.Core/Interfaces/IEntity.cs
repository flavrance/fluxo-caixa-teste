using System;

namespace FluxoCaixa.Domain.Core.Interfaces
{
    /// <summary>
    /// Interface base para todas as entidades
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; }
    }
} 