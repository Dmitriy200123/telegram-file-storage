using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// Провайдер для получения прав пользователя из токена
    /// </summary>
    public interface IAccessesFromTokenProvider
    {
        /// <summary>
        /// Возвращает права пользователя для переданного токена
        /// </summary>
        /// <param name="request">Токен для получения прав пользователя</param>
        Task<IEnumerable<Accesses>> GetAccessesFromTokenAsync(HttpRequest request);
    }
}