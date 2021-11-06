using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Newtonsoft.Json;

namespace FileStorageAPI.Tests
{
    public class APIChatsShould : IDisposable
    {
        private readonly HttpClient _apiClient;
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IChatConverter _chatConverter = new ChatConverter();

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public APIChatsShould()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseConfiguration(Config)
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            _apiClient = server.CreateClient();
            _apiClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var dbConfig = new DataBaseConfig($"Server={Config["DbHost"]};" +
                                              $"Username={Config["DbUser"]};" +
                                              $"Database={Config["DbName"]};" +
                                              $"Port={Config["DbPort"]};" +
                                              $"Password={Config["DbPassword"]};" +
                                              "SSLMode=Prefer");
            _infoStorageFactory = new InfoStorageFactory(dbConfig);
        }

        public void Dispose()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chats = chatStorage.GetAllAsync().Result;
            Task.WaitAll(chats.Select(chat => chatStorage.DeleteAsync(chat.Id)).ToArray<Task>());
        }

        [Fact]
        public async Task GetChatById_ReturnCorrectChat_ThenCalled()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chatGuid = Guid.NewGuid();
            var chatInDb = new Chat {Id = chatGuid, TelegramId = 0, ImageId = new Guid(), Name = "test"};
            await chatStorage.AddAsync(chatInDb);

            var response = await _apiClient.GetAsync($"/api/chats/{chatGuid}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Models.Chat>(responseString);
            actual.Should().BeEquivalentTo(_chatConverter.ConvertToChatInApi(chatInDb));
        }

        [Fact]
        public async Task GetChatById_404_ThenCalledWithOtherId()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            await chatStorage.AddAsync(new Chat {Id = new Guid(), TelegramId = 0, ImageId = new Guid(), Name = "test"});

            var response = await _apiClient.GetAsync($"/api/chats/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task SearchChat_ReturnCorrectChat_ThenCalledWithExistingName()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            const string chatName = "testName";
            var chatInDb = new Chat {Id = new Guid(), TelegramId = 0, ImageId = new Guid(), Name = chatName};
            await chatStorage.AddAsync(chatInDb);

            var response = await _apiClient.GetAsync($"/api/chats/search?chatName={chatName}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Chat>>(responseString);
            actual.Should().BeEquivalentTo(new List<Models.Chat> {_chatConverter.ConvertToChatInApi(chatInDb)});
        }

        [Fact]
        public async Task SearchChat_ReturnZeroArray_ThenCalledWithNotExistingName()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            const string chatName = "testName";
            await chatStorage.AddAsync(new Chat {Id = new Guid(), TelegramId = 0, ImageId = new Guid(), Name = "anotherChat"});

            var response = await _apiClient.GetAsync($"/api/chats/search?chatName={chatName}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Chat>>(responseString);
            actual.Should().BeEmpty();
        }
    }
}