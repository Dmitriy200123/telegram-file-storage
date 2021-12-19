using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FileStorageAPI.Tests
{
    public class APIFileSenderShould : BaseShould
    {
        private static readonly ISenderConverter SenderConverter = new SenderConverter();

        [TearDown]
        public async Task TearDown()
        {
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senders = await senderStorage.GetAllAsync();
            foreach (var sender in senders)
                await senderStorage.DeleteAsync(sender.Id);
        }

        [Test]
        public async Task GetSenderById_ReturnCorrectSender_ThenCalled()
        {
            using var apiClient = CreateHttpClient();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senderGuid = Guid.NewGuid();
            var senderInDb = new FileSender
            {
                Id = senderGuid,
                TelegramId = 0,
                TelegramUserName = "Test",
                FullName = "Test",
            };
            await senderStorage.AddAsync(senderInDb);

            var response = await apiClient.GetAsync($"/api/senders/{senderGuid}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Models.Sender>(responseString);
            actual.Should().BeEquivalentTo(SenderConverter.ConvertFileSender(senderInDb));
        }

        [Test]
        public async Task GetSenderById_ReturnNotFound_ThenCalledWithOtherId()
        {
            using var apiClient = CreateHttpClient();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senderInDb = new FileSender
            {
                Id = Guid.NewGuid(),
                TelegramId = 0,
                TelegramUserName = "Test",
                FullName = "Test",
            };
            await senderStorage.AddAsync(senderInDb);

            var response = await apiClient.GetAsync($"/api/senders/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetSendersById_ReturnCorrectSenders_ThenCalled()
        {
            using var apiClient = CreateHttpClient();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senderInDb1 = new FileSender
            {
                Id = Guid.NewGuid(),
                TelegramId = 0,
                TelegramUserName = "Test",
                FullName = "Test",
            };
            var senderInDb2 = new FileSender
            {
                Id = Guid.NewGuid(),
                TelegramId = 1,
                TelegramUserName = "Test1",
                FullName = "Test1",
            };
            var expected = SenderConverter.ConvertFileSenders(new[] {senderInDb1, senderInDb2}.ToList());
            await senderStorage.AddAsync(senderInDb1);
            await senderStorage.AddAsync(senderInDb2);

            var response = await apiClient.GetAsync("/api/senders/");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Sender>>(responseString);
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase("es")]
        [TestCase("Test")]
        [TestCase("t")]
        public async Task GetSenderByTelegramNameSubstring_ReturnCorrectSenders_ThenCalled(string substring)
        {
            using var apiClient = CreateHttpClient();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senderGuid = Guid.NewGuid();
            var senderInDb = new FileSender
            {
                Id = senderGuid,
                TelegramId = 0,
                TelegramUserName = "Test",
                FullName = "Test",
            };
            var expected = new List<Models.Sender> {SenderConverter.ConvertFileSender(senderInDb)};
            await senderStorage.AddAsync(senderInDb);

            var response = await apiClient.GetAsync($"/api/senders/search/telegramname?telegramName={substring}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Sender>>(responseString);
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase("aaa")]
        [TestCase("b")]
        [TestCase("z")]
        public async Task GetSenderByTelegramNameSubstring_ReturnEmptySenders_ThenNoMatching(string substring)
        {
            using var apiClient = CreateHttpClient();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senderGuid = Guid.NewGuid();
            var senderInDb = new FileSender
            {
                Id = senderGuid,
                TelegramId = 0,
                TelegramUserName = "Test",
                FullName = "Test",
            };
            await senderStorage.AddAsync(senderInDb);

            var response = await apiClient.GetAsync($"/api/senders/search/telegramname?telegramName={substring}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Sender>>(responseString);
            actual.Should().BeEmpty();
        }

        [TestCase("es")]
        [TestCase("Test")]
        [TestCase("t")]
        public async Task GetSenderByFullNameSubstring_ReturnCorrectSenders_ThenCalled(string substring)
        {
            using var apiClient = CreateHttpClient();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senderGuid = Guid.NewGuid();
            var senderInDb = new FileSender
            {
                Id = senderGuid,
                TelegramId = 0,
                TelegramUserName = "Test",
                FullName = "Test",
            };
            var expected = new List<Models.Sender> {SenderConverter.ConvertFileSender(senderInDb)};
            await senderStorage.AddAsync(senderInDb);

            var response = await apiClient.GetAsync($"/api/senders/search/fullname?fullname={substring}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Sender>>(responseString);
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase("aaa")]
        [TestCase("b")]
        [TestCase("z")]
        public async Task GetSenderByFullNameSubstring_ReturnEmptySenders_ThenNoMatching(string substring)
        {
            using var apiClient = CreateHttpClient();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senderGuid = Guid.NewGuid();
            var senderInDb = new FileSender
            {
                Id = senderGuid,
                TelegramId = 0,
                TelegramUserName = "Test",
                FullName = "Test",
            };
            await senderStorage.AddAsync(senderInDb);

            var response = await apiClient.GetAsync($"/api/senders/search/telegramname?telegramName={substring}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.Sender>>(responseString);
            actual.Should().BeEmpty();
        }

        [Test]
        public async Task PostTelegramUser_ReturnCorrectUser_ThenCalled()
        {
            using var apiClient = CreateHttpClient();
            var telegramUser = new TelegramUser
            {
                TelegramUserName = "Test",
                FullName = "Test",
                TelegramId = 100
            };
            var expected = new Sender
            {
                Id = default,
                TelegramUserName = "Test",
                FullName = "Test"
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(telegramUser), Encoding.UTF8, "application/json");

            var response = await apiClient.PostAsync("/api/senders/", stringContent);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Models.Sender>(responseString);
            actual.Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id));
        }
    }
}