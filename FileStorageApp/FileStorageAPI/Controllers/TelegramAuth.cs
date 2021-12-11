using System;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("auth/telegram")]
    [SwaggerTag("Авторизация через телеграм")]
    public class TelegramAuth : Controller
    {
        private readonly ITelegramService _telegramService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="telegramService"></param>
        public TelegramAuth(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [Authorize]
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status302Found, "Редирект на бота")]
        public IActionResult ExternalLoginCallback()
        {
            var authHeader = Request.Headers[HeaderNames.Authorization];
            var userToken = authHeader.ToString().Split(' ')[1];
            var applicationUser = _telegramService.LogIn(userToken);
            return (applicationUser.ResponseCode switch
            {
                HttpStatusCode.BadRequest => BadRequest(applicationUser.Message),
                HttpStatusCode.OK => Redirect(applicationUser.Value),
                _ => throw new ArgumentException("Unknown response code")
            })!;
        }
    }
}