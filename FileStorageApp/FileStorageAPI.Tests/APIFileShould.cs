using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Amazon.S3;
using FilesStorage;
using FilesStorage.Interfaces;
using FileStorageAPI.Converters;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using Chat = FileStorageAPI.Models.Chat;
using File = FileStorageApp.Data.InfoStorage.Models.File;

namespace FileStorageAPI.Tests
{
    public class APIFileShould
    {
        private readonly HttpClient _apiClient;
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IFilesStorageFactory _filesStorageFactory;
        private const string FileSenderId = "00000000-0000-0000-0000-000000000001";

        private static readonly ISenderConverter SenderConverter = new SenderConverter();
        private static readonly IChatConverter ChatConverter = new ChatConverter();

        private static readonly IFileInfoConverter FilesConverter =
            new FileInfoConverter(ChatConverter, SenderConverter);

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public APIFileShould()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseConfiguration(Config)
                .UseEnvironment("Development")
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
            var config = new AmazonS3Config
            {
                ServiceURL = Config["S3serviceUrl"],
                ForcePathStyle = true
            };
            var s3Config = new S3FilesStorageOptions(Config["S3accessKey"], Config["S3secretKey"],
                Config["S3bucketName"], config, S3CannedACL.PublicReadWrite,
                TimeSpan.FromHours(1));
            _filesStorageFactory = new S3FilesStorageFactory(s3Config);
        }

        [TearDown]
        public async Task TearDown()
        {
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var senders = await senderStorage.GetAllAsync();
            foreach (var sender in senders)
                await senderStorage.DeleteAsync(sender.Id);

            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chats = await chatStorage.GetAllAsync();
            foreach (var chat in chats)
                await chatStorage.DeleteAsync(chat.Id);

            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            var files = await chatStorage.GetAllAsync();
            foreach (var file in files)
                await fileStorage.DeleteAsync(file.Id);
        }

