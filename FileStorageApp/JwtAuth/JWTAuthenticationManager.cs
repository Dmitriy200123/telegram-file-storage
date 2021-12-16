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
        }

        /// <inheritdoc />
        public AuthenticationResponse Authenticate(string username, Claim[] claims)
        {
            var token = GenerateTokenString(DateTime.UtcNow, claims);
            var refreshToken = _refreshTokenGenerator.GenerateToken();

            return new AuthenticationResponse(token, refreshToken);
        }

        /// <inheritdoc />
        public AuthenticationResponse Authenticate(string username)
        {
            var claim = new Claim(ClaimTypes.Name, username);
            var claims = new[] {claim};

            return Authenticate(username, claims);
        }

        private string GenerateTokenString(DateTime expires, IEnumerable<Claim> claims)
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