using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// API информации об отправителях из Telegram.
    /// </summary>
    /// [ApiController]
    [Route("api/senders")]
    [SwaggerTag("Информация информации об отправителях из Telegram")]
    public class SendersController : Controller
    {
        private readonly ISenderService _senderService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SendersController"/>
        /// </summary>
        /// <param name="senderService">Сервис для взаимодействия с информацией об отправителях</param>
        public SendersController(ISenderService senderService)
        {
            _senderService = senderService ?? throw new ArgumentNullException(nameof(senderService));
        }

        /// <summary>
        /// Возвращает отправителя по id
        /// </summary>
        /// <param name="id">Идентификатор отправителя</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает отправителя по заданному идентификатору", typeof(List<Sender>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если отправитель с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> GetSenderById(Guid id)
        {
            var result = await _senderService.GetSenderByIdAsync(id);
            return result.ResponseCode switch
            {
                HttpStatusCode.NotFound => NotFound(result.Message),
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает список отправителей, к которым пользователь имеет доступ
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает всех доступных отправителей для текущего пользователя", typeof(List<Sender>))]
        public async Task<IActionResult> GetSenders()
        {
            var result = await _senderService.GetSendersAsync();
            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает отправителя по заданной подстроке, если оба параметры не были заданы, вернет 404
        /// Если были заданы оба параметра, то вернет их пересечение
        /// </summary>
        /// <param name="telegramName">подстрока для поиска по телеграм нику</param>
        /// <param name="fullName">подстрока для поиска по имени отправителя</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("search")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает отправителя по заданной подстроке, если заданы обе подстроки, возвращает их пересечение", typeof(List<Sender>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если оба параметра не были заданы", typeof(string))]
        public async Task<IActionResult> GetSendersByTelegramNameSubstring(string? telegramName, string? fullName)
        {
            var senders = await _senderService.GetSendersByUserNameAndTelegramNameSubstringAsync(fullName, telegramName);
            return senders.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(senders.Value),
                HttpStatusCode.NotFound => NotFound(senders.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}