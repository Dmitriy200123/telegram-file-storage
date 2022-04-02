using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentsIndex.Config;
using DocumentsIndex.Contracts;
using DocumentsIndex.Factories;
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
        public async Task IndexDocumentBySubstring_SuccessfullyUpload_ThenCalled()
        {
            var expected = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(expected, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);
            var searchResponse = await _documentIndexStorage.SearchBySubstringAsync("NEST");

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
            var searchResponse = await _documentIndexStorage.SearchByNameAsync(DocumentName);

            var actual = searchResponse.First();

            actual.Should().Be(expected);
        }

        [TestCase(DocumentName)]
        [TestCase("attachments")]
        [TestCase("NEST")]
        [TestCase("and")]
        [TestCase("example")]
        [TestCase("exam")]
        [TestCase("one")]
        public async Task FindInTextOrNameAsync_SuccessFound_ThenCalled(string query)
        {
            var expected = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(expected, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.FindInTextOrNameAsync(query);

            actual.Should().NotBeEmpty();
        }

        [TestCase("aboba")]
        [TestCase("mons")]
        [TestCase("dex")]
        [TestCase("es")]
        public async Task FindInTextOrNameAsync_UnsuccessFound_ThenCalled(string query)
        {
            var expected = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(expected, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.FindInTextOrNameAsync(query);

            actual.Should().BeEmpty();
        }

        [TestCase(true, "exam")]
        [TestCase(true, "example")]
        [TestCase(true, "exa", "doc")]
        [TestCase(true, "zzzz", "one")]
        [TestCase(false, "aboba", "hfdjkass")]
        [TestCase(false, "z")]
        [TestCase(false, "amzz")]
        [TestCase(false, "dex")]
        public async Task IsContainsInNameAsync_ReturnCorrect_ThenCalled(bool expected, params string[] text)
        {
            var guid = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(guid, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.IsContainsInNameAsync(guid, text);

            actual.Should().Be(expected);
        }

        [Test]
        public async Task IsContainsInNameAsync_ReturnFalse_ThenIncorrectId()
        {
            var guid = Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(DocumentName);
            var document = new Document(guid, bytes, DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.IsContainsInNameAsync(Guid.NewGuid(), new[] {"doc"});

            actual.Should().BeFalse();
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