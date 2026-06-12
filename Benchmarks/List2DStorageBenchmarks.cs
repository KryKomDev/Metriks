using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

namespace Metriks.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class List2DStorageBenchmarks {
    [Params(10, 100, 500)] public int Size { get; set; }

    private int[][] _jagged = null!;
    private int[] _flat = null!;
    private int _ySize;
    private int _xSize;

    [GlobalSetup]
    public void Setup() {
        _xSize = Size;
        _ySize = Size;

        // Initialize Jagged (Column-major jagged array, where _items[x][y] corresponds to x-column and y-row)
        _jagged = new int[_xSize][];

        for (int i = 0; i < _xSize; i++) {
            _jagged[i] = new int[_ySize];

            for (int j = 0; j < _ySize; j++) {
                _jagged[i][j] = i * _ySize + j;
            }
        }

        // Initialize Flat (1D array storage where offset is x * _ySize + y)
        _flat = new int[_xSize * _ySize];

        for (int i = 0; i < _xSize; i++) {
            for (int j = 0; j < _ySize; j++) {
                _flat[i * _ySize + j] = i * _ySize + j;
            }
        }
    }

    [Benchmark]
    public int Read_Jagged() {
        int sum = 0;
        var jagged = _jagged;
        var xSize = _xSize;
        var ySize = _ySize;

        for (int x = 0; x < xSize; x++) {
            var col = jagged[x];

            for (int y = 0; y < ySize; y++) {
                sum += col[y];
            }
        }

        return sum;
    }

    [Benchmark]
    public int Read_Flat() {
        int sum = 0;
        var flat = _flat;
        var xSize = _xSize;
        var ySize = _ySize;

        for (int x = 0; x < xSize; x++) {
            var offset = x * ySize;

            for (int y = 0; y < ySize; y++) {
                sum += flat[offset + y];
            }
        }

        return sum;
    }

    [Benchmark]
    public void Write_Jagged() {
        var jagged = _jagged;
        var xSize = _xSize;
        var ySize = _ySize;

        for (int x = 0; x < xSize; x++) {
            var col = jagged[x];

            for (int y = 0; y < ySize; y++) {
                col[y] = x + y;
            }
        }
    }

    [Benchmark]
    public void Write_Flat() {
        var flat = _flat;
        var xSize = _xSize;
        var ySize = _ySize;

        for (int x = 0; x < xSize; x++) {
            var offset = x * ySize;

            for (int y = 0; y < ySize; y++) {
                flat[offset + y] = x + y;
            }
        }
    }

    [Benchmark]
    public int[][] InsertColumn_Jagged() {
        var xSize = _xSize;
        var ySize = _ySize;
        var insertIndex = xSize / 2;

        var newJagged = new int[xSize + 1][];
        Array.Copy(_jagged, 0, newJagged, 0, insertIndex);
        newJagged[insertIndex] = new int[ySize];
        Array.Copy(_jagged, insertIndex, newJagged, insertIndex + 1, xSize - insertIndex);

        return newJagged;
    }

    [Benchmark]
    public int[] InsertColumn_Flat() {
        var xSize = _xSize;
        var ySize = _ySize;
        var insertIndex = xSize / 2;

        var newFlat = new int[(xSize + 1) * ySize];

        // Copy columns before insert index
        Array.Copy(_flat, 0, newFlat, 0, insertIndex * ySize);
        // Column at insertIndex remains default/zero
        // Copy columns after insert index
        Array.Copy(_flat, insertIndex * ySize, newFlat, (insertIndex + 1) * ySize, (xSize - insertIndex) * ySize);

        return newFlat;
    }

    [Benchmark]
    public int[][] InsertRow_Jagged() {
        var xSize = _xSize;
        var ySize = _ySize;
        var insertIndex = ySize / 2;

        var newJagged = new int[xSize][];

        for (int x = 0; x < xSize; x++) {
            var newCol = new int[ySize + 1];
            Array.Copy(_jagged[x], 0, newCol, 0, insertIndex);
            // Element at insertIndex remains zero
            Array.Copy(_jagged[x], insertIndex, newCol, insertIndex + 1, ySize - insertIndex);
            newJagged[x] = newCol;
        }

        return newJagged;
    }

    [Benchmark]
    public int[] InsertRow_Flat() {
        var xSize = _xSize;
        var ySize = _ySize;
        var insertIndex = ySize / 2;

        var newFlat = new int[xSize * (ySize + 1)];
        var newYSize = ySize + 1;

        for (int x = 0; x < xSize; x++) {
            Array.Copy(_flat, x * ySize, newFlat, x * newYSize, insertIndex);
            Array.Copy(_flat, x * ySize + insertIndex, newFlat, x * newYSize + insertIndex + 1, ySize - insertIndex);
        }

        return newFlat;
    }

    [Benchmark]
    public int[][] Resize_Jagged() {
        var xSize = _xSize;
        var ySize = _ySize;
        var newX = xSize * 2;
        var newY = ySize * 2;

        var newJagged = new int[newX][];

        for (int x = 0; x < xSize; x++) {
            var newCol = new int[newY];
            Array.Copy(_jagged[x], newCol, ySize);
            newJagged[x] = newCol;
        }

        for (int x = xSize; x < newX; x++) {
            newJagged[x] = new int[newY];
        }

        return newJagged;
    }

    [Benchmark]
    public int[] Resize_Flat() {
        var xSize = _xSize;
        var ySize = _ySize;
        var newX = xSize * 2;
        var newY = ySize * 2;

        var newFlat = new int[newX * newY];

        for (int x = 0; x < xSize; x++) {
            Array.Copy(_flat, x * ySize, newFlat, x * newY, ySize);
        }

        return newFlat;
    }
}