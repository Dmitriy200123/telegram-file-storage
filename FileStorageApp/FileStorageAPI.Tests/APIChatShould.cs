using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FileStorageAPI.Tests
{
    public class APIChatShould : BaseShould
    {
        private static readonly IChatConverter ChatConverter = new ChatConverter();

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
            using var apiClient = CreateHttpClient();
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chatGuid = Guid.NewGuid();
            var chatInDb = new Chat {Id = chatGuid, TelegramId = 0, ImageId = new Guid(), Name = "test"};
            await chatStorage.AddAsync(chatInDb);

            var response = await apiClient.GetAsync($"/api/chats/{chatGuid}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Models.Chat>(responseString);
            actual.Should().BeEquivalentTo(ChatConverter.ConvertToChatInApi(chatInDb));
        }

        [Test]
        public async Task GetChatById_404_ThenCalledWithOtherId()
        {
            using var apiClient = CreateHttpClient();
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            await chatStorage.AddAsync(new Chat {Id = new Guid(), TelegramId = 0, ImageId = new Guid(), Name = "test"});

            var response = await apiClient.GetAsync($"/api/chats/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task SearchChat_ReturnCorrectChat_ThenCalledWithExistingName()
        {
            using var apiClient = CreateHttpClient();
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            const string chatName = "testName";
            var chatInDb = new Chat {Id = new Guid(), TelegramId = 0, ImageId = new Guid(), Name = chatName};
            await chatStorage.AddAsync(chatInDb);

            var response = await apiClient.GetAsync($"/api/chats/search?chatName={chatName}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Chat>>(responseString);
            actual.Should().BeEquivalentTo(new List<Models.Chat> {ChatConverter.ConvertToChatInApi(chatInDb)});
        }

        [Test]
        public async Task SearchChat_ReturnZeroArray_ThenCalledWithNotExistingName()
        {
            using var apiClient = CreateHttpClient();
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            const string chatName = "testName";
            await chatStorage.AddAsync(new Chat
                {Id = new Guid(), TelegramId = 0, ImageId = new Guid(), Name = "anotherChat"});

            var response = await apiClient.GetAsync($"/api/chats/search?chatName={chatName}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Chat>>(responseString);
            actual.Should().BeEmpty();
        }
    }
}