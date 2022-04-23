using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JwtAuth
{
    /// <summary>
    /// Менеджер токенов.
    /// </summary>
    public interface IJwtAuthenticationManager
    {
        /// <summary>
        /// Рефреш токены пользователей.
        /// </summary>
        /// <summary>
        /// Создать токен и рефреш токен с определенными клаймами.
        /// </summary>
        /// <param name="username">Пользователь для которого создается токен и рефреш токен</param>
        /// <param name="expires">Дата истечения токена</param>
        /// <param name="claims">Клаймы</param>
        Task<AuthenticationResponse> Authenticate(string username, DateTime expires = default, params Claim[] claims);
    }
}