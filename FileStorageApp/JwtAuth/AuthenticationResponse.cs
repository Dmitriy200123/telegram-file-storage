namespace JwtAuth
{
    /// <summary>
    /// Класс отвечающий за токен и рефреш токен
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        /// Конструктор, который принимает
        /// </summary>
        /// <param name="jwtToken">Токен</param>
        /// <param name="refreshToken">Рефреш токен</param>
        public AuthenticationResponse(string jwtToken, string refreshToken)
        {
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Токен
        /// </summary>
        public string JwtToken { get; set; }
        /// <summary>
        /// Рефреш токен
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
