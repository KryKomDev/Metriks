#if METRIKS_ENABLE_JAGGED_LIST
using BenchmarkDotNet.Attributes;

namespace Metriks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 100, invocationCount: 300)]
public class List2DPerformanceBenchmarks {

    [Params(10, 100, 500)] public int Size { get; set; }

    private List2DJagged<int> _jList = null!;
    private List2D<int>       _fList = null!;

    private int[,] _matrix = null!;

    [GlobalSetup]
    public void Setup() {
        _matrix = new int[Size, Size];

        for (int x = 0; x < Size; x++) {
            for (int y = 0; y < Size; y++) {
                _matrix[x, y] = x * Size + y;
            }
        }

        _jList = new List2DJagged<int>(_matrix);
        _fList = new List2D<int>(_matrix);
    }

    [Benchmark]
    public int Read_Old() {
        int sum = 0;
        var list = _jList;
        int xs = list.XSize;
        int ys = list.YSize;

        for (int x = 0; x < xs; x++) {
            for (int y = 0; y < ys; y++) {
                sum += list[x, y];
            }
        }

        return sum;
    }

    [Benchmark]
    public int Read_New() {
        int sum = 0;
        var list = _fList;
        int xs = list.XSize;
        int ys = list.YSize;

        for (int x = 0; x < xs; x++) {
            for (int y = 0; y < ys; y++) {
                sum += list[x, y];
            }
        }

        return sum;
    }

    [Benchmark]
    public void Write_Old() {
        var list = _jList;
        int xs = list.XSize;
        int ys = list.YSize;

        for (int x = 0; x < xs; x++) {
            for (int y = 0; y < ys; y++) {
                list[x, y] = x + y;
            }
        }
    }

    [Benchmark]
    public void Write_New() {
        var list = _fList;
        int xs = list.XSize;
        int ys = list.YSize;

        for (int x = 0; x < xs; x++) {
            for (int y = 0; y < ys; y++) {
                list[x, y] = x + y;
            }
        }
    }

    [Benchmark]
    public bool Contains_Old() {
        return _jList.Contains(-1);
    }

    [Benchmark]
    public bool Contains_New() {
        return _fList.Contains(-1);
    }

    [Benchmark]
    public void Insert_Old() {
        var list = new List2DJagged<int>(_matrix);
        list.InsertAtX(Size / 2);
        list.InsertAtY(Size / 2);
    }

    [Benchmark]
    public void Insert_New() {
        var list = new List2D<int>(_matrix);
        list.InsertAtX(Size / 2);
        list.InsertAtY(Size / 2);
    }

    [Benchmark]
    public void Remove_Old() {
        var list = new List2DJagged<int>(_matrix);
        list.RemoveAtX(Size / 2);
        list.RemoveAtY(Size / 2);
    }

    [Benchmark]
    public void Remove_New() {
        var list = new List2D<int>(_matrix);
        list.RemoveAtX(Size / 2);
        list.RemoveAtY(Size / 2);
    }

    [Benchmark]
    public int[,] ToArray_Old() {
        return _jList.ToArray();
    }

    [Benchmark]
    public int[,] ToArray_New() {
        return _fList.ToArray();
    }
}

#endif