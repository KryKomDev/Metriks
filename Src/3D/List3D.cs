// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Collections;
using System.Runtime.CompilerServices;

namespace Metriks;

/// <summary>
/// Represents a strongly typed, three-dimensional list of elements that can be accessed by X, Y, and Z indices.
/// Provides methods to search, sort, and manipulate 3D lists.
/// </summary>
/// <typeparam name="T">The type of elements in the three-dimensional list.</typeparam>
public class List3D<T> : IList3D<T>, ICollection3D, IReadOnlyList3D<T> {
    private const int INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR = 2f;

    private T[][][] _items;

    /// <summary>
    /// Gets the underlying three-dimensional array used to store the elements of the <see cref="List3D{T}"/> instance.
    /// PROVIDED FOR INTERNAL USE ONLY. DO NOT USE. <b>!!!DO NOT MODIFY THE ARRAY IN ANY WAY!!!</b>
    /// </summary>
    internal T[][][] Items {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _items;
    }

    protected void UnsafeSet(int x, int y, int z, T value) => _items[x][y][z] = value;
    protected T UnsafeGet(int x, int y, int z) => _items[x][y][z];

    private int _xSize;
    private int _ySize;
    private int _zSize;
    private int _xCapacity;
    private int _yCapacity;
    private int _zCapacity;

    /// <summary>
    /// Initializes a new instance of the <see cref="List3D{T}"/> class with the specified initial capacity for each dimension.
    /// </summary>
    /// <param name="xCapacity">The initial capacity along the X-axis.</param>
    /// <param name="yCapacity">The initial capacity along the Y-axis.</param>
    /// <param name="zCapacity">The initial capacity along the Z-axis.</param>
    public List3D(
        int xCapacity = INITIAL_CAPACITY,
        int yCapacity = INITIAL_CAPACITY,
        int zCapacity = INITIAL_CAPACITY) {
        _items = new T[xCapacity][][];
        _xSize = 0;
        _ySize = 0;
        _zSize = 0;
        _xCapacity = xCapacity;
        _yCapacity = yCapacity;
        _zCapacity = zCapacity;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="List3D{T}"/> class populated with elements from the specified 3D array.
    /// </summary>
    /// <param name="collection">The 3D array of elements to copy from.</param>
    public List3D(T[,,] collection) : this(
        collection.GetLength(0),
        collection.GetLength(1),
        collection.GetLength(2)) {
        var len0 = collection.GetLength(0);
        var len1 = collection.GetLength(1);
        var len2 = collection.GetLength(2);

        for (int x = 0; x < len0; x++) {
            _items[x] = new T[len1][];
        }

        if (len1 > 0 && len2 > 0) {
            for (int x = 0; x < len0; x++) {
                var rowX = _items[x];
                for (int y = 0; y < len1; y++) {
                    rowX[y] = new T[len2];
                    var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref collection[x, y, 0], len2);
                    var dstSpan = new Span<T>(rowX[y]);
                    srcSpan.CopyTo(dstSpan);
                }
            }
        }
        else if (len1 > 0) {
            for (int x = 0; x < len0; x++) {
                var rowX = _items[x];
                for (int y = 0; y < len1; y++) {
                    rowX[y] = Array.Empty<T>();
                }
            }
        }

        _xSize = len0;
        _ySize = len1;
        _zSize = len2;
    }

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
    /// Gets a <see cref="Size3D"/> representing the current size of the list in all three dimensions.
    /// </summary>
    public Size3D Size => new(_xSize, _ySize, _zSize);

    /// <summary>
    /// Gets the total number of elements contained in the <see cref="List3D{T}"/>.
    /// </summary>
    public int Count => _xSize * _ySize * _zSize;

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
    /// Gets a value indicating whether the <see cref="List3D{T}"/> is read-only.
    /// </summary>
    public bool IsReadOnly => false;

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

