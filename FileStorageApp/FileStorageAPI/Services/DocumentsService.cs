using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API;
using DocumentsIndex;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentToFileConverter _documentToFileConverter;
        private readonly IFileService _fileService;
        private readonly IDocumentIndexStorage _documentIndexStorage;

        /// <summary>
        /// Сервис отвечающий за работу с документами
        /// </summary>
        /// <param name="documentToFileConverter">Преобразователь модели поиска документа в модель поиска файла</param>
        /// <param name="fileService">Сервис отвечающий за работу с файлами</param>
        /// <param name="documentIndexStorage">Хранилище проиндексированных документов</param>
        public DocumentsService(
            IDocumentToFileConverter documentToFileConverter, IFileService fileService,
            IDocumentIndexStorage documentIndexStorage)
        {
            _documentToFileConverter = documentToFileConverter ?? throw new ArgumentNullException(nameof(documentToFileConverter));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _documentIndexStorage = documentIndexStorage ?? throw new ArgumentNullException(nameof(documentIndexStorage));
        }

        /// <inheritdoc />
        public async Task<RequestResult<int>> GetFilesCountAsync(DocumentSearchParameters documentSearchParameters,
            HttpRequest request)
        {
            var foundedDocuments = await TryFindInIndexStorage(documentSearchParameters.Phrase);
            
            var convertedParameters = _documentToFileConverter.ToFileSearchParameters(documentSearchParameters);
            var filesCount = await _fileService.GetDocumentsCountByParametersAndIds(convertedParameters, foundedDocuments, request);

            return RequestResult.Ok(filesCount.Value);
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(
            DocumentSearchParameters documentSearchParameters, int skip, int take, HttpRequest request)
        {
            var foundedDocuments = await TryFindInIndexStorage(documentSearchParameters.Phrase);
            
            var convertedParameters = _documentToFileConverter.ToFileSearchParameters(documentSearchParameters);
            var fileInfo = await _fileService.GetDocumentsByParametersAndIds(convertedParameters, foundedDocuments, request, skip, take);

            return fileInfo;
        }

        private async Task<List<Guid>?> TryFindInIndexStorage(string? phrase)
        {
            if (phrase is null)
                return null;
            return await _documentIndexStorage.FindInTextOrNameAsync(phrase);
        }
    }
}