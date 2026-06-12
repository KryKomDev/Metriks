// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metriks;

/// <summary>
/// Represents a strongly typed, four-dimensional list of elements that can be accessed by W, X, Y, and Z indices.
/// Provides methods to search, sort, and manipulate 4D lists.
/// </summary>
/// <typeparam name="T">The type of elements in the four-dimensional list.</typeparam>
public class List4D<T> : IList4D<T>, ICollection4D, IReadOnlyList4D<T> {

    private const int INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR = 2f;

    private T[][][][] _items;
    
    /// <summary>
    /// Gets the underlying four-dimensional array used to store the elements of the <see cref="List4D{T}"/> instance.
    /// PROVIDED FOR INTERNAL USE ONLY. DO NOT USE. <b>!!!DO NOT MODIFY THE ARRAY IN ANY WAY!!!</b>
    /// </summary>
    internal T[][][][] Items {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _items;
    }

    protected void UnsafeSet(int w, int x, int y, int z, T value) => _items[w][x][y][z] = value;
    protected T UnsafeGet(int w, int x, int y, int z) => _items[w][x][y][z];

    private int _wSize;
    private int _xSize;
    private int _ySize;
    private int _zSize;
    private int _wCapacity;
    private int _xCapacity;
    private int _yCapacity;
    private int _zCapacity;

    /// <summary>
    /// Initializes a new instance of the <see cref="List4D{T}"/> class with the specified initial capacity for each dimension.
    /// </summary>
    /// <param name="wCapacity">The initial capacity along the W-axis.</param>
    /// <param name="xCapacity">The initial capacity along the X-axis.</param>
    /// <param name="yCapacity">The initial capacity along the Y-axis.</param>
    /// <param name="zCapacity">The initial capacity along the Z-axis.</param>
    public List4D(
        int wCapacity = INITIAL_CAPACITY,
        int xCapacity = INITIAL_CAPACITY,
        int yCapacity = INITIAL_CAPACITY, 
        int zCapacity = INITIAL_CAPACITY) 
    {
        _items     = new T[wCapacity][][][];
        _wSize     = 0;
        _xSize     = 0;
        _ySize     = 0;
        _zSize     = 0;
        _wCapacity = wCapacity;
        _xCapacity = xCapacity;
        _yCapacity = yCapacity;
        _zCapacity = zCapacity;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="List4D{T}"/> class populated with elements from the specified 4D array.
    /// </summary>
    /// <param name="collection">The 4D array of elements to copy from.</param>
    public List4D(T[,,,] collection) : this(
        collection.GetLength(0), 
        collection.GetLength(1), 
        collection.GetLength(2),
        collection.GetLength(3)) 
    {
        var len0 = collection.GetLength(0);
        var len1 = collection.GetLength(1);
        var len2 = collection.GetLength(2);
        var len3 = collection.GetLength(3);

        for (int w = 0; w < len0; w++) {
            _items[w] = new T[len1][][];
        }

        if (len1 > 0 && len2 > 0 && len3 > 0) {
            for (int w = 0; w < len0; w++) {
                var rowW = _items[w];
                for (int x = 0; x < len1; x++) {
                    rowW[x] = new T[len2][];
                    var rowWX = rowW[x];
                    for (int y = 0; y < len2; y++) {
                        rowWX[y] = new T[len3];
                        var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref collection[w, x, y, 0], len3);
                        var dstSpan = new Span<T>(rowWX[y]);
                        srcSpan.CopyTo(dstSpan);
                    }
                }
            }
        }
        else {
            for (int w = 0; w < len0; w++) {
                var rowW = _items[w];
                for (int x = 0; x < len1; x++) {
                    rowW[x] = new T[len2][];
                    var rowWX = rowW[x];
                    for (int y = 0; y < len2; y++) {
                        rowWX[y] = Array.Empty<T>();
                    }
                }
            }
        }
        
        _wSize = len0;
        _xSize = len1;
        _ySize = len2;
        _zSize = len3;
    }
    
    /// <summary>
    /// Gets the size (number of elements) along the W-axis.
    /// </summary>
    public int WSize => _wSize;

    /// <summary>
    /// Gets the size (number of elements) along the X-axis.
    /// </summary>
    public int XSize => _xSize;

    /// <summary>
    /// Gets the size (number of elements) along the Y-axis.
    /// </summary>
    public int YSize => _ySize;

    /// <summary>
    /// Gets the size (number of elements) along the Z-axis.
    /// </summary>
    public int ZSize => _zSize;

    /// <summary>
    /// Gets a <see cref="Size4D"/> representing the current size of the list in all four dimensions.
    /// </summary>
    public Size4D Size => new(_wSize, _xSize, _ySize, _zSize);

    /// <summary>
    /// Gets the capacity along the W-axis.
    /// </summary>
    public int WCapacity => _wCapacity;

    /// <summary>
    /// Gets the capacity along the X-axis.
    /// </summary>
    public int XCapacity => _xCapacity;

    /// <summary>
    /// Gets the capacity along the Y-axis.
    /// </summary>
    public int YCapacity => _yCapacity;

    /// <summary>
    /// Gets the capacity along the Z-axis.
    /// </summary>
    public int ZCapacity => _zCapacity;

    /// <summary>
    /// Gets the total number of elements contained in the <see cref="List4D{T}"/>.
    /// </summary>
    public int Count => _wSize * _xSize * _ySize * _zSize;

    /// <summary>
    /// Gets the size (number of elements) along the W-axis.
    /// </summary>
    public int WCount => _wSize;

    /// <summary>
    /// Gets the size (number of elements) along the X-axis.
    /// </summary>
    public int XCount => _xSize;

    /// <summary>
    /// Gets the size (number of elements) along the Y-axis.
    /// </summary>
    public int YCount => _ySize;

    /// <summary>
    /// Gets the size (number of elements) along the Z-axis.
    /// </summary>
    public int ZCount => _zSize;
    
    /// <summary>
    /// Gets a value indicating whether the <see cref="List4D{T}"/> is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    public T this[int w, int x, int y, int z] {
        get {
            if (w < 0 || w >= _wSize) throw new IndexOutOfRangeException("Index 'w' is out of range.");
            if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            if (z < 0 || z >= _zSize) throw new IndexOutOfRangeException("Index 'z' is out of range.");
            return _items[w][x][y][z];
        }
        set {
            if (w < 0 || w >= _wSize) throw new IndexOutOfRangeException("Index 'w' is out of range.");
            if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            if (z < 0 || z >= _zSize) throw new IndexOutOfRangeException("Index 'z' is out of range.");
            _items[w][x][y][z] = value;
        }
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    public T[] this[Range w, int x, int y, int z] {
        get {
            var (offset, length) = w.GetOffsetAndLength(_wSize);
            var result = new T[length];
            for (int i = 0; i < length; i++) result[i] = _items[offset + i][x][y][z];
            return result;
        }
    }
    public T[] this[int w, Range x, int y, int z] {
        get {
            var (offset, length) = x.GetOffsetAndLength(_xSize);
            var result = new T[length];
            for (int i = 0; i < length; i++) result[i] = _items[w][offset + i][y][z];
            return result;
        }
    }
    public T[] this[int w, int x, Range y, int z] {
        get {
            var (offset, length) = y.GetOffsetAndLength(_ySize);
            var result = new T[length];
            for (int i = 0; i < length; i++) result[i] = _items[w][x][offset + i][z];
            return result;
        }
    }
    public T[] this[int w, int x, int y, Range z] {
        get {
            var (offset, length) = z.GetOffsetAndLength(_zSize);
            var result = new T[length];
            for (int i = 0; i < length; i++) result[i] = _items[w][x][y][offset + i];
            return result;
        }
    }
    #endif
    
