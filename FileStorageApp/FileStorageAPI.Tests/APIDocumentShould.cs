using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon.S3;
using FilesStorage;
using FilesStorage.Interfaces;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using FileInfo = FileStorageAPI.Models.FileInfo;

namespace FileStorageAPI.Tests
{
    public class APIDocumentShould : BaseShould
    {
        private readonly IFilesStorageFactory _filesStorageFactory;
        private const string FileSenderId = "00000000-0000-0000-0000-000000000001";

        private readonly IsoDateTimeConverter _dateTimeConverter = new() {DateTimeFormat = "dd.MM.yyyy HH:mm:ss"};
        
        public APIDocumentShould()
        {
            var config = new AmazonS3Config
            {
                ServiceURL = Config["S3serviceUrl"],
                ForcePathStyle = true,
            };
            var s3Config = new S3FilesStorageOptions(Config["S3accessKey"], Config["S3secretKey"],
                Config["S3bucketName"], config, S3CannedACL.PublicReadWrite,
                TimeSpan.FromHours(1), Config["S3host"],Config["S3hostReal"]);
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

            using var fileInfoStorage = _infoStorageFactory.CreateFileStorage();
            var fileInfos = await chatStorage.GetAllAsync();
            foreach (var fileInfo in fileInfos)
                await fileInfoStorage.DeleteAsync(fileInfo.Id);

            using var filesStorage = await _filesStorageFactory.CreateAsync();
            foreach (var file in await filesStorage.GetFilesAsync())
                await filesStorage.DeleteFileAsync(file.Key);
        }
        
        [Test]
        public async Task GetDocumentsInfos_ReturnCorrectDocumentInfo_ThenCalled()
        {
            using var apiClient = CreateHttpClient();
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            await UploadFile();
            var response = await apiClient.GetAsync($"/api/files/documents?skip=0&take=2&phrase=good");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<List<FileInfo>>(responseString,_dateTimeConverter);
            actual.Should().NotBeEmpty();
        }
        
        [Test]
        public async Task GetDocumentsCount_ReturnCorrectCount_ThenCalled()
        {
            using var apiClient = CreateHttpClient();
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            await UploadFile();
            var response = await apiClient.GetAsync($"/api/files/documents/count?skip=0&take=2&phrase=good");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<int>(responseString,_dateTimeConverter);
            actual.Should().Be(1);
        }
        
       
        public async Task<FileInfo?> UploadFile()
        {
            using var apiClient = CreateHttpClient();
            using var fileStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var form = CreateMultipartFormDataContent("text_document.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            var sender = new FileSender
            {
                Id = Guid.Parse(FileSenderId),
                TelegramId = -1,
                TelegramUserName = "Загрузчик с сайта",
                FullName = "Загрузчик с сайта",
            };
            await senderStorage.AddAsync(sender);

            var response = await apiClient.PostAsync("/api/files/", form);
            var responseString = await response.Content.ReadAsStringAsync();

            var actual = JsonConvert.DeserializeObject<FileInfo>(responseString,_dateTimeConverter);
            return actual;
        }

        private static MultipartFormDataContent CreateMultipartFormDataContent(string fileName, string contentType)
        {
            var stream = System.IO.File.OpenRead($"{Directory.GetCurrentDirectory()}/FilesForTests/{fileName}");
            var form = new MultipartFormDataContent {{new StreamContent(stream), "file", fileName}};
            form.First().Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return form;
        }
    }
}