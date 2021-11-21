using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FileStorageAPI.Tests
{
    public class APIChatShould
    {
        private readonly HttpClient _apiClient;
        private readonly IInfoStorageFactory _infoStorageFactory;

        private static readonly IChatConverter ChatConverter = new ChatConverter();

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public APIChatShould()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseConfiguration(Config)
                .UseEnvironment("Debug")
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

        [TearDown]
        public async Task TearDown()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chats = await chatStorage.GetAllAsync();
            foreach (var chat in chats)
                await chatStorage.DeleteAsync(chat.Id);
        }

        [Test]
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
            actual.Should().BeEquivalentTo(ChatConverter.ConvertToChatInApi(chatInDb));
        }

        [Test]
        public async Task GetChatById_404_ThenCalledWithOtherId()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            await chatStorage.AddAsync(new Chat {Id = new Guid(), TelegramId = 0, ImageId = new Guid(), Name = "test"});

            var response = await _apiClient.GetAsync($"/api/chats/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
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
            actual.Should().BeEquivalentTo(new List<Models.Chat> {ChatConverter.ConvertToChatInApi(chatInDb)});
        }

        [Test]
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