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
    public class FileSenderStorageShould
    {
        private readonly List<FileSender> elementsToDelete = new();
        private readonly IInfoStorageFactory infoStorageFactory;

        public FileSenderStorageShould()
        {
            var config = new DataBaseConfig();
            config.SetConnectionString(
                "");
            infoStorageFactory = new InfoStorageFactory(config);
        }


        [SetUp]
        public void Setup()
        {
        }
        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetBySenderNameSubstringAsync_ReturnCorrectChat_WhenNameHasSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateFileSenderStorage();
            var expected = new List<FileSender>();
            var chat = new FileSender
            {
                TelegramUserName = "",
                FullName = "Substring",
            };
            expected.Add(chat);
            elementsToDelete.Add(chat);
            await chatStorage.AddAsync(chat);

            var actual = await chatStorage.GetBySenderNameSubstringAsync(substring);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestCase("aboba")]
        [TestCase("bus")]
        public async Task GetBySenderNameSubstringAsync_NoChats_WhenNameHasNoSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateFileSenderStorage();
            var chat = new FileSender
            {
                TelegramUserName = "",
                FullName = "Substring",
            };
            elementsToDelete.Add(chat);
            await chatStorage.AddAsync(chat);

            var actual = await chatStorage.GetBySenderNameSubstringAsync(substring);

            actual.Should().BeEmpty();
        }
        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetByTelegramNameSubstringAsync_ReturnCorrectChat_WhenNameHasSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateFileSenderStorage();
            var expected = new List<FileSender>();
            var chat = new FileSender
            {
                TelegramUserName = "Substring",
                FullName = "",
            };
            expected.Add(chat);
            elementsToDelete.Add(chat);
            await chatStorage.AddAsync(chat);

            var actual = await chatStorage.GetByTelegramNameSubstringAsync(substring);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestCase("aboba")]
        [TestCase("bus")]
        public async Task GetByTelegramNameSubstringAsync_NoChats_WhenNameHasNoSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateFileSenderStorage();
            var chat = new FileSender
            {
                TelegramUserName = "Substring",
                FullName = "",
            };
            elementsToDelete.Add(chat);
            await chatStorage.AddAsync(chat);

            var actual = await chatStorage.GetByTelegramNameSubstringAsync(substring);

            actual.Should().BeEmpty();
        }
        [Test]
        public async Task GetAll_CorrectSort_WhenNoSameNames()
        {
            using var chatStorage = infoStorageFactory.CreateFileSenderStorage();
            var expected = new List<FileSender>();
            var chat = new FileSender
            {
                TelegramUserName = "",
                FullName = "bbbb",
            };
            var chat2 = new FileSender
            {
                TelegramUserName = "",
                FullName = "cccc",
            };
            var chat3 = new FileSender
            {
                TelegramUserName = "",
                FullName = "aaaaa",
            };
            expected.Add(chat3);
            expected.Add(chat);
            expected.Add(chat2);
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
            using var chatStorage = infoStorageFactory.CreateFileSenderStorage();
            var expected = new List<FileSender>();
            var chat = new FileSender
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                TelegramUserName = "",
                FullName = "Substring",
            };
            var chat2 = new FileSender
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                TelegramUserName = "",
                FullName = "Substring",
            };
            var chat3 = new FileSender
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                TelegramUserName = "",
                FullName = "Substring",
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
            using var fileSenderStorage = infoStorageFactory.CreateFileSenderStorage();
            foreach (var elem in elementsToDelete)
                await fileSenderStorage.DeleteAsync(elem.Id);
            elementsToDelete.Clear();
        }
    }
}