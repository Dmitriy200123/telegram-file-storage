using System;
using System.IO;
using System.Linq;
using DocumentsIndex;
using DocumentsIndex.Config;
using DocumentsIndex.Factories;
using DocumentsIndex.Model;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsIndexTests
{
    public class DocumentIndexShould
    {
        private ElasticConfig _config;
        private IDocumentIndexStorage _documentIndexStorage;
        private const string WordThatContainsDocument = "NEST";
        [SetUp]
        public void Setup()
        {
            _config = new ElasticConfig("http://localhost:9200", "testindex");
            _documentIndexStorage = DocumentIndexFactory.CreateDocumentIndexStorage(_config);
        }

        [TearDown]
        public void TearDown()
        {
            var searchResponse = _documentIndexStorage.Search(WordThatContainsDocument).GetAwaiter().GetResult();
            foreach (var hit in searchResponse.Hits)
                _documentIndexStorage.Delete(hit.Source.Id);
        }

        [Test]
        public void IndexDocument_SuccessfullyUpload_ThenCalled()
        {
            var expected = Guid.NewGuid();
            var bytes = ReadBytesFromFileName("example_one.docx");
            _documentIndexStorage.IndexDocument(bytes, expected).GetAwaiter().GetResult();
            var searchResponse = _documentIndexStorage.Search("NEST").GetAwaiter().GetResult();

            var actual = searchResponse.Hits.First().Source.Id;

            actual.Should().Be(expected);
        }
        
        [Test]
        public void DeleteDocument_SuccessfullyDelete_ThenCalled()
        {
            var guid = Guid.NewGuid();
            var bytes = ReadBytesFromFileName("example_one.docx");
            _documentIndexStorage.IndexDocument(bytes, guid).GetAwaiter().GetResult();
            
            var actual = _documentIndexStorage.Delete(guid).GetAwaiter().GetResult();

            actual.Should().BeTrue();
        }

        private byte[] ReadBytesFromFileName(string fileName)
        {
            var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/FilesForTest/{fileName}");
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}

