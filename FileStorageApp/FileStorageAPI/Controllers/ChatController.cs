using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// API информации о чатах из Telegram.
    /// </summary>
    [ApiController]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ChatController"/>
        /// </summary>
        /// <param name="chatService">Сервис для взаимодействия с информацией о чатах</param>
        public ChatController(IChatService chatService)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        }

        /// <summary>
        /// Возвращает весь список чатов.
        /// </summary>
        /// <response code="200">Возвращает список чатов</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Chat>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChats()
        {
            var chats = await _chatService.GetAllChats();

            return Ok(chats);
        }

        /// <summary>
        /// Возвращает информацию о чате по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор чата</param>
        /// <response code="200">Возвращает чат по заданному идентификатору</response>
        /// <response code="404">Если чат с таким идентификатором не найден</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Chat), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
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
        /// <param name="chatNameSubstring">Название чата</param>
        /// <response code="200">Возвращает список чатов по совпадению с chatNameSubstring</response>
        /// <response code="400">Если query-параметр "chatNameSubstring" пуст</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<Chat>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchChat([FromQuery(Name = "chatNameSubstring"), Required]
            string chatNameSubstring)
        {
            var chats = await _chatService.GetByChatNameSubstringAsync(chatNameSubstring);
            return Ok(chats);
        }
    }
}