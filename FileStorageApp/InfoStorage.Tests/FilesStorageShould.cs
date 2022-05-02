using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Contracts;
using FileStorageApp.Data.InfoStorage.Enums;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;

namespace InfoStorage.Tests
{
    public class FilesStorageShould
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private Chat _chat;
        private FileSender _fileSender;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public FilesStorageShould()
        {
            var config = new DataBaseConfig($"Server={Config["DbHost"]};" +
                                            $"Username={Config["DbUser"]};" +
                                            $"Database={Config["DbName"]};" +
                                            $"Port={Config["DbPort"]};" +
                                            $"Password={Config["DbPassword"]};" +
                                            "SSLMode=Prefer");
            _infoStorageFactory = new InfoStorageFactory(config);
        }

        [SetUp]
        public async Task SetUp()
        {
            _chat = CreateChat(Guid.NewGuid());
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            await chatStorage.AddAsync(_chat);

            _fileSender = CreateFileSender(Guid.NewGuid());
            using var fileSenderStorage = _infoStorageFactory.CreateFileSenderStorage();
            await fileSenderStorage.AddAsync(_fileSender);
        }


        [TestCase("ubs")]
        [TestCase("Substring")]
        [TestCase("")]
        public async Task GetByFileNameSubstringAsync_ReturnCorrectFiles_WhenNameHasSubstring(string substring)
        {
            using var chatStorage = _infoStorageFactory.CreateFileStorage();
            var expected = new List<File>();
            var file = new File
            {
                Name = "Substring",
                Extension = "xlsx",
                Type = FileType.Document,
                UploadDate = DateTime.Now,
                FileSenderId = _fileSender.Id,
                ChatId = _chat.Id,
            };
            expected.Add(file);
            await chatStorage.AddAsync(file);

            var actual = await chatStorage.GetByFileNameSubstringAsync(substring);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestCase("aboba")]
        [TestCase("bus")]
        public async Task GetByFileNameSubstringAsync_NoFiles_WhenNameHasNoSubstring(string substring)
        {
            using var chatStorage = _infoStorageFactory.CreateFileStorage();
            var file = new File
            {
                Name = "Substring",
                Extension = "xlsx",
                Type = FileType.Document,
                UploadDate = DateTime.Now,
                FileSenderId = _fileSender.Id,
                ChatId = _chat.Id,
            };
            await chatStorage.AddAsync(file);

            var actual = await chatStorage.GetByFileNameSubstringAsync(substring);

            actual.Should().BeEmpty();
        }

        [Test]
        public async Task GetAll_CorrectSort_WhenNoSameDates()
        {
            using var chatStorage = _infoStorageFactory.CreateFileStorage();
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
            using var chatStorage = _infoStorageFactory.CreateFileStorage();
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

        [Test]
        public async Task GetByFilePropertiesAsync_CorrectResult_WhenTakeByExpression()
        {
            using var chatStorage = _infoStorageFactory.CreateFileStorage();
            var dateTime = DateTime.Now;
            var file = CreateFile(dateTime, Guid.NewGuid());
            var file2 = CreateFile(dateTime, Guid.NewGuid());
            var file3 = CreateFile(dateTime.AddHours(1), Guid.NewGuid());
            file.Extension = "docx";
            await chatStorage.AddAsync(file);
            await chatStorage.AddAsync(file2);
            await chatStorage.AddAsync(file3);
            Expression<Func<File, bool>> selector = (f) => f.Extension == "xlsx";
            var expected = new List<File> {file3, file2};

            var actual = await chatStorage.GetByFilePropertiesAsync(selector);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
        
        [Test]
        public async Task AddClassificationAsync_Added()
        {
            var dateTime = DateTime.Now;
            var file = CreateFile(dateTime, Guid.NewGuid());
         
            using var storage = _infoStorageFactory.CreateFileStorage();
            await storage.AddAsync(file);

            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, "test");

            var added = await storage.AddClassificationAsync(file.Id, classificationId);
            added.Should().BeTrue();

            using var secondStorage = _infoStorageFactory.CreateFileStorage();
            var actualFile = await secondStorage.GetByIdAsync(file.Id, true);

            actualFile.Should().NotBeNull();
            actualFile.Classification.Should().NotBeNull();
        }
        
        [Test]
        public async Task DeleteClassificationAsync_Added()
        {
            var dateTime = DateTime.Now;
            var file = CreateFile(dateTime, Guid.NewGuid());
         
            using var storage = _infoStorageFactory.CreateFileStorage();
            await storage.AddAsync(file);

            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, "test");

            var added = await storage.AddClassificationAsync(file.Id, classificationId);
            added.Should().BeTrue();

            using var secondStorage = _infoStorageFactory.CreateFileStorage();
            var deleted = await secondStorage.DeleteClassificationAsync(file.Id, classificationId);
            deleted.Should().BeTrue();
            
            using var thirdStorage = _infoStorageFactory.CreateFileStorage();
            var actualFile = await thirdStorage.GetByIdAsync(file.Id, true);

            actualFile.Should().NotBeNull();
            actualFile.Classification.Should().BeNull();
        }

        [TearDown]
        public async Task TearDown()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            using var fileSenderStorage = _infoStorageFactory.CreateFileSenderStorage();

            foreach (var elem in await fileStorage.GetAllAsync())
                await fileStorage.DeleteAsync(elem.Id);
            await chatStorage.DeleteAsync(_chat.Id);
            await fileSenderStorage.DeleteAsync(_fileSender.Id);
            
            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();

            foreach (var element in await storage.GetAllAsync())
                await storage.DeleteAsync(element.Id);
        }

        private Chat CreateChat(Guid chatId)
        {
            return new Chat
            {
                TelegramId = 0,
                Name = "",
                ImageId = Guid.NewGuid(),
                Id = chatId
            };
        }

        private FileSender CreateFileSender(Guid senderId)
        {
            return new FileSender
            {
                TelegramId = 0,
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
                Type = FileType.TextDocument,
                UploadDate = dateTime,
                FileSenderId = _fileSender.Id,
                ChatId = _chat.Id,
            };
            return file;
        }
        
        private async Task AddClassification(Guid classificationId, string classificationName)
        {
            var classification = new Classification()
            {
                Id = classificationId,
                Name = classificationName
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);
        }
    }
}