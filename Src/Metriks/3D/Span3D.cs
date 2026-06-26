using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
/// Represents a three-dimensional view of a contiguous or strided memory region.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
public readonly ref struct Span3D<T> {
    private readonly Span<T> _span;
    private readonly int _xSize;
    private readonly int _ySize;
    private readonly int _zSize;
    private readonly int _strideX;
    private readonly int _strideY;

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
    /// Gets the size of the 3D span.
    /// </summary>
    public Size3D Size => new(_xSize, _ySize, _zSize);
    
    /// <summary>
    /// Gets the total number of elements in the span.
    /// </summary>
    public int Length => _xSize * _ySize * _zSize;

    /// <summary>
    /// Gets the total number of elements in the span.
    /// </summary>
    public int Count => _xSize * _ySize * _zSize;
    
    /// <summary>
    /// Gets a value indicating whether the span is empty.
    /// </summary>
    public bool IsEmpty => _xSize == 0 || _ySize == 0 || _zSize == 0;
    
    /// <summary>
    /// Gets the stride along the X-axis.
    /// </summary>
    public int StrideX => _strideX;

    /// <summary>
    /// Gets the stride along the Y-axis.
    /// </summary>
    public int StrideY => _strideY;

    /// <summary>
    /// Initializes a new instance of the <see cref="Span3D{T}"/> struct wrapping a three-dimensional array.
    /// </summary>
    /// <param name="array">The three-dimensional array to wrap.</param>
    public Span3D(T[,,] array) {
        if (array is null) throw new ArgumentNullException(nameof(array));
        _xSize = array.GetLength(0);
        _ySize = array.GetLength(1);
        _zSize = array.GetLength(2);
        _strideX = _ySize * _zSize;
        _strideY = _zSize;
        _span = _xSize == 0 || _ySize == 0 || _zSize == 0
            ? Span<T>.Empty
            : MemoryMarshal.CreateSpan(ref array[0, 0, 0], array.Length);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Span3D{T}"/> struct wrapping a flat span with given dimensions.
    /// </summary>
    /// <param name="span">The contiguous memory span.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="zSize">The size along the Z-axis.</param>
    public Span3D(Span<T> span, int xSize, int ySize, int zSize) {
        if (xSize < 0) throw new ArgumentOutOfRangeException(nameof(xSize));
        if (ySize < 0) throw new ArgumentOutOfRangeException(nameof(ySize));
        if (zSize < 0) throw new ArgumentOutOfRangeException(nameof(zSize));
        if (xSize * ySize * zSize > span.Length) throw new ArgumentException("Span length is less than the specified dimensions.");
        
        _span = span;
        _xSize = xSize;
        _ySize = ySize;
        _zSize = zSize;
        _strideX = ySize * zSize;
        _strideY = zSize;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Span3D{T}"/> struct wrapping a strided memory region.
    /// </summary>
    /// <param name="span">The memory span starting at the minimum index element.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="zSize">The size along the Z-axis.</param>
    /// <param name="strideX">The X-axis stride.</param>
    /// <param name="strideY">The Y-axis stride.</param>
    public Span3D(Span<T> span, int xSize, int ySize, int zSize, int strideX, int strideY) {
        if (xSize < 0) throw new ArgumentOutOfRangeException(nameof(xSize));
        if (ySize < 0) throw new ArgumentOutOfRangeException(nameof(ySize));
        if (zSize < 0) throw new ArgumentOutOfRangeException(nameof(zSize));
        if (strideY < zSize) throw new ArgumentOutOfRangeException(nameof(strideY));
        if (strideX < ySize * strideY) throw new ArgumentOutOfRangeException(nameof(strideX));
        
        int requiredLength = xSize == 0 || ySize == 0 || zSize == 0 ? 0 : (xSize - 1) * strideX + (ySize - 1) * strideY + zSize;
        if (requiredLength > span.Length) throw new ArgumentException("Span length is less than the specified dimensions and strides.");

        _span = span;
        _xSize = xSize;
        _ySize = ySize;
        _zSize = zSize;
        _strideX = strideX;
        _strideY = strideY;
    }

    /// <summary>
    /// Gets a reference to the element at the specified 3D indices.
    /// </summary>
    public ref T this[int x, int y, int z] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if ((uint)x >= (uint)_xSize || (uint)y >= (uint)_ySize || (uint)z >= (uint)_zSize) {
                throw new IndexOutOfRangeException();
            }
            return ref _span[x * _strideX + y * _strideY + z];
        }
    }

    /// <summary>
    /// Gets a reference to the element at the specified point.
    /// </summary>
    public ref T this[Point3D point] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this[point.X, point.Y, point.Z];
    }

    /// <summary>
    /// Slices the three-dimensional span.
    /// </summary>
    public Span3D<T> Slice(int x, int y, int z, int xSize, int ySize, int zSize) {
        if (x < 0 || y < 0 || z < 0 || xSize < 0 || ySize < 0 || zSize < 0) 
            throw new ArgumentOutOfRangeException();
        if (x + xSize > _xSize || y + ySize > _ySize || z + zSize > _zSize) 
            throw new ArgumentOutOfRangeException();
            
        if (xSize == 0 || ySize == 0 || zSize == 0) {
            return default;
        }
        
        int offset = x * _strideX + y * _strideY + z;
        int length = (xSize - 1) * _strideX + (ySize - 1) * _strideY + zSize;
        return new Span3D<T>(_span.Slice(offset, length), xSize, ySize, zSize, _strideX, _strideY);
    }

    /// <summary>
    /// Slices the three-dimensional span.
    /// </summary>
    public Span3D<T> Slice(Point3D offset, Size3D size) => Slice(offset.X, offset.Y, offset.Z, size.X, size.Y, size.Z);

    /// <summary>
    /// Slices the three-dimensional span.
    /// </summary>
    public Span3D<T> Slice(Area3D area) => Slice(area.Lower, area.Size + Size3D.One);

    /// <summary>
    /// Gets a 2D span representing the plane at the specified X index.
    /// </summary>
    public Span2D<T> GetPlaneAtX(int x) {
        if ((uint)x >= (uint)_xSize) throw new ArgumentOutOfRangeException(nameof(x));
        int offset = x * _strideX;
        int length = _ySize == 0 || _zSize == 0 ? 0 : (_ySize - 1) * _strideY + _zSize;
        return new Span2D<T>(_span.Slice(offset, length), _ySize, _zSize, _strideY);
    }

    /// <summary>
    /// Copies the contents of this span to a destination <see cref="Span3D{T}"/>.
    /// </summary>
    public void CopyTo(Span3D<T> destination) {
        if (_xSize != destination._xSize || _ySize != destination._ySize || _zSize != destination._zSize) {
            throw new ArgumentException("Destination span must have the same dimensions.");
        }
        
        for (int i = 0; i < _xSize; i++) {
            GetPlaneAtX(i).CopyTo(destination.GetPlaneAtX(i));
        }
    }

    /// <summary>
    /// Attempts to copy the contents of this span to a destination <see cref="Span3D{T}"/>.
    /// </summary>
    public bool TryCopyTo(Span3D<T> destination) {
        if (_xSize != destination._xSize || _ySize != destination._ySize || _zSize != destination._zSize) {
            return false;
        }
        
        for (int i = 0; i < _xSize; i++) {
            if (!GetPlaneAtX(i).TryCopyTo(destination.GetPlaneAtX(i))) {
                return false;
            }
        }
        
        return true;
    }

    /// <summary>
    /// Fills the span with the specified value.
    /// </summary>
    public void Fill(T value) {
        for (int i = 0; i < _xSize; i++) {
            GetPlaneAtX(i).Fill(value);
        }
    }

    /// <summary>
    /// Clears the span.
    /// </summary>
    public void Clear() {
        for (int i = 0; i < _xSize; i++) {
            GetPlaneAtX(i).Clear();
        }
    }

    /// <summary>
    /// Implicitly converts a three-dimensional array to a <see cref="Span3D{T}"/>.
    /// </summary>
    public static implicit operator Span3D<T>(T[,,] array) => new(array);

    /// <summary>
    /// Implicitly converts a <see cref="Span3D{T}"/> to a <see cref="ReadOnlySpan3D{T}"/>.
    /// </summary>
    public static implicit operator ReadOnlySpan3D<T>(Span3D<T> span) => 
        new(span._span, span._xSize, span._ySize, span._zSize, span._strideX, span._strideY);

    /// <summary>
    /// Returns an enumerator for this span.
    /// </summary>
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Enumerates elements of a <see cref="Span3D{T}"/>.
    /// </summary>
    public ref struct Enumerator {
        private readonly Span3D<T> _span;
        private int _x;
        private int _y;
        private int _z;

        internal Enumerator(Span3D<T> span) {
            _span = span;
            _x = 0;
            _y = 0;
            _z = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element.
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
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        public ref T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _span[_x, _y, _z];
        }
    }
}
