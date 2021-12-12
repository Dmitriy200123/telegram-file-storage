using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth
{
    /// <inheritdoc />
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        /// <inheritdoc />
        public IDictionary<string, string> UsersRefreshTokens { get; set; }

        private readonly string _tokenKey;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="JwtAuthenticationManager"/>.
        /// </summary>
        /// <param name="tokenKey"></param>
        /// <param name="refreshTokenGenerator"></param>
        public JwtAuthenticationManager(string tokenKey, IRefreshTokenGenerator refreshTokenGenerator)
        {
            _tokenKey = tokenKey ?? throw new ArgumentNullException(nameof(tokenKey));
            _refreshTokenGenerator = refreshTokenGenerator ?? throw new ArgumentNullException(nameof(refreshTokenGenerator));
            UsersRefreshTokens = new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public AuthenticationResponse Authenticate(string username, Claim[] claims)
        {
            var token = GenerateTokenString(username, DateTime.UtcNow, claims);
            var refreshToken = _refreshTokenGenerator.GenerateToken();

            if (UsersRefreshTokens.ContainsKey(username))
            {
                UsersRefreshTokens[username] = refreshToken;
            }
            else
            {
                UsersRefreshTokens.Add(username, refreshToken);
            }

            return new AuthenticationResponse(token, refreshToken);
        }

        /// <inheritdoc />
        public AuthenticationResponse Authenticate(string username)
        {
            var token = GenerateTokenString(username, DateTime.UtcNow);
            var refreshToken = _refreshTokenGenerator.GenerateToken();

            if (UsersRefreshTokens.ContainsKey(username))
            {
                UsersRefreshTokens[username] = refreshToken;
            }
            else
            {
                UsersRefreshTokens.Add(username, refreshToken);
            }

            return new AuthenticationResponse(token, refreshToken);
        }

        private string GenerateTokenString(string username, DateTime expires, Claim[] claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}