    public T this[int x, int y, int z] {
        get {
            if (x < 0 || x >= _xSize || y < 0 || y >= _ySize || z < 0 || z >= _zSize) 
                throw new IndexOutOfRangeException();

            return _items[x][y][z];
        }
        set {
            if (x < 0 || x >= _xSize || y < 0 || y >= _ySize || z < 0 || z >= _zSize)
                throw new IndexOutOfRangeException();

            _items[x][y][z] = value;
        }
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    public T this[Index x, Index y, Index z] {
        get => this[x.GetOffset(_xSize), y.GetOffset(_ySize), z.GetOffset(_zSize)];
        set => this[x.GetOffset(_xSize), y.GetOffset(_ySize), z.GetOffset(_zSize)] = value;
    }

    public T[] this[Range x, int y, int z] {
        get {
            var (offset, length) = x.GetOffsetAndLength(_xSize);
            var result = new T[length];
            for (int i = 0; i < length; i++) result[i] = _items[offset + i][y][z];

            return result;
        }
    }

    public T[] this[int x, Range y, int z] {
        get {
            var (offset, length) = y.GetOffsetAndLength(_ySize);
            var result = new T[length];
            for (int i = 0; i < length; i++) result[i] = _items[x][offset + i][z];

            return result;
        }
    }

    public T[] this[int x, int y, Range z] {
        get {
            var (offset, length) = z.GetOffsetAndLength(_zSize);
            var result = new T[length];
            for (int i = 0; i < length; i++) result[i] = _items[x][y][offset + i];

            return result;
        }
    }
    #endif

    public void InsertAtX(int x) {
        if (x < 0 || x > _xSize) throw new IndexOutOfRangeException();

        if (_xSize + 1 >= _xCapacity) _xCapacity = (int)(_xCapacity * GROWTH_FACTOR);
        var newItems = new T[_xCapacity][][];
        Array.Copy(_items, newItems, x);
        newItems[x] = new T[_yCapacity][];
        for (int i = 0; i < _yCapacity; i++) newItems[x][i] = new T[_zCapacity];
        Array.Copy(_items, x, newItems, x + 1, _xSize - x);
        _xSize++;
        _items = newItems;
    }

    public void InsertAtY(int y) {
        if (y < 0 || y > _ySize) throw new IndexOutOfRangeException();

        if (_ySize + 1 >= _yCapacity) _yCapacity = (int)(_yCapacity * GROWTH_FACTOR);

        for (int x = 0; x < _xSize; x++) {
            var newArray = new T[_yCapacity][];
            Array.Copy(_items[x], newArray, y);
            newArray[y] = new T[_zCapacity];
            Array.Copy(_items[x], y, newArray, y + 1, _ySize - y);
            _items[x] = newArray;
        }

        _ySize++;
    }

    public void InsertAtZ(int z) {
        if (z < 0 || z > _zSize) throw new IndexOutOfRangeException();

        if (_zSize + 1 >= _zCapacity) _zCapacity = (int)(_zCapacity * GROWTH_FACTOR);

        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                var newArray = new T[_zCapacity];
                Array.Copy(_items[x][y], newArray, z);
                Array.Copy(_items[x][y], z, newArray, z + 1, _zSize - z);
                _items[x][y] = newArray;
            }
        }

