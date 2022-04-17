using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using API;
using DocumentsIndex;
using DocumentsIndex.Contracts;
using FilesStorage.Interfaces;
using FileStorageAPI.Converters;
using FileStorageAPI.Extensions;
using FileStorageAPI.Models;
using FileStorageAPI.Providers;
using FileStorageApp.Data.InfoStorage.Enums;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
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
        private readonly IAccessesByUserIdProvider _accessesByUserIdProvider;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;
        private readonly IDocumentIndexStorage _documentIndexStorage;

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
        /// <param name="userIdFromTokenProvider">Поставщик для получения Id пользователя из токена</param>
        /// <param name="accessesByUserIdProvider">Поставщик для получения прав пользователя по Id</param>
        /// <param name="documentIndexStorage">Хранилище текстовых файлов с поиском по содержимому</param>
        public FileService(IInfoStorageFactory infoStorageFactory,
            IFileInfoConverter fileInfoConverter,
            IFilesStorageFactory filesStorageFactory,
            IFileTypeProvider fileTypeProvider,
            IExpressionFileFilterProvider expressionFileFilterProvider,
            IDownloadLinkProvider downloadLinkProvider,
            ISenderFormTokenProvider senderFormTokenProvider,
            IAccessesByUserIdProvider accessesByUserIdProvider,
            IUserIdFromTokenProvider userIdFromTokenProvider, 
            IDocumentIndexStorage documentIndexStorage)
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
            _accessesByUserIdProvider = accessesByUserIdProvider ??
                                        throw new ArgumentNullException(nameof(accessesByUserIdProvider));
            _userIdFromTokenProvider = userIdFromTokenProvider ??
                                       throw new ArgumentNullException(nameof(userIdFromTokenProvider));
            _documentIndexStorage = documentIndexStorage ?? throw new ArgumentNullException(nameof(documentIndexStorage));
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(FileSearchParameters fileSearchParameters,
            int skip, int take, HttpRequest request)
        {
            if (skip < 0 || take < 0)
                return RequestResult.BadRequest<List<FileInfo>>($"Skip or take less than 0");
            var chatsId = await GetUserChats(request);
           
            var expression = _expressionFileFilterProvider.GetExpression(fileSearchParameters, chatsId);
            var files = await GetFileInfoFromStorage(expression, skip, take);

            return RequestResult.Ok(files);
        }

        /// <inheritdoc />
        public async Task<RequestResult<int>> GetFilesCountAsync(FileSearchParameters fileSearchParameters,
            HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var sender = await GetNotNullSenderAsync(request);
            
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            var chatsId = hasAnyFilesAccess ? null : sender.Chats.Select(chat => chat.Id).ToList();
            var expression = _expressionFileFilterProvider.GetExpression(fileSearchParameters, chatsId);
            var filesCount = await filesStorage.GetFilesCountAsync(expression);

            return RequestResult.Ok(filesCount);
        }

        /// <inheritdoc />
        public async Task<RequestResult<FileInfo>> GetFileInfoByIdAsync(Guid id, HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var file = await filesStorage.GetByIdAsync(id, true);
            if (file is null)
                return RequestResult.NotFound<FileInfo>($"File with identifier {id} not found");
            var sender = await GetNotNullSenderAsync(request);
            
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            var filesToFilter = new List<DataBaseFile> {file};
            var filteredFiles = hasAnyFilesAccess ? filesToFilter : filesToFilter.FilterFiles(sender);
            SetFileChat(filteredFiles);
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

            var sender = await GetNotNullSenderAsync(request);
            
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            var filesToFilter = new List<DataBaseFile> {file};
            var filteredFiles = hasAnyFilesAccess ? filesToFilter : filesToFilter.FilterFiles(sender);
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
                Type = _fileTypeProvider.GetFileType(uploadFile.FileName),
                UploadDate = DateTime.Now,
                FileSenderId = fileSender.Id
            };

            await using var memoryStream = new MemoryStream();
            await uploadFile.CopyToAsync(memoryStream);

            if (!await UploadFile(file, memoryStream))
                return RequestResult.InternalServerError<(string uri, FileInfo info)>("Can't add to database");

            if (file.Type == FileType.TextDocument)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                var document = new Document(file.Id, memoryStream.ToArray(), file.Name);
                if (!await _documentIndexStorage.IndexDocumentAsync(document))
                    return RequestResult.InternalServerError<(string uri, FileInfo info)>("Can't add to elastic");
            }


            var chat = new Chat { Id = Guid.Empty, Name = "Ручная загрузка файла" };
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
            if (file.Type == FileType.TextDocument)
            {
                using var physicalStorage = await _filesStorageFactory.CreateAsync();
                var stream = await physicalStorage.GetFileStreamAsync(id.ToString());
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                await _documentIndexStorage.DeleteAsync(id);
                var document = new Document(file.Id, memoryStream.ToArray(), file.Name);
                if(!await _documentIndexStorage.IndexDocumentAsync(document))
                    return RequestResult.InternalServerError<(string uri, FileInfo info)>("Can't add to elastic");
            }

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
                var deleteResult = await filesStorage.DeleteAsync(id);
                await physicalFilesStorage.DeleteFileAsync(id.ToString());
                await _documentIndexStorage.DeleteAsync(id);
                return deleteResult
                    ? RequestResult.NoContent<FileInfo>()
                    : RequestResult.InternalServerError<FileInfo>($"Something wrong with dataBase");
            }
            catch (Exception)
            {
                return RequestResult.NotFound<FileInfo>($"File with identifier {id} not found");
            }
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<string>>> GetFileNamesAsync(HttpRequest request)
        {
            using var fileInfoStorage = _infoStorageFactory.CreateFileStorage();
            var files = await fileInfoStorage.GetAllAsync();
            var sender = await GetNotNullSenderAsync(request);
            
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            var filterFiles = hasAnyFilesAccess ? files : files.FilterFiles(sender);
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
            var sender = await GetNotNullSenderAsync(request);
            
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            var filesToFilter = new List<DataBaseFile> {file};
            var filteredFiles = hasAnyFilesAccess ? filesToFilter : filesToFilter.FilterFiles(sender);
            if (filteredFiles.Count == 0)
                return RequestResult.Forbidden<string>("Don't have access to this link");
            using var physicalFileStorage = await _filesStorageFactory.CreateAsync();
            using var streamReader = new StreamReader(await physicalFileStorage.GetFileStreamAsync(file.Id.ToString()));
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
            var sender = await GetNotNullSenderAsync(request);
            
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            var filesToFilter = new List<DataBaseFile> {file};
            var filteredFiles = hasAnyFilesAccess ? filesToFilter : filesToFilter.FilterFiles(sender);
            if (filteredFiles.Count == 0)
                return RequestResult.Forbidden<string>("Don't have access to this message");
            using var physicalFileStorage = await _filesStorageFactory.CreateAsync();
            using var streamReader = new StreamReader(await physicalFileStorage.GetFileStreamAsync(file.Id.ToString()));
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
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(uploadTextData.Value));
            if (!await UploadFile(file, stream))
                return RequestResult.InternalServerError<(string uri, Guid Guid)>("Can't add to database");
            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(file.Id, file.Name);
            return RequestResult.Created<(string Uri, Guid Guid)>((downloadLink, file.Id));
        }

        /// <inheritdoc />
        public async Task<RequestResult<(string Uri, Guid Guid)>> PostLink(UploadTextData uploadTextData,
            HttpRequest request)
        {
            if (!Uri.IsWellFormedUriString(uploadTextData.Value, UriKind.Absolute))
                return RequestResult.BadRequest<(string Uri, Guid Guid)>("This is not url");
            var fileSender = await _senderFormTokenProvider.GetSenderFromToken(request);
            if (fileSender is null)
                return RequestResult.BadRequest<(string Uri, Guid Guid)>("Does not have this sender in database");
            var file = CreateFile(FileType.Link, fileSender.Id, uploadTextData.Name);
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(uploadTextData.Value));
            if (!await UploadFile(file, stream))
                return RequestResult.InternalServerError<(string uri, Guid Guid)>("Can't add to database");
            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(file.Id, file.Name);
            return RequestResult.Created<(string Uri, Guid Guid)>((downloadLink, file.Id));
        }

        /// <inheritdoc />
        public async Task<RequestResult<int>> GetDocumentsCountByParametersAndIds(FileSearchParameters fileSearchParameters, 
            List<Guid> guidsToFind, HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var sender = await GetNotNullSenderAsync(request);
            
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            var chatsId = hasAnyFilesAccess ? null : sender.Chats.Select(chat => chat.Id).ToList();
            var expression = _expressionFileFilterProvider.GetDocumentExpression(fileSearchParameters, guidsToFind, chatsId);
            var filesCount = await filesStorage.GetFilesCountAsync(expression);

            return RequestResult.Ok(filesCount);
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<FileInfo>>> GetDocumentsByParametersAndIds(
            FileSearchParameters fileSearchParameters, List<Guid> guidsToFind, HttpRequest request, int skip, int take)
        {
            if (skip < 0 || take < 0)
                return RequestResult.BadRequest<List<FileInfo>>($"Skip or take less than 0");
            var chatsId = await GetUserChats(request);
           
            var expression = _expressionFileFilterProvider.GetDocumentExpression(fileSearchParameters, chatsId);
            var files = await GetFileInfoFromStorage(expression, skip, take);

            return RequestResult.Ok(files);
        }

        private async Task<List<FileInfo>> GetFileInfoFromStorage(Expression<Func<DataBaseFile, bool>> expression,
            int? skip, int? take)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var filesFromDataBase = await filesStorage.GetByFilePropertiesAsync(expression, true, skip, take);
            SetFileChat(filesFromDataBase);
            var convertedFiles = filesFromDataBase
                .Select(_fileInfoConverter.ConvertFileInfo)
                .ToList();
            return convertedFiles;
        }

        private async Task<List<Guid>?> GetUserChats(HttpRequest request)
        {
            var sender = await GetNotNullSenderAsync(request);
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var hasAnyFilesAccess = await HasAnyFilesAccessAsync(request);
            return hasAnyFilesAccess ? null : sender.Chats.Select(chat => chat.Id).ToList();
        }

        private async Task<bool> UploadFile(DataBaseFile file, Stream stream)
        {
            using var physicalFilesStorage = await _filesStorageFactory.CreateAsync();
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            await physicalFilesStorage.SaveFileAsync(file.Id.ToString(), stream);
            return await filesStorage.AddAsync(file);
        }

        private async Task<FileSender> GetNotNullSenderAsync(HttpRequest request)
        {
            var sender = await _senderFormTokenProvider.GetSenderFromToken(request);
            
            if (sender == null)
                throw new InvalidOperationException("Sender not found");

            return sender;
        }

        private async Task<bool> HasAnyFilesAccessAsync(HttpRequest request)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(request, Settings.Key);
            var accesses = await _accessesByUserIdProvider.GetAccessesByUserIdAsync(userId);
            return accesses.Any(access => access == Accesses.ViewAnyFiles);
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

        private static void SetFileChat(IEnumerable<DataBaseFile> files)
        {
            var chat = new Chat
            {
                Id = Guid.Empty,
                TelegramId = 0,
                Name = "Загрузка с сайта",
                ImageId = null,
            };
            foreach (var file in files)
            {
                file.Chat ??= chat;
            }
        }
    }
}