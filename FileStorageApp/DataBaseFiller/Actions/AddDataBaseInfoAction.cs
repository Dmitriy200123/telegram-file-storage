using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.S3;
using FilesStorage;
using FilesStorage.Interfaces;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;
using Microsoft.Extensions.Configuration;
using File = FileStorageApp.Data.InfoStorage.Models.File;
using IFilesStorage = FileStorageApp.Data.InfoStorage.Storages.Files.IFilesStorage;

namespace DataBaseFiller.Actions
{
    public class AddDataBaseInfoAction : IAction
    {
        private Guid _fileSenderId;
        private Guid _chatId;

        private readonly IFilesStorageFactory _filesStorageFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public AddDataBaseInfoAction()
        {
            var config = new AmazonS3Config
            {
                ServiceURL = Config["S3serviceUrl"],
                ForcePathStyle = true
            };
            var amazonConfig = new S3FilesStorageOptions(Config["S3accessKey"], Config["S3secretKey"],
                Config["S3bucketName"], config, S3CannedACL.PublicReadWrite,
                TimeSpan.FromHours(1));
            _filesStorageFactory = new S3FilesStorageFactory(amazonConfig);
        }

        public void DoAction(IInfoStorageFactory infoStorageFactory)
        {
            using var filesStorage = infoStorageFactory.CreateFileStorage();
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            using var senderStorage = infoStorageFactory.CreateFileSenderStorage();
            FillSenders(senderStorage);
            FillChats(chatStorage);
            FillFiles(filesStorage);
            Console.WriteLine("Done!");
        }

        private void FillSenders(IFileSenderStorage fileSenderStorage)
        {
            _fileSenderId = Guid.NewGuid();
            var fileSender = new FileSender
            {
                Id = _fileSenderId,
                TelegramId = 100,
                TelegramUserName = "@testUser",
                FullName = "Тестовый пользователь",
            };
            var result = fileSenderStorage.AddAsync(fileSender).Result;
            if (!result)
                throw new Exception("Can't add file sender");
        }

        private void FillChats(IChatStorage chatStorage)
        {
            _chatId = Guid.NewGuid();
            var chat = new Chat
            {
                Id = _chatId,
                TelegramId = 1022,
                Name = "Тестовый чат",
                ImageId = Guid.NewGuid(),
            };
            var result = chatStorage.AddAsync(chat).Result;
            if (!result)
                throw new Exception("Can't add chat");
        }

        private void FillFiles(IFilesStorage filesStorage)
        {
            var fileNames = new List<string>
            {
                "audio.mp3",
                "document.txt",
                "image.png",
                "video.mp4"
            };
            foreach (var name in fileNames)
            {
                var uploadedFileId = UploadFile(name);
                var file = new File
                {
                    Id = uploadedFileId,
                    Name = name,
                    Extension = name.Split(".").Last(),
                    TypeId = (int) FileTypeProvider.GetFileType(name),
                    UploadDate = DateTime.Now,
                    FileSenderId = _fileSenderId,
                    ChatId = _chatId
                };
                var result = filesStorage.AddAsync(file).Result;
                if (!result)
                    throw new Exception("Can't add file");
            }
        }

        private Guid UploadFile(string fileName)
        {
            var guid = Guid.NewGuid();
            using var fileStream = System.IO.File.OpenRead($"{Directory.GetCurrentDirectory()}/Files/{fileName}");
            using var physicalStorage = _filesStorageFactory.CreateAsync().Result;
            physicalStorage.SaveFileAsync(guid.ToString(), fileStream);
            return guid;
        }
    }
}