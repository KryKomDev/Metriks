// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
///     Represents a strongly typed, three-dimensional list of elements that can be accessed by X, Y, and Z indices.
///     Provides methods to search, sort, and manipulate 3D lists.
/// </summary>
/// <typeparam name="T">The type of elements in the three-dimensional list.</typeparam>
public class List3D<T> : IList3D<T>, ICollection3D, IReadOnlyList3D<T> {
    private const int   INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR    = 2f;

    private T[] _items;
    private int _xSize;
    private int _ySize;
    private int _zSize;
    private int _xCapacity;
    private int _yCapacity;
    private int _zCapacity;

    /// <summary>
    ///     Initializes a new instance of the <see cref="List3D{T}" /> class with the specified initial capacity for each
    ///     dimension.
    /// </summary>
    /// <param name="xCapacity">The initial capacity along the X-axis.</param>
    /// <param name="yCapacity">The initial capacity along the Y-axis.</param>
    /// <param name="zCapacity">The initial capacity along the Z-axis.</param>
    public List3D(
        int xCapacity = INITIAL_CAPACITY,
        int yCapacity = INITIAL_CAPACITY,
        int zCapacity = INITIAL_CAPACITY
    ) {
        _items    = new T[xCapacity * yCapacity * zCapacity];
        XSize     = 0;
        YSize     = 0;
        ZSize     = 0;
        XCapacity = xCapacity;
        YCapacity = yCapacity;
        ZCapacity = zCapacity;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="List3D{T}" /> class populated with elements from the specified 3D
    ///     array.
    /// </summary>
    /// <param name="collection">The 3D array of elements to copy from.</param>
    public List3D(T[,,] collection) {
        ArgumentNullException.ThrowIfNull(collection);

        XSize     = collection.GetLength(0);
        YSize     = collection.GetLength(1);
        ZSize     = collection.GetLength(2);
        XCapacity = XSize;
        YCapacity = YSize;
        ZCapacity = ZSize;

        var totalElements = XSize * YSize * ZSize;

        if (totalElements == 0) {
            _items = Array.Empty<T>();

            return;
        }

        _items = new T[totalElements];
        var sourceSpan = MemoryMarshal.CreateReadOnlySpan(ref collection[0, 0, 0], totalElements);
        sourceSpan.CopyTo(_items);
    }

    /// <summary>
    ///     Gets the underlying flat array used to store the elements of the <see cref="List3D{T}" /> instance.
    ///     PROVIDED FOR INTERNAL USE ONLY. DO NOT USE. <b>!!!DO NOT MODIFY THE ARRAY IN ANY WAY!!!</b>
    /// </summary>
    internal T[] Items {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _items;
    }

    /// <summary>
    ///     Gets the size (number of elements) along the X-axis.
    /// </summary>
    public int XSize {
        get => _xSize;
        private set => _xSize = value;
    }

    /// <summary>
    ///     Gets the size (number of elements) along the Y-axis.
    /// </summary>
    public int YSize {
        get => _ySize;
        private set => _ySize = value;
    }

    /// <summary>
    ///     Gets the size (number of elements) along the Z-axis.
    /// </summary>
    public int ZSize {
        get => _zSize;
        private set => _zSize = value;
    }

    /// <summary>
    ///     Gets a <see cref="Size3D" /> representing the current size of the list in all three dimensions.
    /// </summary>
    public Size3D Size => new(XSize, YSize, ZSize);

    /// <summary>
    ///     Gets the capacity along the X-axis.
    /// </summary>
    public int XCapacity {
        get => _xCapacity;
        private set => _xCapacity = value;
    }

    /// <summary>
    ///     Gets the capacity along the Y-axis.
    /// </summary>
    public int YCapacity {
        get => _yCapacity;
        private set => _yCapacity = value;
    }

    /// <summary>
    ///     Gets the capacity along the Z-axis.
    /// </summary>
    public int ZCapacity {
        get => _zCapacity;
        private set => _zCapacity = value;
    }

    /// <summary>
    ///     Copies the elements of the 3D list to a standard multidimensional array, starting at the specified destination
    ///     index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    public void CopyTo(Array array, Point3D index) {
        ArgumentNullException.ThrowIfNull(array);

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
        for (var z = 0; z < ZSize; z++)
            array.SetValue(UnsafeGet(x, y, z), x + index.X, y + index.Y, z + index.Z);
    }

    /// <summary>
    ///     Gets the total number of elements contained in the <see cref="List3D{T}" />.
    /// </summary>
    public int Count => XSize * YSize * ZSize;

    /// <summary>
    ///     Gets the size (number of elements) along the X-axis.
    /// </summary>
    public int XCount => XSize;

    /// <summary>
    ///     Gets the size (number of elements) along the Y-axis.
    /// </summary>
    public int YCount => YSize;

    /// <summary>
    ///     Gets the size (number of elements) along the Z-axis.
    /// </summary>
    public int ZCount => ZSize;

