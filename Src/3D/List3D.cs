// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Collections;
using System.Runtime.CompilerServices;

namespace Metriks;

public class List3D<T> : IList3D<T>, ICollection3D, IReadOnlyList3D<T> {
    private const int INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR = 2f;

    private T[][][] _items;

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

    public List3D(T[,,] collection) : this(
        collection.GetLength(0),
        collection.GetLength(1),
        collection.GetLength(2)) {
        for (int x = 0; x < collection.GetLength(0); x++) {
            _items[x] = new T[collection.GetLength(1)][];

            for (int y = 0; y < collection.GetLength(1); y++) {
                _items[x][y] = new T[collection.GetLength(2)];

                for (int z = 0; z < collection.GetLength(2); z++) {
                    _items[x][y][z] = collection[x, y, z];
                }
            }
        }

        _xSize = _xCapacity;
        _ySize = _yCapacity;
        _zSize = _zCapacity;
    }

    public int XSize => _xSize;
    public int YSize => _ySize;
    public int ZSize => _zSize;
    public Size3D Size => new(_xSize, _ySize, _zSize);
    public int Count => _xSize * _ySize * _zSize;
    public int XCount => _xSize;
    public int YCount => _ySize;
    public int ZCount => _zSize;
    public bool IsReadOnly => false;
    public int XCapacity => _xCapacity;
    public int YCapacity => _yCapacity;
    public int ZCapacity => _zCapacity;

    public T this[int x, int y, int z] {
        get {
            if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException();
            if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException();
            if (z < 0 || z >= _zSize) throw new IndexOutOfRangeException();

            return _items[x][y][z];
        }
        set {
            if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException();
            if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException();
            if (z < 0 || z >= _zSize) throw new IndexOutOfRangeException();

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
                for (int y = 0; y < _ySize; y++) {
                    var newArray = new T[_zCapacity];
                    Array.Copy(_items[x][y], newArray, _zSize);
                    if (defaultValue is not null) Array.Fill(newArray, defaultValue, _zSize, zSize - _zSize);
                    _items[x][y] = newArray;
                }
            }
        }

        for (int x = _xSize; x < xSize; x++) {
            _items[x] = new T[_yCapacity][];

            for (int y = 0; y < ySize; y++) {
                _items[x][y] = new T[_zCapacity];
                if (defaultValue is not null) Array.Fill(_items[x][y], defaultValue, 0, zSize);
            }
        }

        for (int x = 0; x < _xSize; x++) {
            for (int y = _ySize; y < ySize; y++) {
                _items[x][y] = new T[_zCapacity];
                if (defaultValue is not null) Array.Fill(_items[x][y], defaultValue, 0, zSize);
            }

            if (defaultValue is null) continue;

            for (int y = 0; y < _ySize; y++) Array.Fill(_items[x][y], defaultValue, _zSize, zSize - _zSize);
        }

