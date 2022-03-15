using System;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для управления правами пользователей.
    /// </summary>
    public interface IRightsService
    {
        /// <summary>
        /// Получение прав текущего пользователя по его идентификатору.
        /// </summary>
        /// <param name="userId"></param>
        Task<RequestResult<int[]>> GetCurrentUserRights(Guid userId);

        /// <summary>
        /// Сопоставление описания и номера права.
        /// </summary>
        RequestResult<RightDescription[]> GetRightsDescription();

        /// <summary>
        /// Обновить права пользователя.
        /// </summary>
        Task<RequestResult<bool>> UpdateUserRights(RightEdition rightEdition);

        /// <summary>
        /// Возвращает права пользователя по его индентификатору.
        /// </summary>
        /// <param name="userId"></param>
        Task<RequestResult<int[]>> GetUserRights(Guid userId);
    }
}