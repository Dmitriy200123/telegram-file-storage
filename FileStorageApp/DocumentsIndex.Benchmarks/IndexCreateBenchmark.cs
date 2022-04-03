using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
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
        protected IDocumentIndexStorage ElasticClient;
        protected static readonly List<Guid> UsedFilesGuids = new();

        protected bool CreateManyDocumentsIndexes;
        protected int DocumentCountModifer = 10;

        private class Config : ManualConfig
        {
            public Config()
            {
                AddColumn(StatisticColumn.Max);
                SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(50);
            }
        }


        [GlobalSetup]
        public async void Init()
        {
            var config = new ElasticConfig("http://localhost:9200", "testindex");
            var factory = new DocumentIndexFactory(new PipelineCreator(), config);
            ElasticClient = factory.CreateDocumentIndexStorage();
            if (CreateManyDocumentsIndexes)
            {
                for (var i = 0; i < DocumentCountModifer; i++)
                {
                    foreach (var document in AllTestFiles())
                    {
                        await ElasticClient.IndexDocumentAsync(document);
                        UsedFilesGuids.Add(document.Id);
                    }
                }
            }
        }

        [GlobalCleanup]
        public async Task CleanupAsync()
        {
            foreach (var guid in UsedFilesGuids)
            {
                await ElasticClient.DeleteAsync(guid);
            }
        }

        public IEnumerable<Document> AllTestFiles()
        {
            var directoryInfo = new DirectoryInfo(ResourcePath);

            foreach (var file in directoryInfo.GetFiles())
                yield return LoadFileByFilenameAsync(file.Name).GetAwaiter().GetResult();
        }


        public async Task<Document> LoadFileByFilenameAsync(string fileName)
        {
            var stream = File.OpenRead($"{ResourcePath}/{fileName}");
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var guid = Guid.NewGuid();

            return new Document(guid, fileBytes, fileName);
        }
    }


    [SimpleJob(RunStrategy.ColdStart, targetCount: 5, launchCount: 5, warmupCount: 0)]
    public class IndexCreateBenchmark : BaseIndexBenchmark
    {
        [Benchmark]
        [ArgumentsSource(nameof(AllTestFiles))]
        public async Task<bool> IndexFileAsync(Document document)
        {
            var result = await ElasticClient.IndexDocumentAsync(document);
            UsedFilesGuids.Add(document.Id);

            return result;
        }
    }

    [SimpleJob(RunStrategy.ColdStart, targetCount: 10, launchCount: 10, warmupCount: 0)]
    public class IndexSearchBenchmark : BaseIndexBenchmark
    {
        public IndexSearchBenchmark()
        {
            CreateManyDocumentsIndexes = true;
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetRandomWords))]
        public async Task<int> IndexFileSearchByNameAsync(string fileName)
        {
            var result = await ElasticClient.SearchByNameAsync(fileName);
            return result.Count();
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetRandomWords))]
        public async Task<int> IndexFileSearchByContentAsync(string fileName)
        {
            var result = await ElasticClient.SearchBySubstringAsync(fileName);
            return result.Count();
        }

        public IEnumerable<string> GetRandomWords => new[]
        {
            "test", "doc", "docx", "pdf", "txt", "lorem", "toolkit", "test", "consistent",
            "Curabitur fringilla, enim eu fringilla convallis, eros est tempus urna, eget egestas nibh dolor eget nunc"
        };
    }


    public static class Program
    {
        public static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<IndexCreateBenchmark>();
            var summary = BenchmarkRunner.Run<IndexSearchBenchmark>();
        }
    }
}