        [Test]
        public async Task GetFileInfoById_ReturnCorrectFileInfo_ThenCalled()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = Guid.NewGuid();
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);
            fileInDb.FileSender = senderInDb;

            var response = await _apiClient.GetAsync($"/api/files/{fileGuid}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Models.FileInfo>(responseString);
            actual.Should().BeEquivalentTo(FilesConverter.ConvertFileInfo(fileInDb));
        }

        [Test]
        public async Task GetFileInfoById_ReturnNotFound_ThenDifferentId()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = Guid.NewGuid();
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);

            var response = await _apiClient.GetAsync($"/api/files/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetFilesInfos_ReturnCorrectFileInfo_ThenCalled()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = Guid.NewGuid();
            var fileInDb = CreateFile(fileGuid);
            var fileGuid2 = Guid.NewGuid();
            var fileInDb2 = CreateFile(fileGuid2);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);
            await fileStorage.AddAsync(fileInDb2);
            fileInDb.FileSender = senderInDb;
            fileInDb2.FileSender = senderInDb;
            var expected = new[] {fileInDb, fileInDb2}.Select(x => FilesConverter.ConvertFileInfo(x)).ToList();

            var response = await _apiClient.GetAsync($"/api/files?skip=0&take=2");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<Models.FileInfo>>(responseString);
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase(-1, -1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        public async Task GetFilesInfos_ReturnBadRequest_ThenInvalidArguments(int take, int skip)
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = Guid.NewGuid();
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);

            var response = await _apiClient.GetAsync($"/api/files?skip={skip}&take={take}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetFileDownloadLink_ReturnDownloadLink_ThenCalled()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = await UploadFile("document.txt");
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);

            var response = await _apiClient.GetAsync($"/api/files/{fileGuid}/downloadlink");
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetFileDownloadLink_ReturnNotFound_ThenCalledWithOtherId()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = await UploadFile("document.txt");
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);
            var anotherGuid = Guid.NewGuid();

            var response = await _apiClient.GetAsync($"/api/files/{anotherGuid}/downloadlink");
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseString.Should().Be($"\"File with identifier {anotherGuid} not found\"");
        }

        [TestCase("document.txt", "text/plain")]
        [TestCase("audio.mp3", "audio/mpeg")]
        [TestCase("image.png", "image/png")]
        [TestCase("video.mp4", "video/mp4")]
        public async Task PostFile_ReturnFileInfo_ThenCalled(string fileName, string contentType)
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var form = CreateMultipartFormDataContent(fileName, contentType);
            var chat = new FileStorageApp.Data.InfoStorage.Models.Chat
            {
                Id = Guid.Empty,
                Name = "Ручная загрузка файла",
                ImageId = Guid.Empty
            };
            var sender = new FileSender
            {
                Id = Guid.Parse(FileSenderId),
                TelegramId = -1,
                TelegramUserName = "Загрузчик с сайта",
                FullName = "Загрузчик с сайта",
            };

            var response = await _apiClient.PostAsync("/api/files/", form);
            var fileInfo = fileStorage.GetAllAsync().Result.First();
            fileInfo.Chat = chat;
            fileInfo.FileSender = sender;
            var expected = FilesConverter.ConvertFileInfo(fileInfo);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Models.FileInfo>(responseString);
            actual.Should().BeEquivalentTo(expected, o => o.Excluding(x => x.UploadDate));
        }

        [Test]
        public async Task PutFile_ReturnCorrectFileInfo_ThenCalled()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = await UploadFile("document.txt");
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);
            fileInDb.FileSender = senderInDb;
            var httpContent = JsonContent.Create(new {FileName = "aboba"});

            var response = await _apiClient.PutAsync($"/api/files/{fileGuid}", httpContent);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Models.FileInfo>(responseString);
            fileInDb.Name = "aboba";
            actual.Should().BeEquivalentTo(FilesConverter.ConvertFileInfo(fileInDb));
        }

        [Test]
        public async Task PutFile_ReturnNotFound_ThenDifferentId()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = Guid.NewGuid();
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);
            var httpContent = JsonContent.Create(new {FileName = "aboba"});

            var response = await _apiClient.PutAsync($"/api/files/{Guid.NewGuid()}", httpContent);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task DeleteFileInfoById_ReturnCorrectFileInfo_ThenCalled()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = await UploadFile("document.txt");
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);
            fileInDb.FileSender = senderInDb;

            var response = await _apiClient.DeleteAsync($"/api/files/{fileGuid}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task DeleteFileInfoById_ReturnNotFound_ThenDifferentId()
        {
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileGuid = await UploadFile("document.txt");
            var fileInDb = CreateFile(fileGuid);
            var senderInDb = CreateFileSender();
            await senderStorage.AddAsync(senderInDb);
            await fileStorage.AddAsync(fileInDb);

            var response = await _apiClient.DeleteAsync($"/api/files/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        private async Task<Guid> UploadFile(string fileName)
        {
            var guid = Guid.NewGuid();
            await using var fileStream =
                System.IO.File.OpenRead($"{Directory.GetCurrentDirectory()}/FilesForTests/{fileName}");
            using var physicalStorage = await _filesStorageFactory.CreateAsync();
            await physicalStorage.SaveFileAsync(guid.ToString(), fileStream);
            return await Task.FromResult(guid);
        }

        private static MultipartFormDataContent CreateMultipartFormDataContent(string fileName, string contentType)
        {
            var stream = System.IO.File.OpenRead($"{Directory.GetCurrentDirectory()}/FilesForTests/{fileName}");
            var form = new MultipartFormDataContent();
            form.Add(new StreamContent(stream), "file", fileName);
            form.First().Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return form;
        }

        private File CreateFile(Guid fileGuid)
        {
            return new File
            {
                Id = fileGuid,
                Name = "test",
                Extension = "test",
                TypeId = 0,
                UploadDate = new DateTime(2021, 11, 20, 19, 16, 11),
                FileSenderId = Guid.Parse(FileSenderId),
                ChatId = null
            };
        }

        private FileSender CreateFileSender()
        {
            return new FileSender
            {
                Id = Guid.Parse(FileSenderId),
                TelegramId = 0,
                TelegramUserName = "Test",
                FullName = "Test",
            };
        }

        private Chat CreateChat()
        {
            return new Chat
            {
                Id = Guid.Empty,
                Name = "Ручная загрузка файла",
                ImageId = Guid.Empty
            };
        }
    }
}