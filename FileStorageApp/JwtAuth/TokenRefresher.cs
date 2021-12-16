using System;
using System.IdentityModel.Tokens.Jwt;
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(refreshCred.JwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false 
                }, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token passed!");
            }

            var userName = principal.Identity?.Name;
            if (userName == null)
                throw new InvalidOperationException("Doesn't contain name in Identity");
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var refreshToken = await usersStorage.GetRefreshToken(Guid.Parse(userName));
            if (refreshCred.RefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid token passed!");

            return _jWtAuthenticationManager.Authenticate(userName, principal.Claims.ToArray());
        }
    }
}