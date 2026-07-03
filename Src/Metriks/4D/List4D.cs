// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
///     Represents a strongly typed, four-dimensional list of elements that can be accessed by W, X, Y, and Z indices.
///     Provides methods to search, sort, and manipulate 4D lists.
/// </summary>
/// <typeparam name="T">The type of elements in the four-dimensional list.</typeparam>
public class List4D<T> : IList4D<T>, ICollection4D, IReadOnlyList4D<T> {
    private const int   INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR    = 2f;

    private T[] _items;

    /// <summary>
    ///     Initializes a new instance of the <see cref="List4D{T}" /> class with the specified initial capacity for each
    ///     dimension.
    /// </summary>
    /// <param name="wCapacity">The initial capacity along the W-axis.</param>
    /// <param name="xCapacity">The initial capacity along the X-axis.</param>
    /// <param name="yCapacity">The initial capacity along the Y-axis.</param>
    /// <param name="zCapacity">The initial capacity along the Z-axis.</param>
    public List4D(
        int wCapacity = INITIAL_CAPACITY,
        int xCapacity = INITIAL_CAPACITY,
        int yCapacity = INITIAL_CAPACITY,
        int zCapacity = INITIAL_CAPACITY
    ) {
        _items    = new T[wCapacity * xCapacity * yCapacity * zCapacity];
        WSize     = 0;
        XSize     = 0;
        YSize     = 0;
        ZSize     = 0;
        WCapacity = wCapacity;
        XCapacity = xCapacity;
        YCapacity = yCapacity;
        ZCapacity = zCapacity;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="List4D{T}" /> class populated with elements from the specified 4D
    ///     array.
    /// </summary>
    /// <param name="collection">The 4D array of elements to copy from.</param>
    public List4D(T[,,,] collection) {
        ArgumentNullException.ThrowIfNull(collection);

        WSize     = collection.GetLength(0);
        XSize     = collection.GetLength(1);
        YSize     = collection.GetLength(2);
        ZSize     = collection.GetLength(3);
        WCapacity = WSize;
        XCapacity = XSize;
        YCapacity = YSize;
        ZCapacity = ZSize;

        int totalElements = WSize * XSize * YSize * ZSize;

        if (totalElements == 0) {
            _items = Array.Empty<T>();

            return;
        }

        _items = new T[totalElements];
        var sourceSpan = MemoryMarshal.CreateReadOnlySpan(ref collection[0, 0, 0, 0], totalElements);
        sourceSpan.CopyTo(_items);
    }

    /// <summary>
    ///     Gets the underlying flat array used to store the elements of the <see cref="List4D{T}" /> instance.
    ///     PROVIDED FOR INTERNAL USE ONLY. DO NOT USE. <b>!!!DO NOT MODIFY THE ARRAY IN ANY WAY!!!</b>
    /// </summary>
    internal T[] Items {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _items;
    }

    /// <summary>Gets the size along the W-axis.</summary>
    public int WSize { get; private set; }

    /// <summary>Gets the size along the X-axis.</summary>
    public int XSize { get; private set; }

    /// <summary>Gets the size along the Y-axis.</summary>
    public int YSize { get; private set; }

    /// <summary>Gets the size along the Z-axis.</summary>
    public int ZSize { get; private set; }

    /// <summary>Gets a <see cref="Size4D" /> representing the current size of the list in all four dimensions.</summary>
    public Size4D Size => new(WSize, XSize, YSize, ZSize);

    /// <summary>Gets the capacity along the W-axis.</summary>
    public int WCapacity { get; private set; }

    /// <summary>Gets the capacity along the X-axis.</summary>
    public int XCapacity { get; private set; }

    /// <summary>Gets the capacity along the Y-axis.</summary>
    public int YCapacity { get; private set; }

    /// <summary>Gets the capacity along the Z-axis.</summary>
    public int ZCapacity { get; private set; }

    /// <summary>Copies the elements of the 4D list to a standard multidimensional array, starting at the specified destination index.</summary>
    public void CopyTo(Array array, Point4D index) {
        ArgumentNullException.ThrowIfNull(array);

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
        for (var z = 0; z < ZSize; z++)
            array.SetValue(UnsafeGet(w, x, y, z), w + index.W, x + index.X, y + index.Y, z + index.Z);
    }

    /// <summary>Gets the total number of elements contained in the list.</summary>
    public int Count => WSize * XSize * YSize * ZSize;

    /// <summary>Gets the size along the W-axis.</summary>
    public int WCount => WSize;

    /// <summary>Gets the size along the X-axis.</summary>
    public int XCount => XSize;

    /// <summary>Gets the size along the Y-axis.</summary>
    public int YCount => YSize;

    /// <summary>Gets the size along the Z-axis.</summary>
    public int ZCount => ZSize;

    /// <summary>Gets a value indicating whether the list is read-only.</summary>
    public bool IsReadOnly => false;

    public T this[int w, int x, int y, int z] {
        get {
            ValidateIndexBounds(w, x, y, z);

            return _items[GetOffset(w, x, y, z)];
        }
        set {
            ValidateIndexBounds(w, x, y, z);
            _items[GetOffset(w, x, y, z)] = value;
        }
    }

    /// <summary>
    ///     Gets or sets the element at the specified <see cref="Point4D" />.
    /// </summary>
    public T this[Point4D point] {
        get => this[point.W, point.X, point.Y, point.Z];
        set => this[point.W, point.X, point.Y, point.Z] = value;
    }

    [StackTraceHidden]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateIndexBounds(int w, int x, int y, int z) {
        if (w < 0 || w >= WSize || x < 0 || x >= XSize || y < 0 || y >= YSize || z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException($"Index [{w}, {x}, {y}, {z}] is out of bounds.");
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetOffset(int w, int x, int y, int z) => ((w * XCapacity + x) * YCapacity + y) * ZCapacity + z;

    /// <summary>Inserts a new element at the specified index along W-axis.</summary>
    public void InsertAtW(int w) {
        if (w < 0 || w > WSize)
            throw new IndexOutOfRangeException();

        EnsureWCapacity(WSize + 1);
        var stride = XCapacity * YCapacity * ZCapacity;

        if (w < WSize) {
            var srcStart = w           * stride;
            var dstStart = (w     + 1) * stride;
            var length   = (WSize - w) * stride;
            Array.Copy(_items, srcStart, _items, dstStart, length);
        }

        _items.AsSpan(w * stride, stride).Clear();
        WSize++;
    }

    /// <summary>Inserts a new element at the specified index along X-axis.</summary>
    public void InsertAtX(int x) {
        if (x < 0 || x > XSize)
            throw new IndexOutOfRangeException();

        EnsureXCapacity(XSize   + 1);
        var shiftLength = XSize - x;
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        if (shiftLength > 0)
            for (var w = WSize - 1; w >= 0; w--) {
                var srcStart = w * wStride + x       * sliceStride;
                var dstStart = w * wStride + (x + 1) * sliceStride;
                Array.Copy(_items, srcStart, _items, dstStart, shiftLength * sliceStride);
                _items.AsSpan(srcStart, sliceStride).Clear();
            }
        else
            for (var w = 0; w < WSize; w++)
                _items.AsSpan(w * wStride + x * sliceStride, sliceStride).Clear();

        XSize++;
    }

    /// <summary>Inserts a new element at the specified index along Y-axis.</summary>
    public void InsertAtY(int y) {
        if (y < 0 || y > YSize)
            throw new IndexOutOfRangeException();

        EnsureYCapacity(YSize   + 1);
        var shiftLength = YSize - y;
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        if (shiftLength > 0)
            for (var w = WSize - 1; w >= 0; w--)
            for (var x = XSize - 1; x >= 0; x--) {
                var srcStart = w * wStride + x * sliceStride + y       * ZCapacity;
                var dstStart = w * wStride + x * sliceStride + (y + 1) * ZCapacity;
                Array.Copy(_items, srcStart, _items, dstStart, shiftLength * ZCapacity);
                _items.AsSpan(srcStart, ZCapacity).Clear();
            }
        else
            for (var w = 0; w < WSize; w++)
            for (var x = 0; x < XSize; x++)
                _items.AsSpan(w * wStride + x * sliceStride + y * ZCapacity, ZCapacity).Clear();

        YSize++;
    }

    /// <summary>Inserts a new element at the specified index along Z-axis.</summary>
    public void InsertAtZ(int z) {
        if (z < 0 || z > ZSize)
            throw new IndexOutOfRangeException();

        EnsureZCapacity(ZSize   + 1);
        var shiftLength = ZSize - z;
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        if (shiftLength > 0)
            for (var w = WSize - 1; w >= 0; w--)
            for (var x = XSize - 1; x >= 0; x--)
            for (var y = YSize - 1; y >= 0; y--) {
                var srcStart = w * wStride                    + x * sliceStride + y * ZCapacity + z;
                Array.Copy(_items, srcStart, _items, srcStart + 1, shiftLength);
                _items[srcStart] = default!;
            }
        else
            for (var w = 0; w < WSize; w++)
            for (var x = 0; x < XSize; x++)
            for (var y = 0; y < YSize; y++)
                _items[w * wStride + x * sliceStride + y * ZCapacity + z] = default!;

        ZSize++;
    }

    /// <summary>Adds a new element at the end of the W-axis.</summary>
    public void AddW() => InsertAtW(WSize);

    /// <summary>Adds a new element at the end of the X-axis.</summary>
    public void AddX() => InsertAtX(XSize);

    /// <summary>Adds a new element at the end of the Y-axis.</summary>
    public void AddY() => InsertAtY(YSize);

    /// <summary>Adds a new element at the end of the Z-axis.</summary>
    public void AddZ() => InsertAtZ(ZSize);

    /// <summary>Removes the element at the specified index along W-axis.</summary>
    public void RemoveAtW(int w) {
        if (w < 0 || w >= WSize)
            throw new IndexOutOfRangeException();

        WSize--;
        var stride = XCapacity * YCapacity * ZCapacity;

        if (w < WSize) {
            var srcStart = (w + 1)     * stride;
            var dstStart = w           * stride;
            var length   = (WSize - w) * stride;
            Array.Copy(_items, srcStart, _items, dstStart, length);
        }

        _items.AsSpan(WSize * stride, stride).Clear();
    }

    /// <summary>Removes the element at the specified index along X-axis.</summary>
    public void RemoveAtX(int x) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException();

        XSize--;
        var shiftLength = XSize - x;
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++) {
            var wStart = w * wStride;

            if (shiftLength > 0)
                Array.Copy(_items, wStart + (x + 1) * sliceStride, _items, wStart + x * sliceStride, shiftLength * sliceStride);

            _items.AsSpan(wStart + XSize * sliceStride, sliceStride).Clear();
        }
    }

    /// <summary>Removes the element at the specified index along Y-axis.</summary>
    public void RemoveAtY(int y) {
        if (y < 0 || y >= YSize)
            throw new IndexOutOfRangeException();

        YSize--;
        var shiftLength = YSize - y;
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++) {
            var colStart = w * wStride + x * sliceStride;

            if (shiftLength > 0)
                Array.Copy(_items, colStart + (y + 1) * ZCapacity, _items, colStart + y * ZCapacity, shiftLength * ZCapacity);

            _items.AsSpan(colStart + YSize * ZCapacity, ZCapacity).Clear();
        }
    }