        _zSize++;
    }

    public void AddX() => InsertAtX(_xSize);
    public void AddY() => InsertAtY(_ySize);
    public void AddZ() => InsertAtZ(_zSize);

    /// <summary>
    /// Expands the dimensions of the 3D list to the specified size, filling new elements with a default value.
    /// </summary>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="zSize">The new size along the Z-axis.</param>
    /// <param name="defaultValue">The value to fill the new elements with.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any new size is smaller than the current size.</exception>
    public void Expand(int xSize, int ySize, int zSize, T? defaultValue = default!) {
        if (xSize < _xSize || ySize < _ySize || zSize < _zSize) throw new ArgumentOutOfRangeException();

        if (xSize == _xSize && ySize == _ySize && zSize == _zSize) return;

        if (xSize > _xCapacity) {
            _xCapacity = xSize + 1;
            var newItems = new T[_xCapacity][][];
            Array.Copy(_items, newItems, _xSize);
            _items = newItems;
        }

        if (ySize > _yCapacity) {
            _yCapacity = ySize + 1;

            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_yCapacity][];
                Array.Copy(_items[x], newArray, _ySize);
                for (int i = _ySize; i < _yCapacity; i++) newArray[i] = new T[_zCapacity];
                _items[x] = newArray;
            }
        }

        if (zSize > _zCapacity) {
            _zCapacity = zSize + 1;

            for (int x = 0; x < _xSize; x++) {
                var rowX = _items[x];
                for (int y = 0; y < _ySize; y++) {
                    var newArray = new T[_zCapacity];
                    Array.Copy(rowX[y], newArray, _zSize);
                    if (defaultValue is not null) Array.Fill(newArray, defaultValue, _zSize, zSize - _zSize);
                    rowX[y] = newArray;
                }
            }
        }

        for (int x = _xSize; x < xSize; x++) {
            var rowX = new T[_yCapacity][];
            _items[x] = rowX;

            for (int y = 0; y < ySize; y++) {
                var cellXY = new T[_zCapacity];
                rowX[y] = cellXY;
                if (defaultValue is not null) Array.Fill(cellXY, defaultValue, 0, zSize);
            }
        }

        for (int x = 0; x < _xSize; x++) {
            var rowX = _items[x];
            for (int y = _ySize; y < ySize; y++) {
                var cellXY = new T[_zCapacity];
                rowX[y] = cellXY;
                if (defaultValue is not null) Array.Fill(cellXY, defaultValue, 0, zSize);
            }

            if (defaultValue is null) continue;

            for (int y = 0; y < _ySize; y++) Array.Fill(rowX[y], defaultValue, _zSize, zSize - _zSize);
        }

        _xSize = xSize;
        _ySize = ySize;
        _zSize = zSize;
    }

    /// <summary>
    /// Expands the dimensions of the 3D list to the specified size, generating new elements using the specified factory function.
    /// </summary>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="zSize">The new size along the Z-axis.</param>
    /// <param name="defaultValueFactory">A factory function that generates values for the new elements.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any new size is smaller than the current size.</exception>
    public void Expand(int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        if (xSize < _xSize || ySize < _ySize || zSize < _zSize) throw new ArgumentOutOfRangeException();
        if (xSize == _xSize && ySize == _ySize && zSize == _zSize) return;

        if (xSize > _xCapacity) {
            _xCapacity = xSize + 1;
            var newItems = new T[_xCapacity][][];
            Array.Copy(_items, newItems, _xSize);
            _items = newItems;
        }

        if (ySize > _yCapacity) {
            _yCapacity = ySize + 1;
            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_yCapacity][];
                Array.Copy(_items[x], newArray, _ySize);
                for (int y = _ySize; y < _yCapacity; y++) {
                    newArray[y] = new T[_zCapacity];
                }
                _items[x] = newArray;
            }
        }

        if (zSize > _zCapacity) {
            _zCapacity = zSize + 1;
            for (int x = 0; x < _xSize; x++) {
                var rowX = _items[x];
                for (int y = 0; y < _ySize; y++) {
                    var newArray = new T[_zCapacity];
                    Array.Copy(rowX[y], newArray, _zSize);
                    rowX[y] = newArray;
                }
            }
        }

        // New X
        for (int x = _xSize; x < xSize; x++) {
            var rowX = new T[_yCapacity][];
            _items[x] = rowX;
            for (int y = 0; y < ySize; y++) {
                var cellXY = new T[_zCapacity];
                rowX[y] = cellXY;
                for (int z = 0; z < zSize; z++) cellXY[z] = defaultValueFactory();
            }
        }

        // Existing X, new Y
        for (int x = 0; x < _xSize; x++) {
            var rowX = _items[x];
            for (int y = _ySize; y < ySize; y++) {
                var cellXY = new T[_zCapacity];
                rowX[y] = cellXY;
                for (int z = 0; z < zSize; z++) cellXY[z] = defaultValueFactory();
            }
            // Existing X, existing Y, new Z
            for (int y = 0; y < _ySize; y++) {
                var cellXY = rowX[y];
                for (int z = _zSize; z < zSize; z++) {
                    cellXY[z] = defaultValueFactory();
                }
            }
        }

        _xSize = xSize; _ySize = ySize; _zSize = zSize;
    }

    public void Resize(int xSize, int ySize, int zSize, T? defaultValue = default) {
        if (xSize < 0 || ySize < 0 || zSize < 0) throw new ArgumentOutOfRangeException();
        var newItems = new T[xSize][][];
        for (int x = 0; x < xSize; x++) {
            newItems[x] = new T[ySize][];
            for (int y = 0; y < ySize; y++) {
                newItems[x][y] = new T[zSize];
                if (defaultValue is not null) Array.Fill(newItems[x][y], defaultValue);
            }
        }
        int minX = Math.Min(_xSize, xSize);
        int minY = Math.Min(_ySize, ySize);
        int minZ = Math.Min(_zSize, zSize);
        for (int x = 0; x < minX; x++) {
            for (int y = 0; y < minY; y++) Array.Copy(_items[x][y], newItems[x][y], minZ);
        }
        _items = newItems;
        _xSize = xSize; _ySize = ySize; _zSize = zSize;
        _xCapacity = xSize; _yCapacity = ySize; _zCapacity = zSize;
    }

    public void Resize(int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        if (xSize < 0 || ySize < 0 || zSize < 0) throw new ArgumentOutOfRangeException();
        var newItems = new T[xSize][][];
        
        for (int x = 0; x < xSize; x++) {
            newItems[x] = new T[ySize][];
            for (int y = 0; y < ySize; y++) {
                newItems[x][y] = new T[zSize];
                for (int z = 0; z < zSize; z++) {
                    newItems[x][y][z] = defaultValueFactory();
                }
            }
        }

        int minX = Math.Min(_xSize, xSize);
        int minY = Math.Min(_ySize, ySize);
        int minZ = Math.Min(_zSize, zSize);

        for (int x = 0; x < minX; x++) {
            for (int y = 0; y < minY; y++) {
                Array.Copy(_items[x][y], newItems[x][y], minZ);
            }
        }
        
        _items = newItems;
        _xSize = xSize; _ySize = ySize; _zSize = zSize;
        _xCapacity = xSize; _yCapacity = ySize; _zCapacity = zSize;
    }

    public void Shrink(int xSize, int ySize, int zSize) {
        if (xSize > _xSize || ySize > _ySize || zSize > _zSize) throw new ArgumentOutOfRangeException();

        Resize(xSize, ySize, zSize);
    }

    public void RemoveAtX(int x) {
        if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException();

        _xSize--;
        var newItems = new T[_xSize][][];
        Array.Copy(_items, newItems, x);
        Array.Copy(_items, x + 1, newItems, x, _xSize - x);
        _items = newItems;
    }

    public void RemoveAtY(int y) {
        if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException();

        _ySize--;

        for (int x = 0; x < _xSize; x++) {
            Array.Copy(_items[x], y + 1, _items[x], y, _ySize - y);
            _items[x][_ySize] = null!;
        }
    }

    public void RemoveAtZ(int z) {
        if (z < 0 || z >= _zSize) throw new IndexOutOfRangeException();

        _zSize--;

        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                Array.Copy(_items[x][y], z + 1, _items[x][y], z, _zSize - z);
                _items[x][y][_zSize] = default!;
            }
        }
    }

    public void ShrinkX() {
        if (_xSize > 0) _xSize--;
    }

    public void ShrinkY() {
        if (_ySize > 0) _ySize--;
    }

    public void ShrinkZ() {
        if (_zSize > 0) _zSize--;
    }

    /// <summary>
    /// Determines whether the 3D list contains a specific value.
    /// </summary>
    /// <param name="value">The value to locate in the 3D list.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool Contains(T value) {
        for (int x = 0; x < _xSize; x++) {
            var rowX = _items[x];
            for (int y = 0; y < _ySize; y++) {
                if (Array.IndexOf(rowX[y], value, 0, _zSize) >= 0)
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

        var rowX = _items[x];
        for (int y = 0; y < _ySize; y++) {
            if (Array.IndexOf(rowX[y], value, 0, _zSize) >= 0)
                return true;
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

        for (int x = 0; x < _xSize; x++) {
            if (Array.IndexOf(_items[x][y], value, 0, _zSize) >= 0)
                return true;
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

        for (int x = 0; x < _xSize; x++) {
            var rowX = _items[x];
            for (int y = 0; y < _ySize; y++) {
                if (EqualityComparer<T>.Default.Equals(rowX[y][z], value))
                    return true;
            }
        }

        return false;
    }

    public void Clear() {
        _xSize = 0;
        _ySize = 0;
        _zSize = 0;
        _items = new T[INITIAL_CAPACITY][][];
    }

    /// <summary>
    /// Places a 3D matrix into this List3D at the specified offset. If the matrix extends beyond
    /// the current bounds of the List3D, the List3D is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 3D array of elements to place into this List3D.</param>
    /// <param name="offsetPoint">
    /// An optional offset defining where the top-left corner of the matrix will be placed.
    /// If not provided, the matrix will be placed at the origin of the List3D.
    /// </param>
    /// <param name="resize">If true, enables automatic resizing of this list depending on
    /// the size and offset of the placed array.</param>
    public void Place(T[,,] matrix, Point3D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(_xSize, placedMax.X), Math.Max(_ySize, placedMax.Y), Math.Max(_zSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0), Math.Min(offset.Y, 0), Math.Min(offset.Z, 0));
        
        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X][][];
            
            for (int x = 0; x < newSize.X; x++) {
                newItems[x] = new T[newSize.Y][];
                for (int y = 0; y < newSize.Y; y++) newItems[x][y] = new T[newSize.Z];
            }

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (int x = 0; x < _xSize; x++) {
                var srcRowX = _items[x];
                var dstRowX = newItems[x + oldXOffset];
                for (int y = 0; y < _ySize; y++) {
                    Array.Copy(srcRowX[y], 0, dstRowX[y + oldYOffset], oldZOffset, _zSize);
                }
            }

            // Copy new
            var len2 = matrix.GetLength(2);
            if (len2 > 0) {
                var len0 = matrix.GetLength(0);
                var len1 = matrix.GetLength(1);
                for (int x = 0; x < len0; x++) {
                    var dstRowX = newItems[x + newXOffset];
                    for (int y = 0; y < len1; y++) {
                        var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref matrix[x, y, 0], len2);
                        var dstSpan = new Span<T>(dstRowX[y + newYOffset], newZOffset, len2);
                        srcSpan.CopyTo(dstSpan);
                    }
                }
            }

            _items = newItems;
            _xSize = newSize.X;
            _ySize = newSize.Y;
            _zSize = newSize.Z;
            _xCapacity = _xSize;
            _yCapacity = _ySize;
            _zCapacity = _zSize;
        }
        else {
            if ((placedMax.X > _xSize || placedMax.Y > _ySize || placedMax.Z > _zSize) && resize) {
                Expand(newSize.X, newSize.Y, newSize.Z);
            }

            var startX = Math.Clamp(offset.X, 0, _xSize);
            var endX = Math.Min(_xSize, offset.X + matrix.GetLength(0));
            var startY = Math.Clamp(offset.Y, 0, _ySize);
            var endY = Math.Min(_ySize, offset.Y + matrix.GetLength(1));
            var startZ = Math.Clamp(offset.Z, 0, _zSize);
            var endZ = Math.Min(_zSize, offset.Z + matrix.GetLength(2));
            var zCount = endZ - startZ;

            if (zCount > 0) {
                for (int x = startX; x < endX; x++) {
                    var rowX = _items[x];
                    var srcX = x - offset.X;
                    for (int y = startY; y < endY; y++) {
                        var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref matrix[srcX, y - offset.Y, startZ - offset.Z], zCount);
                        var dstSpan = new Span<T>(rowX[y], startZ, zCount);
                        srcSpan.CopyTo(dstSpan);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places the contents of the specified 3D list into the current List3D instance, optionally offset by a specified point.
    /// </summary>
    /// <param name="matrix">The List3D instance containing the elements to be placed.</param>
    /// <param name="offsetPoint">
    /// An optional point specifying the offset at which the matrix should be placed.
    /// If null, the matrix will be placed starting at the origin (0, 0, 0).
    /// </param>
    /// <param name="resize">If true, enables automatic resizing of this list depending on
    /// the size and offset of the placed array.</param>
    public void Place(List3D<T> matrix, Point3D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(_xSize, placedMax.X), Math.Max(_ySize, placedMax.Y), Math.Max(_zSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0), Math.Min(offset.Y, 0), Math.Min(offset.Z, 0));
        
        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X][][];
            
            for (int x = 0; x < newSize.X; x++) {
                newItems[x] = new T[newSize.Y][];
                for (int y = 0; y < newSize.Y; y++) newItems[x][y] = new T[newSize.Z];
            }

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (int x = 0; x < _xSize; x++) {
                var srcRowX = _items[x];
                var dstRowX = newItems[x + oldXOffset];
                for (int y = 0; y < _ySize; y++) {
                    Array.Copy(srcRowX[y], 0, dstRowX[y + oldYOffset], oldZOffset, _zSize);
                }
            }

            // Copy new
            for (int x = 0; x < matrix._xSize; x++) {
                var srcRowX = matrix._items[x];
                var dstRowX = newItems[x + newXOffset];
                for (int y = 0; y < matrix._ySize; y++) {
                    Array.Copy(srcRowX[y], 0, dstRowX[y + newYOffset], newZOffset, matrix._zSize);
                }
            }

            _items = newItems;
            _xSize = newSize.X;
            _ySize = newSize.Y;
            _zSize = newSize.Z;
            _xCapacity = _xSize;
            _yCapacity = _ySize;
            _zCapacity = _zSize;
        }
        else {
            if ((placedMax.X > _xSize || placedMax.Y > _ySize || placedMax.Z > _zSize) && resize) {
                Expand(newSize.X, newSize.Y, newSize.Z);
            }

            var startX = Math.Clamp(offset.X, 0, _xSize);
            var endX = Math.Min(_xSize, offset.X + matrix._xSize);
            var startY = Math.Clamp(offset.Y, 0, _ySize);
            var endY = Math.Min(_ySize, offset.Y + matrix._ySize);
            var startZ = Math.Clamp(offset.Z, 0, _zSize);
            var endZ = Math.Min(_zSize, offset.Z + matrix._zSize);
            var zCount = endZ - startZ;

            if (zCount > 0) {
                for (int x = startX; x < endX; x++) {
                    var dstRowX = _items[x];
                    var srcRowX = matrix._items[x - offset.X];
                    for (int y = startY; y < endY; y++) {
                        Array.Copy(srcRowX[y - offset.Y], startZ - offset.Z, dstRowX[y], startZ, zCount);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places a 3D matrix into this List3D at the specified offset. If the matrix extends beyond
    /// the current bounds of the List3D, the List3D is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 3D array of elements to place into this List3D.</param>
    /// <param name="predicate">A function determining whether the item should be placed into this array.
    /// The first argument is an item from this array that is being overwritten by the second one.</param>
    /// <param name="offsetPoint">
    /// An optional offset defining where the top-left corner of the matrix will be placed.
    /// If not provided, the matrix will be placed at the origin of the List3D.
    /// </param>
    /// <param name="resize">If true, enables automatic resizing of this list depending on
    /// the size and offset of the placed array.</param>
    public void Place(T[,,] matrix, Func<T, T, bool> predicate, Point3D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(_xSize, placedMax.X), Math.Max(_ySize, placedMax.Y), Math.Max(_zSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0), Math.Min(offset.Y, 0), Math.Min(offset.Z, 0));
        
        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X][][];
            
            for (int x = 0; x < newSize.X; x++) {
                newItems[x] = new T[newSize.Y][];
                for (int y = 0; y < newSize.Y; y++) newItems[x][y] = new T[newSize.Z];
            }

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (int x = 0; x < _xSize; x++) {
                var srcRowX = _items[x];
                var dstRowX = newItems[x + oldXOffset];
                for (int y = 0; y < _ySize; y++) {
                    Array.Copy(srcRowX[y], 0, dstRowX[y + oldYOffset], oldZOffset, _zSize);
                }
            }

            // Copy new
            var len0 = matrix.GetLength(0);
            var len1 = matrix.GetLength(1);
            var len2 = matrix.GetLength(2);
            for (int x = 0; x < len0; x++) {
                var dstRowX = newItems[x + newXOffset];
                for (int y = 0; y < len1; y++) {
                    var dstRowXY = dstRowX[y + newYOffset];
                    for (int z = 0; z < len2; z++) {
                        var val = matrix[x, y, z];
                        if (predicate(dstRowXY[z + newZOffset], val)) {
                            dstRowXY[z + newZOffset] = val;
                        }
                    }
                }
            }

            _items = newItems;
            _xSize = newSize.X;
            _ySize = newSize.Y;
            _zSize = newSize.Z;
            _xCapacity = _xSize;
            _yCapacity = _ySize;
            _zCapacity = _zSize;
        }
        else {
            if ((placedMax.X > _xSize || placedMax.Y > _ySize || placedMax.Z > _zSize) && resize) {
                Expand(newSize.X, newSize.Y, newSize.Z);
            }

            var startX = Math.Clamp(offset.X, 0, _xSize);
            var endX = Math.Min(_xSize, offset.X + matrix.GetLength(0));
            var startY = Math.Clamp(offset.Y, 0, _ySize);
            var endY = Math.Min(_ySize, offset.Y + matrix.GetLength(1));
            var startZ = Math.Clamp(offset.Z, 0, _zSize);
            var endZ = Math.Min(_zSize, offset.Z + matrix.GetLength(2));

            for (int x = startX; x < endX; x++) {
                var rowX = _items[x];
                var srcX = x - offset.X;
                for (int y = startY; y < endY; y++) {
                    var rowXY = rowX[y];
                    var srcY = y - offset.Y;
                    for (int z = startZ; z < endZ; z++) {
                        var val = matrix[srcX, srcY, z - offset.Z];
                        if (predicate(rowXY[z], val)) {
                            rowXY[z] = val;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places the contents of the specified 3D list into the current List3D instance, optionally offset by a specified point.
    /// </summary>
    /// <param name="matrix">The List3D instance containing the elements to be placed.</param>
    /// <param name="predicate">A function determining whether the item should be placed into this array.
    /// The first argument is an item from this array that is being overwritten by the second one.</param>
    /// <param name="offsetPoint">
    /// An optional point specifying the offset at which the matrix should be placed.
    /// If null, the matrix will be placed starting at the origin (0, 0, 0).
    /// </param>
    /// <param name="resize">If true, enables automatic resizing of this list depending on
    /// the size and offset of the placed array.</param>
    public void Place(List3D<T> matrix, Func<T, T, bool> predicate, Point3D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(_xSize, placedMax.X), Math.Max(_ySize, placedMax.Y), Math.Max(_zSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0), Math.Min(offset.Y, 0), Math.Min(offset.Z, 0));
        
        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X][][];
            
            for (int x = 0; x < newSize.X; x++) {
                newItems[x] = new T[newSize.Y][];
                for (int y = 0; y < newSize.Y; y++) newItems[x][y] = new T[newSize.Z];
            }

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (int x = 0; x < _xSize; x++) {
                var srcRowX = _items[x];
                var dstRowX = newItems[x + oldXOffset];
                for (int y = 0; y < _ySize; y++) {
                    Array.Copy(srcRowX[y], 0, dstRowX[y + oldYOffset], oldZOffset, _zSize);
                }
            }

            // Copy new
            for (int x = 0; x < matrix._xSize; x++) {
                var srcRowX = matrix._items[x];
                var dstRowX = newItems[x + newXOffset];
                for (int y = 0; y < matrix._ySize; y++) {
                    var srcRowXY = srcRowX[y];
                    var dstRowXY = dstRowX[y + newYOffset];
                    for (int z = 0; z < matrix._zSize; z++) {
                        var val = srcRowXY[z];
                        if (predicate(dstRowXY[z + newZOffset], val)) {
                            dstRowXY[z + newZOffset] = val;
                        }
                    }
                }
            }

            _items = newItems;
            _xSize = newSize.X;
            _ySize = newSize.Y;
            _zSize = newSize.Z;
            _xCapacity = _xSize;
            _yCapacity = _ySize;
            _zCapacity = _zSize;
        }
        else {
            if ((placedMax.X > _xSize || placedMax.Y > _ySize || placedMax.Z > _zSize) && resize) {
                Expand(newSize.X, newSize.Y, newSize.Z);
            }

            var startX = Math.Clamp(offset.X, 0, _xSize);
            var endX = Math.Min(_xSize, offset.X + matrix._xSize);
            var startY = Math.Clamp(offset.Y, 0, _ySize);
            var endY = Math.Min(_ySize, offset.Y + matrix._ySize);
            var startZ = Math.Clamp(offset.Z, 0, _zSize);
            var endZ = Math.Min(_zSize, offset.Z + matrix._zSize);

            for (int x = startX; x < endX; x++) {
                var dstRowX = _items[x];
                var srcRowX = matrix._items[x - offset.X];
                for (int y = startY; y < endY; y++) {
                    var dstRowXY = dstRowX[y];
                    var srcRowXY = srcRowX[y - offset.Y];
                    for (int z = startZ; z < endZ; z++) {
                        var val = srcRowXY[z - offset.Z];
                        if (predicate(dstRowXY[z], val)) {
                            dstRowXY[z] = val;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fills the entire 3D list with the specified value.
    /// </summary>
    /// <param name="item">The value to fill the 3D list with.</param>
    public void Fill(T item) {
        for (int x = 0; x < _xSize; x++)
            for (int y = 0; y < _ySize; y++)
                Array.Fill(_items[x][y], item, 0, _zSize);
    }

    /// <summary>
    /// Fills the specified 3D region of the list with a given value.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="xStart">The starting index on the X-axis (inclusive).</param>
    /// <param name="xCount">The number of elements to be filled along the X-axis.</param>
    /// <param name="yStart">The starting index on the Y-axis (inclusive).</param>
    /// <param name="yCount">The number of elements to be filled along the Y-axis.</param>
    /// <param name="zStart">The starting index on the Z-axis (inclusive).</param>
    /// <param name="zCount">The number of elements to be filled along the Z-axis.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified region exceeds the bounds of the list or one of the count parameters is negative.
    /// </exception>
    public void Fill(T item, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        int xEnd = xStart + xCount;
        int yEnd = yStart + yCount;
        int zEnd = zStart + zCount;

        if (xStart < 0 || yStart < 0 || zStart < 0 || xEnd > _xSize || yEnd > _ySize || zEnd > _zSize || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        for (int x = xStart; x < xEnd; x++)
            for (int y = yStart; y < yEnd; y++)
                Array.Fill(_items[x][y], item, zStart, zCount);
    }

    /// <summary>
    /// Fills the 3D list with the specified value in the given region.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(T item, Point3D offset, Size3D size)
        => Fill(item, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>
    /// Fills the entire 3D list with the values generated by the specified factory function.
    /// </summary>
    /// <param name="factory">A function that generates values to fill the 3D list.</param>
    public void Fill(Func<T> factory) {
        for (int x = 0; x < _xSize; x++) {
            var rowX = _items[x];
            for (int y = 0; y < _ySize; y++) {
                var rowXY = rowX[y];
                for (int z = 0; z < _zSize; z++) {
                    rowXY[z] = factory();
                }
            }
        }
    }

    /// <summary>
    /// Fills the 3D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="xStart">Start x coordinate of the filled region.</param>
    /// <param name="xCount">The number of units in x-axis to be filled in the filled region.</param>
    /// <param name="yStart">Start y coordinate of the filled region.</param>
    /// <param name="yCount">The number of units in y-axis to be filled in the filled region.</param>
    /// <param name="zStart">Start z coordinate of the filled region.</param>
    /// <param name="zCount">The number of units in z-axis to be filled in the filled region.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified region exceeds the bounds of the list or one of the count parameters is negative.
    /// </exception>
    public void Fill(Func<T> factory, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        int xEnd = xStart + xCount;
        int yEnd = yStart + yCount;
        int zEnd = zStart + zCount;

        if (xStart < 0 || yStart < 0 || zStart < 0 || xEnd > _xSize || yEnd > _ySize || zEnd > _zSize || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        for (int x = xStart; x < xEnd; x++) {
            var rowX = _items[x];
            for (int y = yStart; y < yEnd; y++) {
                var rowXY = rowX[y];
                for (int z = zStart; z < zEnd; z++) {
                    rowXY[z] = factory();
                }
            }
        }
    }

    /// <summary>
    /// Fills the 3D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(Func<T> factory, Point3D offset, Size3D size)
        => Fill(factory, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>
    /// Copies the elements of the 3D list to a 3D array, starting at the specified destination index.
    /// </summary>
    /// <param name="array">The destination 3D array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    /// <exception cref="ArgumentException">Thrown if the destination array is not large enough.</exception>
    public void CopyTo(T[,,] array, Point3D index) {
        if (array.GetLength(0) < _xSize + index.X) throw new ArgumentException("Destination array is not large enough in x dimension.");
        if (array.GetLength(1) < _ySize + index.Y) throw new ArgumentException("Destination array is not large enough in y dimension.");
        if (array.GetLength(2) < _zSize + index.Z) throw new ArgumentException("Destination array is not large enough in z dimension.");
        if (_xSize == 0 || _ySize == 0 || _zSize == 0) return;

        for (int x = 0; x < _xSize; x++) {
            var rowX = _items[x];
            for (int y = 0; y < _ySize; y++) {
                var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref rowX[y][0], _zSize);
                var dstSpan = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref array[x + index.X, y + index.Y, index.Z], _zSize);
                srcSpan.CopyTo(dstSpan);
            }
        }
    }

    /// <summary>
    /// Copies the elements of the 3D list to a standard multidimensional array, starting at the specified destination index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    public void CopyTo(Array array, Point3D index) {
        for (int x = 0; x < _xSize; x++) {
            var rowX = _items[x];
            for (int y = 0; y < _ySize; y++) {
                var rowXY = rowX[y];
                for (int z = 0; z < _zSize; z++) {
                    array.SetValue(rowXY[z], x + index.X, y + index.Y, z + index.Z);
                }
            }
        }
    }

    public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
        for (int x = 0; x < _xSize; x++) yield return GetAtX(x);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();

    public IEnumerable2D<T> GetAtX(int x) => new SliceX(this, x);
    public IEnumerable2D<T> GetAtY(int y) => new SliceY(this, y);
    public IEnumerable2D<T> GetAtZ(int z) => new SliceZ(this, z);

    IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
    IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
    IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);

    private class SliceX : IEnumerable2D<T> {
        private readonly List3D<T> _parent;
        private readonly int _x;

        public SliceX(List3D<T> parent, int x) {
            _parent = parent;
            _x = x;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int y = 0; y < _parent._ySize; y++) yield return GetAtY(y);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtY(int y) {
            for (int z = 0; z < _parent._zSize; z++) yield return _parent._items[_x][y][z];
        }

        public IEnumerable<T> GetAtX(int x) => GetAtY(x);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceY : IEnumerable2D<T> {
        private readonly List3D<T> _parent;
        private readonly int _y;

        public SliceY(List3D<T> parent, int y) {
            _parent = parent;
            _y = y;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int x = 0; x < _parent._xSize; x++) yield return GetAtX(x);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int x) {
            for (int z = 0; z < _parent._zSize; z++) yield return _parent._items[x][_parent._ySize > _y ? _y : 0][z];
        }

        public IEnumerable<T> GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceZ : IEnumerable2D<T> {
        private readonly List3D<T> _parent;
        private readonly int _z;

        public SliceZ(List3D<T> parent, int z) {
            _parent = parent;
            _z = z;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (int x = 0; x < _parent._xSize; x++) yield return GetAtX(x);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int x) {
            for (int y = 0; y < _parent._ySize; y++) yield return _parent._items[x][y][_parent._zSize > _z ? _z : 0];
        }

        public IEnumerable<T> GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    /// <summary>
    /// Converts the 3D list into a three-dimensional array. (Creates a copy)
    /// </summary>
    /// <returns>A three-dimensional array containing the elements of the 3D list.</returns>
    public T[,,] ToArray() {
        var arr = new T[_xSize, _ySize, _zSize];
        if (_xSize == 0 || _ySize == 0 || _zSize == 0) return arr;

        for (int x = 0; x < _xSize; x++) {
            var rowX = _items[x];
            for (int y = 0; y < _ySize; y++) {
                var srcSpan = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref rowX[y][0], _zSize);
                var dstSpan = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref arr[x, y, 0], _zSize);
                srcSpan.CopyTo(dstSpan);
            }
        }

        return arr;
    }

    /// <summary>
    /// Converts the 3D list into a jagged array. (Creates a copy)
    /// </summary>
    /// <returns>A jagged array representation of the 3D list.</returns>
    [Pure]
    public T[][][] ToJagged() {
        var arr = new T[_xSize][][];

        for (int x = 0; x < _xSize; x++) {
            arr[x] = new T[_ySize][];
            
            for (int y = 0; y < _ySize; y++) {
                arr[x][y] = new T[_zSize];
                Array.Copy(_items[x][y], arr[x][y], _zSize);
            }
        }

        return arr;
    }
}