// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

#if METRIKS_ENABLE_JAGGED_LIST
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Metriks;

/// <summary>
///     Represents a strongly typed, four-dimensional list of elements that can be accessed by W, X, Y, and Z indices.
///     Provides methods to search, sort, and manipulate 4D lists.
/// </summary>
/// <typeparam name="T">The type of elements in the four-dimensional list.</typeparam>
public class List4DJagged<T> : IList4D<T>, ICollection4D, IReadOnlyList4D<T> {

    private const int   INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR = 2f;

    /// <summary>
    ///     Initializes a new instance of the <see cref="List4DJagged{T}" /> class with the specified initial capacity for each
    ///     dimension.
    /// </summary>
    /// <param name="wCapacity">The initial capacity along the W-axis.</param>
    /// <param name="xCapacity">The initial capacity along the X-axis.</param>
    /// <param name="yCapacity">The initial capacity along the Y-axis.</param>
    /// <param name="zCapacity">The initial capacity along the Z-axis.</param>
    public List4DJagged(
        int wCapacity = INITIAL_CAPACITY,
        int xCapacity = INITIAL_CAPACITY,
        int yCapacity = INITIAL_CAPACITY,
        int zCapacity = INITIAL_CAPACITY
    ) {
        Items = new T[wCapacity][][][];
        WSize = 0;
        XSize = 0;
        YSize = 0;
        ZSize = 0;
        WCapacity = wCapacity;
        XCapacity = xCapacity;
        YCapacity = yCapacity;
        ZCapacity = zCapacity;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="List4DJagged{T}" /> class populated with elements from the specified 4D
    ///     array.
    /// </summary>
    /// <param name="collection">The 4D array of elements to copy from.</param>
    public List4DJagged(T[,,,] collection) : this(
        collection.GetLength(0),
        collection.GetLength(1),
        collection.GetLength(2),
        collection.GetLength(3)
    ) {
        var len0 = collection.GetLength(0);
        var len1 = collection.GetLength(1);
        var len2 = collection.GetLength(2);
        var len3 = collection.GetLength(3);

        for (var w = 0; w < len0; w++)
            Items[w] = new T[len1][][];

        if (len1 > 0 && len2 > 0 && len3 > 0)
            for (var w = 0; w < len0; w++) {
                var rowW = Items[w];

                for (var x = 0; x < len1; x++) {
                    rowW[x] = new T[len2][];
                    var rowWX = rowW[x];

                    for (var y = 0; y < len2; y++) {
                        rowWX[y] = new T[len3];
                        var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref collection[w, x, y, 0], len3);
                        var dstSpan = new Span<T>(rowWX[y]);
                        srcSpan.CopyTo(dstSpan);
                    }
                }
            }
        else
            for (var w = 0; w < len0; w++) {
                var rowW = Items[w];

                for (var x = 0; x < len1; x++) {
                    rowW[x] = new T[len2][];
                    var rowWX = rowW[x];

                    for (var y = 0; y < len2; y++)
                        rowWX[y] = Array.Empty<T>();
                }
            }

        WSize = len0;
        XSize = len1;
        YSize = len2;
        ZSize = len3;
    }

    /// <summary>
    ///     Gets the underlying four-dimensional array used to store the elements of the <see cref="List4DJagged{T}" /> instance.
    ///     PROVIDED FOR INTERNAL USE ONLY. DO NOT USE. <b>!!!DO NOT MODIFY THE ARRAY IN ANY WAY!!!</b>
    /// </summary>
    internal T[][][][] Items {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    }

    /// <summary>
    ///     Gets the size (number of elements) along the W-axis.
    /// </summary>
    public int WSize { get; private set; }

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
    ///     Gets a <see cref="Size4D" /> representing the current size of the list in all four dimensions.
    /// </summary>
    public Size4D Size => new(WSize, XSize, YSize, ZSize);

