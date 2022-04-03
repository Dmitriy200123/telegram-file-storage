using BenchmarkDotNet.Running;

namespace DocumentsIndex.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<IndexCreateBenchmark>();
            // _ = BenchmarkRunner.Run<IndexSearchBenchmark>();
        }
    }
}