        _xSize = xSize;
        _ySize = ySize;
        _zSize = zSize;
    }

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
                for (int y = 0; y < _ySize; y++) {
                    var newArray = new T[_zCapacity];
                    Array.Copy(_items[x][y], newArray, _zSize);
                    _items[x][y] = newArray;
                }
            }
        }

        // New X
        for (int x = _xSize; x < xSize; x++) {
            _items[x] = new T[_yCapacity][];
            for (int y = 0; y < ySize; y++) {
                _items[x][y] = new T[_zCapacity];
                for (int z = 0; z < zSize; z++) _items[x][y][z] = defaultValueFactory();
            }
        }

        // Existing X, new Y
        for (int x = 0; x < _xSize; x++) {
            for (int y = _ySize; y < ySize; y++) {
                _items[x][y] = new T[_zCapacity];
                for (int z = 0; z < zSize; z++) _items[x][y][z] = defaultValueFactory();
            }
            // Existing X, existing Y, new Z
            for (int y = 0; y < _ySize; y++) {
                for (int z = _zSize; z < zSize; z++) {
                    _items[x][y][z] = defaultValueFactory();
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

    public bool Contains(T value) {
        for (int x = 0; x < _xSize; x++)
        for (int y = 0; y < _ySize; y++)
        for (int z = 0; z < _zSize; z++)
            if (EqualityComparer<T>.Default.Equals(_items[x][y][z], value))
                return true;

        return false;
    }

    public bool ContainsAtX(int x, T value) {
        if (x < 0 || x >= _xSize) return false;

        for (int y = 0; y < _ySize; y++)
        for (int z = 0; z < _zSize; z++)
            if (EqualityComparer<T>.Default.Equals(_items[x][y][z], value))
                return true;

        return false;
    }

    public bool ContainsAtY(int y, T value) {
        if (y < 0 || y >= _ySize) return false;

        for (int x = 0; x < _xSize; x++)
        for (int z = 0; z < _zSize; z++)
            if (EqualityComparer<T>.Default.Equals(_items[x][y][z], value))
                return true;

        return false;
    }

    public bool ContainsAtZ(int z, T value) {
        if (z < 0 || z >= _zSize) return false;

        for (int x = 0; x < _xSize; x++)
        for (int y = 0; y < _ySize; y++)
            if (EqualityComparer<T>.Default.Equals(_items[x][y][z], value))
                return true;

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
            for (int x = 0; x < _xSize; x++)
                for (int y = 0; y < _ySize; y++)
                    Array.Copy(_items[x][y], 0, newItems[x + oldXOffset][y + oldYOffset], oldZOffset, _zSize);

            // Copy new
            for (int x = 0; x < matrix.GetLength(0); x++)
                for (int y = 0; y < matrix.GetLength(1); y++)
                    for (int z = 0; z < matrix.GetLength(2); z++)
                        newItems[x + newXOffset][y + newYOffset][z + newZOffset] = matrix[x, y, z];

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

            for (int x = Math.Clamp(offset.X, 0, _xSize); x < Math.Min(_xSize, offset.X + matrix.GetLength(0)); x++)
                for (int y = Math.Clamp(offset.Y, 0, _ySize); y < Math.Min(_ySize, offset.Y + matrix.GetLength(1)); y++)
                    for (int z = Math.Clamp(offset.Z, 0, _zSize); z < Math.Min(_zSize, offset.Z + matrix.GetLength(2)); z++)
                        _items[x][y][z] = matrix[x - offset.X, y - offset.Y, z - offset.Z];
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
            for (int x = 0; x < _xSize; x++)
                for (int y = 0; y < _ySize; y++)
                    Array.Copy(_items[x][y], 0, newItems[x + oldXOffset][y + oldYOffset], oldZOffset, _zSize);

            // Copy new
            for (int x = 0; x < matrix._xSize; x++)
                for (int y = 0; y < matrix._ySize; y++)
                    Array.Copy(matrix._items[x][y], 0, newItems[x + newXOffset][y + newYOffset], newZOffset, matrix._zSize);

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

            for (int x = Math.Clamp(offset.X, 0, _xSize); x < Math.Min(_xSize, offset.X + matrix._xSize); x++)
                for (int y = Math.Clamp(offset.Y, 0, _ySize); y < Math.Min(_ySize, offset.Y + matrix._ySize); y++)
                    for (int z = Math.Clamp(offset.Z, 0, _zSize); z < Math.Min(_zSize, offset.Z + matrix._zSize); z++)
                        _items[x][y][z] = matrix._items[x - offset.X][y - offset.Y][z - offset.Z];
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
            for (int x = 0; x < _xSize; x++)
                for (int y = 0; y < _ySize; y++)
                    Array.Copy(_items[x][y], 0, newItems[x + oldXOffset][y + oldYOffset], oldZOffset, _zSize);

            // Copy new
            for (int x = 0; x < matrix.GetLength(0); x++)
                for (int y = 0; y < matrix.GetLength(1); y++)
                    for (int z = 0; z < matrix.GetLength(2); z++)
                        if (predicate(newItems[x + newXOffset][y + newYOffset][z + newZOffset], matrix[x, y, z]))
                            newItems[x + newXOffset][y + newYOffset][z + newZOffset] = matrix[x, y, z];

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

            for (int x = Math.Clamp(offset.X, 0, _xSize); x < Math.Min(_xSize, offset.X + matrix.GetLength(0)); x++)
                for (int y = Math.Clamp(offset.Y, 0, _ySize); y < Math.Min(_ySize, offset.Y + matrix.GetLength(1)); y++)
                    for (int z = Math.Clamp(offset.Z, 0, _zSize); z < Math.Min(_zSize, offset.Z + matrix.GetLength(2)); z++)
                        if (predicate(_items[x][y][z], matrix[x - offset.X, y - offset.Y, z - offset.Z]))
                            _items[x][y][z] = matrix[x - offset.X, y - offset.Y, z - offset.Z];
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
            for (int x = 0; x < _xSize; x++)
                for (int y = 0; y < _ySize; y++)
                    Array.Copy(_items[x][y], 0, newItems[x + oldXOffset][y + oldYOffset], oldZOffset, _zSize);

            // Copy new
            for (int x = 0; x < matrix._xSize; x++)
                for (int y = 0; y < matrix._ySize; y++)
                    for (int z = 0; z < matrix._zSize; z++)
                        if (predicate(newItems[x + newXOffset][y + newYOffset][z + newZOffset], matrix._items[x][y][z]))
                            newItems[x + newXOffset][y + newYOffset][z + newZOffset] = matrix._items[x][y][z];

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

            for (int x = Math.Clamp(offset.X, 0, _xSize); x < Math.Min(_xSize, offset.X + matrix._xSize); x++)
                for (int y = Math.Clamp(offset.Y, 0, _ySize); y < Math.Min(_ySize, offset.Y + matrix._ySize); y++)
                    for (int z = Math.Clamp(offset.Z, 0, _zSize); z < Math.Min(_zSize, offset.Z + matrix._zSize); z++)
                        if (predicate(_items[x][y][z], matrix._items[x - offset.X][y - offset.Y][z - offset.Z]))
                            _items[x][y][z] = matrix._items[x - offset.X][y - offset.Y][z - offset.Z];
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
        for (int x = 0; x < _xSize; x++)
            for (int y = 0; y < _ySize; y++)
                for (int z = 0; z < _zSize; z++)
                    _items[x][y][z] = factory();
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

        for (int x = xStart; x < xEnd; x++)
            for (int y = yStart; y < yEnd; y++)
                for (int z = zStart; z < zEnd; z++)
                    _items[x][y][z] = factory();
    }

    /// <summary>
    /// Fills the 3D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(Func<T> factory, Point3D offset, Size3D size)
        => Fill(factory, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    public void CopyTo(T[,,] array, Point3D index) {
        for (int x = 0; x < _xSize; x++)
        for (int y = 0; y < _ySize; y++)
        for (int z = 0; z < _zSize; z++)
            array[x + index.X, y + index.Y, z + index.Z] = _items[x][y][z];
    }

    public void CopyTo(Array array, Point3D index) {
        for (int x = 0; x < _xSize; x++)
        for (int y = 0; y < _ySize; y++)
        for (int z = 0; z < _zSize; z++)
            array.SetValue(_items[x][y][z], x + index.X, y + index.Y, z + index.Z);
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

    public T[,,] ToArray() {
        var arr = new T[_xSize, _ySize, _zSize];

        for (int x = 0; x < _xSize; x++)
        for (int y = 0; y < _ySize; y++)
        for (int z = 0; z < _zSize; z++)
            arr[x, y, z] = _items[x][y][z];

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