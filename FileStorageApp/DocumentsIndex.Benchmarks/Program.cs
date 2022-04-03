using BenchmarkDotNet.Running;

namespace DocumentsIndex.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // var summary = BenchmarkRunner.Run<IndexCreateBenchmark>();
            var summary = BenchmarkRunner.Run<IndexSearchBenchmark>();
        }
    }
}