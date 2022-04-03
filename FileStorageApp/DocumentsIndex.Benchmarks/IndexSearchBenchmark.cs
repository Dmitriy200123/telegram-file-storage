using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace DocumentsIndex.Benchmarks
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 10, launchCount: 10, warmupCount: 0)]
    public class IndexSearchBenchmark : BaseIndexBenchmark
    {
        public static IEnumerable<string> GetRandomWords => new[]
        {
            "test", "doc", "docx", "pdf", "txt", "lorem", "toolkit", "test", "consistent",
            "Curabitur fringilla, enim eu fringilla convallis, eros est tempus urna, eget egestas nibh dolor eget nunc"
        };

        public IndexSearchBenchmark()
        {
            CreateManyDocumentsIndexes = true;
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetRandomWords))]
        public async Task<int> IndexFileSearchByNameAsync(string fileName)
        {
            var result = await DocumentIndexStorage.SearchByNameAsync(fileName);
            return result.Count;
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetRandomWords))]
        public async Task<int> IndexFileSearchByContentAsync(string fileName)
        {
            var result = await DocumentIndexStorage.SearchBySubstringAsync(fileName);
            return result.Count;
        }
    }
}