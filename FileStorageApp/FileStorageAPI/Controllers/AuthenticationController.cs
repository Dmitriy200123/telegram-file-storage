using System;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using JwtAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using IAuthenticationService = FileStorageAPI.Services.IAuthenticationService;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// Авторизация через GitLab
    /// </summary>
    [ApiController]
    [Route("auth/gitlab")]
    [SwaggerTag("Авторизация с помощью GitLab")]
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AuthenticationController"/>
        /// </summary>
        /// <param name="signInManager">Менеджер входа</param>
        /// <param name="authenticationService">Сервис для взаимодействия с аутентификацией</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="accessor"></param>
        public AuthenticationController(SignInManager<ApplicationUser> signInManager,
            IAuthenticationService authenticationService, IActionContextAccessor accessor)
        {
            _signInManager = signInManager;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Базовый эндпоинт для входа
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status300MultipleChoices, "Запускает авторизацию на GitLab")]
        public IActionResult ExternalLogin()
        {
            const string provider = "GitLab";
            const string returnUrl = "auth/gitlab/confirm";
            string redirectUrl =
                Url.Action(nameof(ExternalLoginCallback), "Authentication", new {ReturnUrl = returnUrl});

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        /// <summary>
        /// Callback после авторизации на GitLab
        /// </summary>
        /// <param name="remoteError">Возможная ошибка на GitLab</param>
        /// <returns></returns>
        [Route("confirm")]
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status302Found, "Возвращения пользователя обратно на фронт")]
        public async Task<IActionResult> ExternalLoginCallback(string? remoteError)
        {
            var applicationUser = await _authenticationService.LogIn(remoteError);
            return (applicationUser.ResponseCode switch
            {
                HttpStatusCode.Unauthorized => Unauthorized(applicationUser.Message),
                HttpStatusCode.OK => applicationUser.Value,
                _ => throw new ArgumentException("Unknown response code")
            })!;
        }

        /// <summary>
        /// Эндпоинт для пользователя, который не аутентифицировался
        /// </summary>
        /// <returns></returns>
        [Route("unauthorized")]
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public new IActionResult Unauthorized()
        {
            return base.Unauthorized();
        }

        /// <summary>
        /// Разлогирование пользователя
        /// </summary>
        /// <returns></returns>
        [Route("logout")]
        [HttpGet]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status302Found, "Пользователь успешно разлогинен")]
        public async Task LogOut()
        {
            var result = await _authenticationService.LogOut();
            Redirect(result.Value);
        }

        /// <summary>
        /// Обновление токена
        /// </summary>
        /// <param name="refreshCred">Токен и рефреш токен</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("refresh")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Невалидные токены")]
        [SwaggerResponse(StatusCodes.Status200OK, "Токен обновлен")]
        public IActionResult Refresh([FromBody] RefreshCred refreshCred)
        {
            var refresh = _authenticationService.Refresh(refreshCred);
            return (refresh.ResponseCode switch
            {
                HttpStatusCode.Unauthorized => Unauthorized(refresh.Message),
                HttpStatusCode.OK => refresh.Value,
                _ => throw new ArgumentException("Unknown response code")
            })!;
        }
    }
}