using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth
{
    /// <inheritdoc />
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly string _tokenKey;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="JwtAuthenticationManager"/>.
        /// </summary>
        /// <param name="tokenKey"></param>
        /// <param name="refreshTokenGenerator"></param>
        /// <param name="infoStorageFactory"></param>
        public JwtAuthenticationManager(string tokenKey, IRefreshTokenGenerator refreshTokenGenerator,
            IInfoStorageFactory infoStorageFactory)
        {
            _tokenKey = tokenKey ?? throw new ArgumentNullException(nameof(tokenKey));
            _refreshTokenGenerator =
                refreshTokenGenerator ?? throw new ArgumentNullException(nameof(refreshTokenGenerator));
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
        }

        /// <inheritdoc />
        public async Task<AuthenticationResponse> Authenticate(string username, IEnumerable<Claim> claims)
        {
            var token = GenerateTokenString(DateTime.UtcNow, claims);
            var refreshToken = _refreshTokenGenerator.GenerateToken();
            using var userStorage = _infoStorageFactory.CreateUsersStorage();
            var userId = Guid.Parse(username);
            await userStorage.UpdateRefreshTokenAsync(userId, refreshToken);

            return new AuthenticationResponse(token, refreshToken);
        }

        private string GenerateTokenString(DateTime expires, IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}