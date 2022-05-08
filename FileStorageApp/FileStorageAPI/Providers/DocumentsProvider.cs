using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Helpers;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Http;
using DataBaseFile = FileStorageApp.Data.InfoStorage.Models.File;
using FileInfo = FileStorageAPI.Models.FileInfo;
using Chat = FileStorageApp.Data.InfoStorage.Models.Chat;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class DocumentsProvider : IDocumentsProvider
    {
        private readonly IExpressionFileFilterProvider _expressionFileFilterProvider;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IFileInfoConverter _fileInfoConverter;
        private readonly IDocumentToFileConverter _documentToFileConverter;

        public DocumentsProvider(IExpressionFileFilterProvider expressionFileFilterProvider,
            IHttpRequestHelper httpRequestHelper, IInfoStorageFactory infoStorageFactory,
            IFileInfoConverter fileInfoConverter, IDocumentToFileConverter documentToFileConverter)
        {
            _expressionFileFilterProvider = expressionFileFilterProvider ?? throw new ArgumentNullException(nameof(expressionFileFilterProvider));
            _httpRequestHelper = httpRequestHelper ?? throw new ArgumentNullException(nameof(httpRequestHelper));
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _fileInfoConverter = fileInfoConverter ?? throw new ArgumentNullException(nameof(fileInfoConverter));
            _documentToFileConverter = documentToFileConverter  ?? throw new ArgumentNullException(nameof(documentToFileConverter));
        }

        /// <inheritdoc />
        public async Task<int> GetDocumentsCountByParametersAndIds(FileSearchParameters fileSearchParameters,
            List<Guid>? guidsToFind, HttpRequest request)
        {
            using var filesStorage = _infoStorageFactory.CreateFileStorage();
            var sender = await _httpRequestHelper.GetNotNullSenderAsync(request);

            var hasAnyFilesAccess = await _httpRequestHelper.HasAnyFilesAccessAsync(request);
            var chatsId = hasAnyFilesAccess ? null : sender.Chats.Select(chat => chat.Id).ToList();
            var expression =
                _expressionFileFilterProvider.GetDocumentExpression(fileSearchParameters, guidsToFind, chatsId);
            var filesCount = await filesStorage.GetFilesCountAsync(expression);

            return filesCount;
        }

        /// <inheritdoc />
        public async Task<List<DocumentInfo>> GetDocumentsByParametersAndIds(
            FileSearchParameters fileSearchParameters, List<Guid>? fileIds, HttpRequest request, int skip, int take)
        {
            var chatsId = await _httpRequestHelper.GetUserChats(request);

            var expression =
                _expressionFileFilterProvider.GetDocumentExpression(fileSearchParameters, fileIds, chatsId);
            var files = await GetFileInfoFromStorage(expression, skip, take);

            return files.Select(_documentToFileConverter.ToDocumentModel).ToList();
        }

        /// <inheritdoc />
        public async Task<List<FileInfo>> GetFileInfoFromStorage(Expression<Func<DataBaseFile, bool>> expression,
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