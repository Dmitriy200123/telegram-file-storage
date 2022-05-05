using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RightServices;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// АPI прав на использование функций сервиса.
    /// </summary>
    [ApiController]
    [Authorize]
    [SwaggerTag("Права на использование некоторых функций сервиса")]
    [Route("rights")]
    public class RightsController : Controller
    {
        private readonly IRightsService _rightsService;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RightsController"/>.
        /// </summary>
        /// <param name="rightsService">Сервис прав</param>
        /// <param name="userIdFromTokenProvider"></param>
        public RightsController(IRightsService rightsService, IUserIdFromTokenProvider userIdFromTokenProvider)
        {
            _rightsService = rightsService ?? throw new ArgumentNullException(nameof(rightsService));
            _userIdFromTokenProvider = userIdFromTokenProvider ?? throw new ArgumentNullException(nameof(userIdFromTokenProvider));
        }

        /// <summary>
        /// Возвращает информацию о правах текущего пользователя.
        /// </summary>
        [HttpGet("currentUserRights")]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о правах пользователя", typeof(int[]))]
        public async Task<IActionResult> GetUserRights()
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(Request, Settings.Key);
            var currentUserRights = await _rightsService.GetCurrentUserRights(userId);

            return Ok(currentUserRights.Value);
        }

        /// <summary>
        /// Возвращает информацию доступных права.
        /// </summary>
        [HttpGet("description")]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о доступных правах", typeof(RightDescription[]))]
        public IActionResult GetRightsDescription()
        {
            var rightsDescription = _rightsService.GetRightsDescription();

            return Ok(rightsDescription.Value);
        }

        /// <summary>
        /// Добавляет или удаляет права пользователя. Требуется право "UserAccessesManagement".
        /// </summary>
        [HttpPost("set")]
        [RightsFilter(Accesses.UserAccessesManagement)]
        [SwaggerResponse(StatusCodes.Status200OK, "Успешно изменили права")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Нет такого пользователя")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Невалидные данные")]
        public async Task<IActionResult> UpdateUserRights([FromBody] RightEdition rightEdition)
        {
            var updateUserRights = await _rightsService.UpdateUserRights(rightEdition);

            return updateUserRights.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(updateUserRights.Value),
                HttpStatusCode.NotFound => NotFound(updateUserRights.Message),
                HttpStatusCode.BadRequest => BadRequest(updateUserRights.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает информацию о правах пользователя по его идентификатору. Требуется право "UserAccessesManagement".
        /// </summary>
        [HttpGet("userRights")]
        [RightsFilter(Accesses.UserAccessesManagement)]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о правах пользователя по его идентификатору",
            typeof(int[]))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Нет такого пользователя")]
        public async Task<IActionResult> GetUserRights([FromQuery(Name = "userId"), Required] Guid userId)
        {
            var userRights = await _rightsService.GetUserRights(userId);

            return userRights.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(userRights.Value),
                HttpStatusCode.NotFound => NotFound(userRights.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}