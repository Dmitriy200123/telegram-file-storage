using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilesStorage.Interfaces;
using FileStorageAPI.Converters;
using FileStorageAPI.Extensions;
using FileStorageAPI.Models;
using FileStorageAPI.Providers;
using FileStorageApp.Data.InfoStorage.Enums;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Http;
using Chat = FileStorageApp.Data.InfoStorage.Models.Chat;
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
        private readonly ISenderFormTokenProvider _senderFormTokenProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FileService"/>
        /// </summary>
        /// <param name="infoStorageFactory">Фабрика для получения доступа к хранилищу файлов</param>
        /// <param name="fileInfoConverter">Конвертер для преобразования файлов в API-контракты</param>
        /// <param name="filesStorageFactory">Фабрика для получения доступа к физическому хранилищу чатов</param>
        /// <param name="fileTypeProvider">Поставщик типа файла</param>
        /// <param name="expressionFileFilterProvider">Поставщик query Expression для поиска данных</param>
        /// <param name="downloadLinkProvider">Поставщик для получения ссылки на файл</param>
        /// <param name="senderFormTokenProvider"></param>
        public FileService(IInfoStorageFactory infoStorageFactory,
            IFileInfoConverter fileInfoConverter,
            IFilesStorageFactory filesStorageFactory,
            IFileTypeProvider fileTypeProvider,
            IExpressionFileFilterProvider expressionFileFilterProvider,
            IDownloadLinkProvider downloadLinkProvider,
            ISenderFormTokenProvider senderFormTokenProvider)
        {
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _fileInfoConverter = fileInfoConverter ?? throw new ArgumentNullException(nameof(fileInfoConverter));
            _filesStorageFactory = filesStorageFactory ?? throw new ArgumentNullException(nameof(filesStorageFactory));
            _fileTypeProvider = fileTypeProvider ?? throw new ArgumentNullException(nameof(fileTypeProvider));
            _expressionFileFilterProvider = expressionFileFilterProvider ??
                                            throw new ArgumentNullException(nameof(expressionFileFilterProvider));
            _downloadLinkProvider =
                downloadLinkProvider ?? throw new ArgumentNullException(nameof(downloadLinkProvider));
            _senderFormTokenProvider = senderFormTokenProvider ??
                                       throw new ArgumentNullException(nameof(senderFormTokenProvider));
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(FileSearchParameters fileSearchParameters,
            int skip, int take, HttpRequest request)
        {
            if (skip < 0 || take < 0)
                return RequestResult.BadRequest<List<FileInfo>>($"Skip or take less than 0");

            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var expression = _expressionFileFilterProvider.GetExpression(fileSearchParameters);
            var filesFromDataBase = await filesStorage.GetByFilePropertiesAsync(expression, true, skip, take);
            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            var filteredFiles = filesFromDataBase.FilterFiles(sender);
            var convertedFiles = filteredFiles
                .Select(_fileInfoConverter.ConvertFileInfo)
                .ToList();

            return RequestResult.Ok(convertedFiles);
        }

        /// <inheritdoc />
        public async Task<RequestResult<FileInfo>> GetFileInfoByIdAsync(Guid id, HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.GetByIdAsync(id, true);
            if (file is null)
                return RequestResult.NotFound<FileInfo>($"File with identifier {id} not found");
            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            var filesToFilter = new List<DataBaseFile> {file};
            var filteredFiles = filesToFilter.FilterFiles(sender);
            return filteredFiles.Count == 0
                ? RequestResult.Forbidden<FileInfo>("Don't have access to this file")
                : RequestResult.Ok(_fileInfoConverter.ConvertFileInfo(file));
        }

        /// <inheritdoc />
        public async Task<RequestResult<string>> GetFileDownloadLinkByIdAsync(Guid id, HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.GetByIdAsync(id);
            if (file is null)
                return RequestResult.NotFound<string>($"File with identifier {id} not found");

            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            var filesToFilter = new List<DataBaseFile> {file};
            var filteredFiles = filesToFilter.FilterFiles(sender);
            if (filteredFiles.Count == 0)
                return RequestResult.Forbidden<string>("Don't have access to this file");
            var result = await _downloadLinkProvider.GetDownloadLinkAsync(id, file.Name);

            return RequestResult.Ok(result);
        }

        /// <inheritdoc />
        public async Task<RequestResult<(string Uri, FileInfo Info)>> CreateFileAsync(IFormFile uploadFile,
            HttpRequest request)
        {
            var fileSender = await _senderFormTokenProvider.GetSenderFromToken(request);
            if (fileSender is null)
                return RequestResult.BadRequest<(string Uri, FileInfo Info)>("Does not have this sender in database");
            var file = new DataBaseFile
            {
                Id = Guid.NewGuid(),
                Name = uploadFile.FileName,
                Extension = Path.GetExtension(uploadFile.FileName),
                Type = _fileTypeProvider.GetFileType(uploadFile.Headers["Content-Type"]),
                UploadDate = DateTime.Now,
                FileSenderId = fileSender.Id
            };

            await using var memoryStream = new MemoryStream();
            await uploadFile.CopyToAsync(memoryStream);

            if (!await UploadFile(file, memoryStream))
                return RequestResult.InternalServerError<(string uri, FileInfo info)>("Can't add to database");

            var chat = new Chat {Id = Guid.Empty, Name = "Ручная загрузка файла"};
            file.Chat = chat;
            using var fileSenderStorage = _infoStorageFactory.CreateFileSenderStorage();
            file.FileSender = await fileSenderStorage.GetByIdAsync(fileSender.Id);
            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(file.Id, file.Name);

            return RequestResult.Created<(string uri, FileInfo info)>((downloadLink,
                _fileInfoConverter.ConvertFileInfo(file)));
        }


        /// <inheritdoc />
        public async Task<RequestResult<(string Uri, FileInfo Info)>> UpdateFileAsync(Guid id, string fileName)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var file = await filesStorage.GetByIdAsync(id, true);
            if (file is null)
                return RequestResult.NotFound<(string uri, FileInfo info)>($"File with identifier {id} not found");
            file.Name = fileName;
            await filesStorage.UpdateAsync(file);

            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(id, fileName);

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
        public async Task<RequestResult<int>> GetFilesCountAsync(FileSearchParameters fileSearchParameters,
            HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            var chatsId = sender!.Chats.Select(chat => chat.Id).ToList();
            var expression = _expressionFileFilterProvider.GetExpression(fileSearchParameters, chatsId);
            var filesCount = await filesStorage.GetFilesCountAsync(expression);

            return RequestResult.Ok(filesCount);
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<string>>> GetFileNamesAsync(HttpRequest request)
        {
            using var fileInfoStorage = _infoStorageFactory.CreateFileStorage();
            var files = await fileInfoStorage.GetAllAsync();
            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            var filterFiles = files.FilterFiles(sender);
            var filesNames = filterFiles.Select(x => x.Name).ToList();
            return RequestResult.Ok(filesNames);
        }

        /// <inheritdoc />
        public RequestResult<FileTypeDescription[]> GetFilesTypes()
        {
            var descriptions = Enum.GetValues(typeof(FileType))
                .Cast<FileType>()
                .Select(fileType => new FileTypeDescription {Id = (int) fileType, Name = fileType.GetEnumDescription()})
                .ToArray();

            return RequestResult.Ok(descriptions);
        }

        /// <inheritdoc />
        public async Task<RequestResult<string>> GetLink(Guid id, HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.GetByIdAsync(id);
            if (file is null)
                return RequestResult.NotFound<string>($"Link with identifier {id} not found");
            if (file.Type != FileType.Link)
                return RequestResult.BadRequest<string>("Invalid file type");
            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            var filesToFilter = new List<DataBaseFile> {file};
            var filteredFiles = filesToFilter.FilterFiles(sender);
            if (filteredFiles.Count == 0)
                return RequestResult.Forbidden<string>("Don't have access to this link");
            using var physicalFileStorage = await _filesStorageFactory.CreateAsync();
            using var streamReader = new StreamReader(await physicalFileStorage.GetFile(file.Id.ToString()));
            var text = await streamReader.ReadToEndAsync();
            return RequestResult.Ok(text);
        }

        /// <inheritdoc />
        public async Task<RequestResult<string>> GetMessage(Guid id, HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.GetByIdAsync(id);
            if (file is null)
                return RequestResult.NotFound<string>($"Message with identifier {id} not found");
            if (file.Type != FileType.Text)
                return RequestResult.BadRequest<string>("Invalid file type");
            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            var filesToFilter = new List<DataBaseFile> {file};
            var filteredFiles = filesToFilter.FilterFiles(sender);
            if (filteredFiles.Count == 0)
                return RequestResult.Forbidden<string>("Don't have access to this message");
            using var physicalFileStorage = await _filesStorageFactory.CreateAsync();
            using var streamReader = new StreamReader(await physicalFileStorage.GetFile(file.Id.ToString()));
            var text = await streamReader.ReadToEndAsync();
            return RequestResult.Ok(text);
        }

        /// <inheritdoc />
        public async Task<RequestResult<(string Uri, Guid Guid)>> PostMessage(UploadTextData uploadTextData,
            HttpRequest request)
        {
            var fileSender = await _senderFormTokenProvider.GetSenderFromToken(request);
            if (fileSender is null)
                return RequestResult.BadRequest<(string Uri, Guid Guid)>("Does not have this sender in database");
            var file = CreateFile(FileType.Text, fileSender.Id, uploadTextData.Name);
            var stream = await CreateStream(uploadTextData.Value);
            if (!await UploadFile(file, stream))
                return RequestResult.InternalServerError<(string uri, Guid Guid)>("Can't add to database");
            await stream.DisposeAsync();
            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(file.Id, file.Name);
            return RequestResult.Created<(string Uri, Guid Guid)>((downloadLink, file.Id));
        }

        /// <inheritdoc />
        public async Task<RequestResult<(string Uri, Guid Guid)>> PostLink(UploadTextData uploadTextData,
            HttpRequest request)
        {
            if(!Uri.IsWellFormedUriString(uploadTextData.Value, UriKind.Absolute))
                return RequestResult.BadRequest<(string Uri, Guid Guid)>("This is not url");
            var fileSender = await _senderFormTokenProvider.GetSenderFromToken(request);
            if (fileSender is null)
                return RequestResult.BadRequest<(string Uri, Guid Guid)>("Does not have this sender in database");
            var file = CreateFile(FileType.Link, fileSender.Id, uploadTextData.Name);
            var stream = await CreateStream(uploadTextData.Value);
            if (!await UploadFile(file, stream))
                return RequestResult.InternalServerError<(string uri, Guid Guid)>("Can't add to database");
            await stream.DisposeAsync();
            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(file.Id, file.Name);
            return RequestResult.Created<(string Uri, Guid Guid)>((downloadLink, file.Id));
        }

        private async Task<bool> UploadFile(DataBaseFile file, Stream stream)
        {
            using var physicalFilesStorage = await _filesStorageFactory.CreateAsync();
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            await physicalFilesStorage.SaveFileAsync(file.Id.ToString(), stream);
            return await filesStorage.AddAsync(file);
        }

        private static async Task<Stream> CreateStream(string value)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            await streamWriter.WriteAsync(value);
            await streamWriter.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private static DataBaseFile CreateFile(FileType fileType, Guid senderId, string name)
        {
            return new DataBaseFile
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = fileType,
                UploadDate = DateTime.Now,
                FileSenderId = senderId
            };
        }
    }
}