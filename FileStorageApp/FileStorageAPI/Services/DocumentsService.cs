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
        public async Task<RequestResult<int>> GetFilesCountAsync(DocumentSearchParameters fileSearchParameters,
            HttpRequest request)
        {
            var convertedParameters = _documentToFileConverter.ConvertFile(fileSearchParameters);
            var filesCount = await _fileService.GetFileInfosAsync(convertedParameters, null, null, request);

            if (filesCount.Value is null)
                return RequestResult.InternalServerError<int>("Something wrong with request");

            if (fileSearchParameters.Phrase is null)
                return RequestResult.Ok(filesCount.Value.Count);

            var foundedDocuments = await _documentIndexStorage.FindInTextOrNameAsync(fileSearchParameters.Phrase);

            var filteredFiles = filesCount.Value
                .Where(x => foundedDocuments.Contains(x.FileId))
                .ToList();

            return RequestResult.Ok(filteredFiles.Count);
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(
            DocumentSearchParameters fileSearchParameters, int skip, int take, HttpRequest request)
        {
            var convertedParameters = _documentToFileConverter.ConvertFile(fileSearchParameters);
            var fileInfo = await _fileService.GetFileInfosAsync(convertedParameters, skip, take, request);

            if (fileSearchParameters.Phrase is null || fileInfo.Value is null)
                return fileInfo;

            var foundedDocuments = await _documentIndexStorage.FindInTextOrNameAsync(fileSearchParameters.Phrase);

            var filteredFiles = fileInfo.Value
                .Where(x => foundedDocuments.Contains(x.FileId))
                .ToList();

            return RequestResult.Ok(filteredFiles);
        }
    }
}