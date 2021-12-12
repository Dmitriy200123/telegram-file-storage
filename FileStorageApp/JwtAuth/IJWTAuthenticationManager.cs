using System.Collections.Generic;
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
        /// Создать токен и рефреш токен.
        /// </summary>
        /// <param name="username">Пользователь для которого создается токен и рефреш токен</param>
        Task<AuthenticationResponse> Authenticate(string username);

        /// <summary>
        /// Рефреш токены пользователей.
        /// </summary>
        IDictionary<string, string> UsersRefreshTokens { get; set; }

        /// <summary>
        /// Создать токен и рефреш токен с определенными клаймами.
        /// </summary>
        /// <param name="username">Пользователь для которого создается токен и рефреш токен</param>
        /// <param name="claims">Клаймы</param>
        Task<AuthenticationResponse> Authenticate(string username, IEnumerable<Claim> claims);
    }
}