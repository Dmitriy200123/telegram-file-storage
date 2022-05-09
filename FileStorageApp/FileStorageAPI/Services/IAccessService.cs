using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для работы с правами пользователя
    /// </summary>
    public interface IAccessService
    {
        /// <summary>
        /// Проверяет имеет ли пользователь доступ ко всем файлам
        /// </summary>
        /// <param name="request">Запрос пришедший</param>
        /// <returns></returns>
        Task<bool> HasAnyFilesAccessAsync(HttpRequest request);
    }
}