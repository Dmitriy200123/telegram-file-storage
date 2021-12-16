using System;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Services;
using JwtAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с данными пользователя
    /// </summary>
    [Authorize]
    [Route("userinfo")]
    public class UserInfoController : Controller
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ISettings _settings;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UserInfoController"/>
        /// </summary>
        /// <param name="userInfoService">Сервис для работы с информацией пользователя</param>
        /// <param name="settings">Настройки приложения</param>
        public UserInfoController(IUserInfoService userInfoService, ISettings settings)
        {
            _userInfoService = userInfoService;
            _settings = settings;
        }

        /// <summary>
        /// Возвращает информацию о пользователе
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о пользователе")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Такого пользователя не существует")]
        public async Task<IActionResult> GetUserInfo()
        {
            var authHeader = Request.Headers[HeaderNames.Authorization];
            var userToken = authHeader.ToString().Split(' ')[1];
            var principal = TokenHelper.GetPrincipalFromToken(userToken, _settings.Key);

            var userName = principal.Identity!.Name;
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