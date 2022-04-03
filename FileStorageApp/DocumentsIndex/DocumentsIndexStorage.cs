using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentsIndex.Contracts;
using DocumentsIndex.Model;
using Elasticsearch.Net;
using Nest;
using Analyzers = DocumentsIndex.Constants.Analyzers;

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
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        /// <inheritdoc />
        public async Task<List<Guid>> SearchBySubstringAsync(string subString)
        {
            var searchResponse = await _elasticClient.SearchAsync<ElasticDocument>(s => s
                .Query(q => q
                    .QueryString(m => m
                        .Fields(f => f
                            .Field(p => p.Attachment.Content))
                        .Query(subString)
                        .Analyzer(Analyzers.DocumentNgramAnalyzer)
                    )
                ));
            return searchResponse.Hits.Select(x => x.Source.Id).ToList();
        }

        /// <inheritdoc />
        public async Task<bool> IndexDocumentAsync(Document document)
        {
            var base64File = Convert.ToBase64String(document.Content);
            var indexResponse = await _elasticClient.IndexAsync(new ElasticDocument
                {
                    Id = document.Id,
                    Content = base64File,
                    Name = document.Name.ReplaceDelimitersToWhiteSpaces()
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
        public async Task<List<Guid>> FindInTextOrNameAsync(string query)
        {
            var searchResponse = await _elasticClient.SearchAsync<ElasticDocument>(s => s
                .Query(q =>
                    q.QueryString(m => m
                        .Fields(f => f
                            .Field(p => p.Attachment.Content))
                        .Query(query)
                        .Analyzer(Analyzers.DocumentNgramAnalyzer)
                    )
                    || q.QueryString(m => m
                        .Fields(f => f
                            .Field(p => p.Name))
                        .Query(query)
                        .Analyzer(Analyzers.DocumentNgramAnalyzer))
                ));
            return searchResponse.Hits.Select(x => x.Source.Id).ToList();
        }

        /// <inheritdoc />
        public async Task<bool> IsContainsInNameAsync(Guid documentId, string[] subStrings)
        {
            var searchResponses = await _elasticClient
                .SearchAsync<ElasticDocument>(s => s
                    .Query(q => CreateForSubStrings(q, subStrings)
                                && q.Bool(b => b
                                    .Must(m => m
                                        .Match(ma => ma
                                            .Field(f => f.Id)
                                            .Query(documentId.ToString()))))
                    )
                );

            return searchResponses.Hits.Any();
        }
        
        /// <inheritdoc />
        public async Task<List<Guid>> SearchByNameAsync(string name)
        {
            var searchResponse = await _elasticClient.SearchAsync<ElasticDocument>(s => s
                .Query(q => q
                    .QueryString(m => m
                        .Fields(f => f
                            .Field(p => p.Name))
                        .Query(name)
                        .Analyzer(Analyzers.DocumentNgramAnalyzer))
                )
            );
            return searchResponse.Hits.Select(x => x.Source.Id).ToList();
        }
        
        private QueryContainer CreateForSubStrings(
            QueryContainerDescriptor<ElasticDocument> descriptor, string[] subStrings)
        {
            return subStrings
                .Aggregate(new QueryContainer(), (current, subString) => current || descriptor
                    .QueryString(c => c
                        .Fields(f => f
                            .Field(p => p.Name))
                        .Query(subString)
                        .Analyzer(Analyzers.DocumentNgramAnalyzer)));
        }

    }
}