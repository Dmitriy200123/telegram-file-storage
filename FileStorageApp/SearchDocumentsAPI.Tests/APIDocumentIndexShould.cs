using System;
using System.IO;
using System.Net;
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

namespace SearchDocumentsAPI.Tests
{
    public class APIDocumentIndexShould
    {
        private WebApplicationFactory<Startup> _applicationFactory;
        private IElasticConfig _config;
        private IDocumentIndexStorage _documentIndexStorage;
        private const string WordThatContainsDocument = "NEST";
        private const string DocumentName = "example_one.docx";

        [SetUp]
        public void Setup()
        {
            _applicationFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => { builder.UseEnvironment("Debug"); });

            _config = new ElasticConfig("http://localhost:9200", "testindex");
            var factory = new DocumentIndexFactory(new PipelineCreator(), _config);
            _documentIndexStorage = factory.CreateDocumentIndexStorage();
        }

        [TearDown]
        public async Task TearDown()
        {
            var searchResponse = await _documentIndexStorage.SearchBySubstringAsync(WordThatContainsDocument);
            foreach (var hit in searchResponse)
                await _documentIndexStorage.DeleteAsync(hit);
        }

        [Test]
        public async Task IndexDocument_NoContent_ThenCalled()
        {
            const HttpStatusCode expected = HttpStatusCode.NoContent;
            
            var document = await CreateDocumentAsync();

            var json = JsonConvert.SerializeObject(document);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var apiClient = CreateHttpClient();
            var response = await apiClient.PostAsync($"/api/index/documents", stringContent);

            response.EnsureSuccessStatusCode();

            response.StatusCode.Should().Be(expected);
        }

        [Test]
        public async Task DeleteDocumentById_NoContent_ThenCalled()
        {
            const HttpStatusCode expected = HttpStatusCode.NoContent;
            
            var document = await CreateDocumentAsync();
            await _documentIndexStorage.IndexDocumentAsync(document);

            using var apiClient = CreateHttpClient();
            var response = await apiClient.DeleteAsync($"/api/index/documents/{document.Id}");

            response.EnsureSuccessStatusCode();

            response.StatusCode.Should().Be(expected);
        }

        private static async Task<Document> CreateDocumentAsync(Guid id = default, string documentName = DocumentName)
        {
            if (id == default)
                id = Guid.NewGuid();
            
            var bytes = await ReadBytesFromFileName(documentName);
            
            return new Document(id, bytes, documentName);
        }

        private HttpClient CreateHttpClient()
        {
            var client = _applicationFactory.CreateClient();
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private static async Task<byte[]> ReadBytesFromFileName(string fileName)
        {
            var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/FilesForTest/{fileName}");
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}