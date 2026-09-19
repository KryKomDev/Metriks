using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
///     Represents a read-only three-dimensional view of a contiguous or strided memory region.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
public readonly ref struct ReadOnlySpan3D<T> {
    private readonly ReadOnlySpan<T> _span;

    /// <summary>
    ///     Gets the size of the span along the X-axis.
    /// </summary>
    public int XSize { get; }

    /// <summary>
    ///     Gets the size of the span along the Y-axis.
    /// </summary>
    public int YSize { get; }

    /// <summary>
    ///     Gets the size of the span along the Z-axis.
    /// </summary>
    public int ZSize { get; }

    /// <summary>
    ///     Gets the number of elements along the X-axis.
    /// </summary>
    public int XCount => XSize;

    /// <summary>
    ///     Gets the number of elements along the Y-axis.
    /// </summary>
    public int YCount => YSize;

    /// <summary>
    ///     Gets the number of elements along the Z-axis.
    /// </summary>
    public int ZCount => ZSize;

    /// <summary>
    ///     Gets the size of the 3D span.
    /// </summary>
    public Size3D Size => new(XSize, YSize, ZSize);

    /// <summary>
    ///     Gets the total number of elements in the span.
    /// </summary>
    public int Length => XSize * YSize * ZSize;

    /// <summary>
    ///     Gets the total number of elements in the span.
    /// </summary>
    public int Count => XSize * YSize * ZSize;

    /// <summary>
    ///     Gets a value indicating whether the span is empty.
    /// </summary>
    public bool IsEmpty => XSize == 0 || YSize == 0 || ZSize == 0;

    /// <summary>
    ///     Gets the stride along the X-axis.
    /// </summary>
    public int StrideX { get; }

    /// <summary>
    ///     Gets the stride along the Y-axis.
    /// </summary>
    public int StrideY { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReadOnlySpan3D{T}" /> struct wrapping a three-dimensional array.
    /// </summary>
    /// <param name="array">The three-dimensional array to wrap.</param>
    public ReadOnlySpan3D(T[,,] array) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        XSize   = array.GetLength(0);
        YSize   = array.GetLength(1);
        ZSize   = array.GetLength(2);
        StrideX = YSize * ZSize;
        StrideY = ZSize;

        _span = XSize == 0 || YSize == 0 || ZSize == 0
            ? ReadOnlySpan<T>.Empty
            : MemoryMarshal.CreateReadOnlySpan(ref array[0, 0, 0], array.Length);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReadOnlySpan3D{T}" /> struct wrapping a flat span with given
    ///     dimensions.
    /// </summary>
    /// <param name="span">The contiguous memory span.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="zSize">The size along the Z-axis.</param>
    public ReadOnlySpan3D(ReadOnlySpan<T> span, int xSize, int ySize, int zSize) {
        if (xSize < 0)
            throw new ArgumentOutOfRangeException(nameof(xSize));

        if (ySize < 0)
            throw new ArgumentOutOfRangeException(nameof(ySize));

        if (zSize < 0)
            throw new ArgumentOutOfRangeException(nameof(zSize));

        if (xSize * ySize * zSize > span.Length)
            throw new ArgumentException("Span length is less than the specified dimensions.");

        _span   = span;
        XSize   = xSize;
        YSize   = ySize;
        ZSize   = zSize;
        StrideX = ySize * zSize;
        StrideY = zSize;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReadOnlySpan3D{T}" /> struct wrapping a strided memory region.
    /// </summary>
    /// <param name="span">The memory span starting at the minimum index element.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="zSize">The size along the Z-axis.</param>
    /// <param name="strideX">The X-axis stride.</param>
    /// <param name="strideY">The Y-axis stride.</param>
    public ReadOnlySpan3D(ReadOnlySpan<T> span, int xSize, int ySize, int zSize, int strideX, int strideY) {
        if (xSize < 0)
            throw new ArgumentOutOfRangeException(nameof(xSize));

        if (ySize < 0)
            throw new ArgumentOutOfRangeException(nameof(ySize));

        if (zSize < 0)
            throw new ArgumentOutOfRangeException(nameof(zSize));

        if (strideY < zSize)
            throw new ArgumentOutOfRangeException(nameof(strideY));

        if (strideX < ySize * strideY)
            throw new ArgumentOutOfRangeException(nameof(strideX));

        var requiredLength = xSize == 0 || ySize == 0 || zSize == 0 ? 0 : (xSize - 1) * strideX + (ySize - 1) * strideY + zSize;

        if (requiredLength > span.Length)
            throw new ArgumentException("Span length is less than the specified dimensions and strides.");

        _span   = span;
        XSize   = xSize;
        YSize   = ySize;
        ZSize   = zSize;
        StrideX = strideX;
        StrideY = strideY;
    }

    /// <summary>
    ///     Gets a read-only reference to the element at the specified 3D indices.
    /// </summary>
    public ref readonly T this[int x, int y, int z] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if ((uint)x >= (uint)XSize || (uint)y >= (uint)YSize || (uint)z >= (uint)ZSize)
                throw new IndexOutOfRangeException();

            return ref _span[x * StrideX + y * StrideY + z];
        }
    }

    /// <summary>
    ///     Gets a read-only reference to the element at the specified point.
    /// </summary>
    public ref readonly T this[Point3D point] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this[point.X, point.Y, point.Z];
    }

    /// <summary>
    ///     Slices the three-dimensional read-only span.
    /// </summary>
    public ReadOnlySpan3D<T> Slice(int x, int y, int z, int xSize, int ySize, int zSize) {
        if (x < 0 || y < 0 || z < 0 || xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        if (x + xSize > XSize || y + ySize > YSize || z + zSize > ZSize)
            throw new ArgumentOutOfRangeException();

        if (xSize == 0 || ySize == 0 || zSize == 0)
            return default;

        var offset = x           * StrideX + y           * StrideY + z;
        var length = (xSize - 1) * StrideX + (ySize - 1) * StrideY + zSize;

        return new ReadOnlySpan3D<T>(_span.Slice(offset, length), xSize, ySize, zSize, StrideX, StrideY);
    }

    /// <summary>
    ///     Slices the three-dimensional read-only span.
    /// </summary>
    public ReadOnlySpan3D<T> Slice(Point3D offset, Size3D size) => Slice(offset.X, offset.Y, offset.Z, size.X, size.Y, size.Z);

    /// <summary>
    ///     Slices the three-dimensional read-only span.
    /// </summary>
    public ReadOnlySpan3D<T> Slice(Rect3D area) => Slice(area.Lower, area.Size + Size3D.One);

    /// <summary>
    ///     Gets a 2D read-only span representing the plane at the specified X index.
    /// </summary>
    public ReadOnlySpan2D<T> GetPlaneAtX(int x) {
        if ((uint)x >= (uint)XSize)
            throw new ArgumentOutOfRangeException(nameof(x));

        var offset = x * StrideX;
        var length = YSize == 0 || ZSize == 0 ? 0 : (YSize - 1) * StrideY + ZSize;

        return new ReadOnlySpan2D<T>(_span.Slice(offset, length), YSize, ZSize, StrideY);
    }

    /// <summary>
    ///     Copies the contents of this span to a destination <see cref="Span3D{T}" />.
    /// </summary>
    public void CopyTo(Span3D<T> destination) {
        if (XSize != destination.XSize || YSize != destination.YSize || ZSize != destination.ZSize)
            throw new ArgumentException("Destination span must have the same dimensions.");

        for (var i = 0; i < XSize; i++)
            GetPlaneAtX(i).CopyTo(destination.GetPlaneAtX(i));
    }

    /// <summary>
    ///     Attempts to copy the contents of this span to a destination <see cref="Span3D{T}" />.
    /// </summary>
    public bool TryCopyTo(Span3D<T> destination) {
        if (XSize != destination.XSize || YSize != destination.YSize || ZSize != destination.ZSize)
            return false;

        for (var i = 0; i < XSize; i++)
            if (!GetPlaneAtX(i).TryCopyTo(destination.GetPlaneAtX(i)))
                return false;

        return true;
    }

    /// <summary>
    ///     Implicitly converts a three-dimensional array to a <see cref="ReadOnlySpan3D{T}" />.
    /// </summary>
    public static implicit operator ReadOnlySpan3D<T>(T[,,] array) => new(array);

    /// <summary>
    ///     Returns an enumerator for this span.
    /// </summary>
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    ///     Enumerates elements of a <see cref="ReadOnlySpan3D{T}" />.
    /// </summary>
    public ref struct Enumerator {
        private readonly ReadOnlySpan3D<T> _span;
        private          int               _x;
        private          int               _y;
        private          int               _z;

        internal Enumerator(ReadOnlySpan3D<T> span) {
            _span = span;
            _x    = 0;
            _y    = 0;
            _z    = -1;
        }

        /// <summary>
        ///     Advances the enumerator to the next element.
        /// </summary>
        public bool MoveNext() {
            _z++;

            if (_z >= _span.ZSize) {
                _z = 0;
                _y++;

                if (_y >= _span.YSize) {
                    _y = 0;
                    _x++;
                }
            }

            return _x < _span.XSize;
        }

        /// <summary>
        ///     Gets the element at the current position of the enumerator.
        /// </summary>
        public ref readonly T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _span[_x, _y, _z];
        }
    }
}