    /// <summary>
    ///     Gets the capacity along the W-axis.
    /// </summary>
    public int WCapacity { get; private set; }

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
    ///     Copies the elements of the 4D list to a standard multidimensional array, starting at the specified destination
    ///     index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    public void CopyTo(Array array, Point4D index) {
        for (var w = 0; w < WSize; w++) {
            var rowW = Items[w];

            for (var x = 0; x < XSize; x++) {
                var rowWX = rowW[x];

                for (var y = 0; y < YSize; y++) {
                    var rowWXY = rowWX[y];

                    for (var z = 0; z < ZSize; z++)
                        array.SetValue(rowWXY[z], w + index.W, x + index.X, y + index.Y, z + index.Z);
                }
            }
        }
    }

    /// <summary>
    ///     Gets the total number of elements contained in the <see cref="List4DJagged{T}" />.
    /// </summary>
    public int Count => WSize * XSize * YSize * ZSize;

    /// <summary>
    ///     Gets the size (number of elements) along the W-axis.
    /// </summary>
    public int WCount => WSize;

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
    ///     Gets a value indicating whether the <see cref="List4DJagged{T}" /> is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    public T this[int w, int x, int y, int z] {
        get {
            if (w < 0 || w >= WSize)
                throw new IndexOutOfRangeException("Index 'w' is out of range.");

            if (x < 0 || x >= XSize)
                throw new IndexOutOfRangeException("Index 'x' is out of range.");

            if (y < 0 || y >= YSize)
                throw new IndexOutOfRangeException("Index 'y' is out of range.");

            if (z < 0 || z >= ZSize)
                throw new IndexOutOfRangeException("Index 'z' is out of range.");

            return Items[w][x][y][z];
        }
        set {
            if (w < 0 || w >= WSize)
                throw new IndexOutOfRangeException("Index 'w' is out of range.");

            if (x < 0 || x >= XSize)
                throw new IndexOutOfRangeException("Index 'x' is out of range.");

            if (y < 0 || y >= YSize)
                throw new IndexOutOfRangeException("Index 'y' is out of range.");

            if (z < 0 || z >= ZSize)
                throw new IndexOutOfRangeException("Index 'z' is out of range.");

            Items[w][x][y][z] = value;
        }
    }

    public void InsertAtW(int w) {
        if (w < 0 || w > WSize)
            throw new IndexOutOfRangeException();

        if (WSize + 1 >= WCapacity)
            WCapacity = (int)(WCapacity * GROWTH_FACTOR);

        var newItems = new T[WCapacity][][][];
        Array.Copy(Items, newItems, w);
        newItems[w] = Create3DArray(XCapacity, YCapacity, ZCapacity);
        Array.Copy(Items, w, newItems, w + 1, WSize - w);
        WSize++;
        Items = newItems;
    }

    public void InsertAtX(int x) {
        if (x < 0 || x > XSize)
            throw new IndexOutOfRangeException();

        if (XSize + 1 >= XCapacity)
            XCapacity = (int)(XCapacity * GROWTH_FACTOR);

        for (var w = 0; w < WSize; w++) {
            var newArray = new T[XCapacity][][];
            Array.Copy(Items[w], newArray, x);
            newArray[x] = Create2DArray(YCapacity, ZCapacity);
            Array.Copy(Items[w], x, newArray, x + 1, XSize - x);
            Items[w] = newArray;
        }

        XSize++;
    }

    public void InsertAtY(int y) {
        if (y < 0 || y > YSize)
            throw new IndexOutOfRangeException();

        if (YSize + 1 >= YCapacity)
            YCapacity = (int)(YCapacity * GROWTH_FACTOR);

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++) {
            var newArray = new T[YCapacity][];
            Array.Copy(Items[w][x], newArray, y);
            newArray[y] = new T[ZCapacity];
            Array.Copy(Items[w][x], y, newArray, y + 1, YSize - y);
            Items[w][x] = newArray;
        }

        YSize++;
    }

    public void InsertAtZ(int z) {
        if (z < 0 || z > ZSize)
            throw new IndexOutOfRangeException();

        if (ZSize + 1 >= ZCapacity)
            ZCapacity = (int)(ZCapacity * GROWTH_FACTOR);

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            var newArray = new T[ZCapacity];
            Array.Copy(Items[w][x][y], newArray, z);
            Array.Copy(Items[w][x][y], z,        newArray, z + 1, ZSize - z);
            Items[w][x][y] = newArray;
        }

        ZSize++;
    }

    public void AddW() => InsertAtW(WSize);
    public void AddX() => InsertAtX(XSize);
    public void AddY() => InsertAtY(YSize);
    public void AddZ() => InsertAtZ(ZSize);

    public void ShrinkW() {
        if (WSize == 0)
            throw new InvalidOperationException();

        WSize--;
        Items[WSize] = null!;
    }

    public void ShrinkX() {
        if (XSize == 0)
            throw new InvalidOperationException();

        XSize--;

        for (var w = 0; w < WSize; w++)
            Items[w][XSize] = null!;
    }

    public void ShrinkY() {
        if (YSize == 0)
            throw new InvalidOperationException();

        YSize--;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
            Items[w][x][YSize] = null!;
    }

    public void ShrinkZ() {
        if (ZSize == 0)
            throw new InvalidOperationException();

        ZSize--;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            Items[w][x][y][ZSize] = default!;
    }

    public void RemoveAtW(int w) {
        if (w < 0 || w >= WSize)
            throw new IndexOutOfRangeException();

        WSize--;
        var newItems = new T[WSize][][][];
        Array.Copy(Items, newItems, w);
        Array.Copy(Items, w + 1,    newItems, w, WSize - w);
        Items = newItems;
    }

    public void RemoveAtX(int x) {
        if (x < 0 || x >= XSize)
            throw new IndexOutOfRangeException();

        XSize--;

        for (var w = 0; w < WSize; w++) {
            var newArray = new T[XSize][][];
            Array.Copy(Items[w], newArray, x);
            Array.Copy(Items[w], x + 1,    newArray, x, XSize - x);
            Items[w] = newArray;
        }
    }

    public void RemoveAtY(int y) {
        if (y < 0 || y >= YSize)
            throw new IndexOutOfRangeException();

        YSize--;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++) {
            var newArray = new T[YSize][];
            Array.Copy(Items[w][x], newArray, y);
            Array.Copy(Items[w][x], y + 1,    newArray, y, YSize - y);
            Items[w][x] = newArray;
        }
    }

    public void RemoveAtZ(int z) {
        if (z < 0 || z >= ZSize)
            throw new IndexOutOfRangeException();

        ZSize--;

        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++) {
            Array.Copy(Items[w][x][y], z + 1, Items[w][x][y], z, ZSize - z);
            Items[w][x][y][ZSize] = default!;
        }
    }

    /// <summary>
    ///     Determines whether the 4D list contains a specific value.
    /// </summary>
    /// <param name="value">The value to locate in the 4D list.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool Contains(T value) {
        for (var w = 0; w < WSize; w++) {
            var rowW = Items[w];

            for (var x = 0; x < XSize; x++) {
                var rowWX = rowW[x];

                for (var y = 0; y < YSize; y++)
                    if (Array.IndexOf(rowWX[y], value, 0, ZSize) >= 0)
                        return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Determines whether the specified hyperplane (along the W-axis) contains a specific value.
    /// </summary>
    /// <param name="w">The index on the W-axis.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns><c>true</c> if the value is found; otherwise, <c>false</c>.</returns>
    public bool ContainsAtW(int w, T value) {
        if (w < 0 || w >= WSize)
            return false;

        var rowW = Items[w];

        for (var x = 0; x < XSize; x++) {
            var rowWX = rowW[x];

            for (var y = 0; y < YSize; y++)
                if (Array.IndexOf(rowWX[y], value, 0, ZSize) >= 0)
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

        for (var w = 0; w < WSize; w++) {
            var rowWX = Items[w][x];

            for (var y = 0; y < YSize; y++)
                if (Array.IndexOf(rowWX[y], value, 0, ZSize) >= 0)
                    return true;
        }

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

        for (var w = 0; w < WSize; w++) {
            var rowW = Items[w];

            for (var x = 0; x < XSize; x++)
                if (Array.IndexOf(rowW[x][y], value, 0, ZSize) >= 0)
                    return true;
        }

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

        for (var w = 0; w < WSize; w++) {
            var rowW = Items[w];

            for (var x = 0; x < XSize; x++) {
                var rowWX = rowW[x];

                for (var y = 0; y < YSize; y++)
                    if (EqualityComparer<T>.Default.Equals(rowWX[y][z], value))
                        return true;
            }
        }

        return false;
    }

    public void Clear() {
        WSize = 0;
        XSize = 0;
        YSize = 0;
        ZSize = 0;
        WCapacity = INITIAL_CAPACITY;
        XCapacity = INITIAL_CAPACITY;
        YCapacity = INITIAL_CAPACITY;
        ZCapacity = INITIAL_CAPACITY;
        Items = new T[INITIAL_CAPACITY][][][];
    }

    /// <summary>
    ///     Copies the elements of the 4D list to a 4D array, starting at the specified destination index.
    /// </summary>
    /// <param name="array">The destination 4D array.</param>
    /// <param name="index">The index in the destination array at which copying begins.</param>
    /// <exception cref="ArgumentException">Thrown if the destination array is not large enough.</exception>
    public void CopyTo(T[,,,] array, Point4D index) {
        if (array.GetLength(0) < WSize + index.W)
            throw new ArgumentException("Destination array is not large enough in w dimension.");

        if (array.GetLength(1) < XSize + index.X)
            throw new ArgumentException("Destination array is not large enough in x dimension.");

        if (array.GetLength(2) < YSize + index.Y)
            throw new ArgumentException("Destination array is not large enough in y dimension.");

        if (array.GetLength(3) < ZSize + index.Z)
            throw new ArgumentException("Destination array is not large enough in z dimension.");

        if (WSize == 0 || XSize == 0 || YSize == 0 || ZSize == 0)
            return;

        for (var w = 0; w < WSize; w++) {
            var rowW = Items[w];

            for (var x = 0; x < XSize; x++) {
                var rowWX = rowW[x];

                for (var y = 0; y < YSize; y++) {
                    var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref rowWX[y][0], ZSize);
                    var dstSpan = MemoryMarshal.CreateSpan(ref array[w + index.W, x + index.X, y + index.Y, index.Z], ZSize);
                    srcSpan.CopyTo(dstSpan);
                }
            }
        }
    }

    public IEnumerator<IEnumerable3D<T>> GetEnumerator() {
        for (var w = 0; w < WSize; w++)
            yield return GetAtW(w);
    }

    IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable4D.GetEnumerator() => GetEnumerator();

    public IEnumerable3D<T> GetAtW(int w) => new SliceW(this, w);
    public IEnumerable3D<T> GetAtX(int x) => new SliceX(this, x);
    public IEnumerable3D<T> GetAtY(int y) => new SliceY(this, y);
    public IEnumerable3D<T> GetAtZ(int z) => new SliceZ(this, z);

    IEnumerable3D IEnumerable4D.GetAtW(int w) => GetAtW(w);
    IEnumerable3D IEnumerable4D.GetAtX(int x) => GetAtX(x);
    IEnumerable3D IEnumerable4D.GetAtY(int y) => GetAtY(y);
    IEnumerable3D IEnumerable4D.GetAtZ(int z) => GetAtZ(z);

    protected void UnsafeSet(int w, int x, int y, int z, T value) => Items[w][x][y][z] = value;
    protected T    UnsafeGet(int w, int x, int y, int z) => Items[w][x][y][z];

    /// <summary>
    ///     Expands the dimensions of the 4D list to the specified size, filling new elements with a default value.
    /// </summary>
    /// <param name="wSize">The new size along the W-axis.</param>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="zSize">The new size along the Z-axis.</param>
    /// <param name="defaultValue">The value to fill the new elements with.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any new size is smaller than the current size.</exception>
    public void Expand(int wSize, int xSize, int ySize, int zSize, T? defaultValue = default!) {
        if (wSize < WSize || xSize < XSize || ySize < YSize || zSize < ZSize)
            throw new ArgumentOutOfRangeException();

        if (wSize == WSize && xSize == XSize && ySize == YSize && zSize == ZSize)
            return;

        if (wSize > WCapacity) {
            WCapacity = wSize + 1;
            var newItems = new T[WCapacity][][][];
            Array.Copy(Items, newItems, WSize);
            Items = newItems;
        }

        for (var w = 0; w < wSize; w++) {
            var isNewW = w >= WSize;

            if (isNewW) {
                var rowW = Create3DArray(xSize, ySize, zSize);
                Items[w] = rowW;

                if (defaultValue is not null)
                    for (var x = 0; x < xSize; x++) {
                        var rowWX = rowW[x];

                        for (var y = 0; y < ySize; y++)
                            Array.Fill(rowWX[y], defaultValue);
                    }

                continue;
            }

            // Existing W, expand its children
            var existingW = Items[w];

            if (xSize > existingW.Length) {
                var newX = new T[xSize][][];
                Array.Copy(existingW, newX, XSize);
                Items[w] = newX;
                existingW = newX;
            }

            for (var x = 0; x < xSize; x++) {
                var isNewX = x >= XSize;

                if (isNewX) {
                    var rowWX = Create2DArray(ySize, zSize);
                    existingW[x] = rowWX;

                    if (defaultValue is not null)
                        for (var y = 0; y < ySize; y++)
                            Array.Fill(rowWX[y], defaultValue);

                    continue;
                }

                var existingWX = existingW[x];

                if (ySize > existingWX.Length) {
                    var newY = new T[ySize][];
                    Array.Copy(existingWX, newY, YSize);
                    existingW[x] = newY;
                    existingWX = newY;
                }

                for (var y = 0; y < ySize; y++) {
                    var isNewY = y >= YSize;

                    if (isNewY) {
                        var cellWXY = new T[zSize];
                        existingWX[y] = cellWXY;

                        if (defaultValue is not null)
                            Array.Fill(cellWXY, defaultValue);

                        continue;
                    }

                    var cellWXYExisting = existingWX[y];

                    if (zSize > cellWXYExisting.Length) {
                        var newZ = new T[zSize];
                        Array.Copy(cellWXYExisting, newZ, ZSize);

                        if (defaultValue is not null)
                            Array.Fill(newZ, defaultValue, ZSize, zSize - ZSize);

                        existingWX[y] = newZ;
                    }
                    else if (zSize > ZSize && defaultValue is not null) {
                        Array.Fill(cellWXYExisting, defaultValue, ZSize, zSize - ZSize);
                    }
                }
            }
        }

        WSize = wSize;
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
        WCapacity = Math.Max(WCapacity, wSize);
        XCapacity = Math.Max(XCapacity, xSize);
        YCapacity = Math.Max(YCapacity, ySize);
        ZCapacity = Math.Max(ZCapacity, zSize);
    }

    /// <summary>
    ///     Expands the dimensions of the 4D list to the specified size, generating new elements using the specified factory
    ///     function.
    /// </summary>
    /// <param name="wSize">The new size along the W-axis.</param>
    /// <param name="xSize">The new size along the X-axis.</param>
    /// <param name="ySize">The new size along the Y-axis.</param>
    /// <param name="zSize">The new size along the Z-axis.</param>
    /// <param name="defaultValueFactory">A factory function that generates values for the new elements.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any new size is smaller than the current size.</exception>
    public void Expand(int wSize, int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        if (wSize < WSize || xSize < XSize || ySize < YSize || zSize < ZSize)
            throw new ArgumentOutOfRangeException();

        if (wSize == WSize && xSize == XSize && ySize == YSize && zSize == ZSize)
            return;

        if (wSize > WCapacity) {
            WCapacity = wSize + 1;
            var newItems = new T[WCapacity][][][];
            Array.Copy(Items, newItems, WSize);
            Items = newItems;
        }

        for (var w = 0; w < wSize; w++) {
            var isNewW = w >= WSize;

            if (isNewW) {
                var rowW = Create3DArray(xSize, ySize, zSize);
                Items[w] = rowW;

                for (var x = 0; x < xSize; x++) {
                    var rowWX = rowW[x];

                    for (var y = 0; y < ySize; y++) {
                        var rowWXY = rowWX[y];

                        for (var z = 0; z < zSize; z++)
                            rowWXY[z] = defaultValueFactory();
                    }
                }

                continue;
            }

            var existingW = Items[w];

            if (xSize > existingW.Length) {
                var newX = new T[xSize][][];
                Array.Copy(existingW, newX, XSize);
                Items[w] = newX;
                existingW = newX;
            }

            for (var x = 0; x < xSize; x++) {
                var isNewX = x >= XSize;

                if (isNewX) {
                    var rowWX = Create2DArray(ySize, zSize);
                    existingW[x] = rowWX;

                    for (var y = 0; y < ySize; y++) {
                        var rowWXY = rowWX[y];

                        for (var z = 0; z < zSize; z++)
                            rowWXY[z] = defaultValueFactory();
                    }

                    continue;
                }

                var existingWX = existingW[x];

                if (ySize > existingWX.Length) {
                    var newY = new T[ySize][];
                    Array.Copy(existingWX, newY, YSize);
                    existingW[x] = newY;
                    existingWX = newY;
                }

                for (var y = 0; y < ySize; y++) {
                    var isNewY = y >= YSize;

                    if (isNewY) {
                        var cellWXY = new T[zSize];
                        existingWX[y] = cellWXY;

                        for (var z = 0; z < zSize; z++)
                            cellWXY[z] = defaultValueFactory();

                        continue;
                    }

                    var cellWXYExisting = existingWX[y];

                    if (zSize > cellWXYExisting.Length) {
                        var newZ = new T[zSize];
                        Array.Copy(cellWXYExisting, newZ, ZSize);

                        for (var z = ZSize; z < zSize; z++)
                            newZ[z] = defaultValueFactory();

                        existingWX[y] = newZ;
                    }
                }
            }
        }

        WSize = wSize;
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
        WCapacity = Math.Max(WCapacity, wSize);
        XCapacity = Math.Max(XCapacity, xSize);
        YCapacity = Math.Max(YCapacity, ySize);
        ZCapacity = Math.Max(ZCapacity, zSize);
    }

    public void Resize(int wSize, int xSize, int ySize, int zSize, Func<T> defaultValueFactory) {
        if (wSize < 0 || xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[wSize][][][];

        for (var w = 0; w < wSize; w++) {
            newItems[w] = Create3DArray(xSize, ySize, zSize);

            for (var x = 0; x < xSize; x++)
            for (var y = 0; y < ySize; y++)
            for (var z = 0; z < zSize; z++)
                newItems[w][x][y][z] = defaultValueFactory();
        }

        var minW = Math.Min(WSize, wSize);
        var minX = Math.Min(XSize, xSize);
        var minY = Math.Min(YSize, ySize);
        var minZ = Math.Min(ZSize, zSize);

        for (var w = 0; w < minW; w++)
        for (var x = 0; x < minX; x++)
        for (var y = 0; y < minY; y++)
            Array.Copy(Items[w][x][y], newItems[w][x][y], minZ);

        Items = newItems;
        WSize = wSize;
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
        WCapacity = wSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    public void Resize(int wSize, int xSize, int ySize, int zSize, T? defaultValue = default) {
        if (wSize < 0 || xSize < 0 || ySize < 0 || zSize < 0)
            throw new ArgumentOutOfRangeException();

        var newItems = new T[wSize][][][];

        for (var w = 0; w < wSize; w++) {
            newItems[w] = Create3DArray(xSize, ySize, zSize);

            if (defaultValue is not null)
                for (var x = 0; x < xSize; x++)
                for (var y = 0; y < ySize; y++)
                    Array.Fill(newItems[w][x][y], defaultValue);
        }

        var minW = Math.Min(WSize, wSize);
        var minX = Math.Min(XSize, xSize);
        var minY = Math.Min(YSize, ySize);
        var minZ = Math.Min(ZSize, zSize);

        for (var w = 0; w < minW; w++)
        for (var x = 0; x < minX; x++)
        for (var y = 0; y < minY; y++)
            Array.Copy(Items[w][x][y], newItems[w][x][y], minZ);

        Items = newItems;
        WSize = wSize;
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
        WCapacity = wSize;
        XCapacity = xSize;
        YCapacity = ySize;
        ZCapacity = zSize;
    }

    /// <summary>
    ///     Places a 4D matrix into this List4DJagged at the specified offset. If the matrix extends beyond
    ///     the current bounds of the List4DJagged, the List4DJagged is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 4D array of elements to place into this List4DJagged.</param>
    /// <param name="offsetPoint">
    ///     An optional offset defining where the top-left corner of the matrix will be placed.
    ///     If not provided, the matrix will be placed at the origin of the List4DJagged.
    /// </param>
    /// <param name="resize">
    ///     If true, enables automatic resizing of this list depending on
    ///     the size and offset of the placed array.
    /// </param>
    public void Place(T[,,,] matrix, Point4D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(
            Math.Max(WSize, placedMax.W),
            Math.Max(XSize, placedMax.X),
            Math.Max(YSize, placedMax.Y),
            Math.Max(ZSize, placedMax.Z)
        );

        var min = new Point4D(
            Math.Min(offset.W, 0),
            Math.Min(offset.X, 0),
            Math.Min(offset.Y, 0),
            Math.Min(offset.Z, 0)
        );

        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W][][][];

            for (var w = 0; w < newSize.W; w++)
                newItems[w] = Create3DArray(newSize.X, newSize.Y, newSize.Z);

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);

            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (var w = 0; w < WSize; w++) {
                var rowW = Items[w];
                var dstW = newItems[w + oldWOffset];

                for (var x = 0; x < XSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + oldXOffset];

                    for (var y = 0; y < YSize; y++)
                        Array.Copy(rowWX[y], 0, dstWX[y + oldYOffset], oldZOffset, ZSize);
                }
            }

            // Copy new
            var len3 = matrix.GetLength(3);

            if (len3 > 0) {
                var len0 = matrix.GetLength(0);
                var len1 = matrix.GetLength(1);
                var len2 = matrix.GetLength(2);

                for (var w = 0; w < len0; w++) {
                    var dstW = newItems[w + newWOffset];

                    for (var x = 0; x < len1; x++) {
                        var dstWX = dstW[x + newXOffset];

                        for (var y = 0; y < len2; y++) {
                            var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[w, x, y, 0], len3);
                            var dstSpan = new Span<T>(dstWX[y + newYOffset], newZOffset, len3);
                            srcSpan.CopyTo(dstSpan);
                        }
                    }
                }
            }

            Items = newItems;
            WSize = newSize.W;
            XSize = newSize.X;
            YSize = newSize.Y;
            ZSize = newSize.Z;
            WCapacity = WSize;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.W > WSize || placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);

            var startW = Math.Clamp(offset.W, 0, WSize);
            var endW = Math.Min(WSize, offset.W + matrix.GetLength(0));
            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX = Math.Min(XSize, offset.X + matrix.GetLength(1));
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY = Math.Min(YSize, offset.Y + matrix.GetLength(2));
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ = Math.Min(ZSize, offset.Z + matrix.GetLength(3));
            var zCount = endZ - startZ;

            if (zCount > 0)
                for (var w = startW; w < endW; w++) {
                    var rowW = Items[w];
                    var srcW = w - offset.W;

                    for (var x = startX; x < endX; x++) {
                        var rowWX = rowW[x];
                        var srcX = x - offset.X;

                        for (var y = startY; y < endY; y++) {
                            var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref matrix[srcW, srcX, y - offset.Y, startZ - offset.Z], zCount);
                            var dstSpan = new Span<T>(rowWX[y], startZ, zCount);
                            srcSpan.CopyTo(dstSpan);
                        }
                    }
                }
        }
    }

    /// <summary>
    ///     Places the contents of the specified 4D list into the current List4DJagged instance, optionally offset by a specified
    ///     point.
    /// </summary>
    /// <param name="matrix">The List4DJagged instance containing the elements to be placed.</param>
    /// <param name="offsetPoint">
    ///     An optional point specifying the offset at which the matrix should be placed.
    ///     If null, the matrix will be placed starting at the origin (0, 0, 0, 0).
    /// </param>
    /// <param name="resize">
    ///     If true, enables automatic resizing of this list depending on
    ///     the size and offset of the placed array.
    /// </param>
    public void Place(List4DJagged<T> matrix, Point4D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(
            Math.Max(WSize, placedMax.W),
            Math.Max(XSize, placedMax.X),
            Math.Max(YSize, placedMax.Y),
            Math.Max(ZSize, placedMax.Z)
        );

        var min = new Point4D(
            Math.Min(offset.W, 0),
            Math.Min(offset.X, 0),
            Math.Min(offset.Y, 0),
            Math.Min(offset.Z, 0)
        );

        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W][][][];

            for (var w = 0; w < newSize.W; w++)
                newItems[w] = Create3DArray(newSize.X, newSize.Y, newSize.Z);

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);

            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (var w = 0; w < WSize; w++) {
                var rowW = Items[w];
                var dstW = newItems[w + oldWOffset];

                for (var x = 0; x < XSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + oldXOffset];

                    for (var y = 0; y < YSize; y++)
                        Array.Copy(rowWX[y], 0, dstWX[y + oldYOffset], oldZOffset, ZSize);
                }
            }

            // Copy new
            for (var w = 0; w < matrix.WSize; w++) {
                var rowW = matrix.Items[w];
                var dstW = newItems[w + newWOffset];

                for (var x = 0; x < matrix.XSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + newXOffset];

                    for (var y = 0; y < matrix.YSize; y++)
                        Array.Copy(rowWX[y], 0, dstWX[y + newYOffset], newZOffset, matrix.ZSize);
                }
            }

            Items = newItems;
            WSize = newSize.W;
            XSize = newSize.X;
            YSize = newSize.Y;
            ZSize = newSize.Z;
            WCapacity = WSize;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.W > WSize || placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);

            var startW = Math.Clamp(offset.W, 0, WSize);
            var endW = Math.Min(WSize, offset.W + matrix.WSize);
            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX = Math.Min(XSize, offset.X + matrix.XSize);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY = Math.Min(YSize, offset.Y + matrix.YSize);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ = Math.Min(ZSize, offset.Z + matrix.ZSize);
            var zCount = endZ - startZ;

            if (zCount > 0)
                for (var w = startW; w < endW; w++) {
                    var dstW = Items[w];
                    var srcW = matrix.Items[w - offset.W];

                    for (var x = startX; x < endX; x++) {
                        var dstWX = dstW[x];
                        var srcWX = srcW[x - offset.X];

                        for (var y = startY; y < endY; y++)
                            Array.Copy(srcWX[y - offset.Y], startZ - offset.Z, dstWX[y], startZ, zCount);
                    }
                }
        }
    }

    /// <summary>
    ///     Places a 4D matrix into this List4DJagged at the specified offset. If the matrix extends beyond
    ///     the current bounds of the List4DJagged, the List4DJagged is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 4D array of elements to place into this List4DJagged.</param>
    /// <param name="predicate">
    ///     A function determining whether the item should be placed into this array.
    ///     The first argument is an item from this array that is being overwritten by the second one.
    /// </param>
    /// <param name="offsetPoint">
    ///     An optional offset defining where the top-left corner of the matrix will be placed.
    ///     If not provided, the matrix will be placed at the origin of the List4DJagged.
    /// </param>
    /// <param name="resize">
    ///     If true, enables automatic resizing of this list depending on
    ///     the size and offset of the placed array.
    /// </param>
    public void Place(T[,,,] matrix, Func<T, T, bool> predicate, Point4D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(
            Math.Max(WSize, placedMax.W),
            Math.Max(XSize, placedMax.X),
            Math.Max(YSize, placedMax.Y),
            Math.Max(ZSize, placedMax.Z)
        );

        var min = new Point4D(
            Math.Min(offset.W, 0),
            Math.Min(offset.X, 0),
            Math.Min(offset.Y, 0),
            Math.Min(offset.Z, 0)
        );

        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W][][][];

            for (var w = 0; w < newSize.W; w++)
                newItems[w] = Create3DArray(newSize.X, newSize.Y, newSize.Z);

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);

            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (var w = 0; w < WSize; w++) {
                var rowW = Items[w];
                var dstW = newItems[w + oldWOffset];

                for (var x = 0; x < XSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + oldXOffset];

                    for (var y = 0; y < YSize; y++)
                        Array.Copy(rowWX[y], 0, dstWX[y + oldYOffset], oldZOffset, ZSize);
                }
            }

            // Copy new
            var len0 = matrix.GetLength(0);
            var len1 = matrix.GetLength(1);
            var len2 = matrix.GetLength(2);
            var len3 = matrix.GetLength(3);

            for (var w = 0; w < len0; w++) {
                var dstW = newItems[w + newWOffset];

                for (var x = 0; x < len1; x++) {
                    var dstWX = dstW[x + newXOffset];

                    for (var y = 0; y < len2; y++) {
                        var dstWXY = dstWX[y + newYOffset];

                        for (var z = 0; z < len3; z++) {
                            var val = matrix[w, x, y, z];

                            if (predicate(dstWXY[z + newZOffset], val))
                                dstWXY[z + newZOffset] = val;
                        }
                    }
                }
            }

            Items = newItems;
            WSize = newSize.W;
            XSize = newSize.X;
            YSize = newSize.Y;
            ZSize = newSize.Z;
            WCapacity = WSize;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.W > WSize || placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);

            var startW = Math.Clamp(offset.W, 0, WSize);
            var endW = Math.Min(WSize, offset.W + matrix.GetLength(0));
            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX = Math.Min(XSize, offset.X + matrix.GetLength(1));
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY = Math.Min(YSize, offset.Y + matrix.GetLength(2));
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ = Math.Min(ZSize, offset.Z + matrix.GetLength(3));

            for (var w = startW; w < endW; w++) {
                var rowW = Items[w];
                var srcW = w - offset.W;

                for (var x = startX; x < endX; x++) {
                    var rowWX = rowW[x];
                    var srcX = x - offset.X;

                    for (var y = startY; y < endY; y++) {
                        var rowWXY = rowWX[y];
                        var srcY = y - offset.Y;

                        for (var z = startZ; z < endZ; z++) {
                            var val = matrix[srcW, srcX, srcY, z - offset.Z];

                            if (predicate(rowWXY[z], val))
                                rowWXY[z] = val;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Places the contents of the specified 4D list into the current List4DJagged instance, optionally offset by a specified
    ///     point.
    /// </summary>
    /// <param name="matrix">The List4DJagged instance containing the elements to be placed.</param>
    /// <param name="predicate">
    ///     A function determining whether the item should be placed into this array.
    ///     The first argument is an item from this array that is being overwritten by the second one.
    /// </param>
    /// <param name="offsetPoint">
    ///     An optional point specifying the offset at which the matrix should be placed.
    ///     If null, the matrix will be placed starting at the origin (0, 0, 0, 0).
    /// </param>
    /// <param name="resize">
    ///     If true, enables automatic resizing of this list depending on
    ///     the size and offset of the placed array.
    /// </param>
    public void Place(List4DJagged<T> matrix, Func<T, T, bool> predicate, Point4D? offsetPoint = null, bool resize = true) {
        var offset = offsetPoint ?? Point4D.Zero;

        var placedMax = offset + matrix.Size.ToPoint();

        var max = new Point4D(
            Math.Max(WSize, placedMax.W),
            Math.Max(XSize, placedMax.X),
            Math.Max(YSize, placedMax.Y),
            Math.Max(ZSize, placedMax.Z)
        );

        var min = new Point4D(
            Math.Min(offset.W, 0),
            Math.Min(offset.X, 0),
            Math.Min(offset.Y, 0),
            Math.Min(offset.Z, 0)
        );

        var newSize = new Size4D(max.W - min.W, max.X - min.X, max.Y - min.Y, max.Z - min.Z);

        var isBelow = offset.W < 0 || offset.X < 0 || offset.Y < 0 || offset.Z < 0;

        if (isBelow && resize) {
            var newItems = new T[newSize.W][][][];

            for (var w = 0; w < newSize.W; w++)
                newItems[w] = Create3DArray(newSize.X, newSize.Y, newSize.Z);

            var newWOffset = Math.Max(0, offset.W);
            var newXOffset = Math.Max(0, offset.X);
            var newYOffset = Math.Max(0, offset.Y);
            var newZOffset = Math.Max(0, offset.Z);

            var oldWOffset = -Math.Min(0, offset.W);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);
            var oldZOffset = -Math.Min(0, offset.Z);

            // Copy old
            for (var w = 0; w < WSize; w++) {
                var rowW = Items[w];
                var dstW = newItems[w + oldWOffset];

                for (var x = 0; x < XSize; x++) {
                    var rowWX = rowW[x];
                    var dstWX = dstW[x + oldXOffset];

                    for (var y = 0; y < YSize; y++)
                        Array.Copy(rowWX[y], 0, dstWX[y + oldYOffset], oldZOffset, ZSize);
                }
            }

            // Copy new
            for (var w = 0; w < matrix.WSize; w++) {
                var srcW = matrix.Items[w];
                var dstW = newItems[w + newWOffset];

                for (var x = 0; x < matrix.XSize; x++) {
                    var srcWX = srcW[x];
                    var dstWX = dstW[x + newXOffset];

                    for (var y = 0; y < matrix.YSize; y++) {
                        var srcWXY = srcWX[y];
                        var dstWXY = dstWX[y + newYOffset];

                        for (var z = 0; z < matrix.ZSize; z++) {
                            var val = srcWXY[z];

                            if (predicate(dstWXY[z + newZOffset], val))
                                dstWXY[z + newZOffset] = val;
                        }
                    }
                }
            }

            Items = newItems;
            WSize = newSize.W;
            XSize = newSize.X;
            YSize = newSize.Y;
            ZSize = newSize.Z;
            WCapacity = WSize;
            XCapacity = XSize;
            YCapacity = YSize;
            ZCapacity = ZSize;
        }
        else {
            if ((placedMax.W > WSize || placedMax.X > XSize || placedMax.Y > YSize || placedMax.Z > ZSize) && resize)
                Expand(newSize.W, newSize.X, newSize.Y, newSize.Z);

            var startW = Math.Clamp(offset.W, 0, WSize);
            var endW = Math.Min(WSize, offset.W + matrix.WSize);
            var startX = Math.Clamp(offset.X, 0, XSize);
            var endX = Math.Min(XSize, offset.X + matrix.XSize);
            var startY = Math.Clamp(offset.Y, 0, YSize);
            var endY = Math.Min(YSize, offset.Y + matrix.YSize);
            var startZ = Math.Clamp(offset.Z, 0, ZSize);
            var endZ = Math.Min(ZSize, offset.Z + matrix.ZSize);

            for (var w = startW; w < endW; w++) {
                var dstW = Items[w];
                var srcW = matrix.Items[w - offset.W];

                for (var x = startX; x < endX; x++) {
                    var dstWX = dstW[x];
                    var srcWX = srcW[x - offset.X];

                    for (var y = startY; y < endY; y++) {
                        var dstWXY = dstWX[y];
                        var srcWXY = srcWX[y - offset.Y];

                        for (var z = startZ; z < endZ; z++) {
                            var val = srcWXY[z - offset.Z];

                            if (predicate(dstWXY[z], val))
                                dstWXY[z] = val;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Fills the entire 4D list with the specified value.
    /// </summary>
    /// <param name="item">The value to fill the 4D list with.</param>
    public void Fill(T item) {
        for (var w = 0; w < WSize; w++)
        for (var x = 0; x < XSize; x++)
        for (var y = 0; y < YSize; y++)
            Array.Fill(Items[w][x][y], item, 0, ZSize);
    }

    /// <summary>
    ///     Fills the specified 4D region of the list with a given value.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="wStart">The starting index on the W-axis (inclusive).</param>
    /// <param name="wCount">The number of elements to be filled along the W-axis.</param>
    /// <param name="xStart">The starting index on the X-axis (inclusive).</param>
    /// <param name="xCount">The number of elements to be filled along the X-axis.</param>
    /// <param name="yStart">The starting index on the Y-axis (inclusive).</param>
    /// <param name="yCount">The number of elements to be filled along the Y-axis.</param>
    /// <param name="zStart">The starting index on the Z-axis (inclusive).</param>
    /// <param name="zCount">The number of elements to be filled along the Z-axis.</param>
    /// <exception cref="IndexOutOfRangeException">
    ///     Thrown when the specified region exceeds the bounds of the list or one of the count parameters is negative.
    /// </exception>
    public void Fill(T item, int wStart, int wCount, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        var wEnd = wStart + wCount;
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (wStart < 0     || xStart < 0     || yStart < 0     || zStart < 0     ||
            wEnd   > WSize || xEnd   > XSize || yEnd   > YSize || zEnd   > ZSize ||
            wCount < 0     || xCount < 0     || yCount < 0     || zCount < 0)
            throw new IndexOutOfRangeException();

        for (var w = wStart; w < wEnd; w++)
        for (var x = xStart; x < xEnd; x++)
        for (var y = yStart; y < yEnd; y++)
            Array.Fill(Items[w][x][y], item, zStart, zCount);
    }

    /// <summary>
    ///     Fills the 4D list with the specified value in the given region.
    /// </summary>
    /// <param name="item">The value to fill the region with.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(T item, Point4D offset, Size4D size) => Fill(item, offset.W, size.W, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>
    ///     Fills the entire 4D list with the values generated by the specified factory function.
    /// </summary>
    /// <param name="factory">A function that generates values to fill the 4D list.</param>
    public void Fill(Func<T> factory) {
        for (var w = 0; w < WSize; w++) {
            var rowW = Items[w];

            for (var x = 0; x < XSize; x++) {
                var rowWX = rowW[x];

                for (var y = 0; y < YSize; y++) {
                    var rowWXY = rowWX[y];

                    for (var z = 0; z < ZSize; z++)
                        rowWXY[z] = factory();
                }
            }
        }
    }

    /// <summary>
    ///     Fills the 4D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="wStart">Start w coordinate of the filled region.</param>
    /// <param name="wCount">The number of units in w-axis to be filled in the filled region.</param>
    /// <param name="xStart">Start x coordinate of the filled region.</param>
    /// <param name="xCount">The number of units in x-axis to be filled in the filled region.</param>
    /// <param name="yStart">Start y coordinate of the filled region.</param>
    /// <param name="yCount">The number of units in y-axis to be filled in the filled region.</param>
    /// <param name="zStart">Start z coordinate of the filled region.</param>
    /// <param name="zCount">The number of units in z-axis to be filled in the filled region.</param>
    /// <exception cref="IndexOutOfRangeException">
    ///     Thrown when the specified region exceeds the bounds of the list or one of the count parameters is negative.
    /// </exception>
    public void Fill(Func<T> factory, int wStart, int wCount, int xStart, int xCount, int yStart, int yCount, int zStart, int zCount) {
        var wEnd = wStart + wCount;
        var xEnd = xStart + xCount;
        var yEnd = yStart + yCount;
        var zEnd = zStart + zCount;

        if (wStart < 0     || xStart < 0     || yStart < 0     || zStart < 0     ||
            wEnd   > WSize || xEnd   > XSize || yEnd   > YSize || zEnd   > ZSize ||
            wCount < 0     || xCount < 0     || yCount < 0     || zCount < 0)
            throw new IndexOutOfRangeException();

        for (var w = wStart; w < wEnd; w++) {
            var rowW = Items[w];

            for (var x = xStart; x < xEnd; x++) {
                var rowWX = rowW[x];

                for (var y = yStart; y < yEnd; y++) {
                    var rowWXY = rowWX[y];

                    for (var z = zStart; z < zEnd; z++)
                        rowWXY[z] = factory();
                }
            }
        }
    }

    /// <summary>
    ///     Fills the 4D list with the values generated by the specified factory function in the given region.
    /// </summary>
    /// <param name="factory">The factory method to be used when creating new objects.</param>
    /// <param name="offset">The offset of the region from the [0, 0, 0, 0] coordinates.</param>
    /// <param name="size">The size of the region.</param>
    public void Fill(Func<T> factory, Point4D offset, Size4D size) => Fill(factory, offset.W, size.W, offset.X, size.X, offset.Y, size.Y, offset.Z, size.Z);

    /// <summary>
    ///     Converts the 4D list into a four-dimensional array. (Creates a copy)
    /// </summary>
    /// <returns>A four-dimensional array containing the elements of the 4D list.</returns>
    [Pure]
    public T[,,,] ToArray() {
        var arr = new T[WSize, XSize, YSize, ZSize];

        if (WSize == 0 || XSize == 0 || YSize == 0 || ZSize == 0)
            return arr;

        for (var w = 0; w < WSize; w++) {
            var rowW = Items[w];

            for (var x = 0; x < XSize; x++) {
                var rowWX = rowW[x];

                for (var y = 0; y < YSize; y++) {
                    var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref rowWX[y][0], ZSize);
                    var dstSpan = MemoryMarshal.CreateSpan(ref arr[w, x, y, 0], ZSize);
                    srcSpan.CopyTo(dstSpan);
                }
            }
        }

        return arr;
    }

    /// <summary>
    ///     Converts the 4D list into a jagged array. (Creates a copy)
    /// </summary>
    /// <returns>A jagged array representation of the 4D list.</returns>
    [Pure]
    public T[][][][] ToJagged() {
        var arr = new T[WSize][][][];

        for (var w = 0; w < WSize; w++) {
            arr[w] = new T[XSize][][];

            for (var x = 0; x < XSize; x++) {
                arr[w][x] = new T[YSize][];

                for (var y = 0; y < YSize; y++) {
                    arr[w][x][y] = new T[ZSize];
                    Array.Copy(Items[w][x][y], arr[w][x][y], ZSize);
                }
            }
        }

        return arr;
    }

    private T[][][] Create3DArray(int x, int y, int z) {
        var arr = new T[x][][];

        for (var i = 0; i < x; i++)
            arr[i] = Create2DArray(y, z);

        return arr;
    }

    private T[][] Create2DArray(int y, int z) {
        var arr = new T[y][];

        for (var i = 0; i < y; i++)
            arr[i] = new T[z];

        return arr;
    }

    private class SliceW : IEnumerable3D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _w;

        public SliceW(List4DJagged<T> parent, int w) {
            _parent = parent;
            _w = w;
        }

        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (var x = 0; x < _parent.XSize; x++)
                yield return GetAtX(x);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();

        public IEnumerable2D<T> GetAtX(int x) => new SliceWX(_parent, _w, x);
        public IEnumerable2D<T> GetAtY(int y) => new SliceWY(_parent, _w, y);
        public IEnumerable2D<T> GetAtZ(int z) => new SliceWZ(_parent, _w, z);

        IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);
    }

    private class SliceX : IEnumerable3D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _x;

        public SliceX(List4DJagged<T> parent, int x) {
            _parent = parent;
            _x = x;
        }

        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.GetEnumerator() => GetEnumerator();

        public IEnumerable2D<T> GetAtX(int w) => new SliceWX(_parent, w, _x);
        public IEnumerable2D<T> GetAtY(int y) => new SliceXY(_parent, _x, y);
        public IEnumerable2D<T> GetAtZ(int z) => new SliceXZ(_parent, _x, z);

        IEnumerable2D IEnumerable3D.GetAtX(int x) => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y) => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z) => GetAtZ(z);
    }

    private class SliceY : IEnumerable3D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _y;

        public SliceY(List4DJagged<T> parent, int y) {
            _parent = parent;
            _y = y;
        }

        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.  GetEnumerator() => GetEnumerator();
        public IEnumerable2D<T>     GetAtX(int w)   => new SliceWY(_parent, w, _y);
        public IEnumerable2D<T>     GetAtY(int x)   => new SliceXY(_parent, x, _y);
        public IEnumerable2D<T>     GetAtZ(int z)   => new SliceYZ(_parent, _y, z);
        IEnumerable2D IEnumerable3D.GetAtX(int x)   => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y)   => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z)   => GetAtZ(z);
    }

    private class SliceZ : IEnumerable3D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _z;

        public SliceZ(List4DJagged<T> parent, int z) {
            _parent = parent;
            _z = z;
        }

        public IEnumerator<IEnumerable2D<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable3D.  GetEnumerator() => GetEnumerator();
        public IEnumerable2D<T>     GetAtX(int w)   => new SliceWZ(_parent, w, _z);
        public IEnumerable2D<T>     GetAtY(int x)   => new SliceXZ(_parent, x, _z);
        public IEnumerable2D<T>     GetAtZ(int y)   => new SliceYZ(_parent, y, _z);
        IEnumerable2D IEnumerable3D.GetAtX(int x)   => GetAtX(x);
        IEnumerable2D IEnumerable3D.GetAtY(int y)   => GetAtY(y);
        IEnumerable2D IEnumerable3D.GetAtZ(int z)   => GetAtZ(z);
    }

    private class SliceWX : IEnumerable2D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _w, _x;

        public SliceWX(List4DJagged<T> parent, int w, int x) {
            _parent = parent;
            _w = w;
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
                yield return _parent.Items[_w][_x][y][z];
        }

        public IEnumerable<T>     GetAtX(int x) => GetAtY(x);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceWY : IEnumerable2D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _w, _y;

        public SliceWY(List4DJagged<T> parent, int w, int y) {
            _parent = parent;
            _w = w;
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
                yield return _parent.Items[_w][x][_y][z];
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceWZ : IEnumerable2D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _w, _z;

        public SliceWZ(List4DJagged<T> parent, int w, int z) {
            _parent = parent;
            _w = w;
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
                yield return _parent.Items[_w][x][y][_z];
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceXY : IEnumerable2D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _x, _y;

        public SliceXY(List4DJagged<T> parent, int x, int y) {
            _parent = parent;
            _x = x;
            _y = y;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int w) {
            for (var z = 0; z < _parent.ZSize; z++)
                yield return _parent.Items[w][_x][_y][z];
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceXZ : IEnumerable2D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _x, _z;

        public SliceXZ(List4DJagged<T> parent, int x, int z) {
            _parent = parent;
            _x = x;
            _z = z;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int w) {
            for (var y = 0; y < _parent.YSize; y++)
                yield return _parent.Items[w][_x][y][_z];
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    private class SliceYZ : IEnumerable2D<T> {
        private readonly List4DJagged<T> _parent;
        private readonly int             _y, _z;

        public SliceYZ(List4DJagged<T> parent, int y, int z) {
            _parent = parent;
            _y = y;
            _z = z;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator() {
            for (var w = 0; w < _parent.WSize; w++)
                yield return GetAtX(w);
        }

        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable2D.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetAtX(int w) {
            for (var x = 0; x < _parent.XSize; x++)
                yield return _parent.Items[w][x][_y][_z];
        }

        public IEnumerable<T>     GetAtY(int y) => GetAtX(y);
        IEnumerable IEnumerable2D.GetAtX(int x) => GetAtX(x);
        IEnumerable IEnumerable2D.GetAtY(int y) => GetAtY(y);
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    public T[] this[Range w, int x, int y, int z] {
        get {
            var (offset, length) = w.GetOffsetAndLength(WSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = Items[offset + i][x][y][z];

            return result;
        }
    }

    public T[] this[int w, Range x, int y, int z] {
        get {
            var (offset, length) = x.GetOffsetAndLength(XSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = Items[w][offset + i][y][z];

            return result;
        }
    }

    public T[] this[int w, int x, Range y, int z] {
        get {
            var (offset, length) = y.GetOffsetAndLength(YSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = Items[w][x][offset + i][z];

            return result;
        }
    }

    public T[] this[int w, int x, int y, Range z] {
        get {
            var (offset, length) = z.GetOffsetAndLength(ZSize);
            var result = new T[length];

            for (var i = 0; i < length; i++)
                result[i] = Items[w][x][y][offset + i];

            return result;
        }
    }
    #endif
}

#endif