    /// <summary>
    ///     Gets a value indicating whether the <see cref="List3D{T}" /> is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    ///     Gets or sets the element at the specified 3D index.
    /// </summary>
    public T this[int x, int y, int z] {
        get {
            ValidateIndexBounds(x, y, z);

            return _items[GetOffset(x, y, z)];
        }
        set {
            ValidateIndexBounds(x, y, z);
            _items[GetOffset(x, y, z)] = value;
        }
    }

    /// <summary>
    ///     Gets or sets the element at the specified <see cref="Point3D" />.
    /// </summary>
    public T this[Point3D point] {
        get => this[point.X, point.Y, point.Z];
        set => this[point.X, point.Y, point.Z] = value;
    }

    [StackTraceHidden]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateIndexBounds(int x, int y, int z) {
        if (x < 0 || x >= XSize || y < 0 || y >= YSize || z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException($"Index [{x}, {y}, {z}] is out of bounds.");
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetOffset(int x, int y, int z) => x * YCapacity * ZCapacity + y * ZCapacity + z;

    /// <summary>
    ///     Inserts a new element at the specified index along X-axis.
    /// </summary>
    public void InsertAtX(int x) {
        if (x < 0 || x > XSize)
            throw new IndexOutOfRangeException();

        EnsureXCapacity(XSize + 1);

        if (x < XSize) {
            var srcStart = x           * YCapacity * ZCapacity;
            var dstStart = (x     + 1) * YCapacity * ZCapacity;
            var length   = (XSize - x) * YCapacity * ZCapacity;
            Array.Copy(_items, srcStart, _items, dstStart, length);
        }

        _items.AsSpan(x * YCapacity * ZCapacity, YCapacity * ZCapacity).Clear();
        XSize++;
    }

    /// <summary>
    ///     Inserts a new element at the specified index along Y-axis.
    /// </summary>
    public void InsertAtY(int y) {
        if (y < 0 || y > YSize)
            throw new IndexOutOfRangeException();

        EnsureYCapacity(YSize   + 1);
        var shiftLength = YSize - y;

        if (shiftLength > 0)
            for (var x = XSize - 1; x >= 0; x--) {
                var srcStart = x * YCapacity * ZCapacity + y       * ZCapacity;
                var dstStart = x * YCapacity * ZCapacity + (y + 1) * ZCapacity;
                Array.Copy(_items, srcStart, _items, dstStart, shiftLength * ZCapacity);
                _items.AsSpan(srcStart, ZCapacity).Clear();
            }
        else
            for (var x = 0; x < XSize; x++)
                _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZCapacity).Clear();

        YSize++;
    }

    /// <summary>
    ///     Inserts a new element at the specified index along Z-axis.
    /// </summary>
    public void InsertAtZ(int z) {
        if (z < 0 || z > ZSize)
            throw new IndexOutOfRangeException();

        EnsureZCapacity(ZSize   + 1);
        var shiftLength = ZSize - z;

        if (shiftLength > 0)
            for (var x = XSize - 1; x >= 0; x--)
            for (var y = YSize - 1; y >= 0; y--) {
                var srcStart = x * YCapacity * ZCapacity      + y * ZCapacity + z;
                Array.Copy(_items, srcStart, _items, srcStart + 1, shiftLength);
                _items[srcStart] = default!;
            }
        else
            for (var x = 0; x < XSize; x++)
            for (var y = 0; y < YSize; y++)
                _items[x * YCapacity * ZCapacity + y * ZCapacity + z] = default!;

        ZSize++;
    }

    /// <summary>Adds a new element at the end of the X-axis.</summary>
    public void AddX() => InsertAtX(XSize);

    /// <summary>Adds a new element at the end of the Y-axis.</summary>
    public void AddY() => InsertAtY(YSize);

    /// <summary>Adds a new element at the end of the Z-axis.</summary>
    public void AddZ() => InsertAtZ(ZSize);

    /// <summary>Removes the element at the specified index along X-axis.</summary>
    public void RemoveAtX(int x) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException();

        XSize--;

        if (x < XSize) {
            var srcStart = (x + 1)     * YCapacity * ZCapacity;
            var dstStart = x           * YCapacity * ZCapacity;
            var length   = (XSize - x) * YCapacity * ZCapacity;
            Array.Copy(_items, srcStart, _items, dstStart, length);
        }

        _items.AsSpan(XSize * YCapacity * ZCapacity, YCapacity * ZCapacity).Clear();
    }

    /// <summary>Removes the element at the specified index along Y-axis.</summary>
    public void RemoveAtY(int y) {
        if (y < 0 || y >= YSize)
            throw new IndexOutOfRangeException();

        YSize--;
        var shiftLength = YSize - y;

        for (var x = 0; x < XSize; x++) {
            var colStart = x * YCapacity * ZCapacity;

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

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var rowStart = x * YCapacity * ZCapacity + y * ZCapacity;

            if (shiftLength > 0)
                Array.Copy(_items, rowStart + z + 1, _items, rowStart + z, shiftLength);

            _items[rowStart + ZSize] = default!;
        }
    }

