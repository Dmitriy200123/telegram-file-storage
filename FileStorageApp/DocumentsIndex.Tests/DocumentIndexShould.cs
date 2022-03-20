using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentsIndex.Config;
using DocumentsIndex.Factories;
using DocumentsIndex.Model;
using DocumentsIndex.Pipelines;
using FluentAssertions;
using NUnit.Framework;

#pragma warning disable CS8618

namespace DocumentsIndex.Tests
{
    public class DocumentIndexShould
    {
        private IElasticConfig _config;
        private IDocumentIndexStorage _documentIndexStorage;
        private const string WordThatContainsDocument = "NEST";
        private const string DocumentName = "example_one.docx";

        [SetUp]
        public void Setup()
        {
            _config = new ElasticConfig("http://localhost:9200", "testindex");
            var factory = new DocumentIndexFactory(new PipelineCreator());
            _documentIndexStorage = factory.CreateDocumentIndexStorage(_config);
        }

        [TearDown]
        public void TearDown()
        {
            var searchResponse = _documentIndexStorage.SearchBySubstringAsync(WordThatContainsDocument).GetAwaiter().GetResult();
            foreach (var hit in searchResponse)
                _documentIndexStorage.DeleteAsync(hit);
        }

        [Test]
        public async Task IndexDocumentBySubstring_SuccessfullyUpload_ThenCalled()
        {
            var expected = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(expected, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);
            var searchResponse = _documentIndexStorage.SearchBySubstringAsync("NEST").GetAwaiter().GetResult();

            var actual = searchResponse.First();

            actual.Should().Be(expected);
        }

        [Test]
        public async Task DeleteDocument_SuccessfullyDelete_ThenCalled()
        {
            var guid = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(guid, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.DeleteAsync(guid);

            actual.Should().BeTrue();
        }

        [Test]
        public async Task IndexDocumentByName_SuccessfullyUpload_ThenCalled()
        {
            var expected = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(expected, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);
            var searchResponse = _documentIndexStorage.SearchByNameAsync(DocumentName).GetAwaiter().GetResult();

            var actual = searchResponse.First();

            actual.Should().Be(expected);
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