    /// <summary>Removes the element at the specified index along Z-axis.</summary>
    public void RemoveAtZ(int z) {
        if (z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException();

        ZSize--;
        var shiftLength = ZSize - z;
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var rowStart = w * wStride + x * sliceStride + y * ZCapacity;

            if (shiftLength > 0)
                Array.Copy(_items, rowStart + z + 1, _items, rowStart + z, shiftLength);

            _items[rowStart + ZSize] = default!;
        }
    }

    /// <summary>Shrinks the capacity of the W-axis to the current size.</summary>
    public void ShrinkW() => ResizeCapacity(WSize, XCapacity, YCapacity, ZCapacity);

    /// <summary>Shrinks the capacity of the X-axis to the current size.</summary>
    public void ShrinkX() => ResizeCapacity(WCapacity, XSize, YCapacity, ZCapacity);

    /// <summary>Shrinks the capacity of the Y-axis to the current size.</summary>
    public void ShrinkY() => ResizeCapacity(WCapacity, XCapacity, YSize, ZCapacity);

    /// <summary>Shrinks the capacity of the Z-axis to the current size.</summary>
    public void ShrinkZ() => ResizeCapacity(WCapacity, XCapacity, YCapacity, ZSize);

    /// <summary>Checks whether the list contains the specified value.</summary>
    public bool Contains(T value) {
        if (WSize == 0 || XSize == 0 || YSize == 0 || ZSize == 0)
            return false;

        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            if (Array.IndexOf(_items, value, w * wStride + x * sliceStride + y * ZCapacity, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>Checks whether the list contains the specified value along W-axis.</summary>
    public bool ContainsAtW(int w, T value) {
        if (w < 0 || w >= WSize)
            return false;

        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            if (Array.IndexOf(_items, value, w * wStride + x * sliceStride + y * ZCapacity, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>Checks whether the list contains the specified value along X-axis.</summary>
    public bool ContainsAtX(int x, T value) {
        if (x < 0 || x >= XSize)
            return false;

        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var y = 0; y < YSize; y++)
            if (Array.IndexOf(_items, value, w * wStride + x * sliceStride + y * ZCapacity, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>Checks whether the list contains the specified value along Y-axis.</summary>
    public bool ContainsAtY(int y, T value) {
        if (y < 0 || y >= YSize)
            return false;

        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
            if (Array.IndexOf(_items, value, w * wStride + x * sliceStride + y * ZCapacity, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>Checks whether the list contains the specified value along Z-axis.</summary>
    public bool ContainsAtZ(int z, T value) {
        if (z < 0 || z >= ZSize)
            return false;

        var comparer    = EqualityComparer<T>.Default;
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            if (comparer.Equals(_items[w * wStride + x * sliceStride + y * ZCapacity + z], value))
                return true;

        return false;
    }

    /// <summary>
    ///     Resets the list to size 0 and ensures it has at least the specified capacity.
    ///     Clears all existing elements to default.
    /// </summary>
    public void Reset(int wCapacity, int xCapacity, int yCapacity, int zCapacity) {
        WSize     = 0;
        XSize     = 0;
        YSize     = 0;
        ZSize     = 0;
        int requiredLength = wCapacity * xCapacity * yCapacity * zCapacity;
        if (_items == null || _items.Length < requiredLength) {
            _items = new T[requiredLength];
        }
        else {
            Array.Clear(_items, 0, _items.Length);
        }
        WCapacity = wCapacity;
        XCapacity = xCapacity;
        YCapacity = yCapacity;
        ZCapacity = zCapacity;
    }

    /// <summary>Clears the entire 4D list.</summary>
    public void Clear() {
        _items    = new T[INITIAL_CAPACITY * INITIAL_CAPACITY * INITIAL_CAPACITY * INITIAL_CAPACITY];
        WSize     = 0;
        XSize     = 0;
        YSize     = 0;
        ZSize     = 0;
        WCapacity = INITIAL_CAPACITY;
        XCapacity = INITIAL_CAPACITY;
        YCapacity = INITIAL_CAPACITY;
        ZCapacity = INITIAL_CAPACITY;
    }

    /// <summary>Copies elements to the specified multidimensional array.</summary>
    public void CopyTo(T[,,,] array, Point4D index) {
        ArgumentNullException.ThrowIfNull(array);
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var srcSpan = _items.AsSpan(w * wStride + x * sliceStride + y * ZCapacity, ZSize);
            var dstSpan = MemoryMarshal.CreateSpan(ref array[w + index.W, x + index.X, y + index.Y, index.Z], ZSize);
            srcSpan.CopyTo(dstSpan);
        }
    }

    /// <summary>Returns an enumerator that iterates through the 3D slices of the 4D list.</summary>
    public IEnumerator<IEnumerable3D<T>> GetEnumerator() {
        for (var w = 0; w < WSize; w++)
            yield return GetAtW(w);
    }

    IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable4D.GetEnumerator() => GetEnumerator();

    /// <summary>Gets a 3D slice representation at the specified W index.</summary>
    public IEnumerable3D<T> GetAtW(int w) => new SliceW(this, w);

    /// <summary>Gets a 3D slice representation at the specified X index.</summary>
    public IEnumerable3D<T> GetAtX(int x) => new SliceX(this, x);

    /// <summary>Gets a 3D slice representation at the specified Y index.</summary>
    public IEnumerable3D<T> GetAtY(int y) => new SliceY(this, y);

    /// <summary>Gets a 3D slice representation at the specified Z index.</summary>
    public IEnumerable3D<T> GetAtZ(int z) => new SliceZ(this, z);

    IEnumerable3D IEnumerable4D.GetAtW(int w) => GetAtW(w);
    IEnumerable3D IEnumerable4D.GetAtX(int x) => GetAtX(x);
    IEnumerable3D IEnumerable4D.GetAtY(int y) => GetAtY(y);
    IEnumerable3D IEnumerable4D.GetAtZ(int z) => GetAtZ(z);

    /// <summary>Sets the element at the specified 4D index without bounds checking.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void UnsafeSet(int w, int x, int y, int z, T value) => _items[GetOffset(w, x, y, z)] = value;

    /// <summary>Gets the element at the specified 4D index without bounds checking.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T UnsafeGet(int w, int x, int y, int z) => _items[GetOffset(w, x, y, z)];

    private void EnsureWCapacity(int minWCapacity) {
        if (minWCapacity > WCapacity) {
            var newWCapacity = Math.Max(minWCapacity, (int)(WCapacity * GROWTH_FACTOR));

            if (newWCapacity < INITIAL_CAPACITY)
                newWCapacity = INITIAL_CAPACITY;

            ResizeCapacity(newWCapacity, XCapacity, YCapacity, ZCapacity);
        }
    }

    private void EnsureXCapacity(int minXCapacity) {
        if (minXCapacity > XCapacity) {
            var newXCapacity = Math.Max(minXCapacity, (int)(XCapacity * GROWTH_FACTOR));

            if (newXCapacity < INITIAL_CAPACITY)
                newXCapacity = INITIAL_CAPACITY;

            ResizeCapacity(WCapacity, newXCapacity, YCapacity, ZCapacity);
        }
    }

    private void EnsureYCapacity(int minYCapacity) {
        if (minYCapacity > YCapacity) {
            var newYCapacity = Math.Max(minYCapacity, (int)(YCapacity * GROWTH_FACTOR));

            if (newYCapacity < INITIAL_CAPACITY)
                newYCapacity = INITIAL_CAPACITY;

            ResizeCapacity(WCapacity, XCapacity, newYCapacity, ZCapacity);
        }
    }

    private void EnsureZCapacity(int minZCapacity) {
        if (minZCapacity > ZCapacity) {
            var newZCapacity = Math.Max(minZCapacity, (int)(ZCapacity * GROWTH_FACTOR));

            if (newZCapacity < INITIAL_CAPACITY)
                newZCapacity = INITIAL_CAPACITY;

            ResizeCapacity(WCapacity, XCapacity, YCapacity, newZCapacity);
        }
    }

    private void ResizeCapacity(int newWCapacity, int newXCapacity, int newYCapacity, int newZCapacity) {
        var newItems = new T[newWCapacity * newXCapacity * newYCapacity * newZCapacity];

        if (WSize > 0 && XSize > 0 && YSize > 0 && ZSize > 0) {
            if (XCapacity == newXCapacity && YCapacity == newYCapacity && ZCapacity == newZCapacity) {
                _items.AsSpan(0, WSize * XCapacity * YCapacity * ZCapacity).CopyTo(newItems);
            }
            else {
                var copyX = Math.Min(XCapacity, newXCapacity);
                var copyY = Math.Min(YCapacity, newYCapacity);
                var copyZ = Math.Min(ZCapacity, newZCapacity);

                for (var w = 0; w < WSize; w++)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, copyZ)
                        .CopyTo(newItems.AsSpan(((w * newXCapacity + x) * newYCapacity + y) * newZCapacity, copyZ));
            }
        }

        _items    = newItems;
        WCapacity = newWCapacity;
        XCapacity = newXCapacity;
        YCapacity = newYCapacity;
        ZCapacity = newZCapacity;
    }

    /// <summary>Expands the dimensions of the 4D list to the specified size.</summary>
    public void Expand(int wSize, int xSize, int ySize, int zSize, T? defaultValue = default!) {
        if (wSize < WSize || xSize < XSize || ySize < YSize || zSize < ZSize)
            throw new ArgumentOutOfRangeException();

        if (wSize == WSize && xSize == XSize && ySize == YSize && zSize == ZSize)
            return;

        var targetWCapacity = WCapacity;
        var targetXCapacity = XCapacity;
        var targetYCapacity = YCapacity;
        var targetZCapacity = ZCapacity;

        if (wSize > WCapacity)
            targetWCapacity = wSize + 1;

        if (xSize > XCapacity)
            targetXCapacity = xSize + 1;

        if (ySize > YCapacity)
            targetYCapacity = ySize + 1;

        if (zSize > ZCapacity)
            targetZCapacity = zSize + 1;

        if (targetWCapacity != WCapacity || targetXCapacity != XCapacity || targetYCapacity != YCapacity || targetZCapacity != ZCapacity)
            ResizeCapacity(targetWCapacity, targetXCapacity, targetYCapacity, targetZCapacity);

        if (zSize > ZSize)
            for (var w = 0; w < WSize; w++)
            for (var x = 0; x < XSize; x++)
            for (var y = 0; y < YSize; y++)
                _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity + ZSize, zSize - ZSize).Fill(defaultValue!);

        if (ySize > YSize)
            for (var w = 0; w < WSize; w++)
            for (var x = 0; x < XSize; x++)
            for (var y = YSize; y < ySize; y++)
                _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, zSize).Fill(defaultValue!);

        if (xSize > XSize)
            for (var w = 0; w < WSize; w++)
            for (var x = XSize; x < xSize; x++)
            for (var y = 0; y < ySize; y++)
                _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, zSize).Fill(defaultValue!);

        for (var w = WSize; w < wSize; w++)
        for (var x = 0; x < xSize; x++)
        for (var y = 0; y < ySize; y++)
            _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, zSize).Fill(defaultValue!);

        WSize = wSize;
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
    }

    /// <summary>Expands the dimensions of the 4D list with a factory generator.</summary>
    public void Expand(int wSize, int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        ArgumentNullException.ThrowIfNull(defaultValueFactory);

        if (wSize < WSize || xSize < XSize || ySize < YSize || zSize < ZSize)
            throw new ArgumentOutOfRangeException();

        if (wSize == WSize && xSize == XSize && ySize == YSize && zSize == ZSize)
            return;

        var targetWCapacity = WCapacity;
        var targetXCapacity = XCapacity;
        var targetYCapacity = YCapacity;
        var targetZCapacity = ZCapacity;

        if (wSize > WCapacity)
            targetWCapacity = wSize + 1;

        if (xSize > XCapacity)
            targetXCapacity = xSize + 1;

        if (ySize > YCapacity)
            targetYCapacity = ySize + 1;

        if (zSize > ZCapacity)
            targetZCapacity = zSize + 1;

        if (targetWCapacity != WCapacity || targetXCapacity != XCapacity || targetYCapacity != YCapacity || targetZCapacity != ZCapacity)
            ResizeCapacity(targetWCapacity, targetXCapacity, targetYCapacity, targetZCapacity);

        if (zSize > ZSize)
            for (var w = 0; w < WSize; w++)
            for (var x = 0; x < XSize; x++)
            for (var y = 0; y < YSize; y++) {
                var start = ((w * XCapacity + x) * YCapacity + y) * ZCapacity;

                for (var z = ZSize; z < zSize; z++)
                    _items[start + z] = defaultValueFactory();
            }

        if (ySize > YSize)
            for (var w = 0; w < WSize; w++)
            for (var x = 0; x < XSize; x++)
            for (var y = YSize; y < ySize; y++) {
                var start = ((w * XCapacity + x) * YCapacity + y) * ZCapacity;

                for (var z = 0; z < zSize; z++)
                    _items[start + z] = defaultValueFactory();
            }

        if (xSize > XSize)
            for (var w = 0; w < WSize; w++)
            for (var x = XSize; x < xSize; x++)
            for (var y = 0; y < ySize; y++) {
                var start = ((w * XCapacity + x) * YCapacity + y) * ZCapacity;

                for (var z = 0; z < zSize; z++)
                    _items[start + z] = defaultValueFactory();
            }

        for (var w = WSize; w < wSize; w++)
        for (var x = 0; x < xSize; x++)
        for (var y = 0; y < ySize; y++) {
            var start = ((w * XCapacity + x) * YCapacity + y) * ZCapacity;

            for (var z = 0; z < zSize; z++)
                _items[start + z] = defaultValueFactory();
        }

        WSize = wSize;
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
    }

    /// <summary>Resizes the 4D list dimensions.</summary>
    public void Resize(int wSize, int xSize, int ySize, int zSize, T? defaultValue = default) {
        if (wSize < 0 || xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[wSize * xSize * ySize * zSize];

        if (defaultValue is not null)
            newItems.AsSpan().Fill(defaultValue);

        var copyW = Math.Min(WSize, wSize);
        var copyX = Math.Min(XSize, xSize);
        var copyY = Math.Min(YSize, ySize);
        var copyZ = Math.Min(ZSize, zSize);

        if (copyW > 0 && copyX > 0 && copyY > 0 && copyZ > 0)
            for (var w = 0; w < copyW; w++)
            for (var x = 0; x < copyX; x++)
            for (var y = 0; y < copyY; y++)
                _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, copyZ)
                    .CopyTo(newItems.AsSpan(((w * wSize + x) * ySize + y) * zSize, copyZ));

        _items    = newItems;
        WSize     = wSize;
        XSize     = xSize;
        YSize     = ySize;
        ZSize     = zSize;
        WCapacity = wSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    /// <summary>Resizes the 4D list dimensions using a factory method.</summary>
    public void Resize(int wSize, int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        ArgumentNullException.ThrowIfNull(defaultValueFactory);

        if (wSize < 0 || xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[wSize * xSize * ySize * zSize];

        for (var i = 0; i < newItems.Length; i++)
            newItems[i] = defaultValueFactory();

        var copyW = Math.Min(WSize, wSize);
        var copyX = Math.Min(XSize, xSize);
        var copyY = Math.Min(YSize, ySize);
        var copyZ = Math.Min(ZSize, zSize);

        if (copyW > 0 && copyX > 0 && copyY > 0 && copyZ > 0)
            for (var w = 0; w < copyW; w++)
            for (var x = 0; x < copyX; x++)
            for (var y = 0; y < copyY; y++)
                _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, copyZ)
                    .CopyTo(newItems.AsSpan(((w * wSize + x) * ySize + y) * zSize, copyZ));

        _items    = newItems;
        WSize     = wSize;
        XSize     = xSize;
        YSize     = ySize;
        ZSize     = zSize;
        WCapacity = wSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    /// <summary>Shrinks the 4D list dimensions.</summary>
    public void Shrink(int wSize, int xSize, int ySize, int zSize) {
        if (wSize > WSize || xSize > XSize || ySize > YSize || zSize > ZSize)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[wSize * xSize * ySize * zSize];

        if (wSize > 0 && xSize > 0 && ySize > 0 && zSize > 0)
            for (var w = 0; w < wSize; w++)
            for (var x = 0; x < xSize; x++)
            for (var y = 0; y < ySize; y++)
                _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, zSize)
                    .CopyTo(newItems.AsSpan(((w * wSize + x) * ySize + y) * zSize, zSize));

        _items    = newItems;
        WSize     = wSize;
        XSize     = xSize;
        YSize     = ySize;
        ZSize     = zSize;
        WCapacity = wSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    /// <summary>Places a 4D matrix into this list.</summary>
    public void Place(T[,,,] matrix, Point4D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);
        var offset = offsetPoint ?? Point4D.Zero;

        var matW = matrix.GetLength(0);
        var matX = matrix.GetLength(1);
        var matY = matrix.GetLength(2);
        var matZ = matrix.GetLength(3);

        var placedMax = offset + new Point4D(matW, matX, matY, matZ);

        var max = new Point4D(Math.Max(WSize, placedMax.W), Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point4D(Math.Min(offset.W, 0),        Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W * newSize.X * newSize.Y * newSize.Z];

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            if (WSize > 0 && XSize > 0 && YSize > 0 && ZSize > 0)
                for (var w = 0; w < WSize; w++)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, ZSize)
                        .CopyTo(newItems.AsSpan((((w + oldWOffset) * newSize.X + x + oldXOffset) * newSize.Y + y + oldYOffset) * newSize.Z + oldZOffset, ZSize));

            if (matZ > 0)
                for (var w = 0; w < matW; w++)
                for (var x = 0; x < matX; x++)
                for (var y = 0; y < matY; y++) {
                    var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[w, x, y, 0], matZ);
                    var dstSpan = newItems.AsSpan((((w + newWOffset) * newSize.X + x + newXOffset) * newSize.Y + y + newYOffset) * newSize.Z + newZOffset, matZ);
                    srcSpan.CopyTo(dstSpan);
                }

            _items    = newItems;
            WSize     = newSize.W;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            ZSize     = newSize.Z;
            WCapacity = WSize;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.W > WSize || placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);

            var startW = Math.Clamp(offset.W, 0, WSize);
            var endW   = Math.Min(WSize, offset.W + matW);
            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matX);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matY);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ   = Math.Min(ZSize, offset.Z + matZ);
            var zCount = endZ - startZ;

            if (zCount > 0)
                for (var w = startW; w < endW; w++) {
                    var srcW = w - offset.W;

                    for (var x = startX; x < endX; x++) {
                        var srcX = x - offset.X;

                        for (var y = startY; y < endY; y++) {
                            var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[srcW, srcX, y - offset.Y, startZ - offset.Z], zCount);
                            var dstSpan = _items.AsSpan(((w * XCapacity + x) * YCapacity            + y) * ZCapacity + startZ, zCount);
                            srcSpan.CopyTo(dstSpan);
                        }
                    }
                }
        }
    }

