using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using JwtAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIntToGuidConverter _intToGuidConverter;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly ITokenRefresher _tokenRefresher;
        private readonly IActionContextAccessor _accessor;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="signInManager">Менеджер входа</param>
        /// <param name="intToGuidConverter">Конвертер, который превращает int в Guid</param>
        /// <param name="jwtAuthenticationManager">Менеджер токенов</param>
        /// <param name="tokenRefresher">Обновлятель токена</param>
        /// <param name="accessor">Изменятель ответа</param>
        /// <param name="configuration">Конфигурация приложения</param>
        public AuthenticationService(SignInManager<ApplicationUser> signInManager,
            IIntToGuidConverter intToGuidConverter, IJwtAuthenticationManager jwtAuthenticationManager,
            ITokenRefresher tokenRefresher, IActionContextAccessor accessor, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _intToGuidConverter = intToGuidConverter;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _tokenRefresher = tokenRefresher;
            _accessor = accessor;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<RequestResult<RedirectResult>> LogIn(string? remoteError)
        {
            if (remoteError != null)
                return RequestResult.BadRequest<RedirectResult>("Произошла ошибка у GitLab");
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RequestResult.BadRequest<RedirectResult>("Почему-то пользователь пустой");
            var providerKey = _intToGuidConverter.Convert(int.Parse(info.ProviderKey)).ToString();

            var token = _jwtAuthenticationManager.Authenticate(providerKey);

            return RequestResult.Ok(CreateRedirectResult(token));
        }


        /// <inheritdoc />
        public RequestResult<RedirectResult> Refresh(RefreshCred refreshCred)
        {
            var token = _tokenRefresher.Refresh(refreshCred);
            return token is null
                ? RequestResult.Unauthorized<RedirectResult>("No such user")
                : RequestResult.Ok(CreateRedirectResult(token));
        }

        /// <inheritdoc />
        public async Task<RequestResult<string>> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RequestResult.Ok(_configuration["RedirectUrl"]);
        }

        private RedirectResult CreateRedirectResult(AuthenticationResponse token)
        {
            var result = new RedirectResult(_configuration["RedirectUrl"], true)
            {
                UrlHelper = new UrlHelper(_accessor.ActionContext)
            };

            result.UrlHelper
                .ActionContext
                .HttpContext
                .Response.Redirect(_configuration["RedirectUrl"]);

            result.UrlHelper
                .ActionContext
                .HttpContext
                .Response.Headers.Add("token", $"{token.JwtToken}");
            result.UrlHelper
                .ActionContext
                .HttpContext
                .Response.Headers.Add("refreshToken", $"{token.RefreshToken}");
            return result;
        }
    }
}