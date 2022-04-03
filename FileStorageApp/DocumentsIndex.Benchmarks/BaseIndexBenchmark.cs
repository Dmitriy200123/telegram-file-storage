using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using DocumentsIndex.Config;
using DocumentsIndex.Contracts;
using DocumentsIndex.Factories;
using DocumentsIndex.Pipelines;

namespace DocumentsIndex.Benchmarks
{
    [Config(typeof(Config))]
    public class BaseIndexBenchmark
    {
        private static readonly string ResourcePath = $"{Directory.GetCurrentDirectory()}/TestFiles";
        protected IDocumentIndexStorage DocumentIndexStorage;

        protected bool CreateManyDocumentsIndexes;
        protected int DocumentCountModifer = 10;

        public static List<Document> AllTestFiles => new DirectoryInfo(ResourcePath)
            .GetFiles()
            .Select(file => LoadFileByFilenameAsync(file.Name).Result)
            .ToList();

        [GlobalSetup]
        public async void Init()
        {
            var config = new ElasticConfig("http://localhost:9200", "testindex");
            var factory = new DocumentIndexFactory(new PipelineCreator(), config);
            DocumentIndexStorage = factory.CreateDocumentIndexStorage();
            if (CreateManyDocumentsIndexes)
            {
                for (var i = 0; i < DocumentCountModifer; i++)
                {
                    foreach (var document in AllTestFiles)
                    {
                        await DocumentIndexStorage.IndexDocumentAsync(document);
                    }
                }
            }
        }

        [GlobalCleanup]
        public async Task CleanupAsync()
        {
            var searchResponse = await DocumentIndexStorage.SearchBySubstringAsync("");
            foreach (var hit in searchResponse)
                await DocumentIndexStorage.DeleteAsync(hit);
        }

        private static async Task<Document> LoadFileByFilenameAsync(string fileName)
        {
            var stream = File.OpenRead($"{ResourcePath}/{fileName}");
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var guid = Guid.NewGuid();

            return new Document(guid, fileBytes, fileName);
        }

        private class Config : ManualConfig
        {
            public Config()
            {
                AddColumn(StatisticColumn.Max);
                SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(50);
            }
        }
    }
}