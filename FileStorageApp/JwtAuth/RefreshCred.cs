using System;

namespace JwtAuth
{
    /// <summary>
    /// 
    /// </summary>
    public class RefreshCred
    {
        /// <summary>
        /// 
        /// </summary>
        public string JwtToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <param name="refreshToken"></param>
        public RefreshCred(string jwtToken, string refreshToken)
        {
            JwtToken = jwtToken ?? throw new ArgumentNullException(nameof(jwtToken));
            RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
        }
    }
}