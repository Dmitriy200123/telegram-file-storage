using System;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Config;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileStorageAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IChatStorage _chatClient;
        private readonly IConfiguration _configuration;

        public ChatsController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
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
        public async Task<ActionResult<Chat>> GetTodoItem(string id)
        {
            if (Guid.TryParse(id, out var checkedId))
            {
                var chats = await _chatClient.GetAllAsync();
                var chat = chats.Find(x => x.Id == checkedId);
                if (chat != null)
                {
                    return new Chat() {Id = chat.Id, ImageId = chat.ImageId, Name = chat.Name};
                }

                return new NotFoundObjectResult($"Chat with {id} not found");
            }

            return BadRequest("Bad id");
        }
    }
}