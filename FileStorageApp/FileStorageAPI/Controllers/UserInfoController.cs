using System;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.RightsFilters;
using FileStorageAPI.Services;
using FileStorageApp.Data.InfoStorage.Models;
using JwtAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с данными пользователя.
    /// </summary>
    [ApiController]
    [Route("users")]
    [SwaggerTag("Информация о пользователях")]
    [Authorize]
    public class UserInfoController : Controller
    {
        private readonly IUserInfoService _userInfoService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UserInfoController"/>.
        /// </summary>
        /// <param name="userInfoService">Сервис для работы с информацией пользователя</param>
        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }

        /// <summary>
        /// Возвращает информацию о пользователе.
        /// </summary>
        [HttpGet("current")]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о пользователе")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Такого пользователя не существует")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = Request.GetUserIdFromToken(Settings.Key);
            var user = await _userInfoService.GetUserInfo(userId);

            return user.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(user.Value),
                HttpStatusCode.NotFound => NotFound(user.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает информацию о всех авторизированных пользователях. Требуется право "Admin".
        /// </summary>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о пользователях")]
        [RightsFilter(Accesses.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userInfoService.GetUsersInfo();
            return Ok(users.Value);
        }
    }
}