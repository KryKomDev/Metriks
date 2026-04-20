using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Metriks.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ArrayCopyBenchmarks {
    
    private const int SIZE_2D = 1000;
    private int[,] _src2D = null!;
    private int[,] _dst2D = null!;

    private const int SIZE_3D = 100;
    private int[,,] _src3D = null!;
    private int[,,] _dst3D = null!;

    private const int SIZE_4D = 30;
    private int[,,,] _src4D = null!;
    private int[,,,] _dst4D = null!;

    [GlobalSetup]
    public void Setup() {
        _src2D = new int[SIZE_2D, SIZE_2D];
        _dst2D = new int[SIZE_2D, SIZE_2D];
        _src3D = new int[SIZE_3D, SIZE_3D, SIZE_3D];
        _dst3D = new int[SIZE_3D, SIZE_3D, SIZE_3D];
        _src4D = new int[SIZE_4D, SIZE_4D, SIZE_4D, SIZE_4D];
        _dst4D = new int[SIZE_4D, SIZE_4D, SIZE_4D, SIZE_4D];
    }

    [Benchmark(Baseline = true)]
    public void Copy2D_Manual() {
        for (int i = 0; i < SIZE_2D; i++) {
            for (int j = 0; j < SIZE_2D; j++) {
                _dst2D[i, j] = _src2D[i, j];
            }
        }
    }

    [Benchmark]
    public void Copy2D_Metriks() {
        Array2D.Copy(_src2D, 0, 0, _dst2D, 0, 0, SIZE_2D, SIZE_2D);
    }

    [Benchmark]
    public void Copy3D_Manual() {
        for (int i = 0; i < SIZE_3D; i++) {
            for (int j = 0; j < SIZE_3D; j++) {
                for (int k = 0; k < SIZE_3D; k++) {
                    _dst3D[i, j, k] = _src3D[i, j, k];
                }
            }
        }
    }

    [Benchmark]
    public void Copy3D_Metriks() {
        Array3D.Copy(_src3D, 0, 0, 0, _dst3D, 0, 0, 0, SIZE_3D, SIZE_3D, SIZE_3D);
    }

    [Benchmark]
    public void Copy4D_Manual() {
        for (int i = 0; i < SIZE_4D; i++) {
            for (int j = 0; j < SIZE_4D; j++) {
                for (int k = 0; k < SIZE_4D; k++) {
                    for (int l = 0; l < SIZE_4D; l++) {
                        _dst4D[i, j, k, l] = _src4D[i, j, k, l];
                    }
                }
            }
        }
    }

    [Benchmark]
    public void Copy4D_Metriks() {
        Array4D.Copy(_src4D, 0, 0, 0, 0, _dst4D, 0, 0, 0, 0, SIZE_4D, SIZE_4D, SIZE_4D, SIZE_4D);
    }
}