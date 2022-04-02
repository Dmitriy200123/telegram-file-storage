using System;
using System.Threading.Tasks;
using API;
using DocumentsIndex;
using DocumentsIndex.Contracts;

namespace SearchDocumentsAPI.Services.DocumentsIndex
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
        public async Task<RequestResult<Document>> IndexDocumentAsync(Document document)
        {
            var success = await _documentIndexStorage.IndexDocumentAsync(document);

            return success
                ? RequestResult.NoContent<Document>()
                : RequestResult.BadRequest<Document>("Invalid document");
        }

        /// <inheritdoc />
        public async Task<RequestResult<Document>> DeleteAsync(Guid id)
        {
            var success = await _documentIndexStorage.DeleteAsync(id);
            
            return success
                ? RequestResult.NoContent<Document>()
                : RequestResult.BadRequest<Document>("Invalid id");
        }
    }
}