using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    // [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ChatsController"/>
        /// </summary>
        /// <param name="chatService">Сервис для взаимодействия с информацией о чатах</param>
        public ChatsController(IChatService chatService, UserManager<ApplicationUser> userManager)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _userManager = userManager;
        }

        /// <summary>
        /// Возвращает список чатов.
        /// </summary>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает список чатов", typeof(List<Chat>))]
        public async Task<IActionResult> GetChats()
        {
            var claimsPrincipal = User;
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            
            
            var chats = await _chatService.GetAllChatsAsync();

            return chats.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(chats.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
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

            return chat.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(chat.Value),
                HttpStatusCode.NotFound => NotFound(chat.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает чаты, в названии которых содержится подстрока названия чата.
        /// </summary>
        /// <param name="chatName">Название чата</param>
        [HttpGet("search")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает список чатов по совпадению с chatName",
            typeof(List<Chat>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Если query-параметр \"chatName\" пуст", typeof(string))]
        public async Task<IActionResult> SearchChat([FromQuery(Name = "chatName"), Required] string chatName)
        {
            var chats = await _chatService.GetByChatNameSubstringAsync(chatName);

            return chats.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(chats.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}