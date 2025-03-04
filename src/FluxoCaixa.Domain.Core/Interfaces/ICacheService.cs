 using System;
using System.Threading.Tasks;

namespace FluxoCaixa.Domain.Core.Interfaces
{
    /// <summary>
    /// Interface para serviço de cache
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Obtém um valor do cache
        /// </summary>
        /// <typeparam name="T">Tipo do valor</typeparam>
        /// <param name="key">Chave do valor</param>
        /// <returns>Valor armazenado no cache ou null se não existir</returns>
        Task<T?> GetAsync<T>(string key);
        
        /// <summary>
        /// Armazena um valor no cache
        /// </summary>
        /// <typeparam name="T">Tipo do valor</typeparam>
        /// <param name="key">Chave do valor</param>
        /// <param name="value">Valor a ser armazenado</param>
        /// <param name="expiry">Tempo de expiração (opcional)</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        
        /// <summary>
        /// Remove um valor do cache
        /// </summary>
        /// <param name="key">Chave do valor</param>
        Task RemoveAsync(string key);
    }
}