    /// <summary>Shrinks the capacity of the X-axis to the current size.</summary>
    public void ShrinkX() => ResizeCapacity(XSize, YCapacity, ZCapacity);

    /// <summary>Shrinks the capacity of the Y-axis to the current size.</summary>
    public void ShrinkY() => ResizeCapacity(XCapacity, YSize, ZCapacity);

    /// <summary>Shrinks the capacity of the Z-axis to the current size.</summary>
    public void ShrinkZ() => ResizeCapacity(XCapacity, YCapacity, ZSize);

    /// <summary>Checks whether the list contains the specified value.</summary>
    public bool Contains(T value) {
        if (XSize == 0 || YSize == 0 || ZSize == 0)
            return false;

        if (YSize                                                                 == YCapacity && ZSize == ZCapacity)
            return Array.IndexOf(_items, value, 0, XSize * YCapacity * ZCapacity) >= 0;

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            if (Array.IndexOf(_items, value, x * YCapacity * ZCapacity + y * ZCapacity, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>Checks whether the list contains the specified value along X-axis.</summary>
    public bool ContainsAtX(int x, T value) {
        if (x < 0 || x >= XSize)
            return false;

        for (var y = 0; y < YSize; y++)
            if (Array.IndexOf(_items, value, x * YCapacity * ZCapacity + y * ZCapacity, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>Checks whether the list contains the specified value along Y-axis.</summary>
    public bool ContainsAtY(int y, T value) {
        if (y < 0 || y >= YSize)
            return false;

        for (var x = 0; x < XSize; x++)
            if (Array.IndexOf(_items, value, x * YCapacity * ZCapacity + y * ZCapacity, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>Checks whether the list contains the specified value along Z-axis.</summary>
    public bool ContainsAtZ(int z, T value) {
        if (z < 0 || z >= ZSize)
            return false;

        var comparer = EqualityComparer<T>.Default;

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            if (comparer.Equals(_items[x * YCapacity * ZCapacity + y * ZCapacity + z], value))
                return true;

        return false;
    }

    /// <summary>
    ///     Resets the list to size 0 and ensures it has at least the specified capacity.
    ///     Clears all existing elements to default.
    /// </summary>
    public void Reset(int xCapacity, int yCapacity, int zCapacity) {
        _xSize = 0;
        _ySize = 0;
        _zSize = 0;
        int requiredLength = xCapacity * yCapacity * zCapacity;
        if (_items == null || _items.Length < requiredLength) {
            _items = new T[requiredLength];
        }
        else {
            Array.Clear(_items, 0, _items.Length);
        }
        _xCapacity = xCapacity;
        _yCapacity = yCapacity;
        _zCapacity = zCapacity;
    }

    public void Clear() {
        _items    = new T[INITIAL_CAPACITY * INITIAL_CAPACITY * INITIAL_CAPACITY];
        XSize     = 0;
        YSize     = 0;
        ZSize     = 0;
        XCapacity = INITIAL_CAPACITY;
        YCapacity = INITIAL_CAPACITY;
        ZCapacity = INITIAL_CAPACITY;
    }

    /// <summary>Copies elements to the specified multidimensional array.</summary>
    public void CopyTo(T[,,] array, Point3D index) {
        ArgumentNullException.ThrowIfNull(array);

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var srcSpan = _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZSize);
            var dstSpan = MemoryMarshal.CreateSpan(ref array[x + index.X, y + index.Y, index.Z], ZSize);
            srcSpan.CopyTo(dstSpan);
        }
    }

    /// <summary>Returns an enumerator that iterates through the 2D slices of the 3D list.</summary>
    public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
        for (var x = 0; x < XSize; x++)
            yield return GetAtX(x);
    }

    IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();

    /// <summary>Gets a 2D slice representation at the specified X index.</summary>
    public IEnumerable2D<T> GetAtX(int x) => new SliceX(this, x);

    /// <summary>Gets a 2D slice representation at the specified Y index.</summary>
    public IEnumerable2D<T> GetAtY(int y) => new SliceY(this, y);

    /// <summary>Gets a 2D slice representation at the specified Z index.</summary>
    public IEnumerable2D<T> GetAtZ(int z) => new SliceZ(this, z);

    IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
    IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
    IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);

    /// <summary>Sets the element at the specified 3D index without bounds checking.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void UnsafeSet(int x, int y, int z, T value) => _items[GetOffset(x, y, z)] = value;

    /// <summary>Gets the element at the specified 3D index without bounds checking.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T UnsafeGet(int x, int y, int z) => _items[GetOffset(x, y, z)];

    private void EnsureXCapacity(int minXCapacity) {
        if (minXCapacity > XCapacity) {
            var newXCapacity = Math.Max(minXCapacity, (int)(XCapacity * GROWTH_FACTOR));

            if (newXCapacity < INITIAL_CAPACITY)
                newXCapacity = INITIAL_CAPACITY;

            ResizeCapacity(newXCapacity, YCapacity, ZCapacity);
        }
    }

    private void EnsureYCapacity(int minYCapacity) {
        if (minYCapacity > YCapacity) {
            var newYCapacity = Math.Max(minYCapacity, (int)(YCapacity * GROWTH_FACTOR));

            if (newYCapacity < INITIAL_CAPACITY)
                newYCapacity = INITIAL_CAPACITY;

            ResizeCapacity(XCapacity, newYCapacity, ZCapacity);
        }
    }

    private void EnsureZCapacity(int minZCapacity) {
        if (minZCapacity > ZCapacity) {
            var newZCapacity = Math.Max(minZCapacity, (int)(ZCapacity * GROWTH_FACTOR));

            if (newZCapacity < INITIAL_CAPACITY)
                newZCapacity = INITIAL_CAPACITY;

            ResizeCapacity(XCapacity, YCapacity, newZCapacity);
        }
    }

    private void ResizeCapacity(int newXCapacity, int newYCapacity, int newZCapacity) {
        var newItems = new T[newXCapacity * newYCapacity * newZCapacity];

        if (XSize > 0 && YSize > 0 && ZSize > 0) {
            if (YCapacity == newYCapacity && ZCapacity == newZCapacity) {
                _items.AsSpan(0, XSize * YCapacity * ZCapacity).CopyTo(newItems);
            }
            else {
                var copyY = Math.Min(YCapacity, newYCapacity);
                var copyZ = Math.Min(ZCapacity, newZCapacity);

                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, copyZ)
                        .CopyTo(newItems.AsSpan(x * newYCapacity * newZCapacity + y * newZCapacity, copyZ));
            }
        }

        _items    = newItems;
        XCapacity = newXCapacity;
        YCapacity = newYCapacity;
        ZCapacity = newZCapacity;
    }

    /// <summary>Expands the dimensions of the 3D list to the specified size.</summary>
    public void Expand(int xSize, int ySize, int zSize, T? defaultValue = default!) {
        if (xSize < XSize || ySize < YSize || zSize < ZSize)
            throw new ArgumentOutOfRangeException();

        if (xSize == XSize && ySize == YSize && zSize == ZSize)
            return;

        var targetXCapacity = XCapacity;
        var targetYCapacity = YCapacity;
        var targetZCapacity = ZCapacity;

        if (xSize > XCapacity)
            targetXCapacity = xSize + 1;

        if (ySize > YCapacity)
            targetYCapacity = ySize + 1;

        if (zSize > ZCapacity)
            targetZCapacity = zSize + 1;

        if (targetXCapacity != XCapacity || targetYCapacity != YCapacity || targetZCapacity != ZCapacity)
            ResizeCapacity(targetXCapacity, targetYCapacity, targetZCapacity);

        if (zSize > ZSize)
            for (var x = 0; x < XSize; x++)
            for (var y = 0; y < YSize; y++)
                _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity + ZSize, zSize - ZSize).Fill(defaultValue!);

        if (ySize > YSize)
            for (var x = 0; x < XSize; x++)
            for (var y = YSize; y < ySize; y++)
                _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, zSize).Fill(defaultValue!);

        for (var x = XSize; x < xSize; x++)
        for (var y = 0; y < ySize; y++)
            _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, zSize).Fill(defaultValue!);

        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
    }

    /// <summary>Expands the dimensions of the 3D list with a factory generator.</summary>
    public void Expand(int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        ArgumentNullException.ThrowIfNull(defaultValueFactory);

        if (xSize < XSize || ySize < YSize || zSize < ZSize)
            throw new ArgumentOutOfRangeException();

        if (xSize == XSize && ySize == YSize && zSize == ZSize)
            return;

        var targetXCapacity = XCapacity;
        var targetYCapacity = YCapacity;
        var targetZCapacity = ZCapacity;

        if (xSize > XCapacity)
            targetXCapacity = xSize + 1;

        if (ySize > YCapacity)
            targetYCapacity = ySize + 1;

        if (zSize > ZCapacity)
            targetZCapacity = zSize + 1;

        if (targetXCapacity != XCapacity || targetYCapacity != YCapacity || targetZCapacity != ZCapacity)
            ResizeCapacity(targetXCapacity, targetYCapacity, targetZCapacity);

        if (zSize > ZSize)
            for (var x = 0; x < XSize; x++)
            for (var y = 0; y < YSize; y++) {
                var start = x * YCapacity * ZCapacity + y * ZCapacity;

                for (var z = ZSize; z < zSize; z++)
                    _items[start + z] = defaultValueFactory();
            }

        if (ySize > YSize)
            for (var x = 0; x < XSize; x++)
            for (var y = YSize; y < ySize; y++) {
                var start = x * YCapacity * ZCapacity + y * ZCapacity;

                for (var z = 0; z < zSize; z++)
                    _items[start + z] = defaultValueFactory();
            }

        for (var x = XSize; x < xSize; x++)
        for (var y = 0; y < ySize; y++) {
            var start = x * YCapacity * ZCapacity + y * ZCapacity;

            for (var z = 0; z < zSize; z++)
                _items[start + z] = defaultValueFactory();
        }

        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
    }

    /// <summary>Resizes the 3D list dimensions.</summary>
    public void Resize(int xSize, int ySize, int zSize, T? defaultValue = default) {
        if (xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[xSize * ySize * zSize];

        if (defaultValue is not null)
            newItems.AsSpan().Fill(defaultValue);

        var copyX = Math.Min(XSize, xSize);
        var copyY = Math.Min(YSize, ySize);
        var copyZ = Math.Min(ZSize, zSize);

        if (copyX > 0 && copyY > 0 && copyZ > 0)
            for (var x = 0; x < copyX; x++)
            for (var y = 0; y < copyY; y++)
                _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, copyZ)
                    .CopyTo(newItems.AsSpan(x * ySize * zSize + y * zSize, copyZ));

        _items    = newItems;
        XSize     = xSize;
        YSize     = ySize;
        ZSize     = zSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    /// <summary>Resizes the 3D list dimensions using a factory method.</summary>
    public void Resize(int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        ArgumentNullException.ThrowIfNull(defaultValueFactory);

        if (xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[xSize * ySize * zSize];

        for (var i = 0; i < newItems.Length; i++)
            newItems[i] = defaultValueFactory();

        var copyX = Math.Min(XSize, xSize);
        var copyY = Math.Min(YSize, ySize);
        var copyZ = Math.Min(ZSize, zSize);

        if (copyX > 0 && copyY > 0 && copyZ > 0)
            for (var x = 0; x < copyX; x++)
            for (var y = 0; y < copyY; y++)
                _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, copyZ)
                    .CopyTo(newItems.AsSpan(x * ySize * zSize + y * zSize, copyZ));

        _items    = newItems;
        XSize     = xSize;
        YSize     = ySize;
        ZSize     = zSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    /// <summary>Shrinks the 3D list dimensions.</summary>
    public void Shrink(int xSize, int ySize, int zSize) {
        if (xSize > XSize || ySize > YSize || zSize > ZSize)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[xSize * ySize * zSize];

        if (xSize > 0 && ySize > 0 && zSize > 0)
            for (var x = 0; x < xSize; x++)
            for (var y = 0; y < ySize; y++)
                _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, zSize)
                    .CopyTo(newItems.AsSpan(x * ySize * zSize + y * zSize, zSize));

        _items    = newItems;
        XSize     = xSize;
        YSize     = ySize;
        ZSize     = zSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    /// <summary>Places a 3D matrix into this list.</summary>
    public void Place(T[,,] matrix, Point3D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);
        var offset = offsetPoint ?? Point3D.Zero;

        var matX = matrix.GetLength(0);
        var matY = matrix.GetLength(1);
        var matZ = matrix.GetLength(2);

        var placedMax = offset + new Point3D(matX, matY, matZ);

        var max = new Point3D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X * newSize.Y * newSize.Z];

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            if (XSize > 0 && YSize > 0 && ZSize > 0)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZSize)
                        .CopyTo(newItems.AsSpan((x + oldXOffset) * newSize.Y * newSize.Z + (y + oldYOffset) * newSize.Z + oldZOffset, ZSize));

            if (matZ > 0)
                for (var x = 0; x < matX; x++)
                for (var y = 0; y < matY; y++) {
                    var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[x, y, 0], matZ);
                    var dstSpan = newItems.AsSpan((x + newXOffset) * newSize.Y * newSize.Z + (y + newYOffset) * newSize.Z + newZOffset, matZ);
                    srcSpan.CopyTo(dstSpan);
                }

