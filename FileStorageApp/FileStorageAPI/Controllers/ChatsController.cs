using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// API информации о чатах из Telegram.
    /// </summary>
    [ApiController]
    [Route("api/chats")]
    [SwaggerTag("Информация о чатах из Telegram")]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ChatsController"/>
        /// </summary>
        /// <param name="chatService">Сервис для взаимодействия с информацией о чатах</param>
        public ChatsController(IChatService chatService)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        }

        /// <summary>
        /// Возвращает список чатов.
        /// </summary>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает список чатов", typeof(List<Chat>))]
        public async Task<IActionResult> GetChats()
        {
            var chats = await _chatService.GetAllChats();

            return Ok(chats);
        }

        /// <summary>
        /// Возвращает информацию о чате по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор чата</param>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает чат по заданному идентификатору", typeof(Chat))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если чат с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> GetChat(Guid id)
        {
            var chat = await _chatService.GetChatByIdAsync(id);
            if (chat is null)
                return NotFound($"Chat with identifier: {id} not found");

            return Ok(chat);
        }

        /// <summary>
        /// Возвращает чаты, в названии которых содержится подстрока названия чата.
        /// </summary>
        /// <param name="chatName">Название чата</param>
        [HttpGet("search")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает список чатов по совпадению с chatName", typeof(List<Chat>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Если query-параметр \"chatName\" пуст", typeof(string))]
        public async Task<IActionResult> SearchChat([FromQuery(Name = "chatName"), Required]
            string chatName)
        {
            var chats = await _chatService.GetByChatNameSubstringAsync(chatName);
            return Ok(chats);
        }
    }
}