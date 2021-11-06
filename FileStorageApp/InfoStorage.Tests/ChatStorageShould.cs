using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using NUnit.Framework;

namespace InfoStorage.Tests
{
    public class ChatStorageShould
    {
        private List<Chat> _elementsToDelete;
        private readonly IInfoStorageFactory _infoStorageFactory;

        public ChatStorageShould()
        {
            var config = new DataBaseConfig(Settings.SetupString);
            _infoStorageFactory = new InfoStorageFactory(config);
        }


        [SetUp]
        public void Setup()
        {
            _elementsToDelete = new List<Chat>();
        }

        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetByChatNameSubstringAsync_ReturnCorrectChats_WhenNameHasSubstring(string substring)
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var expected = new List<Chat>();
            var chat = new Chat
            {
                TelegramId = 0,
                Name = "Substring",
                ImageId = Guid.NewGuid(),
            };
            expected.Add(chat);
            _elementsToDelete.Add(chat);
            await chatStorage.AddAsync(chat);

            var actual = await chatStorage.GetByChatNameSubstringAsync(substring);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestCase("aboba")]
        [TestCase("bus")]
        public async Task GetByChatNameSubstringAsync_NoChats_WhenNameHasNoSubstring(string substring)
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chat = new Chat
            {
                TelegramId = 0,
                Name = "Substring",
                ImageId = Guid.NewGuid(),
            };
            _elementsToDelete.Add(chat);
            await chatStorage.AddAsync(chat);

            var actual = await chatStorage.GetByChatNameSubstringAsync(substring);

            actual.Should().BeEmpty();
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenNoSameNames()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var expected = new List<Chat>();
            var chat = new Chat
            {
                TelegramId = 0,
                Name = "aaaaa",
                ImageId = Guid.NewGuid(),
            };
            var chat2 = new Chat
            {
                TelegramId = 1,
                Name = "bbbbb",
                ImageId = Guid.NewGuid(),
            };
            var chat3 = new Chat
            {
                TelegramId = 2,
                Name = "cccc",
                ImageId = Guid.NewGuid(),
            };
            expected.Add(chat);
            expected.Add(chat2);
            expected.Add(chat3);
            _elementsToDelete.AddRange(expected);
            await chatStorage.AddAsync(chat);
            await chatStorage.AddAsync(chat2);
            await chatStorage.AddAsync(chat3);

            var actual = await chatStorage.GetAllAsync();

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id).WithStrictOrdering());
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenSameNames()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var expected = new List<Chat>();
            var chat = new Chat
            {
                TelegramId = 0,
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "aaaaa",
                ImageId = Guid.NewGuid(),
            };
            var chat2 = new Chat
            {
                TelegramId = 1,
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "aaaaa",
                ImageId = Guid.NewGuid(),
            };
            var chat3 = new Chat
            {
                TelegramId = 2,
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "aaaaa",
                ImageId = Guid.NewGuid(),
            };
            expected.Add(chat3);
            expected.Add(chat);
            expected.Add(chat2);
            _elementsToDelete.AddRange(expected);
            await chatStorage.AddAsync(chat);
            await chatStorage.AddAsync(chat2);
            await chatStorage.AddAsync(chat3);

            var actual = await chatStorage.GetAllAsync();

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [TearDown]
        public async Task TearDown()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            foreach (var elem in _elementsToDelete)
                await chatStorage.DeleteAsync(elem.Id);
        }
    }
}