using System;
using System.Linq;
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
            var document = new ElasticDocument
            {
                Text = "Name",
                Id = Guid.NewGuid()
            };
            var result = await _elasticClient.IndexDocumentAsync(document);
            return result.IsValid;
        }

        public async Task<bool> DeleteDocument(Guid documentId)
        {
            var deleteResponse = await _elasticClient.DeleteByQueryAsync<ElasticDocument>(x => x
                .Query(qx => qx
                    .Match(y => y
                        .Field(f => f.Id)
                        .Query(documentId.ToString()))));
            return deleteResponse.IsValid;
        }

        public ElasticDocument? GetDoc(Guid guid)
        {
            var a = _elasticClient.Search<ElasticDocument>();
            var firstHit = a.Hits.FirstOrDefault();
            return firstHit?.Source;
        }

        private async void Clear()
        {
            var a = await _elasticClient.DeleteByQueryAsync<ElasticDocument>(del => del
                .Query(q => q.QueryString(qs => qs.Query("*")))
            );
        }
    }
}