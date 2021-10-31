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
    public class FilesStorageShould
    {
        private readonly List<File> filesToDelete = new();
        private readonly IInfoStorageFactory infoStorageFactory;
        private Chat chat;
        private FileSender fileSender;

        public FilesStorageShould()
        {
            var config = new DataBaseConfig();
            config.SetConnectionString(Settings.SetupString);
            infoStorageFactory = new InfoStorageFactory(config);
        }

        [SetUp]
        public async Task SetUp()
        {
            chat = CreateChat(Guid.NewGuid());
            await infoStorageFactory.CreateChatStorage().AddAsync(chat);

            fileSender = CreateFileSender(Guid.NewGuid());
            await infoStorageFactory.CreateFileSenderStorage().AddAsync(fileSender);
        }


        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetByFileNameSubstringAsync_ReturnCorrectFiles_WhenNameHasSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateFileStorage();
            var expected = new List<File>();
            var file = new File
            {
                Name = "Substring",
                Extension = "xlsx",
                Type = "file",
                UploadDate = DateTime.Now,
                FileSenderId = fileSender.Id,
                ChatId = chat.Id,
            };
            expected.Add(file);
            filesToDelete.Add(file);
            await chatStorage.AddAsync(file);

            var actual = await chatStorage.GetByFileNameSubstringAsync(substring);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestCase("aboba")]
        [TestCase("bus")]
        public async Task GetByFileNameSubstringAsync_NoFiles_WhenNameHasNoSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateFileStorage();
            var file = new File
            {
                Name = "Substring",
                Extension = "xlsx",
                Type = "file",
                UploadDate = DateTime.Now,
                FileSenderId = fileSender.Id,
                ChatId = chat.Id,
            };
            filesToDelete.Add(file);
            await chatStorage.AddAsync(file);

            var actual = await chatStorage.GetByFileNameSubstringAsync(substring);

            actual.Should().BeEmpty();
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenNoSameDates()
        {
            using var chatStorage = infoStorageFactory.CreateFileStorage();
            var expected = new List<File>();
            var dateTime = DateTime.Now;
            var file = CreateFile(dateTime, Guid.NewGuid());
            var file2 = CreateFile(dateTime.AddHours(1), Guid.NewGuid());
            var file3 = CreateFile(dateTime.AddHours(-1), Guid.NewGuid());
            expected.Add(file2);
            expected.Add(file);
            expected.Add(file3);
            await chatStorage.AddAsync(file);
            await chatStorage.AddAsync(file2);
            await chatStorage.AddAsync(file3);

            var actual = await chatStorage.GetAllAsync();

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id).WithStrictOrdering());
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenSameDates()
        {
            using var chatStorage = infoStorageFactory.CreateFileStorage();
            var expected = new List<File>();
            var dateTime = DateTime.Now;
            var file = CreateFile(dateTime, Guid.Parse("00000000-0000-0000-0000-000000000002"));
            var file2 = CreateFile(dateTime, Guid.Parse("00000000-0000-0000-0000-000000000003"));
            var file3 = CreateFile(dateTime, Guid.Parse("00000000-0000-0000-0000-000000000001"));
            expected.Add(file3);
            expected.Add(file);
            expected.Add(file2);
            await chatStorage.AddAsync(file);
            await chatStorage.AddAsync(file2);
            await chatStorage.AddAsync(file3);

            var actual = await chatStorage.GetAllAsync();

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [TearDown]
        public async Task TearDown()
        {
            using var fileStorage = infoStorageFactory.CreateFileStorage();
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            using var fileSenderStorage = infoStorageFactory.CreateFileSenderStorage();

            foreach (var elem in filesToDelete)
                await fileStorage.DeleteAsync(elem.Id);
            await chatStorage.DeleteAsync(chat.Id);
            await fileSenderStorage.DeleteAsync(fileSender.Id);

            chat = null;
            fileSender = null;
            filesToDelete.Clear();
        }

        private Chat CreateChat(Guid chatId)
        {
            return new Chat
            {
                Name = "",
                ImageId = Guid.NewGuid(),
                Id = chatId
            };
        }

        private FileSender CreateFileSender(Guid senderId)
        {
            return new FileSender
            {
                TelegramUserName = "",
                FullName = "",
                Id = senderId
            };
        }

        private File CreateFile(DateTime dateTime, Guid id)
        {
            var file = new File
            {
                Id = id,
                Name = "Substring",
                Extension = "xlsx",
                Type = "file",
                UploadDate = dateTime,
                FileSenderId = fileSender.Id,
                ChatId = chat.Id,
            };
            filesToDelete.Add(file);
            return file;
        }
    }
}