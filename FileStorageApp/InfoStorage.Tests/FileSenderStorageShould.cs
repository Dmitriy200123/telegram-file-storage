using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;

namespace InfoStorage.Tests
{
    public class FileSenderStorageShould
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public FileSenderStorageShould()
        {
            var config = new DataBaseConfig($"Server={Config["DbHost"]};" +
                                            $"Username={Config["DbUser"]};" +
                                            $"Database={Config["DbName"]};" +
                                            $"Port={Config["DbPort"]};" +
                                            $"Password={Config["DbPassword"]};" +
                                            "SSLMode=Prefer");
            _infoStorageFactory = new InfoStorageFactory(config);
        }

        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetBySenderNameSubstringAsync_ReturnCorrectFileSenders_WhenNameHasSubstring(string substring)
        {
            using var chatStorage = _infoStorageFactory.CreateFileSenderStorage();
            var expected = new List<FileSender>();
            var fileSender = new FileSender
            {
                TelegramId = 0,
                TelegramUserName = "",
                FullName = "Substring",
            };
            expected.Add(fileSender);
            await chatStorage.AddAsync(fileSender);

            var actual = await chatStorage.GetBySenderNameSubstringAsync(substring);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestCase("aboba")]
        [TestCase("bus")]
        public async Task GetBySenderNameSubstringAsync_NoFileSenders_WhenNameHasNoSubstring(string substring)
        {
            using var chatStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileSender = new FileSender
            {
                TelegramId = 0,
                TelegramUserName = "",
                FullName = "Substring",
            };
            await chatStorage.AddAsync(fileSender);

            var actual = await chatStorage.GetBySenderNameSubstringAsync(substring);

            actual.Should().BeEmpty();
        }

        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetByTelegramNameSubstringAsync_ReturnCorrectFileSenders_WhenNameHasSubstring(
            string substring)
        {
            using var chatStorage = _infoStorageFactory.CreateFileSenderStorage();
            var expected = new List<FileSender>();
            var fileSender = new FileSender
            {
                TelegramId = 0,
                TelegramUserName = "Substring",
                FullName = "",
            };
            expected.Add(fileSender);
            await chatStorage.AddAsync(fileSender);

            var actual = await chatStorage.GetByTelegramNameSubstringAsync(substring);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestCase("aboba")]
        [TestCase("bus")]
        public async Task GetByTelegramNameSubstringAsync_NoFileSenders_WhenNameHasNoSubstring(string substring)
        {
            using var chatStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileSender = new FileSender
            {
                TelegramId = 0,
                TelegramUserName = "Substring",
                FullName = "",
            };
            await chatStorage.AddAsync(fileSender);

            var actual = await chatStorage.GetByTelegramNameSubstringAsync(substring);

            actual.Should().BeEmpty();
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenNoSameNames()
        {
            using var chatStorage = _infoStorageFactory.CreateFileSenderStorage();
            var expected = new List<FileSender>();
            var fileSender = new FileSender
            {
                TelegramId = 0,
                TelegramUserName = "",
                FullName = "bbbb",
            };
            var fileSender2 = new FileSender
            {
                TelegramId = 1,
                TelegramUserName = "",
                FullName = "cccc",
            };
            var fileSender3 = new FileSender
            {
                TelegramId = 2,
                TelegramUserName = "",
                FullName = "aaaaa",
            };
            expected.Add(fileSender3);
            expected.Add(fileSender);
            expected.Add(fileSender2);
            await chatStorage.AddAsync(fileSender);
            await chatStorage.AddAsync(fileSender2);
            await chatStorage.AddAsync(fileSender3);

            var actual = await chatStorage.GetAllAsync();

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id).WithStrictOrdering());
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenSameNames()
        {
            using var chatStorage = _infoStorageFactory.CreateFileSenderStorage();
            var expected = new List<FileSender>();
            var fileSender = new FileSender
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                TelegramId = 0,
                TelegramUserName = "",
                FullName = "Substring",
            };
            var fileSender2 = new FileSender
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                TelegramId = 1,
                TelegramUserName = "",
                FullName = "Substring",
            };
            var fileSender3 = new FileSender
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                TelegramId = 2,
                TelegramUserName = "",
                FullName = "Substring",
            };
            expected.Add(fileSender3);
            expected.Add(fileSender);
            expected.Add(fileSender2);
            await chatStorage.AddAsync(fileSender);
            await chatStorage.AddAsync(fileSender2);
            await chatStorage.AddAsync(fileSender3);

            var actual = await chatStorage.GetAllAsync();

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [TearDown]
        public async Task TearDown()
        {
            using var fileSenderStorage = _infoStorageFactory.CreateFileSenderStorage();
            foreach (var elem in await fileSenderStorage.GetAllAsync())
                await fileSenderStorage.DeleteAsync(elem.Id);
        }
    }
}