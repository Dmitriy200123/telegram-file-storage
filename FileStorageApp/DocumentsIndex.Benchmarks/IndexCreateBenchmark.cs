using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using DocumentsIndex.Contracts;

namespace DocumentsIndex.Benchmarks
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 5, launchCount: 5, warmupCount: 0)]
    public class IndexCreateBenchmark : BaseIndexBenchmark
    {
        [Benchmark]
        [ArgumentsSource(nameof(AllTestFiles))]
        public async Task<bool> IndexFileAsync(Document document)
        {
            return await DocumentIndexStorage.IndexDocumentAsync(document);
        }
    }
}