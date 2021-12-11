using System;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using JwtAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly ITokenRefresher _tokenRefresher;
        private readonly IActionContextAccessor _accessor;
        private readonly ISettings _settings;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="signInManager">Менеджер входа</param>
        /// <param name="jwtAuthenticationManager">Менеджер токенов</param>
        /// <param name="tokenRefresher">Обновлятель токена</param>
        /// <param name="accessor">Изменятель ответа</param>
        /// <param name="settings"></param>
        /// <param name="infoStorageFactory"></param>
        public AuthenticationService(SignInManager<ApplicationUser> signInManager,
            IJwtAuthenticationManager jwtAuthenticationManager,
            ITokenRefresher tokenRefresher, IActionContextAccessor accessor, ISettings settings,
            IInfoStorageFactory infoStorageFactory)
        {
            _signInManager = signInManager;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _tokenRefresher = tokenRefresher;
            _accessor = accessor;
            _settings = settings;
            _infoStorageFactory = infoStorageFactory;
        }

        /// <inheritdoc />
        public async Task<RequestResult<RedirectResult>> LogIn(string? remoteError)
        {
            if (remoteError != null)
                return RequestResult.BadRequest<RedirectResult>("Произошла ошибка у GitLab");
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RequestResult.BadRequest<RedirectResult>("Почему-то пользователь пустой");
            var gitlabId = int.Parse(info.ProviderKey);
            var user = await CreateOrGetUser(gitlabId);

            var token = _jwtAuthenticationManager.Authenticate(user.Id.ToString());

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
            return RequestResult.Ok(_settings.Configuration["RedirectUrl"]);
        }

        private RedirectResult CreateRedirectResult(AuthenticationResponse token)
        {
            var result = new RedirectResult(_settings.Configuration["RedirectUrl"], true)
            {
                UrlHelper = new UrlHelper(_accessor.ActionContext)
            };

            result.UrlHelper
                .ActionContext
                .HttpContext
                .Response.Redirect(_settings.Configuration["RedirectUrl"]);

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

        private async Task<User> CreateOrGetUser(int gitlabId)
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var isRegistered = await usersStorage.IsRegisteredAsync(gitlabId);
            if (isRegistered)
                return await usersStorage.GetByGitLabIdAsync(gitlabId);
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                TelegramId = null,
                GitLabId = gitlabId
            };
            await usersStorage.AddAsync(user);

            return user;
        }
    }
}