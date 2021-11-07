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
    public class SendersController : Controller
    {
        private readonly ISenderService senderService;

        public SendersController(ISenderService senderService)
        {
            this.senderService = senderService;
        }

        [HttpGet("senders/{id:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает отправителя по заданному идентификатору", typeof(List<Sender>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если отправитель с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> GetSenderById(Guid id)
        {
            var result = await senderService.GetSenderByIdAsync(id);
            return result.ResponseCode switch
            {
                HttpStatusCode.NotFound => NotFound(result.Message),
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        [HttpGet("senders")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает всех доступных отправителей для текущего пользователя", typeof(List<Sender>))]
        public async Task<IActionResult> GetSenders()
        {
            var result = await senderService.GetSendersAsync();
            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        [HttpGet("senders/search")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает отправителя по заданной подстроке, если заданы обе подстроки, возвращает их пересечение", typeof(List<Sender>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если оба параметра не были заданы", typeof(string))]
        public async Task<IActionResult> GetSendersByTelegramNameSubstring(string? telegramName, string? fullName)
        {
            var senders = await senderService.GetSendersByUserNameAndTelegramNameSubstringAsync(fullName, telegramName);
            return senders.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(senders.Value),
                HttpStatusCode.NotFound => NotFound(senders.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}