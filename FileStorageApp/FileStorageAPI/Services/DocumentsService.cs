using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API;
using Data;
using DocumentsIndex;
using FileStorageAPI.Converters;
using FileStorageAPI.Extensions;
using FileStorageAPI.Models;
using FileStorageAPI.Providers;
using FileStorageApp.Data.InfoStorage.Enums;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;
using DataBaseFile = FileStorageApp.Data.InfoStorage.Models.File;
using Chat = FileStorageApp.Data.InfoStorage.Models.Chat;


namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentToFileConverter _documentToFileConverter;
        private readonly IFileToDocumentInfoConverter _fileToDocumentInfoConverter;
        private readonly IClassificationToClassificationInfoConverter _classificationConverter;
        private readonly IDocumentIndexStorage _documentIndexStorage;
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IExpressionFileFilterProvider _expressionFileFilterProvider;
        private readonly ISenderFromTokenProvider _senderFromTokenProvider;
        private readonly IAccessService _accessService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DocumentsService"/>.
        /// </summary>
        /// <param name="documentToFileConverter">Преобразователь модели поиска документа в модель поиска файла</param>
        /// <param name="documentIndexStorage">Хранилище проиндексированных документов</param>
        /// <param name="infoStorageFactory">Фабрика для создания хранилищ</param>
        /// <param name="fileToDocumentInfoConverter">Конвертер File в DocumentInfo</param>
        /// <param name="classificationConverter">Конвертер DocumentClassification в ClassificationInfo</param>
        /// <param name="expressionFileFilterProvider">Поставщик query Expression для поиска данных</param>
        /// <param name="senderFromTokenProvider">Поставщик отправителя файла из токена</param>
        /// <param name="accessService">Сервис отвечающий за опции доступа</param>
        public DocumentsService(
            IDocumentToFileConverter documentToFileConverter,
            IDocumentIndexStorage documentIndexStorage,
            IInfoStorageFactory infoStorageFactory,
            IFileToDocumentInfoConverter fileToDocumentInfoConverter,
            IClassificationToClassificationInfoConverter classificationConverter,
            IExpressionFileFilterProvider expressionFileFilterProvider,
            ISenderFromTokenProvider senderFromTokenProvider,
            IAccessService accessService)
        {
            _documentToFileConverter = documentToFileConverter ?? throw new ArgumentNullException(nameof(documentToFileConverter));
            _documentIndexStorage = documentIndexStorage ?? throw new ArgumentNullException(nameof(documentIndexStorage));
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _fileToDocumentInfoConverter = fileToDocumentInfoConverter ?? throw new ArgumentNullException(nameof(fileToDocumentInfoConverter));
            _classificationConverter = classificationConverter ?? throw new ArgumentNullException(nameof(classificationConverter));
            _expressionFileFilterProvider = expressionFileFilterProvider ?? throw new ArgumentNullException(nameof(expressionFileFilterProvider));
            _senderFromTokenProvider = senderFromTokenProvider ?? throw new ArgumentNullException(nameof(senderFromTokenProvider));
            _accessService = accessService ?? throw new ArgumentNullException(nameof(accessService));
        }

        /// <inheritdoc />
        public async Task<RequestResult<int>> GetDocumentsCountAsync(DocumentSearchParameters documentSearchParameters,
            HttpRequest request)
        {
            var foundedDocuments = await TryFindInIndexStorage(documentSearchParameters.Phrase);

            var fileSearchParameters = _documentToFileConverter.ToFileSearchParameters(documentSearchParameters);
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();

            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
            var chatsId = hasAnyFilesAccess ? null : sender.Chats.Select(chat => chat.Id).ToList();
            var expression =
                _expressionFileFilterProvider.GetDocumentExpression(fileSearchParameters, foundedDocuments, chatsId);
            var filesCount = await filesStorage.GetFilesCountAsync(expression);

            return RequestResult.Ok(filesCount);
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<DocumentInfo>>> GetDocumentInfosAsync(
            DocumentSearchParameters documentSearchParameters, int skip, int take, HttpRequest request)
        {
            var foundedDocuments = await TryFindInIndexStorage(documentSearchParameters.Phrase);

            var fileSearchParameters = _documentToFileConverter.ToFileSearchParameters(documentSearchParameters);
            if (skip < 0 || take < 0)
                return RequestResult.BadRequest<List<DocumentInfo>>($"Skip or take less than 0");
            var sender = (await _senderFromTokenProvider.GetSenderFromToken(request)).CheckForNull();
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var hasAnyFilesAccess = await _accessService.HasAccessAsync(request, Access.ViewAnyFiles);
            var chatsId = hasAnyFilesAccess ? null : sender!.Chats.Select(chat => chat.Id).ToList();

            var expression = _expressionFileFilterProvider.GetDocumentExpression(fileSearchParameters, foundedDocuments, chatsId);
            var files = await GetFileInfoFromStorage(expression, skip, take);

            return RequestResult.Ok(files.Select(_fileToDocumentInfoConverter.ConvertToDocumentInfo).ToList());
        }

        private async Task<List<Guid>?> TryFindInIndexStorage(string? phrase)
        {
            if (phrase is null)
                return null;
            return await _documentIndexStorage.FindInTextOrNameAsync(phrase);
        }

        /// <inheritdoc />
        public async Task<RequestResult<DocumentInfo>> FindDocumentById(Guid id)
        {
            using var storage = _infoStorageFactory.CreateFileStorage();
            var file = await storage.GetByIdAsync(id, true);

            if (file is not { Type: FileType.TextDocument })
                return RequestResult.NotFound<DocumentInfo>($"Not found {nameof(DocumentInfo)} with Id {id}");

            var documentInfo = _fileToDocumentInfoConverter.ConvertToDocumentInfo(file);

            return RequestResult.Ok(documentInfo);
        }

        /// <inheritdoc />
        public async Task<RequestResult<ClassificationInfo?>> FindClassificationByDocumentId(Guid documentId)
        {
            using var storage = _infoStorageFactory.CreateFileStorage();
            var file = await storage.GetByIdAsync(documentId, true);

            if (file is not { Type: FileType.TextDocument })
                return RequestResult.NotFoundEntity<ClassificationInfo?>(
                    $"Not found {nameof(DocumentInfo)} with Id {documentId}",
                    nameof(DocumentInfo)
                );

            if (file.Classification == null)
                return RequestResult.NotFoundEntity<ClassificationInfo?>(
                    $"Not found {nameof(ClassificationInfo)} for {nameof(DocumentInfo)} with Id {documentId}",
                    nameof(ClassificationInfo)
                );

            var classificationInfo = _classificationConverter.ConvertToClassificationInfo(file.Classification);

            return RequestResult.Ok(classificationInfo);
        }

        /// <inheritdoc />
        public async Task<RequestResult<DocumentInfo>> AddClassification(Guid documentId, Guid classificationId)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();

            try
            {
                var addResult = await filesStorage.SetClassificationAsync(documentId, classificationId);

                if (!addResult)
                    return RequestResult.InternalServerError<DocumentInfo>("Something wrong with database");

                var file = await filesStorage.GetByIdAsync(documentId, true);
                return RequestResult.Ok(_fileToDocumentInfoConverter.ConvertToDocumentInfo(file!));
            }
            catch (NotFoundException e)
            {
                return e.EntityName == nameof(File)
                    ? RequestResult.NotFoundEntity<DocumentInfo>($"No such document {documentId} in database", nameof(DocumentInfo))
                    : RequestResult.NotFoundEntity<DocumentInfo>($"No such classification {classificationId} in database", nameof(ClassificationInfo));
            }
        }

        /// <inheritdoc />
        public async Task<RequestResult<DocumentInfo>> DeleteClassification(Guid documentId, Guid classificationId)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();

            try
            {
                var deleteResult = await filesStorage.DeleteClassificationAsync(documentId, classificationId);
                if (!deleteResult)
                    return RequestResult.InternalServerError<DocumentInfo>("Something wrong with database");

                var file = await filesStorage.GetByIdAsync(documentId, true);
                return RequestResult.Ok(_fileToDocumentInfoConverter.ConvertToDocumentInfo(file!));
            }
            catch (NotFoundException e)
            {
                return e.EntityName == nameof(File)
                    ? RequestResult.NotFoundEntity<DocumentInfo>($"No such document {documentId} in database", nameof(DocumentInfo))
                    : RequestResult.NotFoundEntity<DocumentInfo>($"No such classification {classificationId} in database", nameof(ClassificationInfo));
            }
        }

        private async Task<List<DataBaseFile>> GetFileInfoFromStorage(Expression<Func<DataBaseFile, bool>> expression, int? skip, int? take)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var filesFromDataBase = await filesStorage.GetByFilePropertiesAsync(expression, true, skip, take);
            SetFileChat(filesFromDataBase);
            return filesFromDataBase;
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