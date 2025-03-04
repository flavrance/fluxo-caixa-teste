using FluxoCaixa.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluxoCaixa.Domain.Core.Entities
{
    /// <summary>
    /// Entidade principal que representa um fluxo de caixa
    /// </summary>
    public sealed class CashFlow : IEntity, IAggregateRoot
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime Date { get; private set; }
        public decimal Balance { get; private set; }
        
        private readonly List<IEntry> _entries = new List<IEntry>();
        public IReadOnlyCollection<IEntry> Entries => _entries.AsReadOnly();
        
        // Propriedades de navegação para o EF Core
        public ICollection<Credit> Credits { get; private set; } = new List<Credit>();
        public ICollection<Debit> Debits { get; private set; } = new List<Debit>();
        
        // Construtor para criar um novo fluxo de caixa
        public CashFlow(string name, DateTime date)
        {
            Id = Guid.NewGuid();
            Name = name;
            Date = date;
            Balance = 0;
        }
        
        // Construtor privado para EF Core
        private CashFlow() { }
        
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio", nameof(name));
                
            Name = name;
        }
        
        public void AddCredit(decimal amount, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Valor deve ser maior que zero", nameof(amount));
                
            var credit = new Credit(Id, amount, description, DateTime.Now);
            _entries.Add(credit);
            Credits.Add(credit);
            Balance += amount;
        }
        

        public void AddEntry(IEntry entry)
        {
            _entries.Add(entry);            
        }

        public void AddEntries(IEnumerable<IEntry> entries)
        {
            foreach (var entry in entries)
            {
                _entries.Add(entry);
            }
        }


        public void AddDebit(decimal amount, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Valor deve ser maior que zero", nameof(amount));
                
            var debit = new Debit(Id, amount, description, DateTime.Now);
            _entries.Add(debit);
            Debits.Add(debit);
            Balance -= amount;
        }
        
        // Métodos existentes adaptados para a nova estrutura
        public decimal GetCurrentBalance()
        {
            return Balance;
        }
        
        public IEntry GetLastEntry()
        {
            return _entries.LastOrDefault();
        }
        
        public IReadOnlyCollection<IEntry> GetEntriesByDate(DateTime date)
        {
            return _entries.Where(e => e.Date.Date == date.Date).ToList().AsReadOnly();
        }
    }
} 
