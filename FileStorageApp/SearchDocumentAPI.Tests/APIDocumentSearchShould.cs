using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            var response = await apiClient.GetAsync($"api/documents/search?query={query}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Guid[]>(content);

            actual.Should().BeEquivalentTo(expected);
        }

        [TestCaseSource(typeof(APIDocumentSearchData), nameof(APIDocumentSearchData.TextsContainsInDocumentNameSource))]
        public async Task TextsContainsInDocumentName_OK_ThenCalled(Document document, string[] texts, bool expected)
        {
            await _documentIndexStorage.IndexDocumentAsync(document);
            
            var json = JsonConvert.SerializeObject(texts);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            
            using var apiClient = CreateHttpClient();
            var response = await apiClient.PostAsync($"api/documents/{document.Id}/containsInName", stringContent);
            
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