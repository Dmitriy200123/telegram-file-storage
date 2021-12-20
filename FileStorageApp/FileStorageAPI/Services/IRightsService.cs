using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для управления правами пользователей
    /// </summary>
    public interface IRightsService
    {
        /// <summary>
        /// Получение прав пользователей по его идентификатору
        /// </summary>
        /// <param name="id"></param>
        Task<RequestResult<int[]>> GetUserRights(Guid id);
        
        /// <summary>
        /// Сопоставление описания и номера права
        /// </summary>
        Task<RequestResult<List<RightDescription>>> GetRightsDescription();

        /// <summary>
        /// Обновить права пользователя
        /// </summary>
        /// <returns></returns>
        Task<RequestResult<bool>> UpdateUserRights(RightEdition rightEdition);
    }
}