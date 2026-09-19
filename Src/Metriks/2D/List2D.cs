// Metriks
// Copyright (c) KryKom 2026

#if !METRIKS_UNSAFE_MODE
#define METRIKS_SAFE_MODE
#endif

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
/// Represents a strongly typed, two-dimensional list of elements that can be accessed by X and Y
/// indices. Provides methods to search, sort, and manipulate 2D lists.
/// </summary>
public class List2D<T> : IList2D<T>, ICollection2D, IReadOnlyList2D<T> {

    private const int   INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR    = 2.0f;

    #region Fields

    private T[] _items;
    private int _xSize;
    private int _ySize;
    private int _xCapacity;
    private int _yCapacity;

    #endregion

    #region Properties

    // /// <summary>
    // ///     Represents the internal storage array for elements in the 2D list.
    // ///     The size of this array is determined by the product of the current x and y capacities.
    // ///     To access the elements, use the formula x * yCapacity + y.
    // /// </summary>
    // private T[] _items {
    //     get => _items;
    //     set => _items = value;
    // }

    /// <summary>
    ///     Gets the size (number of elements) along the X-axis.
    /// </summary>
    public int XSize {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _xSize;
        private set => _xSize = value;
    }

    /// <summary>
    ///     Gets the size (number of elements) along the Y-axis.
    /// </summary>
    public int YSize {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _ySize;
        private set => _ySize = value;
    }

    /// <summary>
    ///     Gets a <see cref="Size2D" /> representing the current size of the list in both dimensions.
    /// </summary>
    public Size2D Size {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(XSize, YSize);
    }

    /// <summary>
    ///     Gets the capacity along the X-axis.
    /// </summary>
    public int XCapacity {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _xCapacity;
        private set => _xCapacity = value;
    }

    /// <summary>
    ///     Gets the capacity along the Y-axis.
    /// </summary>
    public int YCapacity {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _yCapacity;
        private set => _yCapacity = value;
    }

    /// <summary>
    ///     Gets the total number of elements contained in the <see cref="List2D{T}" />.
    /// </summary>
    public int Count {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => XSize * YSize;
    }

    /// <summary>
    ///     Gets the size (number of elements) along the X-axis.
    /// </summary>
    public int XCount {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => XSize;
    }

    /// <summary>
    ///     Gets the size (number of elements) along the Y-axis.
    /// </summary>
    public int YCount {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => YSize;
    }

    /// <summary>
    ///     Gets the total allocated storage capacity of the 2D list, representing the maximum
    ///     number of elements it can hold across all dimensions.
    /// </summary>
    public int TotalCapacity => _items.Length;

    public bool IsReadOnly => false;

    #endregion

    #region Indexers

    public T this[int x, int y] {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            ValidateIndexBounds(x, y);

            ref var space  = ref MetriksHelpers.GetArrayData(_items);
            var     offset = x * (nint)YCapacity + y;

            return Unsafe.Add(ref space, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            ValidateIndexBounds(x, y);

            ref var space  = ref MetriksHelpers.GetArrayData(_items);
            var     offset = x * (nint)YCapacity + y;

            Unsafe.Add(ref space, offset) = value;
        }
    }

    public ref T GetRef(int x, int y) {
        ValidateIndexBounds(x, y);

        ref var space  = ref MetriksHelpers.GetArrayData(_items);
        var     offset = x * (nint)YCapacity + y;

        return ref Unsafe.Add(ref space, offset);
    }

    internal T GetUnsafe(int x, int y) {
        ref var space  = ref MetriksHelpers.GetArrayData(_items);
        var     offset = x * (nint)YCapacity + y;

        return Unsafe.Add(ref space, offset);
    }

    public T this[Index x, Index y] {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[x.GetOffset(XSize), y.GetOffset(YSize)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[x.GetOffset(XSize), y.GetOffset(YSize)] = value;
    }

    public T this[Point2D point] {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[point.X, point.Y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[point.X, point.Y] = value;
    }

    public Span<T> this[Index x, Range y] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            ValidateIndexBounds(x, y);

            var xo = x.GetOffset(XSize);

            // Converts the Range struct into a concrete starting offset and length
            var (yo, ys) = y.GetOffsetAndLength(YCapacity);
            var startIdx = xo * YCapacity + yo;

            return _items.AsSpan(startIdx, ys);
        }
    }

