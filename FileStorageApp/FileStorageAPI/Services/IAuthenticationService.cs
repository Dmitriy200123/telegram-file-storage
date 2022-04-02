using System;
using System.Threading.Tasks;
using API;
using JwtAuth;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис отвечающий за аутентификацию.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Метод, который отвечает за аутентификацию пользователя и добавление его в менеджера пользователей.
        /// </summary>
        /// <param name="token"></param>
        Task<RequestResult<AuthenticationResponse>> LogIn(string token);

        /// <summary>
        /// Метод обновления токена.
        /// </summary>
        /// <param name="refreshCred"></param>
        Task<RequestResult<AuthenticationResponse>> Refresh(RefreshCred refreshCred);

        /// <summary>
        /// Метод разлогировавния пользователя.
        /// </summary>
        Task<RequestResult<string>> LogOut(Guid guid);
    }
}