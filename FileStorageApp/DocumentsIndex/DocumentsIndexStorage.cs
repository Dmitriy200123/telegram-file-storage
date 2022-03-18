using System;
using System.Threading.Tasks;
using DocumentsIndex.Model;
using Elasticsearch.Net;
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

        public async Task<ISearchResponse<ElasticDocument>> Search(string subString)
        {
            var searchResponse = await _elasticClient.SearchAsync<ElasticDocument>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(a => a.Attachment.Content)
                        .Query(subString)
                    )
                )
            );
            return searchResponse;
        }

        public async Task<bool> IndexDocument(byte[] content, Guid guid)
        {
            var base64File = Convert.ToBase64String(content);
            var indexResponse = await _elasticClient.IndexAsync(new ElasticDocument
                {
                    Id = guid,
                    Content = base64File
                }, i => i
                    .Pipeline("attachments")
                    .Refresh(Refresh.WaitFor)
            );
            return indexResponse.IsValid;
        }

        public async Task<bool> Delete(Guid guid)
        {
            var response = await _elasticClient.DeleteAsync(new DeleteRequest("index", guid.ToString()));
            return response.IsValid;
        }
    }
}