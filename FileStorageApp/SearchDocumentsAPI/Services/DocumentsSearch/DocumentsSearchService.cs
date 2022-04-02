using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentsIndex;
using FileStorageAPI;

namespace SearchDocumentsAPI.Services.DocumentsSearch
{
    /// <inheritdoc />
    public class DocumentsSearchService : IDocumentsSearchService
    {
        private readonly IDocumentIndexStorage _documentIndexStorage;

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="DocumentsSearchService"/>
        /// </summary>
        /// <param name="documentIndexStorage">Хранилище документов</param>
        public DocumentsSearchService(IDocumentIndexStorage documentIndexStorage)
        {
            _documentIndexStorage = documentIndexStorage;
        }

        /// <inheritdoc />
        public async Task<RequestResult<IEnumerable<Guid>>> FindMatchingDocumentsAsync(string query)
        {
            var documentIds = await _documentIndexStorage.FindInTextOrNameAsync(query);

            return RequestResult.Ok(documentIds);
        }

        /// <inheritdoc />
        public async Task<RequestResult<bool>> ContainsInDocumentNameByIdAsync(Guid documentId, string[] queries)
        {
            var contains = await _documentIndexStorage.IsContainsInNameAsync(documentId, queries);

            return RequestResult.Ok(contains);
        }
    }
}