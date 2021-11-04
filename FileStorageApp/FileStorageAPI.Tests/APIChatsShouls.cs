using System;
using System.Net.Http;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using FluentAssertions;


namespace FileStorageAPI.Tests
{
    public abstract class TestsBase : IDisposable
    {
        public void Dispose()
        {
            var config = APIChatsShould.config;
            var chatsConfig = new DataBaseConfig();
            chatsConfig.SetConnectionString(
                $"Server={config["DbHost"]};" +
                $"Username={config["DbUser"]};" +
                $"Database={config["DbName"]};" +
                $"Port={config["DbPort"]};" +
                $"Password={config["DbPassword"]};" +
                "SSLMode=Prefer"
            );
            var chatClient = new InfoStorageFactory(chatsConfig).CreateChatStorage();

            var chats = chatClient.GetAllAsync().GetAwaiter().GetResult();
            foreach (var chat in chats)
            {
                chatClient.DeleteAsync(chat.Id).GetAwaiter().GetResult();
            }
        }
    }

    public class APIChatsShould : TestsBase
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly IChatStorage _chatClient;

        public static readonly IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public APIChatsShould()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseConfiguration(config)
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            _client = _server.CreateClient();


            var chatsConfig = new DataBaseConfig();
            chatsConfig.SetConnectionString(
                $"Server={config["DbHost"]};" +
                $"Username={config["DbUser"]};" +
                $"Database={config["DbName"]};" +
                $"Port={config["DbPort"]};" +
                $"Password={config["DbPassword"]};" +
                "SSLMode=Prefer"
            );
            _chatClient = new InfoStorageFactory(chatsConfig).CreateChatStorage();
        }

        [Fact]
        public async Task ReturnHelloWorld()
        {
            var chatGuid = Guid.NewGuid();
            await _chatClient.AddAsync(new Chat() {Id = chatGuid, ImageId = Guid.NewGuid(), Name = "test"});

            var response = await _client.GetAsync($"/api/v1/Chats/{chatGuid}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().Contain($"\"id\":\"{chatGuid}\"");
        }
    }
}