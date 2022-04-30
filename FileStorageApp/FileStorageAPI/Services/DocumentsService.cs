using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API;
using DocumentsIndex;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
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
        private readonly IFileService _fileService;
        private readonly IDocumentIndexStorage _documentIndexStorage;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DocumentsService"/>.
        /// </summary>
        /// <param name="documentToFileConverter">Преобразователь модели поиска документа в модель поиска файла</param>
        /// <param name="fileService">Сервис отвечающий за работу с файлами</param>
        /// <param name="documentIndexStorage">Хранилище проиндексированных документов</param>
        /// <param name="infoStorageFactory">Фабрика для создания хранилищ</param>
        /// <param name="fileToDocumentInfoConverter">Конвертер File в DocumentInfo</param>
        /// <param name="classificationConverter">Конвертер DocumentClassification в ClassificationInfo</param>
        public DocumentsService(
            IDocumentToFileConverter documentToFileConverter, IFileService fileService,
            IDocumentIndexStorage documentIndexStorage,
            IInfoStorageFactory infoStorageFactory,
            IFileToDocumentInfoConverter fileToDocumentInfoConverter,
            IClassificationToClassificationInfoConverter classificationConverter
        )
        {
            _documentToFileConverter = documentToFileConverter ?? throw new ArgumentNullException(nameof(documentToFileConverter));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _documentIndexStorage = documentIndexStorage ?? throw new ArgumentNullException(nameof(documentIndexStorage));
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _fileToDocumentInfoConverter = fileToDocumentInfoConverter ?? throw new ArgumentNullException(nameof(fileToDocumentInfoConverter));
            _classificationConverter = classificationConverter ?? throw new ArgumentNullException(nameof(classificationConverter));
        }

        /// <inheritdoc />
        public async Task<RequestResult<int>> GetFilesCountAsync(DocumentSearchParameters documentSearchParameters,
            HttpRequest request)
        {
            var foundedDocuments = await TryFindInIndexStorage(documentSearchParameters.Phrase);
            
            var fileSearchParameters = _documentToFileConverter.ToFileSearchParameters(documentSearchParameters);
            var filesCount = await _fileService.GetDocumentsCountByParametersAndIds(fileSearchParameters, foundedDocuments, request);

            return RequestResult.Ok(filesCount.Value);
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<DocumentInfo>>> GetFileInfosAsync(
            DocumentSearchParameters documentSearchParameters, int skip, int take, HttpRequest request)
        {
            var foundedDocuments = await TryFindInIndexStorage(documentSearchParameters.Phrase);
            
            var fileSearchParameters = _documentToFileConverter.ToFileSearchParameters(documentSearchParameters);
            var fileInfo = await _fileService.GetDocumentsByParametersAndIds(fileSearchParameters, foundedDocuments, request, skip, take);

            return fileInfo.EditReturnValueIfExist(x => x!.Select(_documentToFileConverter.ToDocumentModel).ToList());
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
                return RequestResult.NotFound<ClassificationInfo?>($"Not found {nameof(DocumentInfo)} with Id {documentId}");
            
            if (file.Classification == null)
                return RequestResult.NotFound<ClassificationInfo?>($"Not found {nameof(ClassificationInfo)} for {nameof(DocumentInfo)} with Id {documentId}");

            var classificationInfo = _classificationConverter.ConvertToClassificationInfo(file.Classification);
            
            return RequestResult.Ok(classificationInfo);
        }
    }
}