    /// <summary>Places a 4D list into this list.</summary>
    public void Place(List4D<T> matrix, Point4D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(Math.Max(WSize, placedMax.W), Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point4D(Math.Min(offset.W, 0),        Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W * newSize.X * newSize.Y * newSize.Z];

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            if (WSize > 0 && XSize > 0 && YSize > 0 && ZSize > 0)
                for (var w = 0; w < WSize; w++)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, ZSize)
                        .CopyTo(newItems.AsSpan((((w + oldWOffset) * newSize.X + x + oldXOffset) * newSize.Y + y + oldYOffset) * newSize.Z + oldZOffset, ZSize));

            if (matrix.ZSize > 0)
                for (var w = 0; w < matrix.WSize; w++)
                for (var x = 0; x < matrix.XSize; x++)
                for (var y = 0; y < matrix.YSize; y++)
                    matrix._items.AsSpan(((w * matrix.XCapacity + x) * matrix.YCapacity + y) * matrix.ZCapacity, matrix.ZSize)
                        .CopyTo(newItems.AsSpan((((w + newWOffset) * newSize.X + x + newXOffset) * newSize.Y + y + newYOffset) * newSize.Z + newZOffset, matrix.ZSize));

            _items    = newItems;
            WSize     = newSize.W;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            ZSize     = newSize.Z;
            WCapacity = WSize;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.W > WSize || placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);

