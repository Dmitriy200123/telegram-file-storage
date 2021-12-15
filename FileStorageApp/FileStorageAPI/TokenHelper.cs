using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace FileStorageAPI
{
    /// <summary>
    /// 
    /// </summary>
    public static class TokenHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static Guid GetUserIdFromHeader(HttpRequest httpRequest, byte[] key)
        {
            var header = httpRequest.Headers["Authorization"].ToString();
            var value = header.Split(' ')[1];
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(value,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false 
                }, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token passed!");
            }

            var userName = principal.Identity?.Name;
            return Guid.Parse(userName);
        }
    }
}