using System;
using System.Threading.Tasks;
using DocumentsIndex.Model;
using Nest;

namespace DocumentsIndex
{
    public class DocumentIndexStorage : IDocumentIndexStorage
    {
        private readonly IElasticClient _elasticClient;

        public DocumentIndexStorage(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<bool> IndexDocument(Guid documentId, string text)
        {
            var document = new Document
            {
                Id = documentId,
                Text = text
            };
            var request = new IndexRequest<Document>(document);
            var result = await _elasticClient.IndexDocumentAsync(request);
            return result.IsValid;
        }

        public async Task<bool> DeleteDocument(Guid documentId)
        {
            var deleteResponse = await _elasticClient.DeleteAsync(new DeleteRequest("attachments",documentId.ToString()));
            return deleteResponse.IsValid;
        }
    }
}