    public List2D<T> this[Range x, Range y] {
        get {
            ValidateIndexBounds(x, y);

            var (xo, xs) = x.GetOffsetAndLength(XSize);
            var (yo, ys) = y.GetOffsetAndLength(YSize);

            var slice = new List2D<T>(xs, ys);

            if (xs == 0 || ys == 0)
                return slice;

            for (var xi = 0; xi < xs; xi++) {
                var sourceStartIdx = (xo + xi) * YCapacity + yo;

                ReadOnlySpan<T> sourceRowSpan = _items.AsSpan(sourceStartIdx, ys);
                var             destRowSpan   = slice._items.AsSpan(xi * ys, ys);

                sourceRowSpan.CopyTo(destRowSpan);
            }

            return slice;
        }
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER

    /// <summary>
    ///     Gets a row of elements at the specified X index range and Y index.
    /// </summary>
    /// <param name="x">The range of X indices.</param>
    /// <param name="y">The Y index.</param>
    /// <returns>A new array containing the row slice.</returns>
    public T[] this[Range x, int y] {
        get {
            var (offset, length) = x.GetOffsetAndLength(XSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = this[offset + i, y];

            return result;
        }
    }

    /// <summary>
    ///     Gets a column range of elements at the specified X index and Y index range.
    /// </summary>
    /// <param name="x">The X index.</param>
    /// <param name="y">The range of Y indices.</param>
    /// <returns>A new array containing the column slice.</returns>
    public T[] this[int x, Range y] {
        get {
            var (offset, length) = y.GetOffsetAndLength(YSize);
            var result = new T[length];

            if (length > 0)
                _items.AsSpan(x * YCapacity + offset, length).CopyTo(result);

            return result;
        }
    }

    #endif

    #endregion

    #region Constructors

    public List2D() : this(INITIAL_CAPACITY, INITIAL_CAPACITY) { }

    public List2D(int xyCapacity) : this(xyCapacity, xyCapacity) { }

    public List2D(int xCapacity, int yCapacity) {
        XCapacity = xCapacity;
        YCapacity = yCapacity;
        XSize     = 0;
        YSize     = 0;
        _items    = new T[XCapacity * YCapacity];
    }

    public List2D(T[,] collection) {
        ArgumentNullException.ThrowIfNull(collection);

        XSize     = collection.GetLength(0);
        YSize     = collection.GetLength(1);
        XCapacity = XSize;
        YCapacity = YSize;

        var totalElements = XSize * YSize;

        if (totalElements == 0) {
            _items = Array.Empty<T>();

            return;
        }

        _items = new T[totalElements];

        var sourceSpan = MemoryMarshal.CreateReadOnlySpan(ref collection[0, 0], totalElements);
        sourceSpan.CopyTo(_items);
    }

    #endregion

    #region Helper Methods

    private void EnsureXCapacity(int minXCapacity) {
        if (minXCapacity > XCapacity) {
            var newXCapacity = Math.Max(minXCapacity, (int)(XCapacity * GROWTH_FACTOR));

            if (newXCapacity < INITIAL_CAPACITY)
                newXCapacity = INITIAL_CAPACITY;

            ResizeCapacity(newXCapacity, YCapacity);
        }
    }

    private void EnsureYCapacity(int minYCapacity) {
        if (minYCapacity > YCapacity) {
            var newYCapacity = Math.Max(minYCapacity, (int)(YCapacity * GROWTH_FACTOR));

            if (newYCapacity < INITIAL_CAPACITY)
                newYCapacity = INITIAL_CAPACITY;

            ResizeCapacity(XCapacity, newYCapacity);
        }
    }

    private void ResizeCapacity(int newXCapacity, int newYCapacity) {
        var newItems = new T[newXCapacity * newYCapacity];

        if (XSize > 0) {
            if (YCapacity == newYCapacity) {
                _items.AsSpan(0, XSize * YCapacity).CopyTo(newItems);
            }
            else {
                var copyLength = Math.Min(YCapacity, newYCapacity);

                for (var x = 0; x < XSize; x++)
                    _items
                        .AsSpan(x * YCapacity, copyLength)
                        .CopyTo(newItems.AsSpan(x * newYCapacity, copyLength));
            }
        }

        _items     = newItems;
        XCapacity = newXCapacity;
        YCapacity = newYCapacity;
    }

    [StackTraceHidden]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Conditional("METRIKS_SAFE_MODE")]
    private void ValidateIndexBounds(int x, int y) {
        if (!InBounds(x, y))
            throw new IndexOutOfRangeException(
                $"Index [{x}, {y}] is out of bounds. Must be [0..{XSize - 1}, 0..{YSize - 1}]"
            );
    }

    [StackTraceHidden]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Conditional("METRIKS_SAFE_MODE")]
    private void ValidateIndexBounds(Range x, Index y) {
        if (
            !InBounds(
                x.Start.GetOffset(XSize),
                y.GetOffset(YSize),
                x.End.GetOffset(XSize),
                y.GetOffset(YSize)
            )
        )
            throw new IndexOutOfRangeException(
                $"Range [{x.ToString()}, {y.ToString()}] is out of bounds for dimension x. " +
                $"Must be [0..{XSize - 1}, 0..{YSize - 1}]"
            );
    }

