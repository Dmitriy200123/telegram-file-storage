using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilesStorage.Interfaces;
using FileStorageAPI.Converters;
using FileStorageAPI.Providers;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;
using Microsoft.AspNetCore.Http;
using IFilesStorage = FileStorageApp.Data.InfoStorage.Storages.Files.IFilesStorage;
using DataBaseFile = FileStorageApp.Data.InfoStorage.Models.File;
using File = FileStorageAPI.Models.File;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class FileService : IFileService
    {
        private readonly IFilesStorage _filesStorage;
        private readonly IFileConverter _fileConverter;
        private readonly IFileSenderStorage _fileSenderStorage;
        private readonly IChatStorage _chatStorage;
        private readonly IFileTypeProvider _fileTypeProvider;
        private readonly FilesStorage.Interfaces.IFilesStorage _physicalFilesStorage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoStorageFactory"></param>
        /// <param name="fileConverter"></param>
        /// <param name="filesStorageFactory"></param>
        /// <param name="fileTypeProvider"></param>
        public FileService(IInfoStorageFactory infoStorageFactory, IFileConverter fileConverter,
            IFilesStorageFactory filesStorageFactory, IFileTypeProvider fileTypeProvider)
        {
            _fileConverter = fileConverter;
            _fileTypeProvider = fileTypeProvider;
            _filesStorage = infoStorageFactory.CreateFileStorage();
            _chatStorage = infoStorageFactory.CreateChatStorage();
            _fileSenderStorage = infoStorageFactory.CreateFileSenderStorage();
            _physicalFilesStorage = filesStorageFactory.CreateAsync().Result;
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<File>>> GetFiles()
        {
            var filesFromDataBase = await _filesStorage.GetAllAsync();
            var convertedFiles = filesFromDataBase
                .Select(_fileConverter.ConvertFile)
                .ToList();
            return RequestResult.Ok(convertedFiles);
        }

        /// <inheritdoc />
        public async Task<RequestResult<File>> GetFileById(Guid id)
        {
            var file = await _filesStorage.GetByIdAsync(id);
            if (file is null)
                return RequestResult.NotFound<File>($"File with identifier {id} not found");
            return RequestResult.Ok(_fileConverter.ConvertFile(file));
        }

        /// <inheritdoc />
        public async Task<RequestResult<File>> CreateFile(IFormFile uploadFile)
        {
            var file = new DataBaseFile
            {
                Id = Guid.NewGuid(),
                Name = uploadFile.FileName,
                Extension = Path.GetExtension(uploadFile.FileName),
                Type = _fileTypeProvider.GetFileType(uploadFile.Headers["Content-Type"]),
                UploadDate = DateTime.Now,
                FileSenderId = Guid.Empty,
                ChatId = Guid.Empty,
            };

            var chat = await _chatStorage.GetByIdAsync(file.ChatId) ?? new Chat {Id = Guid.Empty};
            var sender = await _fileSenderStorage.GetByIdAsync(file.FileSenderId) ?? new FileSender {Id = Guid.Empty};
            file.Chat = chat;
            file.FileSender = sender;
            var memoryStream = new MemoryStream();
            await uploadFile.CopyToAsync(memoryStream);
            await _physicalFilesStorage.SaveFileAsync(file.Id.ToString(), memoryStream);
            return RequestResult.Created(_fileConverter.ConvertFile(file));
        }


        /// <inheritdoc />
        public async Task<RequestResult<File>> UpdateFile(Guid id, string fileName)
        {
            var file = await _filesStorage.GetByIdAsync(id);
            if (file is null)
                return RequestResult.NotFound<File>($"File with identifier {id} not found");
            file.Name = fileName;
            await _filesStorage.UpdateAsync(file);
            return RequestResult.Created(_fileConverter.ConvertFile(file));
        }

        /// <inheritdoc />
        public async Task<RequestResult<File>> DeleteFile(Guid id)
        {
            var file = await _filesStorage.DeleteAsync(id);
            return file
                ? RequestResult.NoContent<File>()
                : RequestResult.NotFound<File>($"File with identifier {id} not found");
        }
    }
}