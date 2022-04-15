using System;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// Провайдер идентификатора пользователя по токену аутентификации.
    /// </summary>
    public interface IUserIdFromTokenProvider
    {
        /// <summary>
        /// Возвращает идентификатор пользователя по токену аутентификации.
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="tokenKey">Ключ токена</param>
        Guid GetUserIdFromToken(HttpRequest request, byte[] tokenKey);
    }
}