using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [ApiController]
    [Route("api/senders")]
    [SwaggerTag("Информация об отправителях из Telegram")]
    public class SendersController : Controller
    {
        private readonly ISenderService _senderService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SendersController"/>.
        /// </summary>
        /// <param name="senderService">Сервис для взаимодействия с информацией об отправителях</param>
        public SendersController(ISenderService senderService)
        {
            _senderService = senderService ?? throw new ArgumentNullException(nameof(senderService));
        }

        /// <summary>
        /// Возвращает отправителя по id.
        /// </summary>
        /// <param name="id">Идентификатор отправителя</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает отправителя по заданному идентификатору", typeof(Sender))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если отправитель с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> GetSenderById(Guid id)
        {
            var sender = await _senderService.GetSenderByIdAsync(id);

            return sender.ResponseCode switch
            {
                HttpStatusCode.NotFound => NotFound(sender.Message),
                HttpStatusCode.OK => Ok(sender.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает список отправителей, к которым пользователь имеет доступ.
        /// </summary>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает всех доступных отправителей для текущего пользователя", typeof(List<Sender>))]
        public async Task<IActionResult> GetSenders()
        {
            var senders = await _senderService.GetSendersAsync();

            return senders.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(senders.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        ///  Возвращает отправителя по заданной подстроке. Проверяет телеграм ник.
        /// </summary>
        /// <param name="telegramName">Подстрока для поиска по телеграм нику</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("search/telegramname")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает отправителей по заданной подстроке", typeof(List<Sender>))]
        public async Task<IActionResult> GetSendersByTelegramNameSubstring([FromQuery(Name = "telegramName"), Required]
            string telegramName)
        {
            var senders = await _senderService.GetSendersByTelegramNameSubstringAsync(telegramName);

            return senders.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(senders.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает отправителя по заданной подстроке. Проверяет имя пользователя
        /// </summary>
        /// <param name="fullName">Подстрока для поиска по имени отправителя</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("search/fullname")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает отправителей по заданной подстроке", typeof(List<Sender>))]
        public async Task<IActionResult> GetSendersByFullNameSubstring([FromQuery(Name = "fullName"), Required]
            string fullName)
        {
            var senders = await _senderService.GetSendersByUserNameSubstringAsync(fullName);

            return senders.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(senders.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}