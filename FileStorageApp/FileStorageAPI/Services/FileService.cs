using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using API;
using DocumentClassificationsAPI.Services;
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
using FileStorageApp.Data.InfoStorage.Storages.DocumentClassifications;
using Microsoft.AspNetCore.Http;
using SearchDocumentsAPI.Services.DocumentsSearch;
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
        private readonly ISenderFromTokenProvider _senderFromTokenProvider;
        private readonly IDocumentIndexStorage _documentIndexStorage;
        private readonly IAccessService _accessService;
        private readonly IDocumentClassificationsService _documentClassificationsService;
        private readonly IDocumentsSearchService _documentsSearchService;
        private readonly IDocumentsService _documentsService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FileService"/>
        /// </summary>
        /// <param name="infoStorageFactory">Фабрика для получения доступа к хранилищу файлов</param>
        /// <param name="fileInfoConverter">Конвертер для преобразования файлов в API-контракты</param>
        /// <param name="filesStorageFactory">Фабрика для получения доступа к физическому хранилищу чатов</param>
        /// <param name="fileTypeProvider">Поставщик типа файла</param>
        /// <param name="expressionFileFilterProvider">Поставщик query Expression для поиска данных</param>
        /// <param name="downloadLinkProvider">Поставщик для получения ссылки на файл</param>
        /// <param name="senderFromTokenProvider">Поставщик отправителя файла из токена</param>
        /// <param name="documentIndexStorage">Хранилище текстовых файлов с поиском по содержимому</param>
        /// <param name="accessService">Сервис отвечающий за опции доступа</param>
        /// <param name="documentClassificationsService">Сервис отвечающий за классификацию документов</param>
        /// <param name="documentsSearchService">Сервис для поиска документов</param>
        /// <param name="documentsService">Сервис для работы с документами</param>
        public FileService(IInfoStorageFactory infoStorageFactory,
            IFileInfoConverter fileInfoConverter,
            IFilesStorageFactory filesStorageFactory,
            IFileTypeProvider fileTypeProvider,
            IExpressionFileFilterProvider expressionFileFilterProvider,
            IDownloadLinkProvider downloadLinkProvider,
            ISenderFromTokenProvider senderFromTokenProvider,
            IDocumentIndexStorage documentIndexStorage,
            IAccessService accessService, 
            IDocumentClassificationsService documentClassificationsService, 
            IDocumentsSearchService documentsSearchService, 
            IDocumentsService documentsService)
        {
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _fileInfoConverter = fileInfoConverter ?? throw new ArgumentNullException(nameof(fileInfoConverter));
            _filesStorageFactory = filesStorageFactory ?? throw new ArgumentNullException(nameof(filesStorageFactory));
            _fileTypeProvider = fileTypeProvider ?? throw new ArgumentNullException(nameof(fileTypeProvider));
            _expressionFileFilterProvider = expressionFileFilterProvider ??
                                            throw new ArgumentNullException(nameof(expressionFileFilterProvider));
            _downloadLinkProvider =
                downloadLinkProvider ?? throw new ArgumentNullException(nameof(downloadLinkProvider));
            _senderFromTokenProvider = senderFromTokenProvider ??
                                       throw new ArgumentNullException(nameof(senderFromTokenProvider));
            _documentIndexStorage = documentIndexStorage ?? throw new ArgumentNullException(nameof(documentIndexStorage));
            _accessService = accessService ?? throw new ArgumentNullException(nameof(accessService));
            _documentClassificationsService = documentClassificationsService;
            _documentsSearchService = documentsSearchService;
            _documentsService = documentsService;
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(FileSearchParameters fileSearchParameters,
            int skip, int take, HttpRequest request)
        {
            if (skip < 0 || take < 0)
                return RequestResult.BadRequest<List<FileInfo>>($"Skip or take less than 0");
            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
            var chatsId = hasAnyFilesAccess ? null : sender!.Chats.Select(chat => chat.Id).ToList();
           
            var expression = _expressionFileFilterProvider.GetExpression(fileSearchParameters, chatsId);
            var files = await GetFileInfoFromStorage(expression, skip, take);

            return RequestResult.Ok(files);
        }

        /// <inheritdoc />
        public async Task<RequestResult<int>> GetFilesCountAsync(FileSearchParameters fileSearchParameters,
            HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();
            
            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
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
            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();
            
            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
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

            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();
            
            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
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
            var fileSender = await _senderFromTokenProvider.GetSenderFromToken(request);
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
            var bytesArray = memoryStream.ToArray();
            await using var copiedStream = new MemoryStream(bytesArray);

            if (!await UploadFile(file, copiedStream, uploadFile.ContentType))
                return RequestResult.InternalServerError<(string uri, FileInfo info)>("Can't add to database");

            if (file.Type == FileType.TextDocument)
            {
                var document = new Document(file.Id, bytesArray, file.Name);
                var indexResult = await _documentIndexStorage.IndexDocumentAsync(document);
                if (indexResult)
                    await SetClassification(file.Id);
                else
                    Console.WriteLine($"Elastic can't index document with id: {file.Id}");
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
            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();
            
            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
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
            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();
            
            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
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
            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();
            
            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
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
            var fileSender = await _senderFromTokenProvider.GetSenderFromToken(request);
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
            var fileSender = await _senderFromTokenProvider.GetSenderFromToken(request);
            if (fileSender is null)
                return RequestResult.BadRequest<(string Uri, Guid Guid)>("Does not have this sender in database");
            var file = CreateFile(FileType.Link, fileSender.Id, uploadTextData.Name);
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(uploadTextData.Value));
            if (!await UploadFile(file, stream))
                return RequestResult.InternalServerError<(string uri, Guid Guid)>("Can't add to database");
            var downloadLink = await _downloadLinkProvider.GetDownloadLinkAsync(file.Id, file.Name);
            return RequestResult.Created<(string Uri, Guid Guid)>((downloadLink, file.Id));
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

        private async Task<bool> UploadFile(DataBaseFile file, Stream stream, string? mimeType = null)
        {
            using var physicalFilesStorage = await _filesStorageFactory.CreateAsync();
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            await physicalFilesStorage.SaveFileAsync(file.Id.ToString(), stream, mimeType);
            return await filesStorage.AddAsync(file);
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

        private async Task SetClassification(Guid fileId)
        {
            const int take = 100;
            var currentSkip = 0;
            while (true)
            {
                var findClassificationResult = await _documentClassificationsService
                    .FindClassificationByQueryAsync("", currentSkip, take, true);
                var currentClassifications = findClassificationResult.Value?.ToList();
                if(currentClassifications is null || currentClassifications.Count == 0)
                    break;
                foreach (var classification in currentClassifications)
                {
                    var words = classification.ClassificationWords.Select(x => x.Value).ToArray();
                    var containsResult = await _documentsSearchService.ContainsInDocumentNameByIdAsync(fileId, words);
                    if (!containsResult.Value)
                        continue;
                    
                    await _documentsService.AddClassification(fileId, classification.Id);
                    break;
                }
                currentSkip += take;
            }
        }
    }
}