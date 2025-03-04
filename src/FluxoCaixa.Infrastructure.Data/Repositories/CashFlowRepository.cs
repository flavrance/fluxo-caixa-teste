using FluxoCaixa.Domain.Core.Entities;
using FluxoCaixa.Domain.Core.Interfaces.Repositories;
using FluxoCaixa.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxoCaixa.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Implementação do repositório de fluxo de caixa
    /// 
    /// Esta classe implementa as interfaces ICashFlowReadOnlyRepository e ICashFlowWriteOnlyRepository,
    /// fornecendo métodos para leitura e escrita de dados relacionados ao fluxo de caixa.
    /// 
    /// Observações importantes:
    /// - A propriedade Entries da classe CashFlow é somente leitura e baseada em interface (IEntry),
    ///   o que requer tratamento especial ao carregar dados do banco.
    /// - Após carregar um CashFlow com suas coleções Credits e Debits, é necessário chamar o método
    ///   AddEntries para popular a coleção interna _entries, garantindo que a propriedade Entries
    ///   retorne todos os dados corretamente.
    /// - O Entity Framework Core não consegue mapear automaticamente coleções baseadas em interfaces,
    ///   por isso é necessário o tratamento manual.
    /// </summary>
    public class CashFlowRepository : ICashFlowReadOnlyRepository, ICashFlowWriteOnlyRepository
    {
        private readonly FluxoCaixaDbContext _context;

        public CashFlowRepository(FluxoCaixaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CashFlow>> GetAllAsync()
        {
            var cashFlows = await _context.CashFlows
                .Include(c => c.Credits)
                .Include(c => c.Debits)
                .ToListAsync();

            // Popula a coleção _entries interna de cada CashFlow com os Credits e Debits carregados
            // Isso é necessário porque o EF Core não consegue mapear automaticamente a coleção Entries
            // que é baseada em interface
            foreach (var cashFlow in cashFlows)
            {
                cashFlow.AddEntries(cashFlow.Credits);
                cashFlow.AddEntries(cashFlow.Debits);
            }   

            return cashFlows;
        }

        public async Task<CashFlow> GetByIdAsync(Guid id)
        {
            var cashFlow = await _context.CashFlows
                .Include(c => c.Credits)
                .Include(c => c.Debits)
                .FirstOrDefaultAsync(c => c.Id == id);

            // Popula a coleção _entries interna do CashFlow com os Credits e Debits carregados
            if (cashFlow != null)
            {
                cashFlow.AddEntries(cashFlow.Credits);
                cashFlow.AddEntries(cashFlow.Debits);
            }

            return cashFlow;
        }

        public async Task<IEnumerable<CashFlow>> GetByDateAsync(DateTime date)
        {
            var cashFlows = await _context.CashFlows
                .Include(c => c.Credits)
                .Include(c => c.Debits)
                .Where(c => c.Date.Date == date.Date)
                .ToListAsync();

            // Popula a coleção _entries interna de cada CashFlow com os Credits e Debits carregados
            foreach (var cashFlow in cashFlows)
            {
                cashFlow.AddEntries(cashFlow.Credits);
                cashFlow.AddEntries(cashFlow.Debits);
            }

            return cashFlows;
        }

        public async Task<bool> AddAsync(CashFlow cashFlow)
        {
            await _context.CashFlows.AddAsync(cashFlow);
            return await SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(CashFlow cashFlow)
        {
            // Primeiro, obtemos o fluxo de caixa existente com suas coleções
            var existingCashFlow = await _context.CashFlows
                .Include(cf => cf.Credits)
                .Include(cf => cf.Debits)
                .FirstOrDefaultAsync(cf => cf.Id == cashFlow.Id);

            if (existingCashFlow == null)
                return false;

            // Atualizamos as propriedades básicas
            _context.Entry(existingCashFlow).CurrentValues.SetValues(cashFlow);

            // Identificamos novos créditos e os adicionamos à coleção
            foreach (var credit in cashFlow.Credits)
            {
                if (!existingCashFlow.Credits.Any(c => c.Id == credit.Id))
                {
                    // Verificamos se o crédito já existe no contexto
                    var existingCredit = await _context.Credits.FindAsync(credit.Id);
                    if (existingCredit == null)
                    {
                        // Se não existir, adicionamos como uma nova entidade
                        await _context.Credits.AddAsync(credit);
                    }
                    else
                    {
                        // Se existir, atualizamos seus valores
                        _context.Entry(existingCredit).CurrentValues.SetValues(credit);
                    }
                    
                    // Adicionamos à coleção do fluxo de caixa
                    existingCashFlow.Credits.Add(credit);
                }
            }

            // Identificamos novos débitos e os adicionamos à coleção
            foreach (var debit in cashFlow.Debits)
            {
                if (!existingCashFlow.Debits.Any(d => d.Id == debit.Id))
                {
                    // Verificamos se o débito já existe no contexto
                    var existingDebit = await _context.Debits.FindAsync(debit.Id);
                    if (existingDebit == null)
                    {
                        // Se não existir, adicionamos como uma nova entidade
                        await _context.Debits.AddAsync(debit);
                    }
                    else
                    {
                        // Se existir, atualizamos seus valores
                        _context.Entry(existingDebit).CurrentValues.SetValues(debit);
                    }
                    
                    // Adicionamos à coleção do fluxo de caixa
                    existingCashFlow.Debits.Add(debit);
                }
            }

            // Atualizamos a coleção _entries interna para garantir consistência
            existingCashFlow.AddEntries(existingCashFlow.Credits);
            existingCashFlow.AddEntries(existingCashFlow.Debits);

            return await SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var cashFlow = await _context.CashFlows.FindAsync(id);
            if (cashFlow == null)
            {
                return false;
            }

            _context.CashFlows.Remove(cashFlow);
            return await SaveChangesAsync();
        }

        public async Task<IEnumerable<Report>> GetReportsByDateAsync(DateTime date)
        {
            var cashFlows = await _context.CashFlows
                .Include(c => c.Credits)
                .Include(c => c.Debits)
                .Where(c => c.Date.Date == date.Date)
                .ToListAsync();

            var reports = new List<Report>();
            foreach (var cashFlow in cashFlows)
            {
                // Popula a coleção _entries interna de cada CashFlow para garantir que todos os dados estejam disponíveis
                cashFlow.AddEntries(cashFlow.Credits);
                cashFlow.AddEntries(cashFlow.Debits);
                
                // Obter todas as entradas (créditos e débitos) para o fluxo de caixa
                var entries = new List<IEntry>();
                entries.AddRange(cashFlow.Credits);
                entries.AddRange(cashFlow.Debits);
                
                // Criar o relatório usando o método estático Create
                var report = Report.Create(
                    cashFlow.Id, 
                    cashFlow.Date, 
                    new Domain.Core.ValueObjects.Amount(cashFlow.Balance), 
                    entries.AsReadOnly()
                );
                
                reports.Add(report);
            }
            
            return reports;
        }

        public async Task<bool> AddReportAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            return await SaveChangesAsync();
        }

        private async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 