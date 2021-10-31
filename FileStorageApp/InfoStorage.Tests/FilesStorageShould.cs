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
        private readonly List<Chat> chatsToDelete = new();
        private readonly List<FileSender> fileSendersToDelete = new();

        public FilesStorageShould()
        {
            var config = new DataBaseConfig();
            config.SetConnectionString(Settings.SetupString);
            infoStorageFactory = new InfoStorageFactory(config);
        }


        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetByFileNameSubstringAsync_ReturnCorrectFiles_WhenNameHasSubstring(string substring)
        {
            using var chatStorage = infoStorageFactory.CreateFileStorage();
            var expected = new List<File>();
            var senderId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            var file = new File
            {
                Name = "Substring",
                Extension = "xlsx",
                Type = "file",
                UploadDate = DateTime.Now,
                FileSenderId = senderId,
                ChatId = chatId,
                Chat = CreateChat(chatId),
                FileSender = CreateFileSender(senderId)
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
            var senderId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            var file = new File
            {
                Name = "Substring",
                Extension = "xlsx",
                Type = "file",
                UploadDate = DateTime.Now,
                FileSenderId = senderId,
                ChatId = chatId,
                Chat = CreateChat(chatId),
                FileSender = CreateFileSender(senderId)
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
            var senderIds = CreateGuids(3);
            var chatIds = CreateGuids(3);
            var dateTime = DateTime.Now;
            var file = CreateFile(dateTime, senderIds[0], chatIds[0], Guid.NewGuid());
            var file2 = CreateFile(dateTime.AddHours(1), senderIds[1], chatIds[1], Guid.NewGuid());
            var file3 = CreateFile(dateTime.AddHours(-1), senderIds[2], chatIds[2], Guid.NewGuid());
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
            var senderIds = CreateGuids(3);
            var chatIds = CreateGuids(3);
            var dateTime = DateTime.Now;
            var file = CreateFile(dateTime, senderIds[0], chatIds[0],
                Guid.Parse("00000000-0000-0000-0000-000000000002"));
            var file2 = CreateFile(dateTime, senderIds[1], chatIds[1],
                Guid.Parse("00000000-0000-0000-0000-000000000003"));
            var file3 = CreateFile(dateTime, senderIds[2], chatIds[2],
                Guid.Parse("00000000-0000-0000-0000-000000000001"));
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
            foreach (var elem in chatsToDelete)
                await chatStorage.DeleteAsync(elem.Id);
            foreach (var elem in fileSendersToDelete)
                await fileSenderStorage.DeleteAsync(elem.Id);
            filesToDelete.Clear();
            chatsToDelete.Clear();
            fileSendersToDelete.Clear();
        }

        private Chat CreateChat(Guid chatId)
        {
            var chat = new Chat
            {
                Name = "",
                ImageId = Guid.NewGuid(),
                Id = chatId
            };
            chatsToDelete.Add(chat);
            return chat;
        }

        private FileSender CreateFileSender(Guid senderId)
        {
            var fileSender = new FileSender
            {
                TelegramUserName = "",
                FullName = "",
                Id = senderId
            };
            fileSendersToDelete.Add(fileSender);
            return fileSender;
        }

        private File CreateFile(DateTime dateTime, Guid senderId, Guid chatId, Guid id)
        {
            var file = new File
            {
                Id = id,
                Name = "Substring",
                Extension = "xlsx",
                Type = "file",
                UploadDate = dateTime,
                FileSenderId = senderId,
                ChatId = chatId,
                Chat = CreateChat(chatId),
                FileSender = CreateFileSender(senderId)
            };
            filesToDelete.Add(file);
            return file;
        }

        private static List<Guid> CreateGuids(int count)
        {
            var result = new List<Guid>();
            for (var i = 0; i < count; i++)
                result.Add(Guid.NewGuid());
            return result;
        }
    }
}