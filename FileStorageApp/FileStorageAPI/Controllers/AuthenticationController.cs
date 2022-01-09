using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Providers;
using JwtAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IAuthenticationService = FileStorageAPI.Services.IAuthenticationService;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// Авторизация через GitLab.
    /// </summary>
    [ApiController]
    [Route("auth/gitlab")]
    [SwaggerTag("Авторизация с помощью GitLab")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AuthenticationController"/>.
        /// </summary>
        /// <param name="authenticationService">Сервис для взаимодействия с аутентификацией</param>
        /// <param name="userIdFromTokenProvider"></param>
        public AuthenticationController(IAuthenticationService authenticationService,
            IUserIdFromTokenProvider userIdFromTokenProvider)
        {
            _authenticationService =
                authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _userIdFromTokenProvider = userIdFromTokenProvider;
        }

        /// <summary>
        /// Получает на вход токен от GitLab и авторизовывает пользователя нашим токеном.
        /// </summary>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Успешно проверили пользователя и создал токен")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Токен GitLab невалидный")]
        public async Task<IActionResult> ExternalLogin([FromHeader] string token)
        {
            var applicationUser = await _authenticationService.LogIn(token);
            return (applicationUser.ResponseCode switch
            {
                HttpStatusCode.Unauthorized => Unauthorized(applicationUser.Message),
                HttpStatusCode.OK => Ok(applicationUser.Value),
                _ => throw new ArgumentException("Unknown response code")
            })!;
        }

        /// <summary>
        /// Разлогирование пользователя.
        /// </summary>
        [Route("logout")]
        [HttpGet]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Такого пользователя нет в базе")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Успешно удалили рефреш токен ")]
        public async Task<IActionResult> LogOut()
        {
            var id = _userIdFromTokenProvider.GetUserIdFromToken(Request, Settings.Key);
            var result = await _authenticationService.LogOut(id);
            return result.ResponseCode switch
            {
                HttpStatusCode.BadRequest => Unauthorized(result.Message),
                HttpStatusCode.NoContent => NoContent(),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Обновление токена.
        /// </summary>
        /// <param name="refreshCred">Токен и рефреш токен</param>
        [AllowAnonymous]
        [HttpPost("refresh")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Невалидные токены")]
        [SwaggerResponse(StatusCodes.Status200OK, "Токен обновлен")]
        public async Task<IActionResult> Refresh([FromBody] RefreshCred refreshCred)
        {
            var refresh = await _authenticationService.Refresh(refreshCred);
            return refresh.ResponseCode switch
            {
                HttpStatusCode.Unauthorized => Unauthorized(refresh.Message),
                HttpStatusCode.OK => Ok(refresh.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}