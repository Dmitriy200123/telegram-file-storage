using System;
using System.Net;
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
        public async Task GetChatById_ReturnCorrectChat_ThenCalled()
        {
            var chatGuid = Guid.NewGuid();
            await _chatClient.AddAsync(new Chat(chatGuid, Guid.NewGuid(), "test"));

            var response = await _client.GetAsync($"/api/v1/Chats/{chatGuid}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain($"\"id\":\"{chatGuid}\"");
        }

        [Fact]
        public async Task GetChatById_404_ThenCalledWithOtherId()
        {
            await _chatClient.AddAsync(new Chat(Guid.NewGuid(), Guid.NewGuid(), "test"));

            var response = await _client.GetAsync($"/api/v1/Chats/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetChatById_400_ThenCalledWithWrongId()
        {
            await _chatClient.AddAsync(new Chat(Guid.NewGuid(), Guid.NewGuid(), "test"));

            var response = await _client.GetAsync("/api/v1/Chats/abc");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SearchChat_ReturnCorrectChat_ThenCalledWithExistingName()
        {
            var chatName = "testName";
            await _chatClient.AddAsync(new Chat(Guid.NewGuid(), Guid.NewGuid(), chatName));

            var response = await _client.GetAsync($"/api/v1/Chats/search?chatName={chatName}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain($"\"name\":\"{chatName}\"");
        }

        [Fact]
        public async Task SearchChat_ReturnZeroArray_ThenCalledWithNotExistingName()
        {
            var chatName = "testName";
            await _chatClient.AddAsync(new Chat(Guid.NewGuid(), Guid.NewGuid(), "anotherChat"));

            var response = await _client.GetAsync($"/api/v1/Chats/search?chatName={chatName}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("[]");
        }
    }
}