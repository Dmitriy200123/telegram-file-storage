using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace FileStorageAPI.Controllers
{
    [Authorize]
    public class UserInfoController : Controller
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ISettings _settings;
        public UserInfoController(IUserInfoService userInfoService, ISettings settings)
        {
            _userInfoService = userInfoService;
            _settings = settings;
        }

        [HttpGet]//todo add dock
        public async Task<IActionResult> GetUserInfo()
        {
            var authHeader = Request.Headers[HeaderNames.Authorization];
            var userToken = authHeader.ToString().Split(' ')[1];
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(userToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_settings.Key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
                }, out validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            if(jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token passed!");
            }

            var userName = principal.Identity.Name;
            var user = await _userInfoService.GetUserInfo(Guid.Parse(userName!));
            
            return user.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(user.Value),
                HttpStatusCode.NotFound => NotFound(user.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}