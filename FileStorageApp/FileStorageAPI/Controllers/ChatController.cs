using System;
using System.Threading.Tasks;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Controllers
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        }

        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var chats = await _chatService.GetAllChats();

            return Ok(chats);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetChat(Guid id)
        {
            var chat = await _chatService.GetChatByIdAsync(id);
            if (chat is null)
                return NotFound($"Chat with identifier: {id} not found");

            return Ok(chat);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchChat([FromQuery(Name = "chatName")] string chatName)
        {
            if (string.IsNullOrEmpty(chatName))
                return BadRequest($"Query parameter \"{nameof(chatName)}\" is empty");

            var chats = await _chatService.GetByChatNameSubstringAsync(chatName);
            return Ok(chats);
        }
    }
}