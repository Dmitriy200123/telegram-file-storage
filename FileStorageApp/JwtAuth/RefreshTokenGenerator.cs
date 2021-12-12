using System;
using System.Security.Cryptography;

namespace JwtAuth
{
    /// <inheritdoc />
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        /// <inheritdoc />
        public string GenerateToken()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}