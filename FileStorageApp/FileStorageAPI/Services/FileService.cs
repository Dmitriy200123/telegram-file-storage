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
using Microsoft.AspNetCore.Http;
using DataBaseFile = FileStorageApp.Data.InfoStorage.Models.File;
using File = FileStorageAPI.Models.File;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class FileService : IFileService
    {
        private readonly IFileConverter _fileConverter;
        private readonly IFileTypeProvider _fileTypeProvider;
        private readonly IFilesStorageFactory _filesStorageFactory;
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IExpressionFileFilterProvider _expressionFileFilterProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoStorageFactory">Фабрика для получения доступа к хранилищу файлов</param>
        /// <param name="fileConverter">Конвертор для преобразования файлов в API-контракты</param>
        /// <param name="filesStorageFactory">Фабрика для получения доступа к физическому хранилищу чатов</param>
        /// <param name="fileTypeProvider">Поставщик типа файла</param>
        public FileService(IInfoStorageFactory infoStorageFactory, IFileConverter fileConverter,
            IFilesStorageFactory filesStorageFactory, IFileTypeProvider fileTypeProvider, IExpressionFileFilterProvider expressionFileFilterProvider)
        {
            _infoStorageFactory = infoStorageFactory;
            _fileConverter = fileConverter;
            _filesStorageFactory = filesStorageFactory;
            _fileTypeProvider = fileTypeProvider;
            _expressionFileFilterProvider = expressionFileFilterProvider;
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<File>>> GetFiles(FileSearchParameters fileSearchParameters)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var expression = _expressionFileFilterProvider.GetExpression(fileSearchParameters);
            var filesFromDataBase = await filesStorage.GetByFilePropertiesAsync(expression);
            var convertedFiles = filesFromDataBase
                .Select(_fileConverter.ConvertFile)
                .ToList();
            return RequestResult.Ok(convertedFiles);
        }

        /// <inheritdoc />
        public async Task<RequestResult<File>> GetFileById(Guid id)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.GetByIdAsync(id);
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
                FileSenderId = null,
                ChatId = null,
            };

            var memoryStream = new MemoryStream();
            await uploadFile.CopyToAsync(memoryStream);
            
            using var physicalFilesStorage = await _filesStorageFactory.CreateAsync();
            await physicalFilesStorage.SaveFileAsync(file.Id.ToString(), memoryStream);
            
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            await filesStorage.AddAsync(file);
            
            var chat = new Chat {Id = Guid.Empty};
            var sender = new FileSender {Id = Guid.Empty};
            file.Chat = chat;
            file.FileSender = sender;

            return RequestResult.Created(_fileConverter.ConvertFile(file));
        }


        /// <inheritdoc />
        public async Task<RequestResult<File>> UpdateFile(Guid id, string fileName)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.GetByIdAsync(id);
            if (file is null)
                return RequestResult.NotFound<File>($"File with identifier {id} not found");
            file.Name = fileName;
            await filesStorage.UpdateAsync(file);
            return RequestResult.Created(_fileConverter.ConvertFile(file));
        }

        /// <inheritdoc />
        public async Task<RequestResult<File>> DeleteFile(Guid id)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.DeleteAsync(id);
            return file
                ? RequestResult.NoContent<File>()
                : RequestResult.NotFound<File>($"File with identifier {id} not found");
        }
    }
}