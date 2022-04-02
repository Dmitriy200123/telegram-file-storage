using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DocumentsIndex;
using DocumentsIndex.Config;
using DocumentsIndex.Contracts;
using DocumentsIndex.Factories;
using DocumentsIndex.Pipelines;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using SearchDocumentAPI.Tests.Data;
using SearchDocumentsAPI;

namespace SearchDocumentAPI.Tests
{
    public class APIDocumentSearchShould
    {
        private WebApplicationFactory<Startup> _applicationFactory;
        private IElasticConfig _config;
        private IDocumentIndexStorage _documentIndexStorage;

        [SetUp]
        public void Setup()
        {
            _applicationFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => { builder.UseEnvironment("Debug"); });

            _config = new ElasticConfig("http://localhost:9200", "testindex");
            var factory = new DocumentIndexFactory(new PipelineCreator());
            _documentIndexStorage = factory.CreateDocumentIndexStorage(_config);
        }

        [TearDown]
        public async Task TearDown()
        {
            var searchResponse = await _documentIndexStorage.SearchBySubstringAsync("");
            foreach (var hit in searchResponse)
                await _documentIndexStorage.DeleteAsync(hit);
        }


        [TestCaseSource(typeof(APIDocumentSearchData), nameof(APIDocumentSearchData.NameDocumentsSource))]
        [TestCaseSource(typeof(APIDocumentSearchData), nameof(APIDocumentSearchData.ContentDocumentsSource))]
        [TestCaseSource(typeof(APIDocumentSearchData), nameof(APIDocumentSearchData.ContentOrNameDocumentsSource))]
        [TestCaseSource(typeof(APIDocumentSearchData), nameof(APIDocumentSearchData.EmptyDocumentsSource))]
        public async Task SearchDocuments_OK_ThenCalled(string query, Document[] documents, Guid[] expected)
        {
            foreach (var document in documents)
                await _documentIndexStorage.IndexDocumentAsync(document);


            using var apiClient = CreateHttpClient();
            var response = await apiClient.GetAsync($"api/search/documents?query={query}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Guid[]>(content);

            actual.Should().BeEquivalentTo(expected);
        }

        [TestCaseSource(typeof(APIDocumentSearchData), nameof(APIDocumentSearchData.TextsContainsInDocumentNameSource))]
        public async Task QueriesContainsInDocumentName_OK_ThenCalled(Document document, string[] queries, bool expected)
        {
            await _documentIndexStorage.IndexDocumentAsync(document);
            
            var queryContent = string.Join("&", queries.Select(query => $"queries={query}"));
            
            using var apiClient = CreateHttpClient();
            var response = await apiClient.GetAsync($"api/search/documents/{document.Id}/containsInName?{queryContent}");
            
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<bool>(content);

            actual.Should().Be(expected);
        }
        
        private HttpClient CreateHttpClient()
        {
            var client = _applicationFactory.CreateClient();
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}