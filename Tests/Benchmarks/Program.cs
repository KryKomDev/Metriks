using BenchmarkDotNet.Running;

namespace Metriks.Benchmarks;

internal static class Program {
    static void Main(string[] args) {
        BenchmarkRunner.Run<ArrayCopyBenchmarks>();
    }
}
