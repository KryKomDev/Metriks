// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

#if METRIKS_ENABLE_JAGGED_LIST
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
///     Represents a strongly typed, three-dimensional list of elements that can be accessed by X, Y, and Z indices.
///     Provides methods to search, sort, and manipulate 3D lists.
/// </summary>
/// <typeparam name="T">The type of elements in the three-dimensional list.</typeparam>
public class List3DJagged<T> : IList3D<T>, ICollection3D, IReadOnlyList3D<T> {
    private const int   INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR = 2f;

    /// <summary>
    ///     Initializes a new instance of the <see cref="List3DJagged{T}" /> class with the specified initial capacity for each
    ///     dimension.
    /// </summary>
    /// <param name="xCapacity">The initial capacity along the X-axis.</param>
    /// <param name="yCapacity">The initial capacity along the Y-axis.</param>
    /// <param name="zCapacity">The initial capacity along the Z-axis.</param>
    public List3DJagged(
        int xCapacity = INITIAL_CAPACITY,
        int yCapacity = INITIAL_CAPACITY,
        int zCapacity = INITIAL_CAPACITY
    ) {
        Items = new T[xCapacity][][];
        XSize = 0;
        YSize = 0;
        ZSize = 0;
        XCapacity = xCapacity;
        YCapacity = yCapacity;
        ZCapacity = zCapacity;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="List3DJagged{T}" /> class populated with elements from the specified 3D
    ///     array.
    /// </summary>
    /// <param name="collection">The 3D array of elements to copy from.</param>
    public List3DJagged(T[,,] collection) : this(
        collection.GetLength(0),
        collection.GetLength(1),
        collection.GetLength(2)
    ) {
        var len0 = collection.GetLength(0);
        var len1 = collection.GetLength(1);
        var len2 = collection.GetLength(2);

        for (var x = 0; x < len0; x++)
            Items[x] = new T[len1][];

        if (len1 > 0 && len2 > 0)
            for (var x = 0; x < len0; x++) {
                var rowX = Items[x];

                for (var y = 0; y < len1; y++) {
                    rowX[y] = new T[len2];
                    var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref collection[x, y, 0], len2);
                    var dstSpan = new Span<T>(rowX[y]);
                    srcSpan.CopyTo(dstSpan);
                }
            }
        else if (len1 > 0)
            for (var x = 0; x < len0; x++) {
                var rowX = Items[x];

                for (var y = 0; y < len1; y++)
                    rowX[y] = Array.Empty<T>();
            }

        XSize = len0;
        YSize = len1;
        ZSize = len2;
    }

    public List3DJagged(List3DJagged<T> other) {
        ArgumentNullException.ThrowIfNull(other);

        XSize     = other.XSize;
        YSize     = other.YSize;
        ZSize     = other.ZSize;
        XCapacity = other.XCapacity;
        YCapacity = other.YCapacity;
        ZCapacity = other.ZCapacity;

        Items = new T[XCapacity][][];
        for (var x = 0; x < XSize; x++) {
            Items[x] = new T[YCapacity][];
            for (var y = 0; y < YSize; y++) {
                Items[x][y] = new T[ZCapacity];
                Array.Copy(other.Items[x][y], Items[x][y], ZSize);
            }
        }
    }

    public List3DJagged(IReadOnlyList3D<T> other) {
        ArgumentNullException.ThrowIfNull(other);

        if (other is List3DJagged<T> jagged) {
            XSize     = jagged.XSize;
            YSize     = jagged.YSize;
            ZSize     = jagged.ZSize;
            XCapacity = jagged.XCapacity;
            YCapacity = jagged.YCapacity;
            ZCapacity = jagged.ZCapacity;

            Items = new T[XCapacity][][];
            for (var x = 0; x < XSize; x++) {
                Items[x] = new T[YCapacity][];
                for (var y = 0; y < YSize; y++) {
                    Items[x][y] = new T[ZCapacity];
                    Array.Copy(jagged.Items[x][y], Items[x][y], ZSize);
                }
            }
            return;
        }

        XSize     = other.XCount;
        YSize     = other.YCount;
        ZSize     = other.ZCount;
        XCapacity = Math.Max(XSize, INITIAL_CAPACITY);
        YCapacity = Math.Max(YSize, INITIAL_CAPACITY);
        ZCapacity = Math.Max(ZSize, INITIAL_CAPACITY);

        Items = new T[XCapacity][][];
        for (var x = 0; x < XSize; x++) {
            Items[x] = new T[YCapacity][];
            for (var y = 0; y < YSize; y++) {
                Items[x][y] = new T[ZCapacity];
                for (var z = 0; z < ZSize; z++) {
                    Items[x][y][z] = other[x, y, z];
                }
            }
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List3DJagged<T> Clone() => new(this);

    /// <summary>
    ///     Gets the underlying three-dimensional array used to store the elements of the <see cref="List3DJagged{T}" /> instance.
    ///     PROVIDED FOR INTERNAL USE ONLY. DO NOT USE. <b>!!!DO NOT MODIFY THE ARRAY IN ANY WAY!!!</b>
    /// </summary>
    internal T[][][] Items {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    }

    /// <summary>
    ///     Gets the size (number of elements) along the X-axis.
    /// </summary>
    public int XSize { get; private set; }

    /// <summary>
    ///     Gets the size (number of elements) along the Y-axis.
    /// </summary>
    public int YSize { get; private set; }

    /// <summary>
    ///     Gets the size (number of elements) along the Z-axis.
    /// </summary>
    public int ZSize { get; private set; }

    /// <summary>
    ///     Gets a <see cref="Size3D" /> representing the current size of the list in all three dimensions.
    /// </summary>
    public Size3D Size => new(XSize, YSize, ZSize);

    /// <summary>
    ///     Gets the capacity along the X-axis.
    /// </summary>
    public int XCapacity { get; private set; }

    /// <summary>
    ///     Gets the capacity along the Y-axis.
    /// </summary>
    public int YCapacity { get; private set; }

    /// <summary>
    ///     Gets the capacity along the Z-axis.
    /// </summary>
    public int ZCapacity { get; private set; }

    /// <summary>
    ///     Copies the elements of the 3D list to a standard multidimensional array, starting at the specified destination
    ///     index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    public void CopyTo(Array array, Point3D index) {
        for (var x = 0; x < XSize; x++) {
            var rowX = Items[x];

            for (var y = 0; y < YSize; y++) {
                var rowXY = rowX[y];

                for (var z = 0; z < ZSize; z++)
                    array.SetValue(rowXY[z], x + index.X, y + index.Y, z + index.Z);
            }
        }
    }

    /// <summary>
    ///     Gets the total number of elements contained in the <see cref="List3DJagged{T}" />.
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
    ///     Gets a value indicating whether the <see cref="List3DJagged{T}" /> is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    public T this[int x, int y, int z] {
        get {
            if (x < 0 || x >= XSize || y < 0 || y >= YSize || z < 0 || z >= ZSize)
                throw new IndexOutOfRangeException();

            return Items[x][y][z];
        }
        set {
            if (x < 0 || x >= XSize || y < 0 || y >= YSize || z < 0 || z >= ZSize)
                throw new IndexOutOfRangeException();

            Items[x][y][z] = value;
        }
    }

