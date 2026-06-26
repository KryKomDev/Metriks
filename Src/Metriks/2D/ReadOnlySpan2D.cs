using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
/// Represents a read-only two-dimensional view of a contiguous or strided memory region.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
public readonly ref struct ReadOnlySpan2D<T> {
    private readonly ReadOnlySpan<T> _span;
    private readonly int _xSize;
    private readonly int _ySize;
    private readonly int _stride;

    /// <summary>
    /// Gets the size of the span along the X-axis.
    /// </summary>
    public int XSize => _xSize;

    /// <summary>
    /// Gets the size of the span along the Y-axis.
    /// </summary>
    public int YSize => _ySize;
    
    /// <summary>
    /// Gets the number of elements along the X-axis.
    /// </summary>
    public int XCount => _xSize;

    /// <summary>
    /// Gets the number of elements along the Y-axis.
    /// </summary>
    public int YCount => _ySize;
    
    /// <summary>
    /// Gets the size of the 2D span.
    /// </summary>
    public Size2D Size => new(_xSize, _ySize);
    
    /// <summary>
    /// Gets the total number of elements in the span.
    /// </summary>
    public int Length => _xSize * _ySize;

    /// <summary>
    /// Gets the total number of elements in the span.
    /// </summary>
    public int Count => _xSize * _ySize;
    
    /// <summary>
    /// Gets a value indicating whether the span is empty.
    /// </summary>
    public bool IsEmpty => _xSize == 0 || _ySize == 0;
    
    /// <summary>
    /// Gets the stride (the distance in elements between the start of consecutive rows).
    /// </summary>
    public int Stride => _stride;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan2D{T}"/> struct wrapping a two-dimensional array.
    /// </summary>
    /// <param name="array">The two-dimensional array to wrap.</param>
    public ReadOnlySpan2D(T[,] array) {
        if (array is null) throw new ArgumentNullException(nameof(array));
        _xSize = array.GetLength(0);
        _ySize = array.GetLength(1);
        _stride = _ySize;
        _span = _xSize == 0 || _ySize == 0 
            ? ReadOnlySpan<T>.Empty 
            : MemoryMarshal.CreateReadOnlySpan(ref array[0, 0], array.Length);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan2D{T}"/> struct wrapping a flat read-only span with given dimensions.
    /// </summary>
    /// <param name="span">The contiguous memory span.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    public ReadOnlySpan2D(ReadOnlySpan<T> span, int xSize, int ySize) {
        if (xSize < 0) throw new ArgumentOutOfRangeException(nameof(xSize));
        if (ySize < 0) throw new ArgumentOutOfRangeException(nameof(ySize));
        if (xSize * ySize > span.Length) throw new ArgumentException("Span length is less than the specified dimensions.");
        
        _span = span;
        _xSize = xSize;
        _ySize = ySize;
        _stride = ySize;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan2D{T}"/> struct wrapping a strided memory region.
    /// </summary>
    /// <param name="span">The memory span starting at the top-left element.</param>
    /// <param name="xSize">The size along the X-axis.</param>
    /// <param name="ySize">The size along the Y-axis.</param>
    /// <param name="stride">The row-to-row stride.</param>
    public ReadOnlySpan2D(ReadOnlySpan<T> span, int xSize, int ySize, int stride) {
        if (xSize < 0) throw new ArgumentOutOfRangeException(nameof(xSize));
        if (ySize < 0) throw new ArgumentOutOfRangeException(nameof(ySize));
        if (stride < ySize) throw new ArgumentOutOfRangeException(nameof(stride));
        
        int requiredLength = xSize == 0 || ySize == 0 ? 0 : (xSize - 1) * stride + ySize;
        if (requiredLength > span.Length) throw new ArgumentException("Span length is less than the specified dimensions and stride.");

        _span = span;
        _xSize = xSize;
        _ySize = ySize;
        _stride = stride;
    }

    /// <summary>
    /// Gets a read-only reference to the element at the specified 2D indices.
    /// </summary>
    /// <param name="x">The X-axis index.</param>
    /// <param name="y">The Y-axis index.</param>
    public ref readonly T this[int x, int y] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if ((uint)x >= (uint)_xSize || (uint)y >= (uint)_ySize) {
                throw new IndexOutOfRangeException();
            }
            return ref _span[x * _stride + y];
        }
    }

    /// <summary>
    /// Gets a read-only reference to the element at the specified point.
    /// </summary>
    /// <param name="point">The coordinate point.</param>
    public ref readonly T this[Point2D point] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this[point.X, point.Y];
    }

    /// <summary>
    /// Slices the two-dimensional read-only span.
    /// </summary>
    /// <param name="x">The starting X index.</param>
    /// <param name="y">The starting Y index.</param>
    /// <param name="xSize">The size along the X-axis of the slice.</param>
    /// <param name="ySize">The size along the Y-axis of the slice.</param>
    /// <returns>A new <see cref="ReadOnlySpan2D{T}"/> representing the sliced region.</returns>
    public ReadOnlySpan2D<T> Slice(int x, int y, int xSize, int ySize) {
        if (x < 0 || y < 0 || xSize < 0 || ySize < 0) 
            throw new ArgumentOutOfRangeException();
        if (x + xSize > _xSize || y + ySize > _ySize) 
            throw new ArgumentOutOfRangeException();
            
        if (xSize == 0 || ySize == 0) {
            return default;
        }
        
        int offset = x * _stride + y;
        int length = (xSize - 1) * _stride + ySize;
        return new ReadOnlySpan2D<T>(_span.Slice(offset, length), xSize, ySize, _stride);
    }

    /// <summary>
    /// Slices the two-dimensional read-only span.
    /// </summary>
    /// <param name="offset">The starting offset point.</param>
    /// <param name="size">The size of the slice.</param>
    /// <returns>A new <see cref="ReadOnlySpan2D{T}"/> representing the sliced region.</returns>
    public ReadOnlySpan2D<T> Slice(Point2D offset, Size2D size) => Slice(offset.X, offset.Y, size.X, size.Y);

    /// <summary>
    /// Slices the two-dimensional read-only span.
    /// </summary>
    /// <param name="area">The area of the slice.</param>
    /// <returns>A new <see cref="ReadOnlySpan2D{T}"/> representing the sliced region.</returns>
    public ReadOnlySpan2D<T> Slice(Area2D area) => Slice(area.Lower, area.Size + Size2D.One);

    /// <summary>
    /// Gets a contiguous read-only span representing the row at the specified X index.
    /// </summary>
    /// <param name="x">The row index.</param>
    /// <returns>A read-only span containing the elements of the row.</returns>
    public ReadOnlySpan<T> GetRow(int x) {
        if ((uint)x >= (uint)_xSize) throw new ArgumentOutOfRangeException(nameof(x));
        return _span.Slice(x * _stride, _ySize);
    }

    /// <summary>
    /// Copies the contents of this span to a destination <see cref="Span2D{T}"/>.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    public void CopyTo(Span2D<T> destination) {
        if (_xSize != destination.XSize || _ySize != destination.YSize) {
            throw new ArgumentException("Destination span must have the same dimensions.");
        }
        
        for (int i = 0; i < _xSize; i++) {
            GetRow(i).CopyTo(destination.GetRow(i));
        }
    }

    /// <summary>
    /// Attempts to copy the contents of this span to a destination <see cref="Span2D{T}"/>.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    /// <returns>true if the copy succeeded; otherwise, false.</returns>
    public bool TryCopyTo(Span2D<T> destination) {
        if (_xSize != destination.XSize || _ySize != destination.YSize) {
            return false;
        }
        
        for (int i = 0; i < _xSize; i++) {
            if (!GetRow(i).TryCopyTo(destination.GetRow(i))) {
                return false;
            }
        }
        
        return true;
    }

    /// <summary>
    /// Implicitly converts a two-dimensional array to a <see cref="ReadOnlySpan2D{T}"/>.
    /// </summary>
    public static implicit operator ReadOnlySpan2D<T>(T[,] array) => new(array);

    /// <summary>
    /// Returns an enumerator for this span.
    /// </summary>
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Enumerates elements of a <see cref="ReadOnlySpan2D{T}"/>.
    /// </summary>
    public ref struct Enumerator {
        private readonly ReadOnlySpan2D<T> _span;
        private int _x;
        private int _y;

        internal Enumerator(ReadOnlySpan2D<T> span) {
            _span = span;
            _x = 0;
            _y = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element.
        /// </summary>
        public bool MoveNext() {
            _y++;
            if (_y >= _span.YSize) {
                _y = 0;
                _x++;
            }
            return _x < _span.XSize;
        }

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        public ref readonly T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _span[_x, _y];
        }
    }
}
