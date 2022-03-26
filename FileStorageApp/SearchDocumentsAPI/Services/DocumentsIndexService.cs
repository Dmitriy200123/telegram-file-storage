using System;
using System.Threading.Tasks;
using DocumentsIndex;
using DocumentsIndex.Model;

namespace SearchDocumentsAPI.Services
{
    /// <inheritdoc />
    public class DocumentsIndexService : IDocumentsIndexService
    {
        private readonly IDocumentIndexStorage _documentIndexStorage;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DocumentsIndexService"/> 
        /// </summary>
        /// <param name="documentIndexStorage">Хранилище для работы с индексацией документов</param>
        public DocumentsIndexService(IDocumentIndexStorage documentIndexStorage)
        {
            _documentIndexStorage = documentIndexStorage ??
                                    throw new ArgumentNullException(nameof(documentIndexStorage));
        }

        /// <inheritdoc />
        public async Task<bool> IndexDocumentAsync(Document document) =>
            await _documentIndexStorage.IndexDocumentAsync(document);

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid id) => await _documentIndexStorage.DeleteAsync(id);
    }
}