using System;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth
{
    /// <inheritdoc />
    public class TokenRefresher : ITokenRefresher
    {
        private readonly byte[] _key;
        private readonly IJwtAuthenticationManager _jWtAuthenticationManager;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jWtAuthenticationManager"></param>
        /// <param name="infoStorageFactory"></param>
        public TokenRefresher(byte[] key, IJwtAuthenticationManager jWtAuthenticationManager,
            IInfoStorageFactory infoStorageFactory)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _jWtAuthenticationManager = jWtAuthenticationManager ?? throw new ArgumentNullException(nameof(jWtAuthenticationManager));
            _infoStorageFactory = infoStorageFactory;
        }

        /// <inheritdoc />
        public async Task<AuthenticationResponse?> Refresh(RefreshCred refreshCred)
        {
            var principal = TokenHelper.GetPrincipalFromToken(refreshCred.JwtToken, _key);

            var userName = principal.Identity?.Name;
            if (userName == null)
                return null;
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var refreshToken = await usersStorage.GetRefreshToken(Guid.Parse(userName));
            if (refreshCred.RefreshToken != refreshToken)
                return null;

            return await _jWtAuthenticationManager.Authenticate(userName, principal.Claims.ToArray());
        }
    }
}