            _items    = newItems;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            ZSize     = newSize.Z;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.X, newSize.Y, newSize.Z);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matX);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matY);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ   = Math.Min(ZSize, offset.Z + matZ);
            var zCount = endZ - startZ;

            if (zCount > 0)
                for (var x = startX; x < endX; x++) {
                    var srcX = x - offset.X;

                    for (var y = startY; y < endY; y++) {
                        var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[srcX, y - offset.Y, startZ - offset.Z], zCount);
                        var dstSpan = _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity + startZ, zCount);
                        srcSpan.CopyTo(dstSpan);
                    }
                }
        }
    }

    /// <summary>Places a 3D list into this list.</summary>
    public void Place(List3D<T> matrix, Point3D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X * newSize.Y * newSize.Z];

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            if (XSize > 0 && YSize > 0 && ZSize > 0)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZSize)
                        .CopyTo(newItems.AsSpan((x + oldXOffset) * newSize.Y * newSize.Z + (y + oldYOffset) * newSize.Z + oldZOffset, ZSize));

            if (matrix.ZSize > 0)
                for (var x = 0; x < matrix.XSize; x++)
                for (var y = 0; y < matrix.YSize; y++)
                    matrix._items.AsSpan(x * matrix.YCapacity * matrix.ZCapacity + y * matrix.ZCapacity, matrix.ZSize)
                        .CopyTo(newItems.AsSpan((x + newXOffset) * newSize.Y * newSize.Z + (y + newYOffset) * newSize.Z + newZOffset, matrix.ZSize));

            _items    = newItems;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            ZSize     = newSize.Z;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.X, newSize.Y, newSize.Z);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matrix.XSize);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matrix.YSize);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ   = Math.Min(ZSize, offset.Z + matrix.ZSize);
            var zCount = endZ - startZ;

            if (zCount > 0)
                for (var x = startX; x < endX; x++) {
                    var srcX = x - offset.X;

                    for (var y = startY; y < endY; y++)
                        matrix._items.AsSpan(srcX * matrix.YCapacity * matrix.ZCapacity + (y - offset.Y) * matrix.ZCapacity + (startZ - offset.Z), zCount)
                            .CopyTo(_items.AsSpan(x * YCapacity * ZCapacity                  + y * ZCapacity + startZ, zCount));
                }
        }
    }

    /// <summary>Places a 3D matrix into this list with a predicate.</summary>
    public void Place(T[,,] matrix, Func<T, T, bool> predicate, Point3D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(predicate);
        var offset = offsetPoint ?? Point3D.Zero;

        var matX = matrix.GetLength(0);
        var matY = matrix.GetLength(1);
        var matZ = matrix.GetLength(2);

        var placedMax = offset + new Point3D(matX, matY, matZ);

        var max = new Point3D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X * newSize.Y * newSize.Z];

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            if (XSize > 0 && YSize > 0 && ZSize > 0)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZSize)
                        .CopyTo(newItems.AsSpan((x + oldXOffset) * newSize.Y * newSize.Z + (y + oldYOffset) * newSize.Z + oldZOffset, ZSize));

            for (var x = 0; x < matX; x++)
            for (var y = 0; y < matY; y++)
            for (var z = 0; z < matZ; z++) {
                var val    = matrix[x, y, z];
                var dstIdx = (x + newXOffset) * newSize.Y * newSize.Z + (y + newYOffset) * newSize.Z + z + newZOffset;

                if (predicate(newItems[dstIdx], val))
                    newItems[dstIdx] = val;
            }

            _items    = newItems;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            ZSize     = newSize.Z;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.X, newSize.Y, newSize.Z);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matX);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matY);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ   = Math.Min(ZSize, offset.Z + matZ);

            for (var x = startX; x < endX; x++) {
                var srcX = x - offset.X;

                for (var y = startY; y < endY; y++) {
                    var srcY     = y                         - offset.Y;
                    var dstStart = x * YCapacity * ZCapacity + y * ZCapacity;

                    for (var z = startZ; z < endZ; z++) {
                        var val = matrix[srcX, srcY, z - offset.Z];

                        if (predicate(_items[dstStart + z], val))
                            _items[dstStart + z] = val;
                    }
                }
            }
        }
    }

    /// <summary>Places a 3D list into this list with a predicate.</summary>
    public void Place(List3D<T> matrix, Func<T, T, bool> predicate, Point3D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(predicate);
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X * newSize.Y * newSize.Z];

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            if (XSize > 0 && YSize > 0 && ZSize > 0)
                for (var x = 0; x < XSize; x++)
                for (var y = 0; y < YSize; y++)
                    _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZSize)
                        .CopyTo(newItems.AsSpan((x + oldXOffset) * newSize.Y * newSize.Z + (y + oldYOffset) * newSize.Z + oldZOffset, ZSize));

            for (var x = 0; x < matrix.XSize; x++)
            for (var y = 0; y < matrix.YSize; y++) {
                var srcStart = x * matrix.YCapacity * matrix.ZCapacity + y * matrix.ZCapacity;

                for (var z = 0; z < matrix.ZSize; z++) {
                    var val    = matrix._items[srcStart + z];
                    var dstIdx = (x                     + newXOffset) * newSize.Y * newSize.Z + (y + newYOffset) * newSize.Z + z + newZOffset;

                    if (predicate(newItems[dstIdx], val))
                        newItems[dstIdx] = val;
                }
            }

            _items    = newItems;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            ZSize     = newSize.Z;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.X, newSize.Y, newSize.Z);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matrix.XSize);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matrix.YSize);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ   = Math.Min(ZSize, offset.Z + matrix.ZSize);

            for (var x = startX; x < endX; x++) {
                var srcX = x - offset.X;

                for (var y = startY; y < endY; y++) {
                    var srcY     = y                                          - offset.Y;
                    var dstStart = x    * YCapacity        * ZCapacity        + y    * ZCapacity;
                    var srcStart = srcX * matrix.YCapacity * matrix.ZCapacity + srcY * matrix.ZCapacity;

                    for (var z = startZ; z < endZ; z++) {
                        var val = matrix._items[srcStart + (z - offset.Z)];

                        if (predicate(_items[dstStart + z], val))
                            _items[dstStart + z] = val;
                    }
                }
            }
        }
    }

    /// <summary>Fills the entire list with a value.</summary>
    public void Fill(T item) {
        if (XSize == 0 || YSize == 0 || ZSize == 0)
            return;

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZSize).Fill(item);
    }

    /// <summary>Fills a 3D sub-region with a value.</summary>
    public void Fill(T item, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (xStart < 0 || yStart < 0 || zStart < 0 || xEnd > XSize || yEnd > YSize || zEnd > ZSize || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        for (var x = xStart; x < xEnd; x++)
        for (var y = yStart; y < yEnd; y++)
            _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity + zStart, zCount).Fill(item);
    }

    /// <summary>Fills a 3D sub-region with a value.</summary>
    public void Fill(T item, Point3D offset, Size3D size) => Fill(item, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>Fills the entire list using a factory function.</summary>
    public void Fill(Func<T> factory) {
        ArgumentNullException.ThrowIfNull(factory);

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var start = x * YCapacity * ZCapacity + y * ZCapacity;

            for (var z = 0; z < ZSize; z++)
                _items[start + z] = factory();
        }
    }

    /// <summary>Fills a 3D sub-region using a factory function.</summary>
    public void Fill(Func<T> factory, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        ArgumentNullException.ThrowIfNull(factory);
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (xStart < 0 || yStart < 0 || zStart < 0 || xEnd > XSize || yEnd > YSize || zEnd > ZSize || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        for (var x = xStart; x < xEnd; x++)
        for (var y = yStart; y < yEnd; y++) {
            var start = x * YCapacity * ZCapacity + y * ZCapacity;

            for (var z = zStart; z < zEnd; z++)
                _items[start + z] = factory();
        }
    }

    /// <summary>Fills a 3D sub-region using a factory function.</summary>
    public void Fill(Func<T> factory, Point3D offset, Size3D size) => Fill(factory, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>Converts the 3D list to a 3D array.</summary>
    public T[,,] ToArray() {
        var arr = new T[XSize, YSize, ZSize];

        if (XSize == 0 || YSize == 0 || ZSize == 0)
            return arr;

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var srcSpan = _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZSize);
            var dstSpan = MemoryMarshal.CreateSpan(ref arr[x, y, 0], ZSize);
            srcSpan.CopyTo(dstSpan);
        }

        return arr;
    }

    /// <summary>Converts the 3D list to a jagged array.</summary>
    [Pure]
    public T[][][] ToJagged() {
        var arr = new T[XSize][][];

        for (var x = 0; x < XSize; x++) {
            arr[x] = new T[YSize][];

            for (var y = 0; y < YSize; y++) {
                arr[x][y] = new T[ZSize];
                _items.AsSpan(x * YCapacity * ZCapacity + y * ZCapacity, ZSize).CopyTo(arr[x][y]);
            }
        }

        return arr;
    }

    private class SliceX : IEnumerable2D<T> {
        private readonly List3D<T> _parent;
        private readonly int       _x;

        public SliceX(List3D<T> parent, int x) {
            _parent = parent;
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
                yield return _parent.UnsafeGet(_x, y, z);
        }

        public IEnumerable<T>     GetAtX(int x) => GetAtY(x);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceY : IEnumerable2D<T> {
        private readonly List3D<T> _parent;
        private readonly int       _y;

        public SliceY(List3D<T> parent, int y) {
            _parent = parent;
            _y      = y;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var x = 0; x < _parent.XSize; x++)
                yield return GetAtX(x);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int x) {
            var actualY = _parent.YSize > _y ? _y : 0;

            for (var z = 0; z < _parent.ZSize; z++)
                yield return _parent.UnsafeGet(x, actualY, z);
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceZ : IEnumerable2D<T> {
        private readonly List3D<T> _parent;
        private readonly int       _z;

        public SliceZ(List3D<T> parent, int z) {
            _parent = parent;
            _z      = z;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var x = 0; x < _parent.XSize; x++)
                yield return GetAtX(x);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int x) {
            var actualZ = _parent.ZSize > _z ? _z : 0;

            for (var y = 0; y < _parent.YSize; y++)
                yield return _parent.UnsafeGet(x, y, actualZ);
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>Gets or sets the element at the specified 3D Indices.</summary>
    public T this[Index x, Index y, Index z] {
        get => this[x.GetOffset(XSize), y.GetOffset(YSize), z.GetOffset(ZSize)];
        set => this[x.GetOffset(XSize), y.GetOffset(YSize), z.GetOffset(ZSize)] = value;
    }

    /// <summary>Gets a 1D slice of elements along the X axis.</summary>
    public T[] this[Range x, int y, int z] {
        get {
            var (offset, length) = x.GetOffsetAndLength(XSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = UnsafeGet(offset + i, y, z);

            return result;
        }
    }

    /// <summary>Gets a 1D slice of elements along the Y axis.</summary>
    public T[] this[int x, Range y, int z] {
        get {
            var (offset, length) = y.GetOffsetAndLength(YSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = UnsafeGet(x, offset + i, z);

            return result;
        }
    }

    /// <summary>Gets a 1D slice of elements along the Z axis.</summary>
    public T[] this[int x, int y, Range z] {
        get {
            var (offset, length) = z.GetOffsetAndLength(ZSize);
            var result = new T[length];

            if (length > 0)
                _items.AsSpan(GetOffset(x, y, offset), length).CopyTo(result);

            return result;
        }
    }
    #endif

    #region Spans

    /// <summary>
    ///     Gets a Span over the Z elements at the specified X and Y coordinates.
    /// </summary>
    /// <param name="x">The zero-based X coordinate.</param>
    /// <param name="y">The zero-based Y coordinate.</param>
    /// <returns>A Span over the elements along the Z axis.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="x"/> or <paramref name="y"/> is out of bounds.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpanAtXY(int x, int y) {
        if (x < 0 || x >= XSize || y < 0 || y >= YSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        return _items.AsSpan(GetOffset(x, y, 0), ZSize);
    }

    /// <summary>
    ///     Copies the elements at the specified X and Y coordinates (along Z axis) into the destination span.
    /// </summary>
    /// <param name="x">The zero-based X coordinate.</param>
    /// <param name="y">The zero-based Y coordinate.</param>
    /// <param name="destination">The destination span.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination span is not large enough.</exception>
    public void CopyAtXYTo(int x, int y, Span<T> destination) {
        if (x < 0 || x >= XSize || y < 0 || y >= YSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        if (destination.Length < ZSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        _items.AsSpan(GetOffset(x, y, 0), ZSize).CopyTo(destination);
    }

    /// <summary>
    ///     Copies the elements at the specified X and Z coordinates (along Y axis) into the destination span.
    /// </summary>
    /// <param name="x">The zero-based X coordinate.</param>
    /// <param name="z">The zero-based Z coordinate.</param>
    /// <param name="destination">The destination span.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination span is not large enough.</exception>
    public void CopyAtXZTo(int x, int z, Span<T> destination) {
        if (x < 0 || x >= XSize || z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        if (destination.Length < YSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        for (var y = 0; y < YSize; y++)
            destination[y] = _items[GetOffset(x, y, z)];
    }

    /// <summary>
    ///     Copies the elements at the specified Y and Z coordinates (along X axis) into the destination span.
    /// </summary>
    /// <param name="y">The zero-based Y coordinate.</param>
    /// <param name="z">The zero-based Z coordinate.</param>
    /// <param name="destination">The destination span.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination span is not large enough.</exception>
    public void CopyAtYZTo(int y, int z, Span<T> destination) {
        if (y < 0 || y >= YSize || z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException("Indices are out of range.");

        if (destination.Length < XSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        for (var x = 0; x < XSize; x++)
            destination[x] = _items[GetOffset(x, y, z)];
    }

    /// <summary>
    ///     Copies a rectangular 3D sub-region (slice) of the list into a contiguous destination span.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    /// <param name="xStart">The starting X coordinate.</param>
    /// <param name="xCount">The number of elements to copy along X-axis.</param>
    /// <param name="yStart">The starting Y coordinate.</param>
    /// <param name="yCount">The number of elements to copy along Y-axis.</param>
    /// <param name="zStart">The starting Z coordinate.</param>
    /// <param name="zCount">The number of elements to copy along Z-axis.</param>
    public void CopySliceTo(Span<T> destination, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (xStart < 0 || yStart < 0 || zStart < 0 || xEnd > XSize || yEnd > YSize || zEnd > ZSize || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException("Specified slice is out of bounds.");

        var requiredLength = xCount * yCount * zCount;

        if (destination.Length < requiredLength)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        if (requiredLength == 0)
            return;

        var destIdx = 0;

        for (var x = 0; x < xCount; x++)
        for (var y = 0; y < yCount; y++) {
            var srcSpan = _items.AsSpan(GetOffset(xStart + x, yStart + y, zStart), zCount);
            var dstSpan = destination.Slice(destIdx, zCount);
            srcSpan.CopyTo(dstSpan);
            destIdx += zCount;
        }
    }

    /// <summary>
    ///     Copies a rectangular 3D sub-region (slice) of the list into a contiguous destination span.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    /// <param name="offset">The offset of the region.</param>
    /// <param name="size">The size of the region.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopySliceTo(Span<T> destination, Point3D offset, Size3D size) => CopySliceTo(destination, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    #endregion

}