    public void InsertAtX(int x) {
        if (x < 0 || x > XSize)
            throw new IndexOutOfRangeException();

        if (XSize + 1 >= XCapacity)
            XCapacity = (int)(XCapacity * GROWTH_FACTOR);

        var newItems = new T[XCapacity][][];
        Array.Copy(Items, newItems, x);
        newItems[x] = new T[YCapacity][];

        for (var i = 0; i < YCapacity; i++)
            newItems[x][i] = new T[ZCapacity];

        Array.Copy(Items, x, newItems, x + 1, XSize - x);
        XSize++;
        Items = newItems;
    }

    public void InsertAtY(int y) {
        if (y < 0 || y > YSize)
            throw new IndexOutOfRangeException();

        if (YSize + 1 >= YCapacity)
            YCapacity = (int)(YCapacity * GROWTH_FACTOR);

        for (var x = 0; x < XSize; x++) {
            var newArray = new T[YCapacity][];
            Array.Copy(Items[x], newArray, y);
            newArray[y] = new T[ZCapacity];
            Array.Copy(Items[x], y, newArray, y + 1, YSize - y);
            Items[x] = newArray;
        }

        YSize++;
    }

    public void InsertAtZ(int z) {
        if (z < 0 || z > ZSize)
            throw new IndexOutOfRangeException();

        if (ZSize + 1 >= ZCapacity)
            ZCapacity = (int)(ZCapacity * GROWTH_FACTOR);

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var newArray = new T[ZCapacity];
            Array.Copy(Items[x][y], newArray, z);
            Array.Copy(Items[x][y], z,        newArray, z + 1, ZSize - z);
            Items[x][y] = newArray;
        }

