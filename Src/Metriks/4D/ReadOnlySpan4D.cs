using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
///     Represents a read-only four-dimensional view of a contiguous or strided memory region.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
public readonly ref struct ReadOnlySpan4D<T> {
    private readonly ReadOnlySpan<T> _span;

    /// <summary>
    ///     Gets the size of the span along the W-axis.
    /// </summary>
    public int WSize { get; }

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
    ///     Gets the number of elements along the W-axis.
    /// </summary>
    public int WCount => WSize;

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
    ///     Gets the size of the 4D span.
    /// </summary>
    public Size4D Size => new(WSize, XSize, YSize, ZSize);

    /// <summary>
    ///     Gets the total number of elements in the span.
    /// </summary>
    public int Length => WSize * XSize * YSize * ZSize;

    /// <summary>
    ///     Gets the total number of elements in the span.
    /// </summary>
    public int Count => WSize * XSize * YSize * ZSize;

    /// <summary>
    ///     Gets a value indicating whether the span is empty.
    /// </summary>
    public bool IsEmpty => WSize == 0 || XSize == 0 || YSize == 0 || ZSize == 0;

    /// <summary>
    ///     Gets the stride along the W-axis.
    /// </summary>
    public int StrideW { get; }

    /// <summary>
    ///     Gets the stride along the X-axis.
    /// </summary>
    public int StrideX { get; }

    /// <summary>
    ///     Gets the stride along the Y-axis.
    /// </summary>
    public int StrideY { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReadOnlySpan4D{T}" /> struct wrapping a four-dimensional array.
    /// </summary>
    /// <param name="array">The four-dimensional array to wrap.</param>
    public ReadOnlySpan4D(T[,,,] array) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        WSize   = array.GetLength(0);
        XSize   = array.GetLength(1);
        YSize   = array.GetLength(2);
        ZSize   = array.GetLength(3);
        StrideW = XSize * YSize * ZSize;
        StrideX = YSize * ZSize;
        StrideY = ZSize;

        _span = WSize == 0 || XSize == 0 || YSize == 0 || ZSize == 0
            ? ReadOnlySpan<T>.Empty
            : MemoryMarshal.CreateReadOnlySpan(ref array[0, 0, 0, 0], array.Length);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReadOnlySpan4D{T}" /> struct wrapping a flat span with given
    ///     dimensions.
    /// </summary>
    /// <param name="span">The contiguous memory span.</param>
    /// <param name="wSize">The size along the W-axis.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="zSize">The size along the Z-axis.</param>
    public ReadOnlySpan4D(ReadOnlySpan<T> span, int wSize, int xSize, int ySize, int zSize) {
        if (wSize < 0)
            throw new ArgumentOutOfRangeException(nameof(wSize));

        if (xSize < 0)
            throw new ArgumentOutOfRangeException(nameof(xSize));

        if (ySize < 0)
            throw new ArgumentOutOfRangeException(nameof(ySize));

        if (zSize < 0)
            throw new ArgumentOutOfRangeException(nameof(zSize));

        if (wSize * xSize * ySize * zSize > span.Length)
            throw new ArgumentException("Span length is less than the specified dimensions.");

        _span   = span;
        WSize   = wSize;
        XSize   = xSize;
        YSize   = ySize;
        ZSize   = zSize;
        StrideW = xSize * ySize * zSize;
        StrideX = ySize * zSize;
        StrideY = zSize;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReadOnlySpan4D{T}" /> struct wrapping a strided memory region.
    /// </summary>
    /// <param name="span">The memory span starting at the minimum index element.</param>
    /// <param name="wSize">The size along the W-axis.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="zSize">The size along the Z-axis.</param>
    /// <param name="strideW">The W-axis stride.</param>
    /// <param name="strideX">The X-axis stride.</param>
    /// <param name="strideY">The Y-axis stride.</param>
    public ReadOnlySpan4D(ReadOnlySpan<T> span, int wSize, int xSize, int ySize, int zSize, int strideW, int strideX, int strideY) {
        if (wSize < 0)
            throw new ArgumentOutOfRangeException(nameof(wSize));

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

        if (strideW < xSize * strideX)
            throw new ArgumentOutOfRangeException(nameof(strideW));

        var requiredLength = wSize == 0 || xSize == 0 || ySize == 0 || zSize == 0
            ? 0
            : (wSize - 1) * strideW + (xSize - 1) * strideX + (ySize - 1) * strideY + zSize;

        if (requiredLength > span.Length)
            throw new ArgumentException("Span length is less than the specified dimensions and strides.");

        _span   = span;
        WSize   = wSize;
        XSize   = xSize;
        YSize   = ySize;
        ZSize   = zSize;
        StrideW = strideW;
        StrideX = strideX;
        StrideY = strideY;
    }

    /// <summary>
    ///     Gets a read-only reference to the element at the specified 4D indices.
    /// </summary>
    public ref readonly T this[int w, int x, int y, int z] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if ((uint)w >= (uint)WSize || (uint)x >= (uint)XSize || (uint)y >= (uint)YSize || (uint)z >= (uint)ZSize)
                throw new IndexOutOfRangeException();

            return ref _span[w * StrideW + x * StrideX + y * StrideY + z];
        }
    }

    /// <summary>
    ///     Gets a read-only reference to the element at the specified point.
    /// </summary>
    public ref readonly T this[Point4D point] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this[point.W, point.X, point.Y, point.Z];
    }

    /// <summary>
    ///     Slices the four-dimensional read-only span.
    /// </summary>
    public ReadOnlySpan4D<T> Slice(int w, int x, int y, int z, int wSize, int xSize, int ySize, int zSize) {
        if (w < 0 || x < 0 || y < 0 || z < 0 || wSize < 0 || xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        if (w + wSize > WSize || x + xSize > XSize || y + ySize > YSize || z + zSize > ZSize)
            throw new ArgumentOutOfRangeException();

        if (wSize == 0 || xSize == 0 || ySize == 0 || zSize == 0)
            return default;

        var offset = w           * StrideW + x           * StrideX + y           * StrideY + z;
        var length = (wSize - 1) * StrideW + (xSize - 1) * StrideX + (ySize - 1) * StrideY + zSize;

        return new ReadOnlySpan4D<T>(_span.Slice(offset, length), wSize, xSize, ySize, zSize, StrideW, StrideX, StrideY);
    }

    /// <summary>
    ///     Slices the four-dimensional read-only span.
    /// </summary>
    public ReadOnlySpan4D<T> Slice(Point4D offset, Size4D size) => Slice(offset.W, offset.X, offset.Y, offset.Z, size.W, size.X, size.Y, size.Z);

    /// <summary>
    ///     Slices the four-dimensional read-only span.
    /// </summary>
    public ReadOnlySpan4D<T> Slice(Rect4D area) => Slice(area.Lower, area.Size + Size4D.One);

    /// <summary>
    ///     Gets a 3D read-only span representing the cube at the specified W index.
    /// </summary>
    public ReadOnlySpan3D<T> GetCubeAtW(int w) {
        if ((uint)w >= (uint)WSize)
            throw new ArgumentOutOfRangeException(nameof(w));

        var offset = w * StrideW;

        var length = XSize == 0 || YSize == 0 || ZSize == 0
            ? 0
            : (XSize - 1) * StrideX + (YSize - 1) * StrideY + ZSize;

        return new ReadOnlySpan3D<T>(_span.Slice(offset, length), XSize, YSize, ZSize, StrideX, StrideY);
    }

    /// <summary>
    ///     Copies the contents of this span to a destination <see cref="Span4D{T}" />.
    /// </summary>
    public void CopyTo(Span4D<T> destination) {
        if (WSize != destination.WSize || XSize != destination.XSize || YSize != destination.YSize || ZSize != destination.ZSize)
            throw new ArgumentException("Destination span must have the same dimensions.");

        for (var i = 0; i < WSize; i++)
            GetCubeAtW(i).CopyTo(destination.GetCubeAtW(i));
    }

    /// <summary>
    ///     Attempts to copy the contents of this span to a destination <see cref="Span4D{T}" />.
    /// </summary>
    public bool TryCopyTo(Span4D<T> destination) {
        if (WSize != destination.WSize || XSize != destination.XSize || YSize != destination.YSize || ZSize != destination.ZSize)
            return false;

        for (var i = 0; i < WSize; i++)
            if (!GetCubeAtW(i).TryCopyTo(destination.GetCubeAtW(i)))
                return false;

        return true;
    }

    /// <summary>
    ///     Implicitly converts a four-dimensional array to a <see cref="ReadOnlySpan4D{T}" />.
    /// </summary>
    public static implicit operator ReadOnlySpan4D<T>(T[,,,] array) => new(array);

    /// <summary>
    ///     Returns an enumerator for this span.
    /// </summary>
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    ///     Enumerates elements of a <see cref="ReadOnlySpan4D{T}" />.
    /// </summary>
    public ref struct Enumerator {
        private readonly ReadOnlySpan4D<T> _span;
        private          int               _w;
        private          int               _x;
        private          int               _y;
        private          int               _z;

        internal Enumerator(ReadOnlySpan4D<T> span) {
            _span = span;
            _w    = 0;
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

                    if (_x >= _span.XSize) {
                        _x = 0;
                        _w++;
                    }
                }
            }

            return _w < _span.WSize;
        }

        /// <summary>
        ///     Gets the element at the current position of the enumerator.
        /// </summary>
        public ref readonly T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _span[_w, _x, _y, _z];
        }
    }
}