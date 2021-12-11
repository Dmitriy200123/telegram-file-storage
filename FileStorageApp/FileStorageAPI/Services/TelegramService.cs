using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class TelegramService : ITelegramService
    {
        private readonly ISettings _settings;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public TelegramService(ISettings settings)
        {
            _settings = settings;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        /// <inheritdoc />
        public RequestResult<string> LogIn(string token)
        {
            const string url = "https://telegram.me/sixty_six_bit_bot?start=";
            var principal = _jwtSecurityTokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_settings.Key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                }, out var validatedToken);
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return RequestResult.BadRequest<string>("Invalid token passed!");
            }
            var userName = principal.Identity!.Name!;
            var resultUrl = $"{url}{userName}";
            return RequestResult.Ok(resultUrl);
        }
    }
}