using System.Threading.Tasks;

namespace JwtAuth
{
    /// <summary>
    /// Класс отвечающий за обновление токена.
    /// </summary>
    public interface ITokenRefresher
    {
        /// <summary>
        /// Обновляет токен.
        /// </summary>
        /// <param name="refreshCred">Токен и рефреш токен пользователя</param>
        AuthenticationResponse Refresh(RefreshCred refreshCred);
    }
}