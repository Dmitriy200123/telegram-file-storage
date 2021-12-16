using System;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для работы с данными пользователя
    /// </summary>
    public interface IUserInfoService
    {
        /// <summary>
        /// Получение инфы о пользователе
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<RequestResult<UserInfo>> GetUserInfo(Guid id);
    }
}