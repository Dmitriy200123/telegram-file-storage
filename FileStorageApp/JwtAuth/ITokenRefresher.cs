namespace JwtAuth
{
    /// <summary>
    /// Класс отвечабщий за обновление токена.
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