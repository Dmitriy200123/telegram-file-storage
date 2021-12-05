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
using FileInfo = FileStorageAPI.Models.FileInfo;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class FileService : IFileService
    {
        private readonly IFileInfoConverter _fileInfoConverter;
        private readonly IFileTypeProvider _fileTypeProvider;
        private readonly IFilesStorageFactory _filesStorageFactory;
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IExpressionFileFilterProvider _expressionFileFilterProvider;
        private readonly IDownloadLinkProvider _downloadLinkProvider;
        private const string TempFileSenderId = "00000000-0000-0000-0000-000000000001";

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FileService"/>
        /// </summary>
        /// <param name="infoStorageFactory">Фабрика для получения доступа к хранилищу файлов</param>
        /// <param name="fileInfoConverter">Конвертор для преобразования файлов в API-контракты</param>
        /// <param name="filesStorageFactory">Фабрика для получения доступа к физическому хранилищу чатов</param>
        /// <param name="fileTypeProvider">Поставщик типа файла</param>
        /// <param name="expressionFileFilterProvider">Поставщик query Expression для поиска данных</param>
        /// <param name="downloadLinkProvider">Поставщик для получения ссылки на файл</param>
        public FileService(IInfoStorageFactory infoStorageFactory,
            IFileInfoConverter fileInfoConverter,
            IFilesStorageFactory filesStorageFactory,
            IFileTypeProvider fileTypeProvider,
            IExpressionFileFilterProvider expressionFileFilterProvider,
            IDownloadLinkProvider downloadLinkProvider)
        {
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _fileInfoConverter = fileInfoConverter ?? throw new ArgumentNullException(nameof(fileInfoConverter));
            _filesStorageFactory = filesStorageFactory ?? throw new ArgumentNullException(nameof(filesStorageFactory));
            _fileTypeProvider = fileTypeProvider ?? throw new ArgumentNullException(nameof(fileTypeProvider));
            _expressionFileFilterProvider = expressionFileFilterProvider ??
                                            throw new ArgumentNullException(nameof(expressionFileFilterProvider));
            _downloadLinkProvider =
                downloadLinkProvider ?? throw new ArgumentNullException(nameof(downloadLinkProvider));
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(FileSearchParameters fileSearchParameters,
            int skip, int take)
        {
            if (skip < 0 || take < 0)
                return RequestResult.BadRequest<List<FileInfo>>($"Skip or take less than 0");

            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var expression = _expressionFileFilterProvider.GetExpression(fileSearchParameters);
            var filesFromDataBase = await filesStorage.GetByFilePropertiesAsync(expression, true, skip, take);
            var convertedFiles = filesFromDataBase
                .Select(_fileInfoConverter.ConvertFileInfo)
                .ToList();

            return RequestResult.Ok(convertedFiles);
        }

        /// <inheritdoc />
        public async Task<RequestResult<FileInfo>> GetFileInfoByIdAsync(Guid id)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.GetByIdAsync(id, true);

            if (file is null)
                return RequestResult.NotFound<FileInfo>($"File with identifier {id} not found");
            return RequestResult.Ok(_fileInfoConverter.ConvertFileInfo(file));
        }

        /// <inheritdoc />
        public async Task<RequestResult<string>> GetFileDownloadLinkByIdAsync(Guid id)
        {
            var result = await _downloadLinkProvider.GetDownloadLinkAsync(id);

            if (result == null)
                return RequestResult.NotFound<string>($"File with identifier {id} not found");
            return RequestResult.Ok(result);
        }

        /// <inheritdoc />
        public async Task<RequestResult<(string Uri, FileInfo Info)>> CreateFileAsync(IFormFile uploadFile)
        {
            var siteFileSender = new FileSender
            {
                Id = Guid.Parse(TempFileSenderId),
                TelegramId = -1,
                TelegramUserName = "Загрузчик с сайта",
                FullName = "Загрузчик с сайта",
            };
            using var fileSenderStorage = _infoStorageFactory.CreateFileSenderStorage();
            await fileSenderStorage.AddAsync(siteFileSender, false);
            var now = DateTime.Now;
            var file = new DataBaseFile
            {
                Id = Guid.NewGuid(),
                Name = uploadFile.FileName,
                Extension = Path.GetExtension(uploadFile.FileName),
                Type = _fileTypeProvider.GetFileType(uploadFile.Headers["Content-Type"]),
                UploadDate = now,
                FileSenderId = Guid.Parse(TempFileSenderId)
            };

            await using var memoryStream = new MemoryStream();
            await uploadFile.CopyToAsync(memoryStream);

            using var physicalFilesStorage = await _filesStorageFactory.CreateAsync();
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            await physicalFilesStorage.SaveFileAsync(file.Id.ToString(), memoryStream);
            if (!await filesStorage.AddAsync(file))
                return RequestResult.InternalServerError<(string uri, FileInfo info)>("Can't add to database");

            var chat = new Chat {Id = Guid.Empty, Name = "Ручная загрузка файла"};
            file.Chat = chat;
            file.FileSender = await fileSenderStorage.GetByIdAsync(Guid.Parse(TempFileSenderId));
            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(file.Id);

            return RequestResult.Created<(string uri, FileInfo info)>((downloadLink,
                _fileInfoConverter.ConvertFileInfo(file)));
        }


        /// <inheritdoc />
        public async Task<RequestResult<(string Uri, FileInfo Info)>> UpdateFileAsync(Guid id, string fileName)
        {
            //todo rename file s3
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var file = await filesStorage.GetByIdAsync(id, true);
            if (file is null)
                return RequestResult.NotFound<(string uri, FileInfo info)>($"File with identifier {id} not found");
            file.Name = fileName;
            await filesStorage.UpdateAsync(file);

            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(id);
            if (downloadLink is null)
                return RequestResult.NotFound<(string uri, FileInfo info)>($"File with identifier {id} not found");

            return RequestResult.Created<(string uri, FileInfo info)>((downloadLink,
                _fileInfoConverter.ConvertFileInfo(file)));
        }

        /// <inheritdoc />
        public async Task<RequestResult<FileInfo>> DeleteFileAsync(Guid id)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();

            using var physicalFilesStorage = await _filesStorageFactory.CreateAsync();
            try
            {
                var file = await filesStorage.DeleteAsync(id);
                await physicalFilesStorage.DeleteFileAsync(id.ToString());
                return file
                    ? RequestResult.NoContent<FileInfo>()
                    : RequestResult.InternalServerError<FileInfo>($"Something wrong with dataBase");
            }
            catch (Exception)
            {
                return RequestResult.NotFound<FileInfo>($"File with identifier {id} not found");
            }
        }

        /// <inheritdoc />
        public async Task<RequestResult<int>> GetFilesCountAsync()
        {
            using var fileInfoStorage = _infoStorageFactory.CreateFileStorage();

            return RequestResult.Ok(await fileInfoStorage.GetFilesCountAsync());
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<string>>> GetFileNamesAsync()
        {
            using var fileInfoStorage = _infoStorageFactory.CreateFileStorage();

            return RequestResult.Ok(await fileInfoStorage.GetFileNamesAsync());
        }
    }
}