    public void InsertAtW(int w) {
        if (w < 0 || w > _wSize) throw new IndexOutOfRangeException();
        if (_wSize + 1 >= _wCapacity) _wCapacity = (int)(_wCapacity * GROWTH_FACTOR);
        
        var newItems = new T[_wCapacity][][][];
        Array.Copy(_items, newItems, w);
        newItems[w] = Create3DArray(_xCapacity, _yCapacity, _zCapacity);
        Array.Copy(_items, w, newItems, w + 1, _wSize - w);
        _wSize++;
        _items = newItems;
    }

    public void InsertAtX(int x) {
        if (x < 0 || x > _xSize) throw new IndexOutOfRangeException();
        if (_xSize + 1 >= _xCapacity) _xCapacity = (int)(_xCapacity * GROWTH_FACTOR);

        for (int w = 0; w < _wSize; w++) {
            var newArray = new T[_xCapacity][][];
            Array.Copy(_items[w], newArray, x);
            newArray[x] = Create2DArray(_yCapacity, _zCapacity);
            Array.Copy(_items[w], x, newArray, x + 1, _xSize - x);
            _items[w] = newArray;
        }
        _xSize++;
    }

    public void InsertAtY(int y) {
        if (y < 0 || y > _ySize) throw new IndexOutOfRangeException();
        if (_ySize + 1 >= _yCapacity) _yCapacity = (int)(_yCapacity * GROWTH_FACTOR);

        for (int w = 0; w < _wSize; w++) {
            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_yCapacity][];
                Array.Copy(_items[w][x], newArray, y);
                newArray[y] = new T[_zCapacity];
                Array.Copy(_items[w][x], y, newArray, y + 1, _ySize - y);
                _items[w][x] = newArray;
            }
        }
        _ySize++;
    }

    public void InsertAtZ(int z) {
        if (z < 0 || z > _zSize) throw new IndexOutOfRangeException();
        if (_zSize + 1 >= _zCapacity) _zCapacity = (int)(_zCapacity * GROWTH_FACTOR);

        for (int w = 0; w < _wSize; w++) {
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    var newArray = new T[_zCapacity];
                    Array.Copy(_items[w][x][y], newArray, z);
                    Array.Copy(_items[w][x][y], z, newArray, z + 1, _zSize - z);
                    _items[w][x][y] = newArray;
                }
            }
        }
        _zSize++;
    }

    public void AddW() => InsertAtW(_wSize);
    public void AddX() => InsertAtX(_xSize);
    public void AddY() => InsertAtY(_ySize);
    public void AddZ() => InsertAtZ(_zSize);

    /// <summary>
    /// Expands the dimensions of the 4D list to the specified size, filling new elements with a default value.
    /// </summary>
    /// <param name="wSize">The new size along the W-axis.</param>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="zSize">The new size along the Z-axis.</param>
    /// <param name="defaultValue">The value to fill the new elements with.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any new size is smaller than the current size.</exception>
    public void Expand(int wSize, int xSize, int ySize, int zSize, T? defaultValue = default!) {
        if (wSize < _wSize || xSize < _xSize || ySize < _ySize || zSize < _zSize) throw new ArgumentOutOfRangeException();
        if (wSize == _wSize && xSize == _xSize && ySize == _ySize && zSize == _zSize) return;
        
        if (wSize > _wCapacity) {
            _wCapacity = wSize + 1;
            var newItems = new T[_wCapacity][][][];
            Array.Copy(_items, newItems, _wSize);
            _items = newItems;
        }

        for (int w = 0; w < wSize; w++) {
            bool isNewW = w >= _wSize;
            if (isNewW) {
                var rowW = Create3DArray(xSize, ySize, zSize);
                _items[w] = rowW;
                if (defaultValue is not null) {
                    for (int x = 0; x < xSize; x++) {
                        var rowWX = rowW[x];
                        for (int y = 0; y < ySize; y++)
                            Array.Fill(rowWX[y], defaultValue);
                    }
                }
                continue;
            }

            // Existing W, expand its children
            var existingW = _items[w];
            if (xSize > existingW.Length) {
                var newX = new T[xSize][][];
                Array.Copy(existingW, newX, _xSize);
                _items[w] = newX;
                existingW = newX;
            }

            for (int x = 0; x < xSize; x++) {
                bool isNewX = x >= _xSize;
                if (isNewX) {
                    var rowWX = Create2DArray(ySize, zSize);
                    existingW[x] = rowWX;
                    if (defaultValue is not null) {
                        for (int y = 0; y < ySize; y++)
                            Array.Fill(rowWX[y], defaultValue);
                    }
                    continue;
                }

                var existingWX = existingW[x];
                if (ySize > existingWX.Length) {
                    var newY = new T[ySize][];
                    Array.Copy(existingWX, newY, _ySize);
                    existingW[x] = newY;
                    existingWX = newY;
                }

                for (int y = 0; y < ySize; y++) {
                    bool isNewY = y >= _ySize;
                    if (isNewY) {
                        var cellWXY = new T[zSize];
                        existingWX[y] = cellWXY;
                        if (defaultValue is not null) Array.Fill(cellWXY, defaultValue);
                        continue;
                    }

                    var cellWXYExisting = existingWX[y];
                    if (zSize > cellWXYExisting.Length) {
                        var newZ = new T[zSize];
                        Array.Copy(cellWXYExisting, newZ, _zSize);
                        if (defaultValue is not null) Array.Fill(newZ, defaultValue, _zSize, zSize - _zSize);
                        existingWX[y] = newZ;
                    }
                    else if (zSize > _zSize && defaultValue is not null) {
                         Array.Fill(cellWXYExisting, defaultValue, _zSize, zSize - _zSize);
                    }
                }
            }
        }

        _wSize = wSize; _xSize = xSize; _ySize = ySize; _zSize = zSize;
        _wCapacity = Math.Max(_wCapacity, wSize);
        _xCapacity = Math.Max(_xCapacity, xSize);
        _yCapacity = Math.Max(_yCapacity, ySize);
        _zCapacity = Math.Max(_zCapacity, zSize);
    }

    /// <summary>
    /// Expands the dimensions of the 4D list to the specified size, generating new elements using the specified factory function.
    /// </summary>
    /// <param name="wSize">The new size along the W-axis.</param>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="zSize">The new size along the Z-axis.</param>
    /// <param name="defaultValueFactory">A factory function that generates values for the new elements.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any new size is smaller than the current size.</exception>
    public void Expand(int wSize, int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        if (wSize < _wSize || xSize < _xSize || ySize < _ySize || zSize < _zSize) throw new ArgumentOutOfRangeException();
        if (wSize == _wSize && xSize == _xSize && ySize == _ySize && zSize == _zSize) return;
        
        if (wSize > _wCapacity) {
            _wCapacity = wSize + 1;
            var newItems = new T[_wCapacity][][][];
            Array.Copy(_items, newItems, _wSize);
            _items = newItems;
        }

        for (int w = 0; w < wSize; w++) {
            bool isNewW = w >= _wSize;
            if (isNewW) {
                var rowW = Create3DArray(xSize, ySize, zSize);
                _items[w] = rowW;
                for (int x = 0; x < xSize; x++) {
                    var rowWX = rowW[x];
                    for (int y = 0; y < ySize; y++) {
                        var rowWXY = rowWX[y];
                        for (int z = 0; z < zSize; z++)
                            rowWXY[z] = defaultValueFactory();
                    }
                }
                continue;
            }

            var existingW = _items[w];
            if (xSize > existingW.Length) {
                var newX = new T[xSize][][];
                Array.Copy(existingW, newX, _xSize);
                _items[w] = newX;
                existingW = newX;
            }

            for (int x = 0; x < xSize; x++) {
                bool isNewX = x >= _xSize;
                if (isNewX) {
                    var rowWX = Create2DArray(ySize, zSize);
                    existingW[x] = rowWX;
                    for (int y = 0; y < ySize; y++) {
                        var rowWXY = rowWX[y];
                        for (int z = 0; z < zSize; z++)
                            rowWXY[z] = defaultValueFactory();
                    }
                    continue;
                }

                var existingWX = existingW[x];
                if (ySize > existingWX.Length) {
                    var newY = new T[ySize][];
                    Array.Copy(existingWX, newY, _ySize);
                    existingW[x] = newY;
                    existingWX = newY;
                }

                for (int y = 0; y < ySize; y++) {
                    bool isNewY = y >= _ySize;
                    if (isNewY) {
                        var cellWXY = new T[zSize];
                        existingWX[y] = cellWXY;
                        for (int z = 0; z < zSize; z++) cellWXY[z] = defaultValueFactory();
                        continue;
                    }

                    var cellWXYExisting = existingWX[y];
                    if (zSize > cellWXYExisting.Length) {
                        var newZ = new T[zSize];
                        Array.Copy(cellWXYExisting, newZ, _zSize);
                        for (int z = _zSize; z < zSize; z++) newZ[z] = defaultValueFactory();
                        existingWX[y] = newZ;
                    }
                }
            }
        }

        _wSize = wSize; _xSize = xSize; _ySize = ySize; _zSize = zSize;
        _wCapacity = Math.Max(_wCapacity, wSize);
        _xCapacity = Math.Max(_xCapacity, xSize);
        _yCapacity = Math.Max(_yCapacity, ySize);
        _zCapacity = Math.Max(_zCapacity, zSize);
    }

    public void Resize(int wSize, int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        if (wSize < 0 || xSize < 0 || ySize < 0 || zSize < 0) throw new ArgumentOutOfRangeException();
        var newItems = new T[wSize][][][];
        
        for (int w = 0; w < wSize; w++) {
            newItems[w] = Create3DArray(xSize, ySize, zSize);
            for (int x = 0; x < xSize; x++) {
                for (int y = 0; y < ySize; y++) {
                    for (int z = 0; z < zSize; z++) {
                        newItems[w][x][y][z] = defaultValueFactory();
                    }
                }
            }
        }

        int minW = Math.Min(_wSize, wSize);
        int minX = Math.Min(_xSize, xSize);
        int minY = Math.Min(_ySize, ySize);
        int minZ = Math.Min(_zSize, zSize);

        for (int w = 0; w < minW; w++) {
            for (int x = 0; x < minX; x++) {
                for (int y = 0; y < minY; y++) {
                    Array.Copy(_items[w][x][y], newItems[w][x][y], minZ);
                }
            }
        }
        
        _items = newItems;
        _wSize = wSize; _xSize = xSize; _ySize = ySize; _zSize = zSize;
        _wCapacity = wSize; _xCapacity = xSize; _yCapacity = ySize; _zCapacity = zSize;
    }

    public void Resize(int wSize, int xSize, int ySize, int zSize, T? defaultValue = default) {
        if (wSize < 0 || xSize < 0 || ySize < 0 || zSize < 0) throw new ArgumentOutOfRangeException();
        var newItems = new T[wSize][][][];
        for (int w = 0; w < wSize; w++) {
            newItems[w] = Create3DArray(xSize, ySize, zSize);
            if (defaultValue is not null) {
                for (int x = 0; x < xSize; x++)
                    for (int y = 0; y < ySize; y++)
                        Array.Fill(newItems[w][x][y], defaultValue);
            }
        }
        int minW = Math.Min(_wSize, wSize);
        int minX = Math.Min(_xSize, xSize);
        int minY = Math.Min(_ySize, ySize);
        int minZ = Math.Min(_zSize, zSize);
        for (int w = 0; w < minW; w++)
            for (int x = 0; x < minX; x++)
                for (int y = 0; y < minY; y++)
                    Array.Copy(_items[w][x][y], newItems[w][x][y], minZ);
        
        _items = newItems;
        _wSize = wSize; _xSize = xSize; _ySize = ySize; _zSize = zSize;
        _wCapacity = wSize; _xCapacity = xSize; _yCapacity = ySize; _zCapacity = zSize;
    }

    public void ShrinkW() {
        if (_wSize == 0) throw new InvalidOperationException();
        _wSize--;
        _items[_wSize] = null!;
    }

    public void ShrinkX() {
        if (_xSize == 0) throw new InvalidOperationException();
        _xSize--;
        for (int w = 0; w < _wSize; w++) _items[w][_xSize] = null!;
    }

    public void ShrinkY() {
        if (_ySize == 0) throw new InvalidOperationException();
        _ySize--;
        for (int w = 0; w < _wSize; w++)
            for (int x = 0; x < _xSize; x++)
                _items[w][x][_ySize] = null!;
    }

    public void ShrinkZ() {
        if (_zSize == 0) throw new InvalidOperationException();
        _zSize--;
        for (int w = 0; w < _wSize; w++)
            for (int x = 0; x < _xSize; x++)
                for (int y = 0; y < _ySize; y++)
                    _items[w][x][y][_zSize] = default!;
    }

    public void RemoveAtW(int w) {
        if (w < 0 || w >= _wSize) throw new IndexOutOfRangeException();
        _wSize--;
        var newItems = new T[_wSize][][][];
        Array.Copy(_items, newItems, w);
        Array.Copy(_items, w + 1, newItems, w, _wSize - w);
        _items = newItems;
    }

    public void RemoveAtX(int x) {
        if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException();
        _xSize--;
        for (int w = 0; w < _wSize; w++) {
            var newArray = new T[_xSize][][];
            Array.Copy(_items[w], newArray, x);
            Array.Copy(_items[w], x + 1, newArray, x, _xSize - x);
            _items[w] = newArray;
        }
    }

    public void RemoveAtY(int y) {
        if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException();
        _ySize--;
        for (int w = 0; w < _wSize; w++)
            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_ySize][];
                Array.Copy(_items[w][x], newArray, y);
                Array.Copy(_items[w][x], y + 1, newArray, y, _ySize - y);
                _items[w][x] = newArray;
            }
    }

    public void RemoveAtZ(int z) {
        if (z < 0 || z >= _zSize) throw new IndexOutOfRangeException();
        _zSize--;
        for (int w = 0; w < _wSize; w++)
            for (int x = 0; x < _xSize; x++)
                for (int y = 0; y < _ySize; y++) {
                    Array.Copy(_items[w][x][y], z + 1, _items[w][x][y], z, _zSize - z);
                    _items[w][x][y][_zSize] = default!;
                }
    }

    /// <summary>
    /// Determines whether the 4D list contains a specific value.
    /// </summary>
    /// <param name="value">The value to locate in the 4D list.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool Contains(T value) {
        for (int w = 0; w < _wSize; w++) {
            var rowW = _items[w];
            for (int x = 0; x < _xSize; x++) {
                var rowWX = rowW[x];
                for (int y = 0; y < _ySize; y++) {
                    if (Array.IndexOf(rowWX[y], value, 0, _zSize) >= 0)
                        return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Determines whether the specified hyperplane (along the W-axis) contains a specific value.
    /// </summary>
    /// <param name="w">The index on the W-axis.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool ContainsAtW(int w, T value) {
        if (w < 0 || w >= _wSize) return false;
        var rowW = _items[w];
        for (int x = 0; x < _xSize; x++) {
            var rowWX = rowW[x];
            for (int y = 0; y < _ySize; y++) {
                if (Array.IndexOf(rowWX[y], value, 0, _zSize) >= 0)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines whether the specified column (along the X-axis) contains a specific value.
    /// </summary>
    /// <param name="x">The column index on the X-axis.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool ContainsAtX(int x, T value) {
        if (x < 0 || x >= _xSize) return false;
        for (int w = 0; w < _wSize; w++) {
            var rowWX = _items[w][x];
            for (int y = 0; y < _ySize; y++) {
                if (Array.IndexOf(rowWX[y], value, 0, _zSize) >= 0)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines whether the specified plane (along the Y-axis) contains a specific value.
    /// </summary>
    /// <param name="y">The index on the Y-axis.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool ContainsAtY(int y, T value) {
        if (y < 0 || y >= _ySize) return false;
        for (int w = 0; w < _wSize; w++) {
            var rowW = _items[w];
            for (int x = 0; x < _xSize; x++) {
                if (Array.IndexOf(rowW[x][y], value, 0, _zSize) >= 0)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines whether the specified plane (along the Z-axis) contains a specific value.
    /// </summary>
    /// <param name="z">The index on the Z-axis.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool ContainsAtZ(int z, T value) {
        if (z < 0 || z >= _zSize) return false;
        for (int w = 0; w < _wSize; w++) {
            var rowW = _items[w];
            for (int x = 0; x < _xSize; x++) {
                var rowWX = rowW[x];
                for (int y = 0; y < _ySize; y++) {
                    if (EqualityComparer<T>.Default.Equals(rowWX[y][z], value)) return true;
                }
            }
        }
        return false;
    }

    public void Clear() {
        _wSize = 0; _xSize = 0; _ySize = 0; _zSize = 0;
        _wCapacity = INITIAL_CAPACITY; _xCapacity = INITIAL_CAPACITY; _yCapacity = INITIAL_CAPACITY; _zCapacity = INITIAL_CAPACITY;
        _items = new T[INITIAL_CAPACITY][][][];
    }

    /// <summary>
    /// Places a 4D matrix into this List4D at the specified offset. If the matrix extends beyond
    /// the current bounds of the List4D, the List4D is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 4D array of elements to place into this List4D.</param>
    /// <param name="offsetPoint">
    /// An optional offset defining where the top-left corner of the matrix will be placed.
    /// If not provided, the matrix will be placed at the origin of the List4D.
    /// </param>
    /// <param name="resize">If true, enables automatic resizing of this list depending on
    /// the size and offset of the placed array.</param>
    public void Place(T[,,,] matrix, Point4D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(
            Math.Max(_wSize, placedMax.W),
            Math.Max(_xSize, placedMax.X),
            Math.Max(_ySize, placedMax.Y),
            Math.Max(_zSize, placedMax.Z));
        var min = new Point4D(
            Math.Min(offset.W, 0),
            Math.Min(offset.X, 0),
            Math.Min(offset.Y, 0),
            Math.Min(offset.Z, 0));
        
        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W][][][];
            
            for (int w = 0; w < newSize.W; w++) {
                newItems[w] = Create3DArray(newSize.X, newSize.Y, newSize.Z);
            }

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            
            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (int w = 0; w < _wSize; w++) {
                var rowW = _items[w];
                var dstW = newItems[w + oldWOffset];
                for (int x = 0; x < _xSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + oldXOffset];
                    for (int y = 0; y < _ySize; y++) {
                        Array.Copy(rowWX[y], 0, dstWX[y + oldYOffset], oldZOffset, _zSize);
                    }
                }
            }

            // Copy new
            var len3 = matrix.GetLength(3);
            if (len3 > 0) {
                var len0 = matrix.GetLength(0);
                var len1 = matrix.GetLength(1);
                var len2 = matrix.GetLength(2);
                for (int w = 0; w < len0; w++) {
                    var dstW = newItems[w + newWOffset];
                    for (int x = 0; x < len1; x++) {
                        var dstWX = dstW[x + newXOffset];
                        for (int y = 0; y < len2; y++) {
                            var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref matrix[w, x, y, 0], len3);
                            var dstSpan = new Span<T>(dstWX[y + newYOffset], newZOffset, len3);
                            srcSpan.CopyTo(dstSpan);
                        }
                    }
                }
            }

            _items = newItems;
            _wSize = newSize.W; _xSize = newSize.X; _ySize = newSize.Y; _zSize = newSize.Z;
            _wCapacity = _wSize; _xCapacity = _xSize; _yCapacity = _ySize; _zCapacity = _zSize;
        }
        else {
            if ((placedMax.W > _wSize || placedMax.X > _xSize || placedMax.Y > _ySize || placedMax.Z > _zSize) && resize) {
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);
            }

            var startW = Math.Clamp(offset.W, 0, _wSize);
            var endW = Math.Min(_wSize, offset.W + matrix.GetLength(0));
            var startX = Math.Clamp(offset.X, 0, _xSize);
            var endX = Math.Min(_xSize, offset.X + matrix.GetLength(1));
            var startY = Math.Clamp(offset.Y, 0, _ySize);
            var endY = Math.Min(_ySize, offset.Y + matrix.GetLength(2));
            var startZ = Math.Clamp(offset.Z, 0, _zSize);
            var endZ = Math.Min(_zSize, offset.Z + matrix.GetLength(3));
            var zCount = endZ - startZ;

            if (zCount > 0) {
                for (int w = startW; w < endW; w++) {
                    var rowW = _items[w];
                    var srcW = w - offset.W;
                    for (int x = startX; x < endX; x++) {
                        var rowWX = rowW[x];
                        var srcX = x - offset.X;
                        for (int y = startY; y < endY; y++) {
                            var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref matrix[srcW, srcX, y - offset.Y, startZ - offset.Z], zCount);
                            var dstSpan = new Span<T>(rowWX[y], startZ, zCount);
                            srcSpan.CopyTo(dstSpan);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places the contents of the specified 4D list into the current List4D instance, optionally offset by a specified point.
    /// </summary>
    /// <param name="matrix">The List4D instance containing the elements to be placed.</param>
    /// <param name="offsetPoint">
    /// An optional point specifying the offset at which the matrix should be placed.
    /// If null, the matrix will be placed starting at the origin (0, 0, 0, 0).
    /// </param>
    /// <param name="resize">If true, enables automatic resizing of this list depending on
    /// the size and offset of the placed array.</param>
    public void Place(List4D<T> matrix, Point4D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(
            Math.Max(_wSize, placedMax.W),
            Math.Max(_xSize, placedMax.X),
            Math.Max(_ySize, placedMax.Y),
            Math.Max(_zSize, placedMax.Z));
        var min = new Point4D(
            Math.Min(offset.W, 0),
            Math.Min(offset.X, 0),
            Math.Min(offset.Y, 0),
            Math.Min(offset.Z, 0));
        
        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W][][][];
            
            for (int w = 0; w < newSize.W; w++) {
                newItems[w] = Create3DArray(newSize.X, newSize.Y, newSize.Z);
            }

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            
            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (int w = 0; w < _wSize; w++) {
                var rowW = _items[w];
                var dstW = newItems[w + oldWOffset];
                for (int x = 0; x < _xSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + oldXOffset];
                    for (int y = 0; y < _ySize; y++) {
                        Array.Copy(rowWX[y], 0, dstWX[y + oldYOffset], oldZOffset, _zSize);
                    }
                }
            }

            // Copy new
            for (int w = 0; w < matrix._wSize; w++) {
                var rowW = matrix._items[w];
                var dstW = newItems[w + newWOffset];
                for (int x = 0; x < matrix._xSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + newXOffset];
                    for (int y = 0; y < matrix._ySize; y++) {
                        Array.Copy(rowWX[y], 0, dstWX[y + newYOffset], newZOffset, matrix._zSize);
                    }
                }
            }

            _items = newItems;
            _wSize = newSize.W; _xSize = newSize.X; _ySize = newSize.Y; _zSize = newSize.Z;
            _wCapacity = _wSize; _xCapacity = _xSize; _yCapacity = _ySize; _zCapacity = _zSize;
        }
        else {
            if ((placedMax.W > _wSize || placedMax.X > _xSize || placedMax.Y > _ySize || placedMax.Z > _zSize) && resize) {
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);
            }

            var startW = Math.Clamp(offset.W, 0, _wSize);
            var endW = Math.Min(_wSize, offset.W + matrix._wSize);
            var startX = Math.Clamp(offset.X, 0, _xSize);
            var endX = Math.Min(_xSize, offset.X + matrix._xSize);
            var startY = Math.Clamp(offset.Y, 0, _ySize);
            var endY = Math.Min(_ySize, offset.Y + matrix._ySize);
            var startZ = Math.Clamp(offset.Z, 0, _zSize);
            var endZ = Math.Min(_zSize, offset.Z + matrix._zSize);
            var zCount = endZ - startZ;

            if (zCount > 0) {
                for (int w = startW; w < endW; w++) {
                    var dstW = _items[w];
                    var srcW = matrix._items[w - offset.W];
                    for (int x = startX; x < endX; x++) {
                        var dstWX = dstW[x];
                        var srcWX = srcW[x - offset.X];
                        for (int y = startY; y < endY; y++) {
                            Array.Copy(srcWX[y - offset.Y], startZ - offset.Z, dstWX[y], startZ, zCount);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places a 4D matrix into this List4D at the specified offset. If the matrix extends beyond
    /// the current bounds of the List4D, the List4D is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 4D array of elements to place into this List4D.</param>
    /// <param name="predicate">A function determining whether the item should be placed into this array.
    /// The first argument is an item from this array that is being overwritten by the second one.</param>
    /// <param name="offsetPoint">
    /// An optional offset defining where the top-left corner of the matrix will be placed.
    /// If not provided, the matrix will be placed at the origin of the List4D.
    /// </param>
    /// <param name="resize">If true, enables automatic resizing of this list depending on
    /// the size and offset of the placed array.</param>
    public void Place(T[,,,] matrix, Func<T, T, bool> predicate, Point4D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(
            Math.Max(_wSize, placedMax.W),
            Math.Max(_xSize, placedMax.X),
            Math.Max(_ySize, placedMax.Y),
            Math.Max(_zSize, placedMax.Z));
        var min = new Point4D(
            Math.Min(offset.W, 0),
            Math.Min(offset.X, 0),
            Math.Min(offset.Y, 0),
            Math.Min(offset.Z, 0));
        
        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W][][][];
            
            for (int w = 0; w < newSize.W; w++) {
                newItems[w] = Create3DArray(newSize.X, newSize.Y, newSize.Z);
            }

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            
            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (int w = 0; w < _wSize; w++) {
                var rowW = _items[w];
                var dstW = newItems[w + oldWOffset];
                for (int x = 0; x < _xSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + oldXOffset];
                    for (int y = 0; y < _ySize; y++) {
                        Array.Copy(rowWX[y], 0, dstWX[y + oldYOffset], oldZOffset, _zSize);
                    }
                }
            }

            // Copy new
            var len0 = matrix.GetLength(0);
            var len1 = matrix.GetLength(1);
            var len2 = matrix.GetLength(2);
            var len3 = matrix.GetLength(3);
            for (int w = 0; w < len0; w++) {
                var dstW = newItems[w + newWOffset];
                for (int x = 0; x < len1; x++) {
                    var dstWX = dstW[x + newXOffset];
                    for (int y = 0; y < len2; y++) {
                        var dstWXY = dstWX[y + newYOffset];
                        for (int z = 0; z < len3; z++) {
                            var val = matrix[w, x, y, z];
                            if (predicate(dstWXY[z + newZOffset], val)) {
                                dstWXY[z + newZOffset] = val;
                            }
                        }
                    }
                }
            }

            _items = newItems;
            _wSize = newSize.W; _xSize = newSize.X; _ySize = newSize.Y; _zSize = newSize.Z;
            _wCapacity = _wSize; _xCapacity = _xSize; _yCapacity = _ySize; _zCapacity = _zSize;
        }
        else {
            if ((placedMax.W > _wSize || placedMax.X > _xSize || placedMax.Y > _ySize || placedMax.Z > _zSize) && resize) {
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);
            }

            var startW = Math.Clamp(offset.W, 0, _wSize);
            var endW = Math.Min(_wSize, offset.W + matrix.GetLength(0));
            var startX = Math.Clamp(offset.X, 0, _xSize);
            var endX = Math.Min(_xSize, offset.X + matrix.GetLength(1));
            var startY = Math.Clamp(offset.Y, 0, _ySize);
            var endY = Math.Min(_ySize, offset.Y + matrix.GetLength(2));
            var startZ = Math.Clamp(offset.Z, 0, _zSize);
            var endZ = Math.Min(_zSize, offset.Z + matrix.GetLength(3));

            for (int w = startW; w < endW; w++) {
                var rowW = _items[w];
                var srcW = w - offset.W;
                for (int x = startX; x < endX; x++) {
                    var rowWX = rowW[x];
                    var srcX = x - offset.X;
                    for (int y = startY; y < endY; y++) {
                        var rowWXY = rowWX[y];
                        var srcY = y - offset.Y;
                        for (int z = startZ; z < endZ; z++) {
                            var val = matrix[srcW, srcX, srcY, z - offset.Z];
                            if (predicate(rowWXY[z], val)) {
                                rowWXY[z] = val;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places the contents of the specified 4D list into the current List4D instance, optionally offset by a specified point.
    /// </summary>
    /// <param name="matrix">The List4D instance containing the elements to be placed.</param>
    /// <param name="predicate">A function determining whether the item should be placed into this array.
    /// The first argument is an item from this array that is being overwritten by the second one.</param>
    /// <param name="offsetPoint">
    /// An optional point specifying the offset at which the matrix should be placed.
    /// If null, the matrix will be placed starting at the origin (0, 0, 0, 0).
    /// </param>
    /// <param name="resize">If true, enables automatic resizing of this list depending on
    /// the size and offset of the placed array.</param>
    public void Place(List4D<T> matrix, Func<T, T, bool> predicate, Point4D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(
            Math.Max(_wSize, placedMax.W),
            Math.Max(_xSize, placedMax.X),
            Math.Max(_ySize, placedMax.Y),
            Math.Max(_zSize, placedMax.Z));
        var min = new Point4D(
            Math.Min(offset.W, 0),
            Math.Min(offset.X, 0),
            Math.Min(offset.Y, 0),
            Math.Min(offset.Z, 0));
        
        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W][][][];
            
            for (int w = 0; w < newSize.W; w++) {
                newItems[w] = Create3DArray(newSize.X, newSize.Y, newSize.Z);
            }

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            
            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (int w = 0; w < _wSize; w++) {
                var rowW = _items[w];
                var dstW = newItems[w + oldWOffset];
                for (int x = 0; x < _xSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + oldXOffset];
                    for (int y = 0; y < _ySize; y++) {
                        Array.Copy(rowWX[y], 0, dstWX[y + oldYOffset], oldZOffset, _zSize);
                    }
                }
            }

            // Copy new
            for (int w = 0; w < matrix._wSize; w++) {
                var srcW = matrix._items[w];
                var dstW = newItems[w + newWOffset];
                for (int x = 0; x < matrix._xSize; x++) {
                    var srcWX = srcW[x];
                    var dstWX = dstW[x + newXOffset];
                    for (int y = 0; y < matrix._ySize; y++) {
                        var srcWXY = srcWX[y];
                        var dstWXY = dstWX[y + newYOffset];
                        for (int z = 0; z < matrix._zSize; z++) {
                            var val = srcWXY[z];
                            if (predicate(dstWXY[z + newZOffset], val)) {
                                dstWXY[z + newZOffset] = val;
                            }
                        }
                    }
                }
            }

            _items = newItems;
            _wSize = newSize.W; _xSize = newSize.X; _ySize = newSize.Y; _zSize = newSize.Z;
            _wCapacity = _wSize; _xCapacity = _xSize; _yCapacity = _ySize; _zCapacity = _zSize;
        }
        else {
            if ((placedMax.W > _wSize || placedMax.X > _xSize || placedMax.Y > _ySize || placedMax.Z > _zSize) && resize) {
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);
            }

            var startW = Math.Clamp(offset.W, 0, _wSize);
            var endW = Math.Min(_wSize, offset.W + matrix._wSize);
            var startX = Math.Clamp(offset.X, 0, _xSize);
            var endX = Math.Min(_xSize, offset.X + matrix._xSize);
            var startY = Math.Clamp(offset.Y, 0, _ySize);
            var endY = Math.Min(_ySize, offset.Y + matrix._ySize);
            var startZ = Math.Clamp(offset.Z, 0, _zSize);
            var endZ = Math.Min(_zSize, offset.Z + matrix._zSize);

            for (int w = startW; w < endW; w++) {
                var dstW = _items[w];
                var srcW = matrix._items[w - offset.W];
                for (int x = startX; x < endX; x++) {
                    var dstWX = dstW[x];
                    var srcWX = srcW[x - offset.X];
                    for (int y = startY; y < endY; y++) {
                        var dstWXY = dstWX[y];
                        var srcWXY = srcWX[y - offset.Y];
                        for (int z = startZ; z < endZ; z++) {
                            var val = srcWXY[z - offset.Z];
                            if (predicate(dstWXY[z], val)) {
                                dstWXY[z] = val;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fills the entire 4D list with the specified value.
    /// </summary>
    /// <param name="item">The value to fill the 4D list with.</param>
    public void Fill(T item) {
        for (int w = 0; w < _wSize; w++)
            for (int x = 0; x < _xSize; x++)
                for (int y = 0; y < _ySize; y++)
                    Array.Fill(_items[w][x][y], item, 0, _zSize);
    }

    /// <summary>
    /// Fills the specified 4D region of the list with a given value.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="wStart">The starting index on the W-axis (inclusive).</param>
    /// <param name="wCount">The number of elements to be filled along the W-axis.</param>
    /// <param name="xStart">The starting index on the X-axis (inclusive).</param>
    /// <param name="xCount">The number of elements to be filled along the X-axis.</param>
    /// <param name="yStart">The starting index on the Y-axis (inclusive).</param>
    /// <param name="yCount">The number of elements to be filled along the Y-axis.</param>
    /// <param name="zStart">The starting index on the Z-axis (inclusive).</param>
    /// <param name="zCount">The number of elements to be filled along the Z-axis.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified region exceeds the bounds of the list or one of the count parameters is negative.
    /// </exception>
    public void Fill(T item, int wStart, int wCount, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        int wEnd = wStart + wCount;
        int xEnd = xStart + xCount;
        int yEnd = yStart + yCount;
        int zEnd = zStart + zCount;

        if (wStart < 0 || xStart < 0 || yStart < 0 || zStart < 0 || 
            wEnd > _wSize || xEnd > _xSize || yEnd > _ySize || zEnd > _zSize || 
            wCount < 0 || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        for (int w = wStart; w < wEnd; w++)
            for (int x = xStart; x < xEnd; x++)
                for (int y = yStart; y < yEnd; y++)
                    Array.Fill(_items[w][x][y], item, zStart, zCount);
    }

    /// <summary>
    /// Fills the 4D list with the specified value in the given region.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(T item, Point4D offset, Size4D size)
        => Fill(item, offset.W, size.W, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>
    /// Fills the entire 4D list with the values generated by the specified factory function.
    /// </summary>
    /// <param name="factory">A function that generates values to fill the 4D list.</param>
    public void Fill(Func<T> factory) {
        for (int w = 0; w < _wSize; w++) {
            var rowW = _items[w];
            for (int x = 0; x < _xSize; x++) {
                var rowWX = rowW[x];
                for (int y = 0; y < _ySize; y++) {
                    var rowWXY = rowWX[y];
                    for (int z = 0; z < _zSize; z++) {
                        rowWXY[z] = factory();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fills the 4D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="wStart">Start w coordinate of the filled region.</param>
    /// <param name="wCount">The number of units in w-axis to be filled in the filled region.</param>
    /// <param name="xStart">Start x coordinate of the filled region.</param>
    /// <param name="xCount">The number of units in x-axis to be filled in the filled region.</param>
    /// <param name="yStart">Start y coordinate of the filled region.</param>
    /// <param name="yCount">The number of units in y-axis to be filled in the filled region.</param>
    /// <param name="zStart">Start z coordinate of the filled region.</param>
    /// <param name="zCount">The number of units in z-axis to be filled in the filled region.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified region exceeds the bounds of the list or one of the count parameters is negative.
    /// </exception>
    public void Fill(Func<T> factory, int wStart, int wCount, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        int wEnd = wStart + wCount;
        int xEnd = xStart + xCount;
        int yEnd = yStart + yCount;
        int zEnd = zStart + zCount;

        if (wStart < 0 || xStart < 0 || yStart < 0 || zStart < 0 || 
            wEnd > _wSize || xEnd > _xSize || yEnd > _ySize || zEnd > _zSize || 
            wCount < 0 || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        for (int w = wStart; w < wEnd; w++) {
            var rowW = _items[w];
            for (int x = xStart; x < xEnd; x++) {
                var rowWX = rowW[x];
                for (int y = yStart; y < yEnd; y++) {
                    var rowWXY = rowWX[y];
                    for (int z = zStart; z < zEnd; z++) {
                        rowWXY[z] = factory();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fills the 4D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(Func<T> factory, Point4D offset, Size4D size)
        => Fill(factory, offset.W, size.W, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>
    /// Converts the 4D list into a four-dimensional array. (Creates a copy)
    /// </summary>
    /// <returns>A four-dimensional array containing the elements of the 4D list.</returns>
    [Pure]
    public T[,,,] ToArray() {
        var arr = new T[_wSize, _xSize, _ySize, _zSize];
        if (_wSize == 0 || _xSize == 0 || _ySize == 0 || _zSize == 0) return arr;

        for (int w = 0; w < _wSize; w++) {
            var rowW = _items[w];
            for (int x = 0; x < _xSize; x++) {
                var rowWX = rowW[x];
                for (int y = 0; y < _ySize; y++) {
                    var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref rowWX[y][0], _zSize);
                    var dstSpan = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref arr[w, x, y, 0], _zSize);
                    srcSpan.CopyTo(dstSpan);
                }
            }
        }
        return arr;
    }

    /// <summary>
    /// Converts the 4D list into a jagged array. (Creates a copy)
    /// </summary>
    /// <returns>A jagged array representation of the 4D list.</returns>
    [Pure]
    public T[][][][] ToJagged() {
        var arr = new T[_wSize][][][];

        for (int w = 0; w < _wSize; w++) {
            arr[w] = new T[_xSize][][];
            for (int x = 0; x < _xSize; x++) {
                arr[w][x] = new T[_ySize][];
                for (int y = 0; y < _ySize; y++) {
                    arr[w][x][y] = new T[_zSize];
                    Array.Copy(_items[w][x][y], arr[w][x][y], _zSize);
                }
            }
        }

        return arr;
    }

    /// <summary>
    /// Copies the elements of the 4D list to a 4D array, starting at the specified destination index.
    /// </summary>
    /// <param name="array">The destination 4D array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    /// <exception cref="ArgumentException">Thrown if the destination array is not large enough.</exception>
    public void CopyTo(T[,,,] array, Point4D index) {
        if (array.GetLength(0) < _wSize + index.W) throw new ArgumentException("Destination array is not large enough in w dimension.");
        if (array.GetLength(1) < _xSize + index.X) throw new ArgumentException("Destination array is not large enough in x dimension.");
        if (array.GetLength(2) < _ySize + index.Y) throw new ArgumentException("Destination array is not large enough in y dimension.");
        if (array.GetLength(3) < _zSize + index.Z) throw new ArgumentException("Destination array is not large enough in z dimension.");
        if (_wSize == 0 || _xSize == 0 || _ySize == 0 || _zSize == 0) return;

        for (int w = 0; w < _wSize; w++) {
            var rowW = _items[w];
            for (int x = 0; x < _xSize; x++) {
                var rowWX = rowW[x];
                for (int y = 0; y < _ySize; y++) {
                    var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref rowWX[y][0], _zSize);
                    var dstSpan = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref array[w + index.W, x + index.X, y + index.Y, index.Z], _zSize);
                    srcSpan.CopyTo(dstSpan);
                }
            }
        }
    }

    /// <summary>
    /// Copies the elements of the 4D list to a standard multidimensional array, starting at the specified destination index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    public void CopyTo(Array array, Point4D index) {
        for (int w = 0; w < _wSize; w++) {
            var rowW = _items[w];
            for (int x = 0; x < _xSize; x++) {
                var rowWX = rowW[x];
                for (int y = 0; y < _ySize; y++) {
                    var rowWXY = rowWX[y];
                    for (int z = 0; z < _zSize; z++) {
                        array.SetValue(rowWXY[z], w + index.W, x + index.X, y + index.Y, z + index.Z);
                    }
                }
            }
        }
    }

    public IEnumerator<IEnumerable3D<T>> GetEnumerator() {
        for (int w = 0; w < _wSize; w++) yield return GetAtW(w);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable4D.GetEnumerator() => GetEnumerator();

    public IEnumerable3D<T> GetAtW(int w) => new SliceW(this, w);
    public IEnumerable3D<T> GetAtX(int x) => new SliceX(this, x);
    public IEnumerable3D<T> GetAtY(int y) => new SliceY(this, y);
    public IEnumerable3D<T> GetAtZ(int z) => new SliceZ(this, z);

    IEnumerable3D IEnumerable4D.GetAtW(int w) => GetAtW(w);
    IEnumerable3D IEnumerable4D.GetAtX(int x) => GetAtX(x);
    IEnumerable3D IEnumerable4D.GetAtY(int y) => GetAtY(y);
    IEnumerable3D IEnumerable4D.GetAtZ(int z) => GetAtZ(z);

    private T[][][] Create3DArray(int x, int y, int z) {
        var arr = new T[x][][];
        for (int i = 0; i < x; i++) arr[i] = Create2DArray(y, z);
        return arr;
    }

    private T[][] Create2DArray(int y, int z) {
        var arr = new T[y][];
        for (int i = 0; i < y; i++) arr[i] = new T[z];
        return arr;
    }

    private class SliceW : IEnumerable3D<T> {
        private readonly List4D<T> _parent;
        private readonly int _w;
        public SliceW(List4D<T> parent, int w) { _parent = parent; _w = w; }
        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (int x = 0; x < _parent._xSize; x++) yield return GetAtX(x);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();

        public IEnumerable2D<T> GetAtX(int x) => new SliceWX(_parent, _w, x);
        public IEnumerable2D<T> GetAtY(int y) => new SliceWY(_parent, _w, y); 
        public IEnumerable2D<T> GetAtZ(int z) => new SliceWZ(_parent, _w, z); 

        IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);
    }

    private class SliceX : IEnumerable3D<T> {
        private readonly List4D<T> _parent;
        private readonly int _x;
        public SliceX(List4D<T> parent, int x) { _parent = parent; _x = x; }
        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (int w = 0; w < _parent._wSize; w++) yield return GetAtX(w);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();

        public IEnumerable2D<T> GetAtX(int w) => new SliceWX(_parent, w, _x);
        public IEnumerable2D<T> GetAtY(int y) => new SliceXY(_parent, _x, y);
        public IEnumerable2D<T> GetAtZ(int z) => new SliceXZ(_parent, _x, z);

        IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);
    }
    
    private class SliceY : IEnumerable3D<T> {
        private readonly List4D<T> _parent;
        private readonly int _y;
        public SliceY(List4D<T> parent, int y) { _parent = parent; _y = y; }
        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (int w = 0; w < _parent._wSize; w++) yield return GetAtX(w);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();
        public IEnumerable2D<T> GetAtX(int w) => new SliceWY(_parent, w, _y);
        public IEnumerable2D<T> GetAtY(int x) => new SliceXY(_parent, x, _y);
        public IEnumerable2D<T> GetAtZ(int z) => new SliceYZ(_parent, _y, z);
        IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);
    }

    private class SliceZ : IEnumerable3D<T> {
        private readonly List4D<T> _parent;
        private readonly int _z;
        public SliceZ(List4D<T> parent, int z) { _parent = parent; _z = z; }
        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (int w = 0; w < _parent._wSize; w++) yield return GetAtX(w);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();
        public IEnumerable2D<T> GetAtX(int w) => new SliceWZ(_parent, w, _z);
        public IEnumerable2D<T> GetAtY(int x) => new SliceXZ(_parent, x, _z);
        public IEnumerable2D<T> GetAtZ(int y) => new SliceYZ(_parent, y, _z);
        IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);
    }

    private class SliceWX : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int _w, _x;
        public SliceWX(List4D<T> parent, int w, int x) { _parent = parent; _w = w; _x = x; }
        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int y = 0; y < _parent._ySize; y++) yield return GetAtY(y);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();
        public IEnumerable<T> GetAtY(int y) {
            for (int z = 0; z < _parent._zSize; z++) yield return _parent._items[_w][_x][y][z];
        }
        public IEnumerable<T> GetAtX(int x) => GetAtY(x);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceWY : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int _w, _y;
        public SliceWY(List4D<T> parent, int w, int y) { _parent = parent; _w = w; _y = y; }
        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int x = 0; x < _parent._xSize; x++) yield return GetAtX(x);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();
        public IEnumerable<T> GetAtX(int x) {
            for (int z = 0; z < _parent._zSize; z++) yield return _parent._items[_w][x][_y][z];
        }
        public IEnumerable<T> GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceWZ : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int _w, _z;
        public SliceWZ(List4D<T> parent, int w, int z) { _parent = parent; _w = w; _z = z; }
        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int x = 0; x < _parent._xSize; x++) yield return GetAtX(x);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();
        public IEnumerable<T> GetAtX(int x) {
            for (int y = 0; y < _parent._ySize; y++) yield return _parent._items[_w][x][y][_z];
        }
        public IEnumerable<T> GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceXY : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int _x, _y;
        public SliceXY(List4D<T> parent, int x, int y) { _parent = parent; _x = x; _y = y; }
        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int w = 0; w < _parent._wSize; w++) yield return GetAtX(w);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();
        public IEnumerable<T> GetAtX(int w) {
            for (int z = 0; z < _parent._zSize; z++) yield return _parent._items[w][_x][_y][z];
        }
        public IEnumerable<T> GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceXZ : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int _x, _z;
        public SliceXZ(List4D<T> parent, int x, int z) { _parent = parent; _x = x; _z = z; }
        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int w = 0; w < _parent._wSize; w++) yield return GetAtX(w);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();
        public IEnumerable<T> GetAtX(int w) {
            for (int y = 0; y < _parent._ySize; y++) yield return _parent._items[w][_x][y][_z];
        }
        public IEnumerable<T> GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceYZ : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int _y, _z;
        public SliceYZ(List4D<T> parent, int y, int z) { _parent = parent; _y = y; _z = z; }
        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int w = 0; w < _parent._wSize; w++) yield return GetAtX(w);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();
        public IEnumerable<T> GetAtX(int w) {
            for (int x = 0; x < _parent._xSize; x++) yield return _parent._items[w][x][_y][_z];
        }
        public IEnumerable<T> GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }
}