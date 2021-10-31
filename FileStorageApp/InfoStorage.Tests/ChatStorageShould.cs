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
        private List<Chat> elementsToDelete;
        private readonly IInfoStorageFactory infoStorageFactory;

        public ChatStorageShould()
        {
            var config = new DataBaseConfig();
            config.SetConnectionString(Settings.SetupString);
            infoStorageFactory = new InfoStorageFactory(config);
        }


        [SetUp]
        public void Setup()
        {
            elementsToDelete = new List<Chat>();
        }

        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetByChatNameSubstringAsync_ReturnCorrectChats_WhenNameHasSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            var expected = new List<Chat>();
            var chat = new Chat
            {
                Name = "Substring",
                ImageId = Guid.NewGuid(),
            };
            expected.Add(chat);
            elementsToDelete.Add(chat);
            await chatStorage.AddAsync(chat);

            var actual = await chatStorage.GetByChatNameSubstringAsync(substring);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestCase("aboba")]
        [TestCase("bus")]
        public async Task GetByChatNameSubstringAsync_NoChats_WhenNameHasNoSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            var chat = new Chat
            {
                Name = "Substring",
                ImageId = Guid.NewGuid(),
            };
            elementsToDelete.Add(chat);
            await chatStorage.AddAsync(chat);

            var actual = await chatStorage.GetByChatNameSubstringAsync(substring);

            actual.Should().BeEmpty();
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenNoSameNames()
        {
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            var expected = new List<Chat>();
            var chat = new Chat
            {
                Name = "aaaaa",
                ImageId = Guid.NewGuid(),
            };
            var chat2 = new Chat
            {
                Name = "bbbbb",
                ImageId = Guid.NewGuid(),
            };
            var chat3 = new Chat
            {
                Name = "cccc",
                ImageId = Guid.NewGuid(),
            };
            expected.Add(chat);
            expected.Add(chat2);
            expected.Add(chat3);
            elementsToDelete.AddRange(expected);
            await chatStorage.AddAsync(chat);
            await chatStorage.AddAsync(chat2);
            await chatStorage.AddAsync(chat3);

            var actual = await chatStorage.GetAllAsync();

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id).WithStrictOrdering());
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenSameNames()
        {
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            var expected = new List<Chat>();
            var chat = new Chat
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "aaaaa",
                ImageId = Guid.NewGuid(),
            };
            var chat2 = new Chat
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "aaaaa",
                ImageId = Guid.NewGuid(),
            };
            var chat3 = new Chat
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "aaaaa",
                ImageId = Guid.NewGuid(),
            };
            expected.Add(chat3);
            expected.Add(chat);
            expected.Add(chat2);
            elementsToDelete.AddRange(expected);
            await chatStorage.AddAsync(chat);
            await chatStorage.AddAsync(chat2);
            await chatStorage.AddAsync(chat3);

            var actual = await chatStorage.GetAllAsync();

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [TearDown]
        public async Task TearDown()
        {
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            foreach (var elem in elementsToDelete)
                await chatStorage.DeleteAsync(elem.Id);
        }
    }
}