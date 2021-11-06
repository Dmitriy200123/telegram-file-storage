using System;
using System.Collections.Generic;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Config;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
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
        private readonly ChatClient _chatClient;
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
            _chatClient = new ChatClient(config);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetChat(string id)
        {
            if (Guid.TryParse(id, out var checkedId))
            {
                var chat = await _chatClient.GetChat(checkedId);
                if (chat != null)
                {
                    return chat;
                }

                return new NotFoundObjectResult($"Chat with {id} not found");
            }

            return BadRequest("Bad id");
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Chat>>> SearchChat([FromQuery(Name = "chatName")] string chatName)
        {
            return await _chatClient.SearchChats(chatName);
        }
    }
}