    [StackTraceHidden]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Conditional("METRIKS_SAFE_MODE")]
    private void ValidateIndexBounds(Index x, Range y) {
        if (
            !InBounds(
                x.GetOffset(XSize),
                y.Start.GetOffset(YSize),
                x.GetOffset(XSize),
                y.End.GetOffset(YSize)
            )
        )
            throw new IndexOutOfRangeException(
                $"Range [{x.ToString()}, {y.ToString()}] is out of bounds for dimension x. " +
                $"Must be [0..{XSize - 1}, 0..{YSize - 1}]"
            );
    }

    [StackTraceHidden]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Conditional("METRIKS_SAFE_MODE")]
    private void ValidateIndexBounds(Range x, Range y) {
        if (
            !InBounds(
                x.Start.GetOffset(XSize),
                y.Start.GetOffset(YSize),
                x.End.GetOffset(XSize),
                y.End.GetOffset(YSize)
            )
        )
            throw new IndexOutOfRangeException(
                $"Range [{x.ToString()}, {y.ToString()}] is out of bounds for dimension x. " +
                $"Must be [0..{XSize - 1}, 0..{YSize - 1}]"
            );
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool InBounds(int x, int y) => x >= 0 && x < XSize && y >= 0 && y < YSize;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool InBounds(int xStart, int yStart, int xEnd, int yEnd) =>
        xStart >= 0 && xEnd < XSize && xStart <= xEnd &&
        yStart >= 0 && yEnd < YSize && yStart <= yEnd;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void UnsafeSet(int x, int y, T value) {
        ref var space  = ref MetriksHelpers.GetArrayData(_items);
        var     offset = x * (nint)YCapacity + y;
        Unsafe.Add(ref space, offset) = value;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T UnsafeGet(int x, int y) {
        ref var space  = ref MetriksHelpers.GetArrayData(_items);
        var     offset = x * (nint)YCapacity + y;

        return Unsafe.Add(ref space, offset);
    }

    #endregion

    #region Spans

    /// <summary>
    ///     Gets a Span over the specified X index (elements along Y-axis for constant X).
    /// </summary>
    /// <param name="x">The zero-based X index.</param>
    /// <returns>A Span over the elements at X.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="x" /> is out of bounds.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpanAtX(int x) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException("Index 'x' is out of range.");

        return _items.AsSpan(x * YCapacity, YSize);
    }

    /// <summary>
    ///     Copies the elements at the specified X coordinate (constant X) into the destination span.
    /// </summary>
    /// <param name="x">The zero-based X index.</param>
    /// <param name="destination">The destination span where elements will be copied.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="x" /> is out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="destination" /> is not large enough.</exception>
    public void CopyAtXTo(int x, Span<T> destination) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException("Index 'x' is out of range.");

        if (destination.Length < YSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        _items.AsSpan(x * YCapacity, YSize).CopyTo(destination);
    }

    /// <summary>
    ///     Copies the elements at the specified Y coordinate (constant Y) into the destination span.
    /// </summary>
    /// <param name="y">The zero-based Y index.</param>
    /// <param name="destination">The destination span where elements will be copied.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="y" /> is out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="destination" /> is not large enough.</exception>
    public void CopyAtYTo(int y, Span<T> destination) {
        if (y < 0 || y >= YSize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");

        if (destination.Length < XSize)
            throw new ArgumentException("Destination span is not large enough.", nameof(destination));

        for (var x = 0; x < XSize; x++)
            destination[x] = _items[x * YCapacity + y];
    }

    /// <summary>
    ///     Copies a rectangular sub-region (slice) of the 2D list into a contiguous destination span column-by-column.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    /// <param name="xStart">The starting X coordinate.</param>
    /// <param name="xCount">The number of columns to copy.</param>
    /// <param name="yStart">The starting Y coordinate.</param>
    /// <param name="yCount">The number of rows to copy.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when specified slice boundaries are out of bounds.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination span is not large enough to hold the sliced elements.</exception>
    public void CopySliceTo(Span<T> destination, int xStart, int xCount, int yStart, int yCount) {
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;

        if (xStart < 0 || yStart < 0 || xEnd > XSize || yEnd > YSize || xCount < 0 || yCount < 0)
            throw new IndexOutOfRangeException("Specified slice is out of bounds.");

        var requiredLength = xCount * yCount;

        if (destination.Length < requiredLength)
            throw new ArgumentException("Destination span is not large enough to hold the slice.", nameof(destination));

        if (requiredLength == 0)
            return;

        for (var x = 0; x < xCount; x++) {
            var srcSpan = _items.AsSpan((xStart + x) * YCapacity + yStart, yCount);
            var dstSpan = destination.Slice(x * yCount, yCount);
            srcSpan.CopyTo(dstSpan);
        }
    }

    /// <summary>
    ///     Copies a rectangular sub-region (slice) of the 2D list into a contiguous destination span.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    /// <param name="offset">The starting Point2D coordinates.</param>
    /// <param name="size">The Size2D of the sub-region.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopySliceTo(Span<T> destination, Point2D offset, Size2D size) => CopySliceTo(destination, offset.X, size.X, offset.Y, size.Y);

    #endregion

    /// <summary>
    ///     Copies all elements of the <see cref="List2D{T}" /> to the specified <see cref="Array" />
    ///     starting at the given coordinates.
    /// </summary>
    /// <param name="array">The destination two-dimensional array.</param>
    /// <param name="index">
    ///     The zero-based 2D coordinates in the destination array at which
    ///     copying begins.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="array" /> is
    ///     null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when array is not two-dimensional or is not
    ///     large enough.
    /// </exception>
    public void CopyTo(Array array, Point2D index) {
        ArgumentNullException.ThrowIfNull(array);

        if (array.Rank != 2)
            throw new ArgumentException("Array must be two-dimensional (Rank = 2).", nameof(array));

        if (array.GetLength(0) < XSize + index.X)
            throw new ArgumentException("Destination array is not large enough in x dimension.");

        if (array.GetLength(1) < YSize + index.Y)
            throw new ArgumentException("Destination array is not large enough in y dimension.");

        for (var x = 0; x < XSize; x++) {
            var start = x * YCapacity;

            for (var y = 0; y < YSize; y++)
                array.SetValue(_items[start + y]!, x + index.X, y + index.Y);
        }
    }

    /// <summary>
    ///     Copies all elements of the <see cref="List2D{T}" /> to the specified generic two-dimensional
    ///     array starting at the given coordinates.
    /// </summary>
    /// <param name="array">The destination generic two-dimensional array.</param>
    /// <param name="index">
    ///     The zero-based 2D coordinates in the destination array at which
    ///     copying begins.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="array" /> is
    ///     null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the destination array is not large
    ///     enough.
    /// </exception>
    public void CopyTo(T[,] array, Point2D index) {
        ArgumentNullException.ThrowIfNull(array);

        if (array.GetLength(0) < XSize + index.X)
            throw new ArgumentException("Destination array is not large enough in x dimension.");

        if (array.GetLength(1) < YSize + index.Y)
            throw new ArgumentException("Destination array is not large enough in y dimension.");

        if (XSize == 0 || YSize == 0)
            return;

        for (var x = 0; x < XSize; x++) {
            var srcSpan = _items.AsSpan(x * YCapacity, YSize);
            var dstSpan = MemoryMarshal.CreateSpan(ref array[x + index.X, index.Y], YSize);
            srcSpan.CopyTo(dstSpan);
        }
    }

    /// <summary>
    ///     Resets the list to size 0 and ensures it has at least the specified capacity.
    ///     Clears all existing elements to default.
    /// </summary>
    public void Reset(int xCapacity, int yCapacity) {
        _xSize = 0;
        _ySize = 0;
        var requiredLength = xCapacity * yCapacity;

        if (_items == null || _items.Length < requiredLength)
            _items = new T[requiredLength];
        else
            Array.Clear(_items, 0, _items.Length);

        _xCapacity = xCapacity;
        _yCapacity = yCapacity;
    }

    /// <summary>
    ///     Clears all elements from the list, resetting its size and capacity to initial defaults.
    /// </summary>
    public void Clear() {
        _items     = new T[INITIAL_CAPACITY * INITIAL_CAPACITY];
        XSize     = 0;
        YSize     = 0;
        XCapacity = INITIAL_CAPACITY;
        YCapacity = INITIAL_CAPACITY;
    }

    /// <summary>
    ///     Adds a new column at the end of the X-axis of the list, increasing XSize by 1.
    /// </summary>
    public void AddX() {
        EnsureXCapacity(XSize + 1);
        _items.AsSpan(XSize * YCapacity, YCapacity).Clear();
        XSize++;
    }

    /// <summary>
    ///     Adds a new row at the end of the Y-axis of the list, increasing YSize by 1.
    /// </summary>
    public void AddY() {
        EnsureYCapacity(YSize + 1);
        YSize++;
    }

    /// <summary>
    ///     Removes the last column from the end of the X-axis, decreasing XSize by 1.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when list XSize is 0.</exception>
    public void ShrinkX() {
        if (XSize == 0)
            throw new InvalidOperationException("Cannot remove from a List2D with x-size = 0.");

        XSize--;
        _items.AsSpan(XSize * YCapacity, YCapacity).Clear();
    }

    /// <summary>
    ///     Removes the last row from the end of the Y-axis, decreasing YSize by 1.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when list YSize is 0.</exception>
    public void ShrinkY() {
        if (YSize == 0)
            throw new InvalidOperationException("Cannot remove from a List2D with y-size = 0.");

        YSize--;

        for (var x = 0; x < XSize; x++)
            _items[x * YCapacity + YSize] = default!;
    }

    /// <summary>
    ///     Determines whether the list contains the specified element.
    /// </summary>
    /// <param name="value">The value to locate in the list.</param>
    /// <returns>True if value is found in the list; otherwise, false.</returns>
    public bool Contains(T value) {
        if (XSize == 0 || YSize == 0)
            return false;

        if (YSize == YCapacity)
            return Array.IndexOf(_items, value, 0, XSize * YCapacity) >= 0;

        for (var x = 0; x < XSize; x++)
            if (Array.IndexOf(_items, value, x * YCapacity, YSize) >= 0)
                return true;

        return false;
    }

    /// <summary>
    ///     Determines whether the specified column (at horizontal coordinate X) contains the specified element.
    /// </summary>
    /// <param name="x">The zero-based column coordinate.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns>True if value is found in the column; otherwise, false.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="x" /> is out of bounds.</exception>
    public bool ContainsAtX(int x, T value) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException("Index 'x' is out of range.");

        return Array.IndexOf(_items, value, x * YCapacity, YSize) >= 0;
    }

    /// <summary>
    ///     Determines whether the specified row (at vertical coordinate Y) contains the specified element.
    /// </summary>
    /// <param name="y">The zero-based row coordinate.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns>True if value is found in the row; otherwise, false.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="y" /> is out of bounds.</exception>
    public bool ContainsAtY(int y, T value) {
        if (y < 0 || y >= YSize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");

        var comparer = EqualityComparer<T>.Default;

        for (var x = 0; x < XSize; x++)
            if (comparer.Equals(_items[x * YCapacity + y], value))
                return true;

        return false;
    }

    /// <summary>
    ///     Inserts a new column at the specified index in the 2D list.
    /// </summary>
    /// <param name="x">The zero-based X-index at which the new column should be inserted.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds.</exception>
    public void InsertAtX(int x) {
        if (x < 0 || x > XSize)
            throw new IndexOutOfRangeException("Index 'x' is out of range.");

        EnsureXCapacity(XSize + 1);

        if (x < XSize) {
            var srcStart = x           * YCapacity;
            var dstStart = (x     + 1) * YCapacity;
            var length   = (XSize - x) * YCapacity;
            Array.Copy(_items, srcStart, _items, dstStart, length);
        }

        _items.AsSpan(x * YCapacity, YCapacity).Clear();
        XSize++;
    }

    /// <summary>
    ///     Inserts a new row at the specified index in the 2D list.
    /// </summary>
    /// <param name="y">The zero-based Y-index at which the new row should be inserted.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds.</exception>
    public void InsertAtY(int y) {
        if (y < 0 || y > YSize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");

        EnsureYCapacity(YSize + 1);

        var shiftLength = YSize - y;

        if (shiftLength > 0)
            for (var x = 0; x < XSize; x++) {
                var colStart = x * YCapacity;
                Array.Copy(_items, colStart + y, _items, colStart + y + 1, shiftLength);
                _items[colStart + y] = default!;
            }
        else
            for (var x = 0; x < XSize; x++)
                _items[x * YCapacity + y] = default!;

        YSize++;
    }

    /// <summary>
    ///     Removes the column at the specified index from the 2D list.
    /// </summary>
    /// <param name="x">The zero-based X-index of the column to remove.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds.</exception>
    /// <exception cref="InvalidOperationException">Thrown when list XSize is 0.</exception>
    public void RemoveAtX(int x) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException("Index 'x' is out of range.");

        if (XSize == 0)
            throw new InvalidOperationException("Cannot remove from a List2D with x-size = 0.");

        XSize--;

        if (x < XSize) {
            var srcStart = (x + 1)     * YCapacity;
            var dstStart = x           * YCapacity;
            var length   = (XSize - x) * YCapacity;
            Array.Copy(_items, srcStart, _items, dstStart, length);
        }

        _items.AsSpan(XSize * YCapacity, YCapacity).Clear();
    }

    /// <summary>
    ///     Removes the row at the specified index from the 2D list.
    /// </summary>
    /// <param name="y">The zero-based Y-index of the row to remove.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds.</exception>
    /// <exception cref="InvalidOperationException">Thrown when list YSize is 0.</exception>
    public void RemoveAtY(int y) {
        if (y < 0 || y >= YSize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");

        if (YSize == 0)
            throw new InvalidOperationException("Cannot remove from a List2D with y-size = 0.");

        YSize--;

        var shiftLength = YSize - y;

        if (shiftLength > 0)
            for (var x = 0; x < XSize; x++) {
                var colStart = x * YCapacity;
                Array.Copy(_items, colStart + y + 1, _items, colStart + y, shiftLength);
                _items[colStart + YSize] = default!;
            }
        else
            for (var x = 0; x < XSize; x++)
                _items[x * YCapacity + YSize] = default!;
    }

    /// <summary>
    ///     Expands the dimensions of the list to the specified sizes, filling new regions
    ///     with a default value.
    /// </summary>
    /// <param name="xSize">The target X-dimension size (must be >= current XSize).</param>
    /// <param name="ySize">The target Y-dimension size (must be >= current YSize).</param>
    /// <param name="defaultValue">The value to fill the newly created elements with.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when target sizes are smaller
    ///     than current sizes.
    /// </exception>
    public void Expand(int xSize, int ySize, T? defaultValue = default!) {
        if (xSize < XSize)
            throw new ArgumentOutOfRangeException(
                nameof(xSize),
                "Cannot shrink the XSize of a List2D using Expand."
            );

        if (ySize < YSize)
            throw new ArgumentOutOfRangeException(
                nameof(ySize),
                "Cannot shrink the YSize of a List2D using Expand."
            );

        if (xSize == XSize && ySize == YSize)
            return;

        var targetXCapacity = XCapacity;
        var targetYCapacity = YCapacity;

        if (xSize > XCapacity)
            targetXCapacity = xSize + 1;

        if (ySize > YCapacity)
            targetYCapacity = ySize + 1;

        if (targetXCapacity != XCapacity || targetYCapacity != YCapacity)
            ResizeCapacity(targetXCapacity, targetYCapacity);

        if (ySize > YSize) {
            var fillCount = ySize - YSize;

            for (var x = 0; x < XSize; x++)
                _items.AsSpan(x * YCapacity + YSize, fillCount).Fill(defaultValue!);
        }

        for (var x = XSize; x < xSize; x++)
            _items.AsSpan(x * YCapacity, ySize).Fill(defaultValue!);

        XSize = xSize;
        YSize = ySize;
    }

    /// <summary>
    ///     Expands the dimensions of the list to the specified sizes, filling new regions using
    ///     the specified factory.
    /// </summary>
    /// <param name="xSize">The target X-dimension size (must be >= current XSize).</param>
    /// <param name="ySize">The target Y-dimension size (must be >= current YSize).</param>
    /// <param name="defaultValueFactory">
    ///     A factory function generating values for newly
    ///     created elements.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when target sizes are smaller
    ///     than current sizes.
    /// </exception>
    public void Expand(int xSize, int ySize, Func<T> defaultValueFactory) {
        if (xSize < XSize)
            throw new ArgumentOutOfRangeException(
                nameof(xSize),
                "Cannot shrink the XSize of a List2D using Expand."
            );

        if (ySize < YSize)
            throw new ArgumentOutOfRangeException(
                nameof(ySize),
                "Cannot shrink the YSize of a List2D using Expand."
            );

        if (xSize == XSize && ySize == YSize)
            return;

        var targetXCapacity = XCapacity;
        var targetYCapacity = YCapacity;

        if (xSize > XCapacity)
            targetXCapacity = xSize + 1;

        if (ySize > YCapacity)
            targetYCapacity = ySize + 1;

        if (targetXCapacity != XCapacity || targetYCapacity != YCapacity)
            ResizeCapacity(targetXCapacity, targetYCapacity);

        if (ySize > YSize)
            for (var x = 0; x < XSize; x++) {
                var start = x * YCapacity;

                for (var y = YSize; y < ySize; y++)
                    _items[start + y] = defaultValueFactory();
            }

        for (var x = XSize; x < xSize; x++) {
            var start = x * YCapacity;

            for (var y = 0; y < ySize; y++)
                _items[start + y] = defaultValueFactory();
        }

        XSize = xSize;
        YSize = ySize;
    }

    /// <summary>
    ///     Resizes the list dimensions to the specified sizes, using a default value to pad
    ///     expanded dimensions.
    /// </summary>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="defaultValue">The value to populate expanded regions with.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when target dimensions are
    ///     negative.
    /// </exception>
    public void Resize(int xSize, int ySize, T? defaultValue = default) {
        if (xSize < 0)
            throw new ArgumentOutOfRangeException(
                nameof(xSize),
                "Cannot resize a List2D with a negative XSize."
            );

        if (ySize < 0)
            throw new ArgumentOutOfRangeException(
                nameof(ySize),
                "Cannot resize a List2D with a negative YSize."
            );

        var newItems = new T[xSize * ySize];

        if (defaultValue is not null)
            newItems.AsSpan().Fill(defaultValue);

        var copyX = Math.Min(XSize, xSize);
        var copyY = Math.Min(YSize, ySize);

        if (copyX > 0 && copyY > 0)
            for (var x = 0; x < copyX; x++)
                _items.AsSpan(x * YCapacity, copyY).CopyTo(newItems.AsSpan(x * ySize, copyY));

        _items     = newItems;
        XSize     = xSize;
        YSize     = ySize;
        XCapacity = xSize;
        YCapacity = ySize;
    }

    /// <summary>
    ///     Resizes the list dimensions to the specified sizes, using a factory function to
    ///     create elements for expanded dimensions.
    /// </summary>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="defaultValueFactory">
    ///     A factory function generating values for newly
    ///     expanded elements.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when target dimensions are
    ///     negative.
    /// </exception>
    public void Resize(int xSize, int ySize, Func<T> defaultValueFactory) {
        if (xSize < 0)
            throw new ArgumentOutOfRangeException(
                nameof(xSize),
                "Cannot resize a List2D with a negative XSize."
            );

        if (ySize < 0)
            throw new ArgumentOutOfRangeException(
                nameof(ySize),
                "Cannot resize a List2D with a negative YSize."
            );

        var newItems = new T[xSize * ySize];

        for (var i = 0; i < newItems.Length; i++)
            newItems[i] = defaultValueFactory();

        var copyX = Math.Min(XSize, xSize);
        var copyY = Math.Min(YSize, ySize);

        if (copyX > 0 && copyY > 0)
            for (var x = 0; x < copyX; x++)
                _items.AsSpan(x * YCapacity, copyY).CopyTo(newItems.AsSpan(x * ySize, copyY));

        _items     = newItems;
        XSize     = xSize;
        YSize     = ySize;
        XCapacity = xSize;
        YCapacity = ySize;
    }

    /// <summary>
    ///     Shrinks the dimensions of the list to the specified sizes.
    /// </summary>
    /// <param name="xSize">The new XSize (must be less than or equal to current XSize).</param>
    /// <param name="ySize">The new YSize (must be less than or equal to current YSize).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when new sizes are larger than
    ///     current sizes.
    /// </exception>
    public void Shrink(int xSize, int ySize) {
        if (xSize > XSize)
            throw new ArgumentOutOfRangeException(
                nameof(xSize),
                "Cannot expand the XSize of a List2D using Shrink."
            );

        if (ySize > YSize)
            throw new ArgumentOutOfRangeException(
                nameof(ySize),
                "Cannot expand the YSize of a List2D using Shrink."
            );

        var newItems = new T[xSize * ySize];

        if (xSize > 0 && ySize > 0)
            for (var x = 0; x < xSize; x++)
                _items.AsSpan(x * YCapacity, ySize).CopyTo(newItems.AsSpan(x * ySize, ySize));

        _items     = newItems;
        XSize     = xSize;
        YSize     = ySize;
        XCapacity = xSize;
        YCapacity = ySize;
    }

    /// <summary>
    ///     Places the contents of the specified generic two-dimensional array into the list,
    ///     optionally offset by a specified coordinate offset point.
    /// </summary>
    /// <param name="matrix">The generic two-dimensional array to place.</param>
    /// <param name="offsetPoint">
    ///     The 2D coordinate offset point defining where to place the
    ///     top-left corner of the matrix.
    /// </param>
    /// <param name="resize">
    ///     True to automatically grow the list sizes if matrix extends
    ///     beyond current bounds; otherwise, false.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="matrix" /> is
    ///     null.
    /// </exception>
    public void Place(T[,] matrix, Point2D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);

        var offset  = offsetPoint ?? Point2D.Zero;
        var matrixX = matrix.GetLength(0);
        var matrixY = matrix.GetLength(1);

        var placedMax = offset + new Point2D(matrixX, matrixY);

        var max = new Point2D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y));
        var min = new Point2D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0));

        var newSize = new Size2D(max.X - min.X, max.Y - min.Y);

        var isBelow = offset.X < 0 || offset.Y < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X * newSize.Y];

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);

            if (XSize > 0 && YSize > 0)
                for (var x = 0; x < XSize; x++)
                    _items.AsSpan(x * YCapacity, YSize)
                        .CopyTo(newItems.AsSpan((x + oldXOffset) * newSize.Y + oldYOffset, YSize));

            if (matrixX > 0 && matrixY > 0)
                for (var x = 0; x < matrixX; x++) {
                    var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[x, 0], matrixY);
                    var dstSpan = newItems.AsSpan((x + newXOffset) * newSize.Y + newYOffset, matrixY);
                    srcSpan.CopyTo(dstSpan);
                }

            _items     = newItems;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            XCapacity = XSize;
            YCapacity = YSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize) && resize)
                Expand(newSize.X, newSize.Y);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matrixX);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matrixY);
            var yCount = endY - startY;

            if (yCount <= 0 || startX >= endX)
                return;

            for (var x = startX; x < endX; x++) {
                var srcSpan = MemoryMarshal.CreateReadOnlySpan(
                    ref matrix[x - offset.X, startY - offset.Y],
                    yCount
                );

                var dstSpan = _items.AsSpan(x * YCapacity + startY, yCount);
                srcSpan.CopyTo(dstSpan);
            }
        }
    }

    /// <summary>
    ///     Places the contents of the specified 2D list into the list, optionally offset by a
    ///     specified coordinate offset point.
    /// </summary>
    /// <param name="matrix">The source 2D list to place.</param>
    /// <param name="offsetPoint">
    ///     The 2D coordinate offset point defining where to place the
    ///     top-left corner of the matrix.
    /// </param>
    /// <param name="resize">
    ///     True to automatically grow the list sizes if matrix extends
    ///     beyond current bounds; otherwise, false.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="matrix" /> is
    ///     null.
    /// </exception>
    public void Place(List2D<T> matrix, Point2D? offsetPoint = null, bool resize = true) {
        ArgumentNullException.ThrowIfNull(matrix);

        var offset  = offsetPoint ?? Point2D.Zero;
        var matrixX = matrix.XSize;
        var matrixY = matrix.YSize;

        var placedMax = offset + new Point2D(matrixX, matrixY);

        var max = new Point2D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y));
        var min = new Point2D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0));

        var newSize = new Size2D(max.X - min.X, max.Y - min.Y);

        var isBelow = offset.X < 0 || offset.Y < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X * newSize.Y];

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);

            if (XSize > 0 && YSize > 0)
                for (var x = 0; x < XSize; x++)
                    _items
                        .AsSpan(x * YCapacity, YSize)
                        .CopyTo(newItems.AsSpan((x + oldXOffset) * newSize.Y + oldYOffset, YSize));

            if (matrixX > 0 && matrixY > 0)
                for (var x = 0; x < matrixX; x++)
                    matrix._items
                        .AsSpan(x * matrix.YCapacity, matrixY)
                        .CopyTo(newItems.AsSpan((x + newXOffset) * newSize.Y + newYOffset, matrixY));

            _items     = newItems;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            XCapacity = XSize;
            YCapacity = YSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize) && resize)
                Expand(newSize.X, newSize.Y);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matrixX);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matrixY);
            var yCount = endY - startY;

            if (yCount <= 0 || startX >= endX)
                return;

            for (var x = startX; x < endX; x++)
                matrix._items
                    .AsSpan(
                        (x - offset.X) * matrix.YCapacity + (startY - offset.Y),
                        yCount
                    )
                    .CopyTo(_items.AsSpan(x * YCapacity + startY, yCount));
        }
    }

    /// <summary>
    ///     Places the contents of the specified generic two-dimensional array into the list,
    ///     applying a predicate to determine if elements should be overwritten.
    /// </summary>
    /// <param name="matrix">The generic two-dimensional array to place.</param>
    /// <param name="predicate">
    ///     A function receiving the existing element and new element,
    ///     returning true if the element should be replaced.
    /// </param>
    /// <param name="offsetPoint">
    ///     The 2D coordinate offset point defining where to place the
    ///     top-left corner of the matrix.
    /// </param>
    /// <param name="resize">
    ///     True to automatically grow the list sizes if matrix extends beyond
    ///     current bounds; otherwise, false.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="matrix" /> or
    ///     <paramref name="predicate" /> is null.
    /// </exception>
    public void Place(
        T[,]             matrix,
        Func<T, T, bool> predicate,
        Point2D?         offsetPoint = null,
        bool             resize      = true
    ) {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(predicate);

        var offset  = offsetPoint ?? Point2D.Zero;
        var matrixX = matrix.GetLength(0);
        var matrixY = matrix.GetLength(1);

        var placedMax = offset + new Point2D(matrixX, matrixY);

        var max = new Point2D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y));
        var min = new Point2D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0));

        var newSize = new Size2D(max.X - min.X, max.Y - min.Y);

        var isBelow = offset.X < 0 || offset.Y < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X * newSize.Y];

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);

            if (XSize > 0 && YSize > 0)
                for (var x = 0; x < XSize; x++)
                    _items.AsSpan(x * YCapacity, YSize)
                        .CopyTo(newItems.AsSpan((x + oldXOffset) * newSize.Y + oldYOffset, YSize));

            for (var x = 0; x < matrixX; x++)
            for (var y = 0; y < matrixY; y++) {
                var targetIdx = (x + newXOffset) * newSize.Y + y + newYOffset;
                var srcVal    = matrix[x, y];

                if (predicate(newItems[targetIdx], srcVal))
                    newItems[targetIdx] = srcVal;
            }

            _items     = newItems;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            XCapacity = XSize;
            YCapacity = YSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize) && resize)
                Expand(newSize.X, newSize.Y);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matrixX);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matrixY);

            for (var x = startX; x < endX; x++) {
                var targetColStart = x * YCapacity;
                var srcColIndex    = x - offset.X;

                for (var y = startY; y < endY; y++) {
                    var targetIdx = targetColStart + y;
                    var srcVal    = matrix[srcColIndex, y - offset.Y];

                    if (predicate(_items[targetIdx], srcVal))
                        _items[targetIdx] = srcVal;
                }
            }
        }
    }

    /// <summary>
    ///     Places the contents of the specified 2D list into the list, applying a predicate to
    ///     determine if elements should be overwritten.
    /// </summary>
    /// <param name="matrix">The source 2D list to place.</param>
    /// <param name="predicate">
    ///     A function receiving the existing element and new element,
    ///     returning true if the element should be replaced.
    /// </param>
    /// <param name="offsetPoint">
    ///     The 2D coordinate offset point defining where to place the
    ///     top-left corner of the matrix.
    /// </param>
    /// <param name="resize">
    ///     True to automatically grow the list sizes if matrix extends beyond
    ///     current bounds; otherwise, false.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="matrix" /> or
    ///     <paramref name="predicate" /> is null.
    /// </exception>
    public void Place(
        List2D<T>        matrix,
        Func<T, T, bool> predicate,
        Point2D?         offsetPoint = null,
        bool             resize      = true
    ) {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(predicate);

        var offset  = offsetPoint ?? Point2D.Zero;
        var matrixX = matrix.XSize;
        var matrixY = matrix.YSize;

        var placedMax = offset + new Point2D(matrixX, matrixY);

        var max = new Point2D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y));
        var min = new Point2D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0));

        var newSize = new Size2D(max.X - min.X, max.Y - min.Y);

        var isBelow = offset.X < 0 || offset.Y < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X * newSize.Y];

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);

            if (XSize > 0 && YSize > 0)
                for (var x = 0; x < XSize; x++)
                    _items.AsSpan(x * YCapacity, YSize)
                        .CopyTo(newItems.AsSpan((x + oldXOffset) * newSize.Y + oldYOffset, YSize));

            for (var x = 0; x < matrixX; x++) {
                var srcColStart    = x * matrix.YCapacity;
                var targetColStart = (x + newXOffset) * newSize.Y + newYOffset;

                for (var y = 0; y < matrixY; y++) {
                    var targetIdx = targetColStart + y;
                    var srcVal    = matrix._items[srcColStart + y];

                    if (predicate(newItems[targetIdx], srcVal))
                        newItems[targetIdx] = srcVal;
                }
            }

            _items     = newItems;
            XSize     = newSize.X;
            YSize     = newSize.Y;
            XCapacity = XSize;
            YCapacity = YSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize) && resize)
                Expand(newSize.X, newSize.Y);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX   = Math.Min(XSize, offset.X + matrixX);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY   = Math.Min(YSize, offset.Y + matrixY);

            for (var x = startX; x < endX; x++) {
                var targetColStart = x              * YCapacity;
                var srcColStart    = (x - offset.X) * matrix.YCapacity;
                var yOffset        = -offset.Y;

                for (var y = startY; y < endY; y++) {
                    var targetIdx = targetColStart + y;
                    var srcVal    = matrix._items[srcColStart + y + yOffset];

                    if (predicate(_items[targetIdx], srcVal))
                        _items[targetIdx] = srcVal;
                }
            }
        }
    }

    /// <summary>
    ///     Fills the entire list with the specified element.
    /// </summary>
    /// <param name="item">The value to populate the list with.</param>
    public void Fill(T item) {
        if (XSize == 0 || YSize == 0)
            return;

        if (YSize == YCapacity)
            _items.AsSpan(0, XSize * YCapacity).Fill(item);
        else
            for (var x = 0; x < XSize; x++)
                _items.AsSpan(x * YCapacity, YSize).Fill(item);
    }

    /// <summary>
    ///     Fills the specified rectangular sub-region with the given value.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="xStart">The starting X coordinate.</param>
    /// <param name="xCount">The number of columns to fill.</param>
    /// <param name="yStart">The starting Y coordinate.</param>
    /// <param name="yCount">The number of rows to fill.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when specified region is out of bounds.</exception>
    public void Fill(T item, int xStart, int xCount, int yStart, int yCount) {
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;

        if (xStart < 0 || yStart < 0 || xEnd > XSize || yEnd > YSize || xCount < 0 || yCount < 0)
            throw new IndexOutOfRangeException();

        if (xCount == 0 || yCount == 0)
            return;

        for (var x = xStart; x < xEnd; x++)
            _items.AsSpan(x * YCapacity + yStart, yCount).Fill(item);
    }

    /// <summary>
    ///     Fills the specified rectangular sub-region defined by offset point and size with the given value.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="offset">The starting Point2D coordinates.</param>
    /// <param name="size">The Size2D of the sub-region.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(T item, Point2D offset, Size2D size) => Fill(item, offset.X, size.X, offset.Y, size.Y);

    /// <summary>
    ///     Fills the entire list with values generated by the specified factory function.
    /// </summary>
    /// <param name="factory">The factory function to generate elements.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory" /> is null.</exception>
    public void Fill(Func<T> factory) {
        ArgumentNullException.ThrowIfNull(factory);

        for (var x = 0; x < XSize; x++) {
            var start = x * YCapacity;

            for (var y = 0; y < YSize; y++)
                _items[start + y] = factory();
        }
    }

    /// <summary>
    ///     Fills the specified rectangular sub-region with values generated by the specified factory function.
    /// </summary>
    /// <param name="factory">The factory function to generate elements.</param>
    /// <param name="xStart">The starting X coordinate.</param>
    /// <param name="xCount">The number of columns to fill.</param>
    /// <param name="yStart">The starting Y coordinate.</param>
    /// <param name="yCount">The number of rows to fill.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory" /> is null.</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown when specified region is out of bounds.</exception>
    public void Fill(Func<T> factory, int xStart, int xCount, int yStart, int yCount) {
        ArgumentNullException.ThrowIfNull(factory);

        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;

        if (xStart < 0 || yStart < 0 || xEnd > XSize || yEnd > YSize || xCount < 0 || yCount < 0)
            throw new IndexOutOfRangeException();

        if (xCount == 0 || yCount == 0)
            return;

        for (var x = xStart; x < xEnd; x++) {
            var start = x * YCapacity;

            for (var y = yStart; y < yEnd; y++)
                _items[start + y] = factory();
        }
    }

    /// <summary>
    ///     Fills the specified rectangular sub-region defined by offset point and size with values generated by the specified
    ///     factory function.
    /// </summary>
    /// <param name="factory">The factory function to generate elements.</param>
    /// <param name="offset">The starting Point2D coordinates.</param>
    /// <param name="size">The Size2D of the sub-region.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(Func<T> factory, Point2D offset, Size2D size) => Fill(factory, offset.X, size.X, offset.Y, size.Y);

    /// <summary>
    ///     Copies all active elements into a new generic two-dimensional array.
    /// </summary>
    /// <returns>A generic two-dimensional array containing copies of the elements.</returns>
    [Pure]
    public T[,] ToArray() {
        var arr = new T[XSize, YSize];

        if (XSize == 0 || YSize == 0)
            return arr;

        for (var x = 0; x < XSize; x++) {
            var srcSpan = _items.AsSpan(x * YCapacity, YSize);
            var dstSpan = MemoryMarshal.CreateSpan(ref arr[x, 0], YSize);
            srcSpan.CopyTo(dstSpan);
        }

        return arr;
    }

    /// <summary>
    ///     Copies all active elements into a new jagged array (array of arrays).
    /// </summary>
    /// <returns>A jagged array containing copies of the elements.</returns>
    [Pure]
    public T[][] ToJagged() {
        var arr = new T[XSize][];

        for (var x = 0; x < XSize; x++) {
            arr[x] = new T[YSize];
            _items.AsSpan(x * YCapacity, YSize).CopyTo(arr[x]);
        }

        return arr;
    }

    #region IEnumerable

    /// <summary>
    ///     Returns an enumerator that iterates through the columns of the 2D list.
    /// </summary>
    public IEnumerator<IEnumerable<T>> GetEnumerator() {
        for (var x = 0; x < XSize; x++)
            yield return GetAtX(x);
    }

    IEnumerator<IEnumerable<T>> IEnumerable<IEnumerable<T>>.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Retrieves the elements in the specified column (constant X).
    /// </summary>
    /// <param name="x">The zero-based column index.</param>
    /// <returns>An enumerable sequence of elements in the specified column.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="x" /> is out of bounds.</exception>
    public IEnumerable<T> GetAtX(int x) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException("Index 'x' is out of range.");

        return GetAtXIterator(x);
    }

    private IEnumerable<T> GetAtXIterator(int x) {
        for (var y = 0; y < YSize; y++)
            yield return _items[x * YCapacity + y];
    }

    /// <summary>
    ///     Retrieves the elements in the specified row (constant Y).
    /// </summary>
    /// <param name="y">The zero-based row index.</param>
    /// <returns>An enumerable sequence of elements in the specified row.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="y" /> is out of bounds.</exception>
    public IEnumerable<T> GetAtY(int y) {
        if (y < 0 || y >= YSize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");

        return GetAtYIterator(y);
    }

    private IEnumerable<T> GetAtYIterator(int y) {
        for (var x = 0; x < XSize; x++)
            yield return _items[x * YCapacity + y];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<IEnumerable<T>> IEnumerable2D<T>.GetEnumerator() => GetEnumerator();

    IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);

    IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);

    IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

    #endregion

}