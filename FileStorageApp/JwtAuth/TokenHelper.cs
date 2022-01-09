using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace JwtAuth
{
    /// <summary>
    /// Вспомогательные методы для работы с токеном
    /// </summary>
    public static class TokenHelper
    {
        /// <summary>
        /// Получение данных о пользователе из токена
        /// </summary>
        /// <param name="token">jwtToken</param>
        /// <param name="key">Ключ которым зашифрован токен</param>
        /// <returns></returns>
        public static ClaimsPrincipal GetPrincipalFromToken(string token, byte[] key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false 
                }, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token passed!");
            }

            return principal;
        }
    }
}