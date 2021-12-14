using System;
using System.Threading.Tasks;
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
        /// <param name="remoteError">Возможная ошибка от GitLab</param>
        Task<RequestResult<AuthenticationResponse>> LogIn(string? remoteError);

        /// <summary>
        /// Метод обновления токена.
        /// </summary>
        /// <param name="refreshCred"></param>
        RequestResult<RedirectResult> Refresh(RefreshCred refreshCred);

        /// <summary>
        /// Метод разлогировавния пользователя.
        /// </summary>
        Task<RequestResult<string>> LogOut();
    }
}