        ZSize++;
    }

    public void AddX() => InsertAtX(XSize);
    public void AddY() => InsertAtY(YSize);
    public void AddZ() => InsertAtZ(ZSize);

    public void RemoveAtX(int x) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException();

        XSize--;
        var newItems = new T[XSize][][];
        Array.Copy(Items, newItems, x);
        Array.Copy(Items, x + 1,    newItems, x, XSize - x);
        Items = newItems;
    }

    public void RemoveAtY(int y) {
        if (y < 0 || y >= YSize)
            throw new IndexOutOfRangeException();

        YSize--;

        for (var x = 0; x < XSize; x++) {
            Array.Copy(Items[x], y + 1, Items[x], y, YSize - y);
            Items[x][YSize] = null!;
        }
    }

    public void RemoveAtZ(int z) {
        if (z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException();

        ZSize--;

        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            Array.Copy(Items[x][y], z + 1, Items[x][y], z, ZSize - z);
            Items[x][y][ZSize] = default!;
        }
    }

    public void ShrinkX() {
        if (XSize > 0)
            XSize--;
    }

    public void ShrinkY() {
        if (YSize > 0)
            YSize--;
    }

    public void ShrinkZ() {
        if (ZSize > 0)
            ZSize--;
    }

    /// <summary>
    ///     Determines whether the 3D list contains a specific value.
    /// </summary>
    /// <param name="value">The value to locate in the 3D list.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool Contains(T value) {
        for (var x = 0; x < XSize; x++) {
            var rowX = Items[x];

            for (var y = 0; y < YSize; y++)
                if (Array.IndexOf(rowX[y], value, 0, ZSize) >= 0)
                    return true;
        }

        return false;
    }

    /// <summary>
    ///     Determines whether the specified column (along the X-axis) contains a specific value.
    /// </summary>
    /// <param name="x">The column index on the X-axis.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool ContainsAtX(int x, T value) {
        if (x < 0 || x >= XSize)
            return false;

        var rowX = Items[x];

        for (var y = 0; y < YSize; y++)
            if (Array.IndexOf(rowX[y], value, 0, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>
    ///     Determines whether the specified plane (along the Y-axis) contains a specific value.
    /// </summary>
    /// <param name="y">The index on the Y-axis.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool ContainsAtY(int y, T value) {
        if (y < 0 || y >= YSize)
            return false;

        for (var x = 0; x < XSize; x++)
            if (Array.IndexOf(Items[x][y], value, 0, ZSize) >= 0)
                return true;

        return false;
    }

    /// <summary>
    ///     Determines whether the specified plane (along the Z-axis) contains a specific value.
    /// </summary>
    /// <param name="z">The index on the Z-axis.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool ContainsAtZ(int z, T value) {
        if (z < 0 || z >= ZSize)
            return false;

        for (var x = 0; x < XSize; x++) {
            var rowX = Items[x];

            for (var y = 0; y < YSize; y++)
                if (EqualityComparer<T>.Default.Equals(rowX[y][z], value))
                    return true;
        }

        return false;
    }

    public void Clear() {
        XSize = 0;
        YSize = 0;
        ZSize = 0;
        Items = new T[INITIAL_CAPACITY][][];
    }

    /// <summary>
    ///     Copies the elements of the 3D list to a 3D array, starting at the specified destination index.
    /// </summary>
    /// <param name="array">The destination 3D array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    /// <exception cref="ArgumentException">Thrown if the destination array is not large enough.</exception>
    public void CopyTo(T[,,] array, Point3D index) {
        if (array.GetLength(0) < XSize + index.X)
            throw new ArgumentException("Destination array is not large enough in x dimension.");

        if (array.GetLength(1) < YSize + index.Y)
            throw new ArgumentException("Destination array is not large enough in y dimension.");

        if (array.GetLength(2) < ZSize + index.Z)
            throw new ArgumentException("Destination array is not large enough in z dimension.");

        if (XSize == 0 || YSize == 0 || ZSize == 0)
            return;

        for (var x = 0; x < XSize; x++) {
            var rowX = Items[x];

            for (var y = 0; y < YSize; y++) {
                var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref rowX[y][0], ZSize);
                var dstSpan = MemoryMarshal.CreateSpan(ref array[x + index.X, y + index.Y, index.Z], ZSize);
                srcSpan.CopyTo(dstSpan);
            }
        }
    }

    public void CopyTo(T[,,] array) => CopyTo(array, Point3D.Zero);

    public void CopyTo(List3DJagged<T> destination) {
        ArgumentNullException.ThrowIfNull(destination);

        if (destination.XSize == 0 && destination.YSize == 0 && destination.ZSize == 0)
            destination.CopyFrom(this);
        else
            destination.CopyFrom(this, Point3D.Zero);
    }

    public void CopyTo(List3DJagged<T> destination, Point3D destinationIndex) {
        ArgumentNullException.ThrowIfNull(destination);
        destination.CopyFrom(this, destinationIndex);
    }

    public void CopyTo(List3DJagged<T> destination, Point3D sourceOffset, Point3D destinationOffset, Size3D size) {
        ArgumentNullException.ThrowIfNull(destination);
        destination.CopyFrom(this, sourceOffset, destinationOffset, size);
    }

    public void CopyFrom(List3DJagged<T> source) {
        ArgumentNullException.ThrowIfNull(source);

        if (ReferenceEquals(this, source))
            return;

        Resize(source.XSize, source.YSize, source.ZSize);

        for (var x = 0; x < XSize; x++) {
            for (var y = 0; y < YSize; y++) {
                Array.Copy(source.Items[x][y], Items[x][y], ZSize);
            }
        }
    }

    public void CopyFrom(IReadOnlyList3D<T> source) {
        ArgumentNullException.ThrowIfNull(source);

        if (source is List3DJagged<T> jagged) {
            CopyFrom(jagged);
            return;
        }

        Resize(source.XCount, source.YCount, source.ZCount);

        for (var x = 0; x < XSize; x++) {
            for (var y = 0; y < YSize; y++) {
                for (var z = 0; z < ZSize; z++) {
                    Items[x][y][z] = source[x, y, z];
                }
            }
        }
    }

    public void CopyFrom(List3DJagged<T> source, Point3D destinationOffset) {
        ArgumentNullException.ThrowIfNull(source);

        if (destinationOffset.X < 0 || destinationOffset.Y < 0 || destinationOffset.Z < 0)
            throw new ArgumentOutOfRangeException(nameof(destinationOffset), "Destination offset must not be negative.");
        if (destinationOffset.X + source.XSize > XSize)
            throw new ArgumentException("Destination list is not large enough in X dimension to accommodate the source.", nameof(destinationOffset));
        if (destinationOffset.Y + source.YSize > YSize)
            throw new ArgumentException("Destination list is not large enough in Y dimension to accommodate the source.", nameof(destinationOffset));
        if (destinationOffset.Z + source.ZSize > ZSize)
            throw new ArgumentException("Destination list is not large enough in Z dimension to accommodate the source.", nameof(destinationOffset));

        for (var x = 0; x < source.XSize; x++) {
            for (var y = 0; y < source.YSize; y++) {
                Array.Copy(source.Items[x][y], 0, Items[destinationOffset.X + x][destinationOffset.Y + y], destinationOffset.Z, source.ZSize);
            }
        }
    }

    public void CopyFrom(IReadOnlyList3D<T> source, Point3D destinationOffset) {
        ArgumentNullException.ThrowIfNull(source);

        if (source is List3DJagged<T> jagged) {
            CopyFrom(jagged, destinationOffset);
            return;
        }

        if (destinationOffset.X < 0 || destinationOffset.Y < 0 || destinationOffset.Z < 0)
            throw new ArgumentOutOfRangeException(nameof(destinationOffset), "Destination offset must not be negative.");
        if (destinationOffset.X + source.XCount > XSize)
            throw new ArgumentException("Destination list is not large enough in X dimension to accommodate the source.", nameof(destinationOffset));
        if (destinationOffset.Y + source.YCount > YSize)
            throw new ArgumentException("Destination list is not large enough in Y dimension to accommodate the source.", nameof(destinationOffset));
        if (destinationOffset.Z + source.ZCount > ZSize)
            throw new ArgumentException("Destination list is not large enough in Z dimension to accommodate the source.", nameof(destinationOffset));

        for (var x = 0; x < source.XCount; x++) {
            for (var y = 0; y < source.YCount; y++) {
                for (var z = 0; z < source.ZCount; z++) {
                    Items[destinationOffset.X + x][destinationOffset.Y + y][destinationOffset.Z + z] = source[x, y, z];
                }
            }
        }
    }

    public void CopyFrom(List3DJagged<T> source, Point3D sourceOffset, Point3D destinationOffset, Size3D size) {
        ArgumentNullException.ThrowIfNull(source);
        if (size.X < 0 || size.Y < 0 || size.Z < 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size dimensions must not be negative.");
        if (sourceOffset.X < 0 || sourceOffset.Y < 0 || sourceOffset.Z < 0)
            throw new ArgumentOutOfRangeException(nameof(sourceOffset), "Source offset must not be negative.");
        if (destinationOffset.X < 0 || destinationOffset.Y < 0 || destinationOffset.Z < 0)
            throw new ArgumentOutOfRangeException(nameof(destinationOffset), "Destination offset must not be negative.");
        if (sourceOffset.X + size.X > source.XSize || sourceOffset.Y + size.Y > source.YSize || sourceOffset.Z + size.Z > source.ZSize)
            throw new ArgumentException("Source region is out of bounds of the source list.", nameof(sourceOffset));
        if (destinationOffset.X + size.X > XSize || destinationOffset.Y + size.Y > YSize || destinationOffset.Z + size.Z > ZSize)
            throw new ArgumentException("Destination region is out of bounds of the destination list.", nameof(destinationOffset));

        if (size.X == 0 || size.Y == 0 || size.Z == 0)
            return;

        if (ReferenceEquals(this, source)) {
            var stepX = destinationOffset.X > sourceOffset.X ? -1 : 1;
            var startX = destinationOffset.X > sourceOffset.X ? size.X - 1 : 0;
            var endX = destinationOffset.X > sourceOffset.X ? -1 : size.X;

            var stepY = destinationOffset.Y > sourceOffset.Y ? -1 : 1;
            var startY = destinationOffset.Y > sourceOffset.Y ? size.Y - 1 : 0;
            var endY = destinationOffset.Y > sourceOffset.Y ? -1 : size.Y;

            for (var dx = startX; dx != endX; dx += stepX) {
                var srcX = sourceOffset.X + dx;
                var dstX = destinationOffset.X + dx;
                for (var dy = startY; dy != endY; dy += stepY) {
                    var srcY = sourceOffset.Y + dy;
                    var dstY = destinationOffset.Y + dy;
                    Array.Copy(Items[srcX][srcY], sourceOffset.Z, Items[dstX][dstY], destinationOffset.Z, size.Z);
                }
            }
            return;
        }

        for (var x = 0; x < size.X; x++) {
            for (var y = 0; y < size.Y; y++) {
                Array.Copy(source.Items[sourceOffset.X + x][sourceOffset.Y + y], sourceOffset.Z,
                           Items[destinationOffset.X + x][destinationOffset.Y + y], destinationOffset.Z,
                           size.Z);
            }
        }
    }

    public void CopyFrom(IReadOnlyList3D<T> source, Point3D sourceOffset, Point3D destinationOffset, Size3D size) {
        ArgumentNullException.ThrowIfNull(source);
        if (source is List3DJagged<T> jagged) {
            CopyFrom(jagged, sourceOffset, destinationOffset, size);
            return;
        }

        if (size.X < 0 || size.Y < 0 || size.Z < 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size dimensions must not be negative.");
        if (sourceOffset.X < 0 || sourceOffset.Y < 0 || sourceOffset.Z < 0)
            throw new ArgumentOutOfRangeException(nameof(sourceOffset), "Source offset must not be negative.");
        if (destinationOffset.X < 0 || destinationOffset.Y < 0 || destinationOffset.Z < 0)
            throw new ArgumentOutOfRangeException(nameof(destinationOffset), "Destination offset must not be negative.");
        if (sourceOffset.X + size.X > source.XCount || sourceOffset.Y + size.Y > source.YCount || sourceOffset.Z + size.Z > source.ZCount)
            throw new ArgumentException("Source region is out of bounds of the source list.", nameof(sourceOffset));
        if (destinationOffset.X + size.X > XSize || destinationOffset.Y + size.Y > YSize || destinationOffset.Z + size.Z > ZSize)
            throw new ArgumentException("Destination region is out of bounds of the destination list.", nameof(destinationOffset));

        for (var x = 0; x < size.X; x++) {
            for (var y = 0; y < size.Y; y++) {
                for (var z = 0; z < size.Z; z++) {
                    Items[destinationOffset.X + x][destinationOffset.Y + y][destinationOffset.Z + z] = source[sourceOffset.X + x, sourceOffset.Y + y, sourceOffset.Z + z];
                }
            }
        }
    }

    public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
        for (var x = 0; x < XSize; x++)
            yield return GetAtX(x);
    }

    IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();

    public IEnumerable2D<T> GetAtX(int x) => new SliceX(this, x);
    public IEnumerable2D<T> GetAtY(int y) => new SliceY(this, y);
    public IEnumerable2D<T> GetAtZ(int z) => new SliceZ(this, z);

    IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
    IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
    IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);

    protected void UnsafeSet(int x, int y, int z, T value) => Items[x][y][z] = value;
    protected T    UnsafeGet(int x, int y, int z) => Items[x][y][z];

    /// <summary>
    ///     Expands the dimensions of the 3D list to the specified size, filling new elements with a default value.
    /// </summary>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="zSize">The new size along the Z-axis.</param>
    /// <param name="defaultValue">The value to fill the new elements with.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any new size is smaller than the current size.</exception>
    public void Expand(int xSize, int ySize, int zSize, T? defaultValue = default!) {
        if (xSize < XSize || ySize < YSize || zSize < ZSize)
            throw new ArgumentOutOfRangeException();

        if (xSize == XSize && ySize == YSize && zSize == ZSize)
            return;

        if (xSize > XCapacity) {
            XCapacity = xSize + 1;
            var newItems = new T[XCapacity][][];
            Array.Copy(Items, newItems, XSize);
            Items = newItems;
        }

        if (ySize > YCapacity) {
            YCapacity = ySize + 1;

            for (var x = 0; x < XSize; x++) {
                var newArray = new T[YCapacity][];
                Array.Copy(Items[x], newArray, YSize);

                for (var i = YSize; i < YCapacity; i++)
                    newArray[i] = new T[ZCapacity];

                Items[x] = newArray;
            }
        }

        if (zSize > ZCapacity) {
            ZCapacity = zSize + 1;

            for (var x = 0; x < XSize; x++) {
                var rowX = Items[x];

                for (var y = 0; y < YSize; y++) {
                    var newArray = new T[ZCapacity];
                    Array.Copy(rowX[y], newArray, ZSize);

                    if (defaultValue is not null)
                        Array.Fill(newArray, defaultValue, ZSize, zSize - ZSize);

                    rowX[y] = newArray;
                }
            }
        }

        for (var x = XSize; x < xSize; x++) {
            var rowX = new T[YCapacity][];
            Items[x] = rowX;

            for (var y = 0; y < ySize; y++) {
                var cellXY = new T[ZCapacity];
                rowX[y] = cellXY;

                if (defaultValue is not null)
                    Array.Fill(cellXY, defaultValue, 0, zSize);
            }
        }

        for (var x = 0; x < XSize; x++) {
            var rowX = Items[x];

            for (var y = YSize; y < ySize; y++) {
                var cellXY = new T[ZCapacity];
                rowX[y] = cellXY;

                if (defaultValue is not null)
                    Array.Fill(cellXY, defaultValue, 0, zSize);
            }

            if (defaultValue is null)
                continue;

            for (var y = 0; y < YSize; y++)
                Array.Fill(rowX[y], defaultValue, ZSize, zSize - ZSize);
        }

        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
    }

    /// <summary>
    ///     Expands the dimensions of the 3D list to the specified size, generating new elements using the specified factory
    ///     function.
    /// </summary>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="zSize">The new size along the Z-axis.</param>
    /// <param name="defaultValueFactory">A factory function that generates values for the new elements.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any new size is smaller than the current size.</exception>
    public void Expand(int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        if (xSize < XSize || ySize < YSize || zSize < ZSize)
            throw new ArgumentOutOfRangeException();

        if (xSize == XSize && ySize == YSize && zSize == ZSize)
            return;

        if (xSize > XCapacity) {
            XCapacity = xSize + 1;
            var newItems = new T[XCapacity][][];
            Array.Copy(Items, newItems, XSize);
            Items = newItems;
        }

        if (ySize > YCapacity) {
            YCapacity = ySize + 1;

            for (var x = 0; x < XSize; x++) {
                var newArray = new T[YCapacity][];
                Array.Copy(Items[x], newArray, YSize);

                for (var y = YSize; y < YCapacity; y++)
                    newArray[y] = new T[ZCapacity];

                Items[x] = newArray;
            }
        }

        if (zSize > ZCapacity) {
            ZCapacity = zSize + 1;

            for (var x = 0; x < XSize; x++) {
                var rowX = Items[x];

                for (var y = 0; y < YSize; y++) {
                    var newArray = new T[ZCapacity];
                    Array.Copy(rowX[y], newArray, ZSize);
                    rowX[y] = newArray;
                }
            }
        }

        // New X
        for (var x = XSize; x < xSize; x++) {
            var rowX = new T[YCapacity][];
            Items[x] = rowX;

            for (var y = 0; y < ySize; y++) {
                var cellXY = new T[ZCapacity];
                rowX[y] = cellXY;

                for (var z = 0; z < zSize; z++)
                    cellXY[z] = defaultValueFactory();
            }
        }

        // Existing X, new Y
        for (var x = 0; x < XSize; x++) {
            var rowX = Items[x];

            for (var y = YSize; y < ySize; y++) {
                var cellXY = new T[ZCapacity];
                rowX[y] = cellXY;

                for (var z = 0; z < zSize; z++)
                    cellXY[z] = defaultValueFactory();
            }

            // Existing X, existing Y, new Z
            for (var y = 0; y < YSize; y++) {
                var cellXY = rowX[y];

                for (var z = ZSize; z < zSize; z++)
                    cellXY[z] = defaultValueFactory();
            }
        }

        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
    }

    public void Resize(int xSize, int ySize, int zSize, T? defaultValue = default) {
        if (xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[xSize][][];

        for (var x = 0; x < xSize; x++) {
            newItems[x] = new T[ySize][];

            for (var y = 0; y < ySize; y++) {
                newItems[x][y] = new T[zSize];

                if (defaultValue is not null)
                    Array.Fill(newItems[x][y], defaultValue);
            }
        }

        var minX = Math.Min(XSize, xSize);
        var minY = Math.Min(YSize, ySize);
        var minZ = Math.Min(ZSize, zSize);

        for (var x = 0; x < minX; x++)
        for (var y = 0; y < minY; y++)
            Array.Copy(Items[x][y], newItems[x][y], minZ);

        Items = newItems;
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    public void Resize(int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        if (xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[xSize][][];

        for (var x = 0; x < xSize; x++) {
            newItems[x] = new T[ySize][];

            for (var y = 0; y < ySize; y++) {
                newItems[x][y] = new T[zSize];

                for (var z = 0; z < zSize; z++)
                    newItems[x][y][z] = defaultValueFactory();
            }
        }

        var minX = Math.Min(XSize, xSize);
        var minY = Math.Min(YSize, ySize);
        var minZ = Math.Min(ZSize, zSize);

        for (var x = 0; x < minX; x++)
        for (var y = 0; y < minY; y++)
            Array.Copy(Items[x][y], newItems[x][y], minZ);

        Items = newItems;
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    public void Shrink(int xSize, int ySize, int zSize) {
        if (xSize > XSize || ySize > YSize || zSize > ZSize)
            throw new ArgumentOutOfRangeException();

        Resize(xSize, ySize, zSize);
    }

    /// <summary>
    ///     Places a 3D matrix into this List3DJagged at the specified offset. If the matrix extends beyond
    ///     the current bounds of the List3DJagged, the List3DJagged is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 3D array of elements to place into this List3DJagged.</param>
    /// <param name="offsetPoint">
    ///     An optional offset defining where the top-left corner of the matrix will be placed.
    ///     If not provided, the matrix will be placed at the origin of the List3DJagged.
    /// </param>
    /// <param name="resize">
    ///     If true, enables automatic resizing of this list depending on
    ///     the size and offset of the placed array.
    /// </param>
    public void Place(T[,,] matrix, Point3D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X][][];

            for (var x = 0; x < newSize.X; x++) {
                newItems[x] = new T[newSize.Y][];

                for (var y = 0; y < newSize.Y; y++)
                    newItems[x][y] = new T[newSize.Z];
            }

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (var x = 0; x < XSize; x++) {
                var srcRowX = Items[x];
                var dstRowX = newItems[x + oldXOffset];

                for (var y = 0; y < YSize; y++)
                    Array.Copy(srcRowX[y], 0, dstRowX[y + oldYOffset], oldZOffset, ZSize);
            }

            // Copy new
            var len2 = matrix.GetLength(2);

            if (len2 > 0) {
                var len0 = matrix.GetLength(0);
                var len1 = matrix.GetLength(1);

                for (var x = 0; x < len0; x++) {
                    var dstRowX = newItems[x + newXOffset];

                    for (var y = 0; y < len1; y++) {
                        var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[x, y, 0], len2);
                        var dstSpan = new Span<T>(dstRowX[y + newYOffset], newZOffset, len2);
                        srcSpan.CopyTo(dstSpan);
                    }
                }
            }

            Items = newItems;
            XSize = newSize.X;
            YSize = newSize.Y;
            ZSize = newSize.Z;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.X, newSize.Y, newSize.Z);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX = Math.Min(XSize, offset.X + matrix.GetLength(0));
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY = Math.Min(YSize, offset.Y + matrix.GetLength(1));
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ = Math.Min(ZSize, offset.Z + matrix.GetLength(2));
            var zCount = endZ - startZ;

            if (zCount > 0)
                for (var x = startX; x < endX; x++) {
                    var rowX = Items[x];
                    var srcX = x - offset.X;

                    for (var y = startY; y < endY; y++) {
                        var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[srcX, y - offset.Y, startZ - offset.Z], zCount);
                        var dstSpan = new Span<T>(rowX[y], startZ, zCount);
                        srcSpan.CopyTo(dstSpan);
                    }
                }
        }
    }

    /// <summary>
    ///     Places the contents of the specified 3D list into the current List3DJagged instance, optionally offset by a specified
    ///     point.
    /// </summary>
    /// <param name="matrix">The List3DJagged instance containing the elements to be placed.</param>
    /// <param name="offsetPoint">
    ///     An optional point specifying the offset at which the matrix should be placed.
    ///     If null, the matrix will be placed starting at the origin (0, 0, 0).
    /// </param>
    /// <param name="resize">
    ///     If true, enables automatic resizing of this list depending on
    ///     the size and offset of the placed array.
    /// </param>
    public void Place(List3DJagged<T> matrix, Point3D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X][][];

            for (var x = 0; x < newSize.X; x++) {
                newItems[x] = new T[newSize.Y][];

                for (var y = 0; y < newSize.Y; y++)
                    newItems[x][y] = new T[newSize.Z];
            }

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (var x = 0; x < XSize; x++) {
                var srcRowX = Items[x];
                var dstRowX = newItems[x + oldXOffset];

                for (var y = 0; y < YSize; y++)
                    Array.Copy(srcRowX[y], 0, dstRowX[y + oldYOffset], oldZOffset, ZSize);
            }

            // Copy new
            for (var x = 0; x < matrix.XSize; x++) {
                var srcRowX = matrix.Items[x];
                var dstRowX = newItems[x + newXOffset];

                for (var y = 0; y < matrix.YSize; y++)
                    Array.Copy(srcRowX[y], 0, dstRowX[y + newYOffset], newZOffset, matrix.ZSize);
            }

            Items = newItems;
            XSize = newSize.X;
            YSize = newSize.Y;
            ZSize = newSize.Z;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.X, newSize.Y, newSize.Z);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX = Math.Min(XSize, offset.X + matrix.XSize);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY = Math.Min(YSize, offset.Y + matrix.YSize);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ = Math.Min(ZSize, offset.Z + matrix.ZSize);
            var zCount = endZ - startZ;

            if (zCount > 0)
                for (var x = startX; x < endX; x++) {
                    var dstRowX = Items[x];
                    var srcRowX = matrix.Items[x - offset.X];

                    for (var y = startY; y < endY; y++)
                        Array.Copy(srcRowX[y - offset.Y], startZ - offset.Z, dstRowX[y], startZ, zCount);
                }
        }
    }

    /// <summary>
    ///     Places a 3D matrix into this List3DJagged at the specified offset. If the matrix extends beyond
    ///     the current bounds of the List3DJagged, the List3DJagged is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 3D array of elements to place into this List3DJagged.</param>
    /// <param name="predicate">
    ///     A function determining whether the item should be placed into this array.
    ///     The first argument is an item from this array that is being overwritten by the second one.
    /// </param>
    /// <param name="offsetPoint">
    ///     An optional offset defining where the top-left corner of the matrix will be placed.
    ///     If not provided, the matrix will be placed at the origin of the List3DJagged.
    /// </param>
    /// <param name="resize">
    ///     If true, enables automatic resizing of this list depending on
    ///     the size and offset of the placed array.
    /// </param>
    public void Place(T[,,] matrix, Func<T, T, bool> predicate, Point3D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X][][];

            for (var x = 0; x < newSize.X; x++) {
                newItems[x] = new T[newSize.Y][];

                for (var y = 0; y < newSize.Y; y++)
                    newItems[x][y] = new T[newSize.Z];
            }

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (var x = 0; x < XSize; x++) {
                var srcRowX = Items[x];
                var dstRowX = newItems[x + oldXOffset];

                for (var y = 0; y < YSize; y++)
                    Array.Copy(srcRowX[y], 0, dstRowX[y + oldYOffset], oldZOffset, ZSize);
            }

            // Copy new
            var len0 = matrix.GetLength(0);
            var len1 = matrix.GetLength(1);
            var len2 = matrix.GetLength(2);

            for (var x = 0; x < len0; x++) {
                var dstRowX = newItems[x + newXOffset];

                for (var y = 0; y < len1; y++) {
                    var dstRowXY = dstRowX[y + newYOffset];

                    for (var z = 0; z < len2; z++) {
                        var val = matrix[x, y, z];

                        if (predicate(dstRowXY[z + newZOffset], val))
                            dstRowXY[z + newZOffset] = val;
                    }
                }
            }

            Items = newItems;
            XSize = newSize.X;
            YSize = newSize.Y;
            ZSize = newSize.Z;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.X, newSize.Y, newSize.Z);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX = Math.Min(XSize, offset.X + matrix.GetLength(0));
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY = Math.Min(YSize, offset.Y + matrix.GetLength(1));
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ = Math.Min(ZSize, offset.Z + matrix.GetLength(2));

            for (var x = startX; x < endX; x++) {
                var rowX = Items[x];
                var srcX = x - offset.X;

                for (var y = startY; y < endY; y++) {
                    var rowXY = rowX[y];
                    var srcY = y - offset.Y;

                    for (var z = startZ; z < endZ; z++) {
                        var val = matrix[srcX, srcY, z - offset.Z];

                        if (predicate(rowXY[z], val))
                            rowXY[z] = val;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Places the contents of the specified 3D list into the current List3DJagged instance, optionally offset by a specified
    ///     point.
    /// </summary>
    /// <param name="matrix">The List3DJagged instance containing the elements to be placed.</param>
    /// <param name="predicate">
    ///     A function determining whether the item should be placed into this array.
    ///     The first argument is an item from this array that is being overwritten by the second one.
    /// </param>
    /// <param name="offsetPoint">
    ///     An optional point specifying the offset at which the matrix should be placed.
    ///     If null, the matrix will be placed starting at the origin (0, 0, 0).
    /// </param>
    /// <param name="resize">
    ///     If true, enables automatic resizing of this list depending on
    ///     the size and offset of the placed array.
    /// </param>
    public void Place(List3DJagged<T> matrix, Func<T, T, bool> predicate, Point3D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point3D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point3D(Math.Max(XSize, placedMax.X), Math.Max(YSize, placedMax.Y), Math.Max(ZSize, placedMax.Z));
        var min = new Point3D(Math.Min(offset.X, 0),        Math.Min(offset.Y, 0),        Math.Min(offset.Z, 0));

        var newSize = new Size3D(max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.X][][];

            for (var x = 0; x < newSize.X; x++) {
                newItems[x] = new T[newSize.Y][];

                for (var y = 0; y < newSize.Y; y++)
                    newItems[x][y] = new T[newSize.Z];
            }

            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (var x = 0; x < XSize; x++) {
                var srcRowX = Items[x];
                var dstRowX = newItems[x + oldXOffset];

                for (var y = 0; y < YSize; y++)
                    Array.Copy(srcRowX[y], 0, dstRowX[y + oldYOffset], oldZOffset, ZSize);
            }

            // Copy new
            for (var x = 0; x < matrix.XSize; x++) {
                var srcRowX = matrix.Items[x];
                var dstRowX = newItems[x + newXOffset];

                for (var y = 0; y < matrix.YSize; y++) {
                    var srcRowXY = srcRowX[y];
                    var dstRowXY = dstRowX[y + newYOffset];

                    for (var z = 0; z < matrix.ZSize; z++) {
                        var val = srcRowXY[z];

                        if (predicate(dstRowXY[z + newZOffset], val))
                            dstRowXY[z + newZOffset] = val;
                    }
                }
            }

            Items = newItems;
            XSize = newSize.X;
            YSize = newSize.Y;
            ZSize = newSize.Z;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.X, newSize.Y, newSize.Z);

            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX = Math.Min(XSize, offset.X + matrix.XSize);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY = Math.Min(YSize, offset.Y + matrix.YSize);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ = Math.Min(ZSize, offset.Z + matrix.ZSize);

            for (var x = startX; x < endX; x++) {
                var dstRowX = Items[x];
                var srcRowX = matrix.Items[x - offset.X];

                for (var y = startY; y < endY; y++) {
                    var dstRowXY = dstRowX[y];
                    var srcRowXY = srcRowX[y - offset.Y];

                    for (var z = startZ; z < endZ; z++) {
                        var val = srcRowXY[z - offset.Z];

                        if (predicate(dstRowXY[z], val))
                            dstRowXY[z] = val;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Fills the entire 3D list with the specified value.
    /// </summary>
    /// <param name="item">The value to fill the 3D list with.</param>
    public void Fill(T item) {
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            Array.Fill(Items[x][y], item, 0, ZSize);
    }

    /// <summary>
    ///     Fills the specified 3D region of the list with a given value.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="xStart">The starting index on the X-axis (inclusive).</param>
    /// <param name="xCount">The number of elements to be filled along the X-axis.</param>
    /// <param name="yStart">The starting index on the Y-axis (inclusive).</param>
    /// <param name="yCount">The number of elements to be filled along the Y-axis.</param>
    /// <param name="zStart">The starting index on the Z-axis (inclusive).</param>
    /// <param name="zCount">The number of elements to be filled along the Z-axis.</param>
    /// <exception cref="IndexOutOfRangeException">
    ///     Thrown when the specified region exceeds the bounds of the list or one of the count parameters is negative.
    /// </exception>
    public void Fill(T item, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (xStart < 0 || yStart < 0 || zStart < 0 || xEnd > XSize || yEnd > YSize || zEnd > ZSize || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        for (var x = xStart; x < xEnd; x++)
        for (var y = yStart; y < yEnd; y++)
            Array.Fill(Items[x][y], item, zStart, zCount);
    }

    /// <summary>
    ///     Fills the 3D list with the specified value in the given region.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(T item, Point3D offset, Size3D size) => Fill(item, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>
    ///     Fills the entire 3D list with the values generated by the specified factory function.
    /// </summary>
    /// <param name="factory">A function that generates values to fill the 3D list.</param>
    public void Fill(Func<T> factory) {
        for (var x = 0; x < XSize; x++) {
            var rowX = Items[x];

            for (var y = 0; y < YSize; y++) {
                var rowXY = rowX[y];

                for (var z = 0; z < ZSize; z++)
                    rowXY[z] = factory();
            }
        }
    }

    /// <summary>
    ///     Fills the 3D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="xStart">Start x coordinate of the filled region.</param>
    /// <param name="xCount">The number of units in x-axis to be filled in the filled region.</param>
    /// <param name="yStart">Start y coordinate of the filled region.</param>
    /// <param name="yCount">The number of units in y-axis to be filled in the filled region.</param>
    /// <param name="zStart">Start z coordinate of the filled region.</param>
    /// <param name="zCount">The number of units in z-axis to be filled in the filled region.</param>
    /// <exception cref="IndexOutOfRangeException">
    ///     Thrown when the specified region exceeds the bounds of the list or one of the count parameters is negative.
    /// </exception>
    public void Fill(Func<T> factory, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (xStart < 0 || yStart < 0 || zStart < 0 || xEnd > XSize || yEnd > YSize || zEnd > ZSize || xCount < 0 || yCount < 0 || zCount < 0)
            throw new IndexOutOfRangeException();

        for (var x = xStart; x < xEnd; x++) {
            var rowX = Items[x];

            for (var y = yStart; y < yEnd; y++) {
                var rowXY = rowX[y];

                for (var z = zStart; z < zEnd; z++)
                    rowXY[z] = factory();
            }
        }
    }

    /// <summary>
    ///     Fills the 3D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(Func<T> factory, Point3D offset, Size3D size) => Fill(factory, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>
    ///     Converts the 3D list into a three-dimensional array. (Creates a copy)
    /// </summary>
    /// <returns>A three-dimensional array containing the elements of the 3D list.</returns>
    public T[,,] ToArray() {
        var arr = new T[XSize, YSize, ZSize];

        if (XSize == 0 || YSize == 0 || ZSize == 0)
            return arr;

        for (var x = 0; x < XSize; x++) {
            var rowX = Items[x];

            for (var y = 0; y < YSize; y++) {
                var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref rowX[y][0], ZSize);
                var dstSpan = MemoryMarshal.CreateSpan(ref arr[x, y, 0], ZSize);
                srcSpan.CopyTo(dstSpan);
            }
        }

        return arr;
    }

    /// <summary>
    ///     Converts the 3D list into a jagged array. (Creates a copy)
    /// </summary>
    /// <returns>A jagged array representation of the 3D list.</returns>
    [Pure]
    public T[][][] ToJagged() {
        var arr = new T[XSize][][];

        for (var x = 0; x < XSize; x++) {
            arr[x] = new T[YSize][];

            for (var y = 0; y < YSize; y++) {
                arr[x][y] = new T[ZSize];
                Array.Copy(Items[x][y], arr[x][y], ZSize);
            }
        }

        return arr;
    }

    private class SliceX : IEnumerable2D<T> {
        private readonly List3DJagged<T> _parent;
        private readonly int             _x;

        public SliceX(List3DJagged<T> parent, int x) {
            _parent = parent;
            _x = x;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var y = 0; y < _parent.YSize; y++)
                yield return GetAtY(y);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtY(int y) {
            for (var z = 0; z < _parent.ZSize; z++)
                yield return _parent.Items[_x][y][z];
        }

        public IEnumerable<T>     GetAtX(int x) => GetAtY(x);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceY : IEnumerable2D<T> {
        private readonly List3DJagged<T> _parent;
        private readonly int             _y;

        public SliceY(List3DJagged<T> parent, int y) {
            _parent = parent;
            _y = y;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var x = 0; x < _parent.XSize; x++)
                yield return GetAtX(x);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int x) {
            for (var z = 0; z < _parent.ZSize; z++)
                yield return _parent.Items[x][_parent.YSize > _y ? _y : 0][z];
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceZ : IEnumerable2D<T> {
        private readonly List3DJagged<T> _parent;
        private readonly int             _z;

        public SliceZ(List3DJagged<T> parent, int z) {
            _parent = parent;
            _z = z;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var x = 0; x < _parent.XSize; x++)
                yield return GetAtX(x);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int x) {
            for (var y = 0; y < _parent.YSize; y++)
                yield return _parent.Items[x][y][_parent.ZSize > _z ? _z : 0];
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    public T this[Index x, Index y, Index z] {
        get => this[x.GetOffset(XSize), y.GetOffset(YSize), z.GetOffset(ZSize)];
        set => this[x.GetOffset(XSize), y.GetOffset(YSize), z.GetOffset(ZSize)] = value;
    }

    public T[] this[Range x, int y, int z] {
        get {
            var (offset, length) = x.GetOffsetAndLength(XSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = Items[offset + i][y][z];

            return result;
        }
    }

    public T[] this[int x, Range y, int z] {
        get {
            var (offset, length) = y.GetOffsetAndLength(YSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = Items[x][offset + i][z];

            return result;
        }
    }

    public T[] this[int x, int y, Range z] {
        get {
            var (offset, length) = z.GetOffsetAndLength(ZSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = Items[x][y][offset + i];

            return result;
        }
    }
    #endif
}

#endif