using System;
using System.Collections.Generic;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Config;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileStorageAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly ILogger<ChatsController> _logger;
        private readonly IChatStorage _chatClient;
        private readonly IConfiguration _configuration;

        public ChatsController(ILogger<ChatsController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            var config = new DataBaseConfig();
            config.SetConnectionString(
                $"Server={configuration["DbHost"]};" +
                $"Username={configuration["DbUser"]};" +
                $"Database={configuration["DbName"]};" +
                $"Port={configuration["DbPort"]};" +
                $"Password={configuration["DbPassword"]};" +
                "SSLMode=Prefer"
            );
            _chatClient = new InfoStorageFactory(config).CreateChatStorage();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetChat(string id)
        {
            if (Guid.TryParse(id, out var checkedId))
            {
                var chats = await _chatClient.GetAllAsync();
                var chat = chats.Find(x => x.Id == checkedId);
                if (chat != null)
                {
                    return new Chat(chat.Id, chat.ImageId, chat.Name);
                }

                return new NotFoundObjectResult($"Chat with {id} not found");
            }

            return BadRequest("Bad id");
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Chat>>> SearchChat([FromQuery(Name = "chatName")] string chatName)
        {
            var chat = await _chatClient.GetByChatNameSubstringAsync(chatName);

            var result = chat.Select(x => new Chat(x.Id, x.ImageId, x.Name));

            return result.ToList();
        }
    }
}