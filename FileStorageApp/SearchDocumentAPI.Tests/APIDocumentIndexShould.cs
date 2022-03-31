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
using SearchDocumentsAPI;

namespace SearchDocumentAPI.Tests
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
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Debug");
                });
            
            _config = new ElasticConfig("http://localhost:9200", "testindex");
            var factory = new DocumentIndexFactory(new PipelineCreator());
            _documentIndexStorage = factory.CreateDocumentIndexStorage(_config);
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

            var id = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(id, bytes, DocumentName);

            var json = JsonConvert.SerializeObject(document);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var apiClient = CreateHttpClient();
            var response = await apiClient.PostAsync($"/api/documents/indexDocument", stringContent);

            response.EnsureSuccessStatusCode();
            var actual = response.StatusCode;

            actual.Should().Be(expected);
        }
        
        [Test]
        public async Task DeleteDocumentById_NoContent_ThenCalled()
        {
            const HttpStatusCode expected = HttpStatusCode.NoContent;
            
            var id = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(id, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);
            
            using var apiClient = CreateHttpClient();
            var response = await apiClient.DeleteAsync($"/api/documents/indexDocument/{id}");

            response.EnsureSuccessStatusCode();
            var actual = response.StatusCode;

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
        
        private static async Task<byte[]> ReadBytesFromFileName(string fileName)
        {
            var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/FilesForTest/{fileName}");
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}