using System;
using System.Net;
using System.Threading.Tasks;
using JwtAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISettings _settings;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AuthenticationController"/>
        /// </summary>
        /// <param name="authenticationService">Сервис для взаимодействия с аутентификацией</param>
        public AuthenticationController(IAuthenticationService authenticationService, ISettings settings)
        {
            _authenticationService =
                authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _settings = settings;
        }

        /// <summary>
        /// Базовый эндпоинт для входа
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Успешно проверили пользователя и создал токен")]
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
        /// Разлогирование пользователя
        /// </summary>
        /// <returns></returns>
        [Route("logout")]
        [HttpGet]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Такого пользователя нет в базе")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Успешно удалили рефреш токен ")]
        public async Task<IActionResult> LogOut()
        {
            var guid = TokenHelper.GetUserIdFromHeader(HttpContext.Request, _settings.Key);
            var result = await _authenticationService.LogOut(guid);
            return result.ResponseCode switch
            {
                HttpStatusCode.BadRequest => Unauthorized(result.Message),
                HttpStatusCode.NoContent => NoContent(),
                _ => throw new ArgumentException("Unknown response code")
            };
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
        public async Task<IActionResult> Refresh([FromBody] RefreshCred refreshCred)
        {
            var refresh =  await _authenticationService.Refresh(refreshCred);
            return refresh.ResponseCode switch
            {
                HttpStatusCode.Unauthorized => Unauthorized(refresh.Message),
                HttpStatusCode.OK => Ok(refresh.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}