            var startW = Math.Clamp(offset.W, 0, WSize);
            var endW   = Math.Min(WSize, offset.W + matrix.WSize);
            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matrix.XSize);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matrix.YSize);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ   = Math.Min(ZSize, offset.Z + matrix.ZSize);
            var zCount = endZ - startZ;

            if (zCount > 0)
                for (var w = startW; w < endW; w++) {
                    var srcW = w - offset.W;

                    for (var x = startX; x < endX; x++) {
                        var srcX = x - offset.X;

                        for (var y = startY; y < endY; y++)
                            matrix._items.AsSpan(((srcW                    * matrix.XCapacity + srcX) * matrix.YCapacity + (y - offset.Y)) * matrix.ZCapacity + (startZ - offset.Z), zCount)
                                .CopyTo(_items.AsSpan(((w * XCapacity + x) * YCapacity        + y)    * ZCapacity        + startZ, zCount));
                    }
                }
        }
    }

    /// <summary>Places a 4D matrix into this list with a predicate.</summary>
    public void Place(T[,,,] matrix, Func<T, T, bool> predicate, Point4D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(predicate);
        var offset = offsetPoint ?? Point4D.Zero;

        var matW = matrix.GetLength(0);
        var matX = matrix.GetLength(1);
        var matY = matrix.GetLength(2);
        var matZ = matrix.GetLength(3);

        var placedMax = offset + new Point4D(matW, matX, matY, matZ);

        var max = new Point4D(Math.Max(WSize, placedMax.W), Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point4D(Math.Min(offset.W, 0),        Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            int newTotal = newSize.W * newSize.X * newSize.Y * newSize.Z;
            var newItems = newTotal == 0 ? Array.Empty<T>() : ArrayPool<T>.Shared.Rent(newTotal);

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            if (WSize > 0 && XSize > 0 && YSize > 0 && ZSize > 0)
                for (var w = 0; w < WSize; w++)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, ZSize)
                        .CopyTo(newItems.AsSpan((((w + oldWOffset) * newSize.X + x + oldXOffset) * newSize.Y + y + oldYOffset) * newSize.Z + oldZOffset, ZSize));

            for (var w = 0; w < matW; w++)
            for (var x = 0; x < matX; x++)
            for (var y = 0; y < matY; y++)
            for (var z = 0; z < matZ; z++) {
                var val    = matrix[w, x, y, z];
                var dstIdx = (((w + newWOffset) * newSize.X + x + newXOffset) * newSize.Y + y + newYOffset) * newSize.Z + z + newZOffset;

                if (predicate(newItems[dstIdx], val))
                    newItems[dstIdx] = val;
            }

            var oldItems = _items;
            _items    = newItems;
            WSize     = newSize.W;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            ZSize     = newSize.Z;
            WCapacity = WSize;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;

            if (oldItems != null && oldItems.Length > 0) {
                ArrayPool<T>.Shared.Return(oldItems, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            }
        }
        else {
            if ((placedMax.W > WSize || placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);

            var startW = Math.Clamp(offset.W, 0, WSize);
            var endW   = Math.Min(WSize, offset.W + matW);
            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matX);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matY);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ   = Math.Min(ZSize, offset.Z + matZ);

            for (var w = startW; w < endW; w++) {
                var srcW = w - offset.W;

                for (var x = startX; x < endX; x++) {
                    var srcX = x - offset.X;

                    for (var y = startY; y < endY; y++) {
                        var srcY     = y - offset.Y;
                        var dstStart = ((w * XCapacity + x) * YCapacity + y) * ZCapacity;

                        for (var z = startZ; z < endZ; z++) {
                            var val = matrix[srcW, srcX, srcY, z - offset.Z];

                            if (predicate(_items[dstStart + z], val))
                                _items[dstStart + z] = val;
                        }
                    }
                }
            }
        }
    }

    /// <summary>Places a 4D list into this list with a predicate.</summary>
    public void Place(List4D<T> matrix, Func<T, T, bool> predicate, Point4D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(predicate);
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(Math.Max(WSize, placedMax.W), Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point4D(Math.Min(offset.W, 0),        Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            int newTotal = newSize.W * newSize.X * newSize.Y * newSize.Z;
            var newItems = newTotal == 0 ? Array.Empty<T>() : ArrayPool<T>.Shared.Rent(newTotal);

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            if (WSize > 0 && XSize > 0 && YSize > 0 && ZSize > 0)
                for (var w = 0; w < WSize; w++)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(((w * XCapacity + x) * YCapacity + y) * ZCapacity, ZSize)
                        .CopyTo(newItems.AsSpan((((w + oldWOffset) * newSize.X + x + oldXOffset) * newSize.Y + y + oldYOffset) * newSize.Z + oldZOffset, ZSize));

            for (var w = 0; w < matrix.WSize; w++)
            for (var x = 0; x < matrix.XSize; x++)
            for (var y = 0; y < matrix.YSize; y++) {
                var srcStart = ((w * matrix.XCapacity + x) * matrix.YCapacity + y) * matrix.ZCapacity;

                for (var z = 0; z < matrix.ZSize; z++) {
                    var val    = matrix._items[srcStart                                       + z];
                    var dstIdx = (((w + newWOffset) * newSize.X + x + newXOffset) * newSize.Y + y + newYOffset) * newSize.Z + z + newZOffset;

                    if (predicate(newItems[dstIdx], val))
                        newItems[dstIdx] = val;
                }
            }

            var oldItems = _items;
            _items    = newItems;
            WSize     = newSize.W;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            ZSize     = newSize.Z;
            WCapacity = WSize;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;

            if (oldItems != null && oldItems.Length > 0) {
                ArrayPool<T>.Shared.Return(oldItems, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            }
        }
        else {
            if ((placedMax.W > WSize || placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);

            var startW = Math.Clamp(offset.W, 0, WSize);
            var endW   = Math.Min(WSize, offset.W + matrix.WSize);
            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matrix.XSize);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matrix.YSize);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ   = Math.Min(ZSize, offset.Z + matrix.ZSize);

            for (var w = startW; w < endW; w++) {
                var srcW = w - offset.W;

                for (var x = startX; x < endX; x++) {
                    var srcX = x - offset.X;

                    for (var y = startY; y < endY; y++) {
                        var srcY     = y - offset.Y;
                        var dstStart = ((w    * XCapacity        + x)    * YCapacity        + y)    * ZCapacity;
                        var srcStart = ((srcW * matrix.XCapacity + srcX) * matrix.YCapacity + srcY) * matrix.ZCapacity;

                        for (var z = startZ; z < endZ; z++) {
                            var val = matrix._items[srcStart + (z - offset.Z)];

                            if (predicate(_items[dstStart + z], val))
                                _items[dstStart + z] = val;
                        }
                    }
                }
            }
        }
    }

    /// <summary>Fills the entire 4D list with the specified value.</summary>
    public void Fill(T item) {
        if (WSize == 0 || XSize == 0 || YSize == 0 || ZSize == 0)
            return;

        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            _items.AsSpan(w * wStride + x * sliceStride + y * ZCapacity, ZSize).Fill(item);
    }

    /// <summary>Fills the specified 4D region of the list with a given value.</summary>
    public void Fill(T item, int wStart, int wCount, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        var wEnd = wStart + wCount;
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (wStart < 0 || xStart < 0 || yStart < 0 || zStart < 0 || wEnd > WSize || xEnd > XSize || yEnd > YSize || zEnd > ZSize || wCount < 0 || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = wStart; w < wEnd; w++)
        for (var x = xStart; x < xEnd; x++)
        for (var y = yStart; y < yEnd; y++)
            _items.AsSpan(w * wStride + x * sliceStride + y * ZCapacity + zStart, zCount).Fill(item);
    }

    /// <summary>Fills the 4D list with the specified value in the given region.</summary>
    public void Fill(T item, Point4D offset, Size4D size) => Fill(item, offset.W, size.W, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>Fills the entire 4D list with the values generated by the specified factory function.</summary>
    public void Fill(Func<T> factory) {
        ArgumentNullException.ThrowIfNull(factory);
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var start = w * wStride + x * sliceStride + y * ZCapacity;

            for (var z = 0; z < ZSize; z++)
                _items[start + z] = factory();
        }
    }

    /// <summary>Fills the 4D list with the values generated by the specified factory function in the given region.</summary>
    public void Fill(Func<T> factory, int wStart, int wCount, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        ArgumentNullException.ThrowIfNull(factory);
        var wEnd = wStart + wCount;
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (wStart < 0 || xStart < 0 || yStart < 0 || zStart < 0 || wEnd > WSize || xEnd > XSize || yEnd > YSize || zEnd > ZSize || wCount < 0 || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = wStart; w < wEnd; w++)
        for (var x = xStart; x < xEnd; x++)
        for (var y = yStart; y < yEnd; y++) {
            var start = w * wStride + x * sliceStride + y * ZCapacity;

            for (var z = zStart; z < zEnd; z++)
                _items[start + z] = factory();
        }
    }

    /// <summary>Fills the 4D list with the values generated by the specified factory function in the given region.</summary>
    public void Fill(Func<T> factory, Point4D offset, Size4D size) => Fill(factory, offset.W, size.W, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>Converts the 4D list into a four-dimensional array.</summary>
    public T[,,,] ToArray() {
        var arr = new T[WSize, XSize, YSize, ZSize];

        if (WSize == 0 || XSize == 0 || YSize == 0 || ZSize == 0)
            return arr;

        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var srcSpan = _items.AsSpan(w * wStride + x * sliceStride + y * ZCapacity, ZSize);
            var dstSpan = MemoryMarshal.CreateSpan(ref arr[w, x, y, 0], ZSize);
            srcSpan.CopyTo(dstSpan);
        }

        return arr;
    }

    /// <summary>Converts the 4D list into a jagged array.</summary>
    [Pure]
    public T[][][][] ToJagged() {
        var arr         = new T[WSize][][][];
        var sliceStride = YCapacity * ZCapacity;
        var wStride     = XCapacity * sliceStride;

        for (var w = 0; w < WSize; w++) {
            arr[w] = new T[XSize][][];

            for (var x = 0; x < XSize; x++) {
                arr[w][x] = new T[YSize][];

                for (var y = 0; y < YSize; y++) {
                    arr[w][x][y] = new T[ZSize];
                    _items.AsSpan(w * wStride + x * sliceStride + y * ZCapacity, ZSize).CopyTo(arr[w][x][y]);
                }
            }
        }

        return arr;
    }

    private class SliceW : IEnumerable3D<T> {
        private readonly List4D<T> _parent;
        private readonly int       _w;

        public SliceW(List4D<T> parent, int w) {
            _parent = parent;
            _w      = w;
        }

        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (var x = 0; x < _parent.XSize; x++)
                yield return GetAtX(x);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
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
        private readonly int       _x;

        public SliceX(List4D<T> parent, int x) {
            _parent = parent;
            _x      = x;
        }

        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
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
        private readonly int       _y;

        public SliceY(List4D<T> parent, int y) {
            _parent = parent;
            _y      = y;
        }

        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.  GetEnumerator() => GetEnumerator();
        public IEnumerable2D<T>     GetAtX(int w)   => new SliceWY(_parent, w, _y);
        public IEnumerable2D<T>     GetAtY(int x)   => new SliceXY(_parent, x, _y);
        public IEnumerable2D<T>     GetAtZ(int z)   => new SliceYZ(_parent, _y, z);
        IEnumerable2D IEnumerable3D.GetAtX(int x)   => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y)   => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z)   => GetAtZ(z);
    }

    private class SliceZ : IEnumerable3D<T> {
        private readonly List4D<T> _parent;
        private readonly int       _z;

        public SliceZ(List4D<T> parent, int z) {
            _parent = parent;
            _z      = z;
        }

        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.  GetEnumerator() => GetEnumerator();
        public IEnumerable2D<T>     GetAtX(int w)   => new SliceWZ(_parent, w, _z);
        public IEnumerable2D<T>     GetAtY(int x)   => new SliceXZ(_parent, x, _z);
        public IEnumerable2D<T>     GetAtZ(int y)   => new SliceYZ(_parent, y, _z);
        IEnumerable2D IEnumerable3D.GetAtX(int x)   => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y)   => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z)   => GetAtZ(z);
    }

    private class SliceWX : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int       _w, _x;

        public SliceWX(List4D<T> parent, int w, int x) {
            _parent = parent;
            _w      = w;
            _x      = x;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var y = 0; y < _parent.YSize; y++)
                yield return GetAtY(y);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtY(int y) {
            for (var z = 0; z < _parent.ZSize; z++)
                yield return _parent.UnsafeGet(_w, _x, y, z);
        }

        public IEnumerable<T>     GetAtX(int x) => GetAtY(x);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceWY : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int       _w, _y;

        public SliceWY(List4D<T> parent, int w, int y) {
            _parent = parent;
            _w      = w;
            _y      = y;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var x = 0; x < _parent.XSize; x++)
                yield return GetAtX(x);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int x) {
            for (var z = 0; z < _parent.ZSize; z++)
                yield return _parent.UnsafeGet(_w, x, _y, z);
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceWZ : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int       _w, _z;

        public SliceWZ(List4D<T> parent, int w, int z) {
            _parent = parent;
            _w      = w;
            _z      = z;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var x = 0; x < _parent.XSize; x++)
                yield return GetAtX(x);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int x) {
            for (var y = 0; y < _parent.YSize; y++)
                yield return _parent.UnsafeGet(_w, x, y, _z);
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceXY : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int       _x, _y;

        public SliceXY(List4D<T> parent, int x, int y) {
            _parent = parent;
            _x      = x;
            _y      = y;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int w) {
            for (var z = 0; z < _parent.ZSize; z++)
                yield return _parent.UnsafeGet(w, _x, _y, z);
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceXZ : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int       _x, _z;

        public SliceXZ(List4D<T> parent, int x, int z) {
            _parent = parent;
            _x      = x;
            _z      = z;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int w) {
            for (var y = 0; y < _parent.YSize; y++)
                yield return _parent.UnsafeGet(w, _x, y, _z);
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceYZ : IEnumerable2D<T> {
        private readonly List4D<T> _parent;
        private readonly int       _y, _z;

        public SliceYZ(List4D<T> parent, int y, int z) {
            _parent = parent;
            _y      = y;
            _z      = z;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int w) {
            for (var x = 0; x < _parent.XSize; x++)
                yield return _parent.UnsafeGet(w, x, _y, _z);
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>Gets a 1D slice of elements along the W axis.</summary>
    public T[] this[Range w, int x, int y, int z] {
        get {
            var (offset, length) = w.GetOffsetAndLength(WSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = UnsafeGet(offset + i, x, y, z);

            return result;
        }
    }

    /// <summary>Gets a 1D slice of elements along the X axis.</summary>
    public T[] this[int w, Range x, int y, int z] {
        get {
            var (offset, length) = x.GetOffsetAndLength(XSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = UnsafeGet(w, offset + i, y, z);

            return result;
        }
    }

    /// <summary>Gets a 1D slice of elements along the Y axis.</summary>
    public T[] this[int w, int x, Range y, int z] {
        get {
            var (offset, length) = y.GetOffsetAndLength(YSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = UnsafeGet(w, x, offset + i, z);

            return result;
        }
    }

    /// <summary>Gets a 1D slice of elements along the Z axis.</summary>
    public T[] this[int w, int x, int y, Range z] {
        get {
            var (offset, length) = z.GetOffsetAndLength(ZSize);
            var result = new T[length];

            if (length > 0)
                _items.AsSpan(GetOffset(w, x, y, offset), length).CopyTo(result);

            return result;
        }
    }
    #endif

    #region Spans

    /// <summary>
    ///     Gets a Span over the Z elements at the specified W, X, and Y coordinates.
    /// </summary>
    /// <param name="w">The zero-based W coordinate.</param>
    /// <param name="x">The zero-based X coordinate.</param>
    /// <param name="y">The zero-based Y coordinate.</param>
    /// <returns>A Span over the elements along the Z axis.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpanAtWXY(int w, int x, int y) {
        if (w < 0 || w >= WSize || x < 0 || x >= XSize || y < 0 || y >= YSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        return _items.AsSpan(GetOffset(w, x, y, 0), ZSize);
    }

    /// <summary>
    ///     Copies the elements at the specified W, X, and Y coordinates (along Z axis) into the destination span.
    /// </summary>
    /// <param name="w">The zero-based W coordinate.</param>
    /// <param name="x">The zero-based X coordinate.</param>
    /// <param name="y">The zero-based Y coordinate.</param>
    /// <param name="destination">The destination span.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination span is not large enough.</exception>
    public void CopyAtWXYTo(int w, int x, int y, Span<T> destination) {
        if (w < 0 || w >= WSize || x < 0 || x >= XSize || y < 0 || y >= YSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        if (destination.Length < ZSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        _items.AsSpan(GetOffset(w, x, y, 0), ZSize).CopyTo(destination);
    }

    /// <summary>
    ///     Copies the elements at the specified W, X, and Z coordinates (along Y axis) into the destination span.
    /// </summary>
    /// <param name="w">The zero-based W coordinate.</param>
    /// <param name="x">The zero-based X coordinate.</param>
    /// <param name="z">The zero-based Z coordinate.</param>
    /// <param name="destination">The destination span.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination span is not large enough.</exception>
    public void CopyAtWXZTo(int w, int x, int z, Span<T> destination) {
        if (w < 0 || w >= WSize || x < 0 || x >= XSize || z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        if (destination.Length < YSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        for (var y = 0; y < YSize; y++)
            destination[y] = _items[GetOffset(w, x, y, z)];
    }

    /// <summary>
    ///     Copies the elements at the specified W, Y, and Z coordinates (along X axis) into the destination span.
    /// </summary>
    /// <param name="w">The zero-based W coordinate.</param>
    /// <param name="y">The zero-based Y coordinate.</param>
    /// <param name="z">The zero-based Z coordinate.</param>
    /// <param name="destination">The destination span.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination span is not large enough.</exception>
    public void CopyAtWYZTo(int w, int y, int z, Span<T> destination) {
        if (w < 0 || w >= WSize || y < 0 || y >= YSize || z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        if (destination.Length < XSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        for (var x = 0; x < XSize; x++)
            destination[x] = _items[GetOffset(w, x, y, z)];
    }

    /// <summary>
    ///     Copies the elements at the specified X, Y, and Z coordinates (along W axis) into the destination span.
    /// </summary>
    /// <param name="x">The zero-based X coordinate.</param>
    /// <param name="y">The zero-based Y coordinate.</param>
    /// <param name="z">The zero-based Z coordinate.</param>
    /// <param name="destination">The destination span.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination span is not large enough.</exception>
    public void CopyAtXYZTo(int x, int y, int z, Span<T> destination) {
        if (x < 0 || x >= XSize || y < 0 || y >= YSize || z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        if (destination.Length < WSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        for (var w = 0; w < WSize; w++)
            destination[w] = _items[GetOffset(w, x, y, z)];
    }

    /// <summary>
    ///     Copies a rectangular 4D sub-region (slice) of the list into a contiguous destination span.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    /// <param name="wStart">The starting W coordinate.</param>
    /// <param name="wCount">The number of elements to copy along W-axis.</param>
    /// <param name="xStart">The starting X coordinate.</param>
    /// <param name="xCount">The number of elements to copy along X-axis.</param>
    /// <param name="yStart">The starting Y coordinate.</param>
    /// <param name="yCount">The number of elements to copy along Y-axis.</param>
    /// <param name="zStart">The starting Z coordinate.</param>
    /// <param name="zCount">The number of elements to copy along Z-axis.</param>
    public void CopySliceTo(Span<T> destination, int wStart, int wCount, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        var wEnd = wStart + wCount;
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (wStart < 0 || xStart < 0 || yStart < 0 || zStart < 0 || wEnd > WSize || xEnd > XSize || yEnd > YSize || zEnd > ZSize || wCount < 0 || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException("Specified slice is out of bounds.");

        var requiredLength = wCount * xCount * yCount * zCount;

        if (destination.Length < requiredLength)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        if (requiredLength == 0)
            return;

        var destIdx = 0;

        for (var w = 0; w < wCount; w++)
        for (var x = 0; x < xCount; x++)
        for (var y = 0; y < yCount; y++) {
            var srcSpan = _items.AsSpan(GetOffset(wStart + w, xStart + x, yStart + y, zStart), zCount);
            var dstSpan = destination.Slice(destIdx, zCount);
            srcSpan.CopyTo(dstSpan);
            destIdx += zCount;
        }
    }

    /// <summary>
    ///     Copies a rectangular 4D sub-region (slice) of the list into a contiguous destination span.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    /// <param name="offset">The offset of the region.</param>
    /// <param name="size">The size of the region.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopySliceTo(Span<T> destination, Point4D offset, Size4D size) => CopySliceTo(destination, offset.W, size.W, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    #endregion

}