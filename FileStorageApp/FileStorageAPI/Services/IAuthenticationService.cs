using System.Threading.Tasks;
using JwtAuth;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис отвечающий за аутентификацию
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Метод, который отвечает за аутентификацию пользователя и добавление его в менеджера пользователей
        /// </summary>
        /// <param name="remoteError">Возможная ошибка от GitLab</param>
        /// <returns></returns>
        Task<RequestResult<RedirectResult>> LogIn(string? remoteError);
        /// <summary>
        /// Метод обновления токена
        /// </summary>
        /// <param name="refreshCred"></param>
        /// <returns></returns>
        RequestResult<RedirectResult> Refresh(RefreshCred refreshCred);

        /// <summary>
        /// Метод разлогировавния пользователя
        /// </summary>
        /// <returns></returns>
        Task<RequestResult<string>> LogOut();
    }
}