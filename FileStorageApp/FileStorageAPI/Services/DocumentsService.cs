using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API;
using DocumentsIndex;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageAPI.Providers;
using FileStorageApp.Data.InfoStorage.Enums;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Http;

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
        private readonly IDocumentsProvider _documentsProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DocumentsService"/>.
        /// </summary>
        /// <param name="documentToFileConverter">Преобразователь модели поиска документа в модель поиска файла</param>
        /// <param name="documentIndexStorage">Хранилище проиндексированных документов</param>
        /// <param name="infoStorageFactory">Фабрика для создания хранилищ</param>
        /// <param name="fileToDocumentInfoConverter">Конвертер File в DocumentInfo</param>
        /// <param name="classificationConverter">Конвертер DocumentClassification в ClassificationInfo</param>
        /// <param name="documentsProvider">Поставщик документов по параметрам</param>
        public DocumentsService(
            IDocumentToFileConverter documentToFileConverter,
            IDocumentIndexStorage documentIndexStorage,
            IInfoStorageFactory infoStorageFactory,
            IFileToDocumentInfoConverter fileToDocumentInfoConverter,
            IClassificationToClassificationInfoConverter classificationConverter, 
            IDocumentsProvider documentsProvider)
        {
            _documentToFileConverter = documentToFileConverter ?? throw new ArgumentNullException(nameof(documentToFileConverter));
            _documentIndexStorage = documentIndexStorage ?? throw new ArgumentNullException(nameof(documentIndexStorage));
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _fileToDocumentInfoConverter = fileToDocumentInfoConverter ?? throw new ArgumentNullException(nameof(fileToDocumentInfoConverter));
            _classificationConverter = classificationConverter ?? throw new ArgumentNullException(nameof(classificationConverter));
            _documentsProvider = documentsProvider ?? throw new ArgumentNullException(nameof(documentsProvider));
        }

        /// <inheritdoc />
        public async Task<RequestResult<int>> GetDocumentsCountAsync(DocumentSearchParameters documentSearchParameters,
            HttpRequest request)
        {
            var foundedDocuments = await TryFindInIndexStorage(documentSearchParameters.Phrase);
            
            var fileSearchParameters = _documentToFileConverter.ToFileSearchParameters(documentSearchParameters);
            var filesCount = await _documentsProvider.GetDocumentsCountByParametersAndIds(fileSearchParameters, foundedDocuments, request);

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
            var fileInfo = await _documentsProvider.GetDocumentsByParametersAndIds(fileSearchParameters, foundedDocuments, request, skip, take);

            return RequestResult.Ok(fileInfo);
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
    }
}