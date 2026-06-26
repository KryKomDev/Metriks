using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
/// Represents a four-dimensional view of a contiguous or strided memory region.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
public readonly ref struct Span4D<T> {
    private readonly Span<T> _span;
    private readonly int     _wSize;
    private readonly int     _xSize;
    private readonly int     _ySize;
    private readonly int     _zSize;
    private readonly int     _strideW;
    private readonly int     _strideX;
    private readonly int     _strideY;

    /// <summary>
    /// Gets the size of the span along the W-axis.
    /// </summary>
    public int WSize => _wSize;

    /// <summary>
    /// Gets the size of the span along the X-axis.
    /// </summary>
    public int XSize => _xSize;

    /// <summary>
    /// Gets the size of the span along the Y-axis.
    /// </summary>
    public int YSize => _ySize;

    /// <summary>
    /// Gets the size of the span along the Z-axis.
    /// </summary>
    public int ZSize => _zSize;

    /// <summary>
    /// Gets the number of elements along the W-axis.
    /// </summary>
    public int WCount => _wSize;

    /// <summary>
    /// Gets the number of elements along the X-axis.
    /// </summary>
    public int XCount => _xSize;

    /// <summary>
    /// Gets the number of elements along the Y-axis.
    /// </summary>
    public int YCount => _ySize;

    /// <summary>
    /// Gets the number of elements along the Z-axis.
    /// </summary>
    public int ZCount => _zSize;

    /// <summary>
    /// Gets the size of the 4D span.
    /// </summary>
    public Size4D Size => new(_wSize, _xSize, _ySize, _zSize);

    /// <summary>
    /// Gets the total number of elements in the span.
    /// </summary>
    public int Length => _wSize * _xSize * _ySize * _zSize;

    /// <summary>
    /// Gets the total number of elements in the span.
    /// </summary>
    public int Count => _wSize * _xSize * _ySize * _zSize;

    /// <summary>
    /// Gets a value indicating whether the span is empty.
    /// </summary>
    public bool IsEmpty => _wSize == 0 || _xSize == 0 || _ySize == 0 || _zSize == 0;

    /// <summary>
    /// Gets the stride along the W-axis.
    /// </summary>
    public int StrideW => _strideW;

    /// <summary>
    /// Gets the stride along the X-axis.
    /// </summary>
    public int StrideX => _strideX;

    /// <summary>
    /// Gets the stride along the Y-axis.
    /// </summary>
    public int StrideY => _strideY;

    /// <summary>
    /// Initializes a new instance of the <see cref="Span4D{T}"/> struct wrapping a four-dimensional array.
    /// </summary>
    /// <param name="array">The four-dimensional array to wrap.</param>
    public Span4D(T[,,,] array) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        _wSize   = array.GetLength(0);
        _xSize   = array.GetLength(1);
        _ySize   = array.GetLength(2);
        _zSize   = array.GetLength(3);
        _strideW = _xSize * _ySize * _zSize;
        _strideX = _ySize * _zSize;
        _strideY = _zSize;

        _span = _wSize == 0 || _xSize == 0 || _ySize == 0 || _zSize == 0
            ? Span<T>.Empty
            : MemoryMarshal.CreateSpan(ref array[0, 0, 0, 0], array.Length);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Span4D{T}"/> struct wrapping a flat span with given dimensions.
    /// </summary>
    /// <param name="span">The contiguous memory span.</param>
    /// <param name="wSize">The size along the W-axis.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="zSize">The size along the Z-axis.</param>
    public Span4D(Span<T> span, int wSize, int xSize, int ySize, int zSize) {
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

        _span    = span;
        _wSize   = wSize;
        _xSize   = xSize;
        _ySize   = ySize;
        _zSize   = zSize;
        _strideW = xSize * ySize * zSize;
        _strideX = ySize * zSize;
        _strideY = zSize;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Span4D{T}"/> struct wrapping a strided memory region.
    /// </summary>
    /// <param name="span">The memory span starting at the minimum index element.</param>
    /// <param name="wSize">The size along the W-axis.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="zSize">The size along the Z-axis.</param>
    /// <param name="strideW">The W-axis stride.</param>
    /// <param name="strideX">The X-axis stride.</param>
    /// <param name="strideY">The Y-axis stride.</param>
    public Span4D(Span<T> span, int wSize, int xSize, int ySize, int zSize, int strideW, int strideX, int strideY) {
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

        int requiredLength = wSize == 0 || xSize == 0 || ySize == 0 || zSize == 0
            ? 0
            : (wSize - 1) * strideW + (xSize - 1) * strideX + (ySize - 1) * strideY + zSize;

        if (requiredLength > span.Length)
            throw new ArgumentException("Span length is less than the specified dimensions and strides.");

        _span    = span;
        _wSize   = wSize;
        _xSize   = xSize;
        _ySize   = ySize;
        _zSize   = zSize;
        _strideW = strideW;
        _strideX = strideX;
        _strideY = strideY;
    }

    /// <summary>
    /// Gets a reference to the element at the specified 4D indices.
    /// </summary>
    public ref T this[int w, int x, int y, int z] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if ((uint)w >= (uint)_wSize || (uint)x >= (uint)_xSize || (uint)y >= (uint)_ySize || (uint)z >= (uint)_zSize) {
                throw new IndexOutOfRangeException();
            }

            return ref _span[w * _strideW + x * _strideX + y * _strideY + z];
        }
    }

    /// <summary>
    /// Gets a reference to the element at the specified point.
    /// </summary>
    public ref T this[Point4D point] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this[point.W, point.X, point.Y, point.Z];
    }

    /// <summary>
    /// Slices the four-dimensional span.
    /// </summary>
    public Span4D<T> Slice(int w, int x, int y, int z, int wSize, int xSize, int ySize, int zSize) {
        if (w < 0 || x < 0 || y < 0 || z < 0 || wSize < 0 || xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        if (w + wSize > _wSize || x + xSize > _xSize || y + ySize > _ySize || z + zSize > _zSize)
            throw new ArgumentOutOfRangeException();

        if (wSize == 0 || xSize == 0 || ySize == 0 || zSize == 0) {
            return default;
        }

        int offset = w           * _strideW + x           * _strideX + y           * _strideY + z;
        int length = (wSize - 1) * _strideW + (xSize - 1) * _strideX + (ySize - 1) * _strideY + zSize;

        return new Span4D<T>(_span.Slice(offset, length), wSize, xSize, ySize, zSize, _strideW, _strideX, _strideY);
    }

    /// <summary>
    /// Slices the four-dimensional span.
    /// </summary>
    public Span4D<T> Slice(Point4D offset, Size4D size) => Slice(offset.W, offset.X, offset.Y, offset.Z, size.W, size.X, size.Y, size.Z);

    /// <summary>
    /// Slices the four-dimensional span.
    /// </summary>
    public Span4D<T> Slice(Area4D area) => Slice(area.Lower, area.Size + Size4D.One);

    /// <summary>
    /// Gets a 3D span representing the cube at the specified W index.
    /// </summary>
    public Span3D<T> GetCubeAtW(int w) {
        if ((uint)w >= (uint)_wSize)
            throw new ArgumentOutOfRangeException(nameof(w));

        int offset = w * _strideW;

        int length = _xSize == 0 || _ySize == 0 || _zSize == 0
            ? 0
            : (_xSize - 1) * _strideX + (_ySize - 1) * _strideY + _zSize;

        return new Span3D<T>(_span.Slice(offset, length), _xSize, _ySize, _zSize, _strideX, _strideY);
    }

    /// <summary>
    /// Copies the contents of this span to a destination <see cref="Span4D{T}"/>.
    /// </summary>
    public void CopyTo(Span4D<T> destination) {
        if (_wSize != destination._wSize ||
            _xSize != destination._xSize ||
            _ySize != destination._ySize ||
            _zSize != destination._zSize) 
        {
            throw new ArgumentException("Destination span must have the same dimensions.");
        }

        for (int i = 0; i < _wSize; i++) {
            GetCubeAtW(i).CopyTo(destination.GetCubeAtW(i));
        }
    }

    /// <summary>
    /// Attempts to copy the contents of this span to a destination <see cref="Span4D{T}"/>.
    /// </summary>
    public bool TryCopyTo(Span4D<T> destination) {
        if (_wSize != destination._wSize ||
            _xSize != destination._xSize ||
            _ySize != destination._ySize ||
            _zSize != destination._zSize)
        {
            return false;
        }

        for (int i = 0; i < _wSize; i++) {
            if (!GetCubeAtW(i).TryCopyTo(destination.GetCubeAtW(i))) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Fills the span with the specified value.
    /// </summary>
    public void Fill(T value) {
        for (int i = 0; i < _wSize; i++) {
            GetCubeAtW(i).Fill(value);
        }
    }

    /// <summary>
    /// Clears the span.
    /// </summary>
    public void Clear() {
        for (int i = 0; i < _wSize; i++) {
            GetCubeAtW(i).Clear();
        }
    }

    /// <summary>
    /// Implicitly converts a four-dimensional array to a <see cref="Span4D{T}"/>.
    /// </summary>
    public static implicit operator Span4D<T>(T[,,,] array) => new(array);

    /// <summary>
    /// Implicitly converts a <see cref="Span4D{T}"/> to a <see cref="ReadOnlySpan4D{T}"/>.
    /// </summary>
    public static implicit operator ReadOnlySpan4D<T>(Span4D<T> span) =>
        new(
            span._span,
            span._wSize,
            span._xSize,
            span._ySize,
            span._zSize,
            span._strideW,
            span._strideX,
            span._strideY
        );

    /// <summary>
    /// Returns an enumerator for this span.
    /// </summary>
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Enumerates elements of a <see cref="Span4D{T}"/>.
    /// </summary>
    public ref struct Enumerator {
        private readonly Span4D<T> _span;
        private          int       _w;
        private          int       _x;
        private          int       _y;
        private          int       _z;

        internal Enumerator(Span4D<T> span) {
            _span = span;
            _w    = 0;
            _x    = 0;
            _y    = 0;
            _z    = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element.
        /// </summary>
        public bool MoveNext() {
            _z++;

            if (_z < _span.ZSize)
                return _w < _span.WSize;

            _z = 0;
            _y++;

            if (_y < _span.YSize)
                return _w < _span.WSize;

            _y = 0;
            _x++;

            if (_x < _span.XSize)
                return _w < _span.WSize;

            _x = 0;
            _w++;

            return _w < _span.WSize;
        }

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        public ref T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _span[_w, _x, _y, _z];
        }
    }
}