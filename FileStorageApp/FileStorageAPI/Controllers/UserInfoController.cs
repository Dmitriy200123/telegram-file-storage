using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Providers;
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
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UserInfoController"/>.
        /// </summary>
        /// <param name="userInfoService">Сервис для работы с информацией пользователя</param>
        /// <param name="userIdFromTokenProvider"></param>
        public UserInfoController(IUserInfoService userInfoService, IUserIdFromTokenProvider userIdFromTokenProvider)
        {
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
            _userIdFromTokenProvider = userIdFromTokenProvider;
        }

        /// <summary>
        /// Возвращает информацию о пользователе.
        /// </summary>
        [HttpGet("current")]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о пользователе", typeof(UserInfo))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Такого пользователя не существует")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(Request, Settings.Key);
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
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о пользователях", typeof(List<UserIdAndFio>))]
        [RightsFilter(Accesses.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userInfoService.GetUsersInfo();
            return Ok(users.Value);
        }
    }
}