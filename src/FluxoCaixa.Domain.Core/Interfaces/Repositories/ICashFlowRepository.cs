using FluxoCaixa.Domain.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluxoCaixa.Domain.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface para repositório de leitura de fluxo de caixa
    /// </summary>
    public interface ICashFlowReadOnlyRepository
    {
        /// <summary>
        /// Obtém todos os fluxos de caixa
        /// </summary>
        /// <returns>Lista de fluxos de caixa</returns>
        Task<IEnumerable<CashFlow>> GetAllAsync();
        
        /// <summary>
        /// Obtém um fluxo de caixa pelo ID
        /// </summary>
        /// <param name="id">ID do fluxo de caixa</param>
        /// <returns>Fluxo de caixa ou null se não encontrado</returns>
        Task<CashFlow?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Obtém fluxos de caixa por data
        /// </summary>
        /// <param name="date">Data dos fluxos de caixa</param>
        /// <returns>Lista de fluxos de caixa</returns>
        Task<IEnumerable<CashFlow>> GetByDateAsync(DateTime date);
        
        /// <summary>
        /// Obtém relatórios por data
        /// </summary>
        /// <param name="date">Data dos relatórios</param>
        /// <returns>Lista de relatórios</returns>
        Task<IEnumerable<Report>> GetReportsByDateAsync(DateTime date);
    }
    
    /// <summary>
    /// Interface para repositório de escrita de fluxo de caixa
    /// </summary>
    public interface ICashFlowWriteOnlyRepository
    {
        /// <summary>
        /// Adiciona um fluxo de caixa
        /// </summary>
        /// <param name="cashFlow">Fluxo de caixa a ser adicionado</param>
        /// <returns>True se adicionado com sucesso, False caso contrário</returns>
        Task<bool> AddAsync(CashFlow cashFlow);
        
        /// <summary>
        /// Atualiza um fluxo de caixa
        /// </summary>
        /// <param name="cashFlow">Fluxo de caixa a ser atualizado</param>
        /// <returns>True se atualizado com sucesso, False caso contrário</returns>
        Task<bool> UpdateAsync(CashFlow cashFlow);
        
        /// <summary>
        /// Exclui um fluxo de caixa
        /// </summary>
        /// <param name="id">ID do fluxo de caixa a ser excluído</param>
        /// <returns>True se excluído com sucesso, False caso contrário</returns>
        Task<bool> DeleteAsync(Guid id);
        
        /// <summary>
        /// Adiciona um relatório
        /// </summary>
        /// <param name="report">Relatório a ser adicionado</param>
        /// <returns>True se adicionado com sucesso, False caso contrário</returns>
        Task<bool> AddReportAsync(Report report);
    }
} 