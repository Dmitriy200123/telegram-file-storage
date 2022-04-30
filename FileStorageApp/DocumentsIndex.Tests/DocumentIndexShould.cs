using System;
using System.Collections.Generic;
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
        private static readonly string ResourcePath = $"{Directory.GetCurrentDirectory()}/FilesForTest";
        private const string WordThatContainsDocument = "toolkit";
        private const string DocumentName = "testWORD.docx";

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
            var expectedGuid = Guid.NewGuid();
            var document = await GetDocumentModelByFilename(DocumentName, expectedGuid);
            await _documentIndexStorage.IndexDocumentAsync(document);
            var searchResponse = await _documentIndexStorage.SearchBySubstringAsync(WordThatContainsDocument);

            searchResponse.Should().NotBeEmpty();
            searchResponse.First().Should().Be(expectedGuid);
        }

        [Test]
        public async Task DeleteDocument_SuccessfullyDelete_ThenCalled()
        {
            var expectedGuid = Guid.NewGuid();
            var document = await GetDocumentModelByFilename(DocumentName, expectedGuid);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.DeleteAsync(expectedGuid);

            actual.Should().BeTrue();
        }

        [Test]
        public async Task IndexDocumentByName_SuccessfullyUpload_ThenCalled()
        {
            var expectedGuid = Guid.NewGuid();
            var document = await GetDocumentModelByFilename(DocumentName, expectedGuid);
            await _documentIndexStorage.IndexDocumentAsync(document);
            var searchResponse = await _documentIndexStorage.SearchByNameAsync("testWORD");

            searchResponse.Should().NotBeEmpty();
            var actual = searchResponse.First();
            actual.Should().Be(expectedGuid);
        }
        
        [TestCase("word")]
        [TestCase("one")]
        [TestCase("success")]
        [TestCase("текст")]
        [TestCase("русский")]
        [TestCase("русская")]
        public async Task FindInTextOrNameAsync_SuccessFound_ThenCalled(string query)
        {
            var document = await GetDocumentModelByFilename(DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var documentName = await _documentIndexStorage.FindInTextOrNameAsync("testWORD");
            var documentContent = await _documentIndexStorage.FindInTextOrNameAsync(query);

            documentContent.Should().NotBeEmpty();
            documentName.Should().BeEquivalentTo(documentContent);
        }

        [Test]
        [TestCaseSource(nameof(AllTestFiles))]
        public async Task FindInTextOrNameAsync_SuccessFoundAllSupportedFiles_ThenCalled(string filename)
        {
            var expectedGuid = Guid.NewGuid();
            var document = await GetDocumentModelByFilename(filename, expectedGuid);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var searchResponse = await _documentIndexStorage.SearchBySubstringAsync(WordThatContainsDocument);

            searchResponse.Should().HaveCount(1);
            searchResponse.Should().NotBeEmpty();
            var actual = searchResponse.First();
            actual.Should().Be(expectedGuid);
        }

        [TestCase("aboba")]
        [TestCase("mons")]
        [TestCase("dex")]
        [TestCase("es")]
        public async Task FindInTextOrNameAsync_UnsuccessFound_ThenCalled(string query)
        {
            var document = await GetDocumentModelByFilename(DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.FindInTextOrNameAsync(query);

            actual.Should().BeEmpty();
        }
        
        [TestCase(true, "testWORD")]
        [TestCase(false, "aboba", "hfdjkass")]
        [TestCase(false, "z")]
        [TestCase(false, "amzz")]
        [TestCase(false, "dex")]
        public async Task IsContainsInNameAsync_ReturnCorrect_ThenCalled(bool expected, params string[] text)
        {
            var guid = Guid.NewGuid();
            var document = await GetDocumentModelByFilename(DocumentName, guid);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.IsContainsInNameAsync(guid, text);

            actual.Should().Be(expected);
        }

        [Test]
        public async Task IsContainsInNameAsync_ReturnFalse_ThenIncorrectId()
        {
            var document = await GetDocumentModelByFilename(DocumentName);
            await _documentIndexStorage.IndexDocumentAsync(document);

            var actual = await _documentIndexStorage.IsContainsInNameAsync(Guid.NewGuid(), new[] { "doc" });

            actual.Should().BeFalse();
        }

        private static async Task<byte[]> ReadBytesFromFileName(string fileName)
        {
            var stream = File.OpenRead($"{ResourcePath}/{fileName}");
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private static async Task<Document> GetDocumentModelByFilename(string filename, Guid? guid = null)
        {
            var resultGuid = guid ?? Guid.NewGuid();
            var bytes = await ReadBytesFromFileName(filename);
            return new Document(resultGuid, bytes, filename);
        }


        private static IEnumerable<string> AllTestFiles()
        {
            var directoryInfo = new DirectoryInfo(ResourcePath);
            foreach (var file in directoryInfo.GetFiles())
            {
                yield return file.Name;
            }
        }
    }
}