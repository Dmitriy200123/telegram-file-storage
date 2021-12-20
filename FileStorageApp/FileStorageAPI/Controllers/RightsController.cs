using System;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.RightsFilters;
using FileStorageAPI.Services;
using FileStorageApp.Data.InfoStorage.Models;
using JwtAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("rights")]
    public class RightsController : Controller
    {
        private readonly IRightsService _rightsService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rightsService"></param>
        public RightsController(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        /// <summary>
        /// Возвращает информацию о пользователе
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("user")]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о правах пользователя")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = Request.GetUserIdFromToken(Settings.Key);
            var userRights = await _rightsService.GetUserRights(userId);

            return Ok(userRights.Value);
        }
        
        /// <summary>
        /// Возвращает информацию доступных права
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("description")]
        [SwaggerResponse(StatusCodes.Status200OK, "Информация о доступных правах")]
        public async Task<IActionResult> GetRightsDescription()
        {
            var rightsDescription = await _rightsService.GetRightsDescription();

            return Ok(rightsDescription.Value);
        }
        
        /// <summary>
        /// Добавляет или удаляет права пользователя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RightsFilter(Accesses.Admin)]
        [Route("set")]
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
    }
}