using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для работы с правами пользователя.
    /// </summary>
    public interface IAccessService
    {
        /// <summary>
        /// Проверяет, обладает ли пользователь из запроса опредленным правом.
        /// </summary>
        /// <param name="request">Запрос пришедший</param>
        /// <param name="access">Требуемое право</param>
        Task<bool> HasAccessAsync(HttpRequest request, Access access);
    }
}