using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentsIndex.Model;
using Elasticsearch.Net;
using Nest;

namespace DocumentsIndex
{
    /// <summary>
    /// Хранилище отвечающее за работу с документами
    /// </summary>
    public class DocumentIndexStorage : IDocumentIndexStorage
    {
        private readonly IElasticClient _elasticClient;

        /// <summary>
        /// Конструктор создающий хранилище
        /// </summary>
        /// <param name="elasticClient">Параметры для создания хранилища</param>
        public DocumentIndexStorage(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Guid>> SearchBySubstringAsync(string subString)
        {
            var searchResponse = await _elasticClient.SearchAsync<ElasticDocument>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(a => a.Attachment.Content)
                        .Query(subString)
                    )
                )
            );
            return searchResponse.Hits.Select(x => x.Source.Id);
        }

        /// <inheritdoc />
        public async Task<bool> IndexDocumentAsync(Document document)
        {
            var base64File = Convert.ToBase64String(document.Content);
            var indexResponse = await _elasticClient.IndexAsync(new ElasticDocument
                {
                    Id = document.Id,
                    Content = base64File,
                    Name = document.Name
                }, i => i
                    .Pipeline(_elasticClient.ConnectionSettings.DefaultIndex)
                    .Refresh(Refresh.WaitFor)
            );
            return indexResponse.IsValid;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(Guid guid)
        {
            var response =
                await _elasticClient.DeleteAsync(new DeleteRequest(_elasticClient.ConnectionSettings.DefaultIndex,
                    guid.ToString()));
            return response.IsValid;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Guid>> SearchByNameAsync(string name)
        {
            var searchResponse = await _elasticClient.SearchAsync<ElasticDocument>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(a => a.Name)
                        .Query(name)
                    )
                )
            );
            return searchResponse.Hits.Select(x => x.Source.Id);
        }
    }
}