// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Metriks;

public class List2D<T> : IList2D<T> {

    private const int INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR = 2f;

    /// <summary>
    /// Represents the underlying two-dimensional array used to store the elements of the List2D instance.
    /// </summary>
    /// <remarks> 
    /// This variable is a jagged array (array of arrays), where each subarray represents a column in the
    /// two-dimensional structure. The size of the jagged array is determined by the x-capacity and
    /// y-capacity of the List2D instance. It is used internally
    /// to store and manage data in the two-dimensional list.
    /// The first index is the x-index, the second index is the y-index.
    /// </remarks>
    private T[][] _items;

    protected void UnsafeSet(int x, int y, T value) => _items[x][y] = value;
    protected T UnsafeGet(int x, int y) => _items[x][y];

    private int _xSize;
    private int _ySize;
    private int _xCapacity;
    private int _yCapacity;

    public bool IsReadonly => false;

    public List2D(int xCapacity = INITIAL_CAPACITY, int yCapacity = INITIAL_CAPACITY) {
        _items     = new T[xCapacity][];
        _xSize     = 0;
        _ySize     = 0;
        _xCapacity = xCapacity;
        _yCapacity = yCapacity;
    }

    public List2D(T[,] collection) : this(collection.Len0, collection.Len1) {
        for (int x = 0; x < collection.Len0; x++) {
            _items[x] = new T[collection.Len1];
            
            for (int y = 0; y < collection.Len1; y++) {
                _items[x][y] = collection[x, y];
            }
        }
        
        _xSize = collection.Len0;
        _ySize = collection.Len1;
    }
    
    
    public int XSize {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _xSize;
    }

    public int YSize {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _ySize;
    }

    public Size2D Size {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_xSize, _ySize);
    }

    public int XCapacity {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _xCapacity;
    }

    public int YCapacity {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _yCapacity;
    }

    public T this[int x, int y] {
        get {
            if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            return _items[x][y];
        }
        set {
            if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            _items[x][y] = value;
        }
    }
    
    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    
    public T this[Index x, Index y] {
        get {
            int xo = x.GetOffset(_xSize);
            int yo = y.GetOffset(_ySize);
            if (xo < 0 || xo >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            return _items[xo][yo];
        }
        set {
            int xo = x.GetOffset(_xSize);
            int yo = y.GetOffset(_ySize);
            if (xo < 0 || xo >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            _items[xo][yo] = value;
        }
    }
    
    #endif

    /// <summary>
    /// Inserts a new column at the specified index in the 2D list.
    /// </summary>
    /// <param name="x">The zero-based index at which the new column should be inserted.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified index is less than 0 or greater than the current XSize.
    /// </exception>
    public void InsertAtX(int x) {
        if (x < 0 || x > _xSize) 
            throw new IndexOutOfRangeException("Index 'x' is out of range.");

        if (_xSize + 1 >= _xCapacity) {
            _xCapacity = (int)(_xCapacity * GROWTH_FACTOR);
        }
        
        var newMatrix = new T[_xCapacity][];
        
        Array.Copy(_items, newMatrix, x);                    // copies elements up to x
        newMatrix[x] = new T[_yCapacity];                    // creates a new array at x
        Array.Copy(_items, x, newMatrix, x + 1, _xSize - x); // copies elements from x to the end of the new array

        _xSize++;
        _items = newMatrix;                                  // set the new matrix
    }

    /// <summary>
    /// Inserts a new row at the specified index in the 2D list.
    /// </summary>
    /// <param name="y">The zero-based index at which the new row should be inserted.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified index is less than 0 or greater than the current YSize.
    /// </exception>
    public void InsertAtY(int y) {
        if (y < 0 || y > _ySize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");
        
        if (_ySize + 1 >= _yCapacity) {
            _yCapacity = (int)(_yCapacity * GROWTH_FACTOR);
        }

        for (int x = 0; x < _xSize; x++) {
            var newArray = new T[_yCapacity];
            Array.Copy(_items[x], newArray, y);                    // copies elements up to y
            Array.Copy(_items[x], y, newArray, y + 1, _ySize - y); // copies elements from y to the end
            _items[x] = newArray;                                  // sets the new array at y
        }
        
        _ySize++;
    }

    /// <summary>
    /// Adds a new column to the two-dimensional list, increasing the x-size by 1.
    /// </summary>
    /// <remarks>
    /// If the current x-size exceeds or equals the x-capacity after adding the new column, the x-capacity is increased
    /// using a growth factor. The underlying matrix is resized to accommodate the increased capacity, and the existing
    /// elements are copied to the new matrix.
    /// </remarks>
    public void AddX() {
        if (_xSize >= _xCapacity) {
            _xCapacity = (int)(_xCapacity * GROWTH_FACTOR);
            
            var newMatrix = new T[_xCapacity][];
        
            Array.Copy(_items, newMatrix, _xSize);
        
            _items = newMatrix;
        }
         
        _items[_xSize] = new T[_yCapacity]; 
        _xSize++;
    }

    /// <summary>
    /// Adds a new row at the end of the 2D list, increasing its YSize by 1.
    /// </summary>
    /// <remarks>
    /// If adding a new row exceeds the current YCapacity, the capacity is increased
    /// using a predefined growth factor, and the existing data is reallocated to fit the new capacity.
    /// </remarks>
    public void AddY() {
        if (_ySize >= _yCapacity) {
            _yCapacity = (int)(_yCapacity * GROWTH_FACTOR);

            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_yCapacity];
                
                Array.Copy(_items[x], newArray, _ySize);
                
                _items[x] = newArray;
            }
        }
        
        _ySize++;
    }

    /// <summary>
    /// Expands the size of the 2D list to the specified dimensions.
    /// </summary>
    /// <param name="xSize">The new number of columns (X-dimension) for the 2D list. Must be
    /// greater than or equal to the current XSize.</param>
    /// <param name="ySize">The new number of rows (Y-dimension) for the 2D list. Must be
    /// greater than or equal to the current YSize.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified xSize is less than the current XSize or
    /// the specified ySize is less than the current YSize.
    /// </exception>
    public void Expand(int xSize, int ySize) {
        if (xSize < _xSize) throw new ArgumentOutOfRangeException(nameof(xSize), "Cannot shrink the XSize of a List2D.");
        if (ySize < _ySize) throw new ArgumentOutOfRangeException(nameof(ySize), "Cannot shrink the YSize of a List2D.");

        if (xSize == _xSize && ySize == _ySize) return;
        
        if (xSize > _xCapacity) {
            _xCapacity = xSize + 1;
            
            var newMatrix = new T[_xCapacity][];
            Array.Copy(_items, newMatrix, _xSize);
            _items = newMatrix;
        }

        if (ySize > _yCapacity) {
            _yCapacity = ySize + 1;

            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_yCapacity];
                Array.Copy(_items[x], newArray, _ySize);
                _items[x] = newArray;
            }
        }

        for (int x = _xSize; x < xSize; x++) {
            _items[x] = new T[_yCapacity];
        }

        _xSize = xSize;
        _ySize = ySize;
    }

    public void RemoveAtX(int x) {
        if (x < 0 || x >= _xSize) 
            throw new IndexOutOfRangeException("Index 'x' is out of range.");
        
        if (_xSize == 0) 
            throw new InvalidOperationException("Cannot remove from a List2D with x-size = 0.");
        
        _xSize--;
        
        var newMatrix = new T[_xSize][];
        
        Array.Copy(_items, newMatrix, x);
        Array.Copy(_items, x + 1, newMatrix, x, _xSize - x);
        
        _items = newMatrix;
    }
    
    public void RemoveAtY(int y) {
        if (y < 0 || y >= _ySize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");

        if (_ySize == 0) 
            throw new InvalidOperationException("Cannot remove from a List2D with y-size = 0.");
        
        _ySize--;

        for (int x = 0; x < _xSize; x++) {
            var newArray = new T[_yCapacity];
            Array.Copy(_items[x], newArray, y);
            Array.Copy(_items[x], y + 1, newArray, y, _ySize - y);
            _items[x] = newArray;
        }
    }

    /// <summary>
    /// Removes the last column from the 2D list, reducing its XSize by one.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to remove a column from an empty 2D list.
    /// </exception>
    public void ShrinkX() {
        if (_xSize == 0) 
            throw new InvalidOperationException("Cannot remove from a List2D with x-size = 0.");
        
        _xSize--;
        
        _items[_xSize] = null!;
    }

    /// <summary>
    /// Removes the last row (Y-dimension) from the 2D list.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to remove a row from an empty 2D list.
    /// </exception>
    public void ShrinkY() {
        if (_ySize == 0)
            throw new InvalidOperationException("Cannot remove from a List2D with y-size = 0.");

        _ySize--;

        for (int x = 0; x < _xSize; x++) {
            _items[x][_ySize] = default!;
        }
    }

    /// <summary>
    /// Determines whether all elements in the specified column satisfy the given predicate.
    /// </summary>
    /// <param name="x">The zero-based index of the column to check.</param>
    /// <param name="predicate">The predicate to evaluate for each element in the column.</param>
    /// <returns>
    /// True if all elements in the column satisfy the predicate; otherwise, false.
    /// </returns>
    public bool AllAtX(int x, Predicate<T> predicate) {
        for (int y = 0; y < _ySize; y++) {
            if (!predicate(_items[x][y])) return false;
        }
        
        return true;
    }

    /// <summary>
    /// Determines whether all elements in the specified row of the 2D list satisfy a given condition.
    /// </summary>
    /// <param name="y">The zero-based index of the row to evaluate.</param>
    /// <param name="predicate">The condition to evaluate against each element in the row.</param>
    /// <returns>
    /// True if all elements in the specified row satisfy the condition; otherwise, false.
    /// </returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified row index is less than 0 or greater than or equal to the current YSize.
    /// </exception>
    public bool AllAtY(int y, Predicate<T> predicate) {
        for (int x = 0; x < _xSize; x++) {
            if (!predicate(_items[x][y])) return false;
        }
        
        return true;
    }


    /// <summary>
    /// Retrieves all elements from the specified column in the 2D list.
    /// </summary>
    /// <param name="x">The zero-based index of the column to retrieve elements from.</param>
    /// <returns>An enumerable collection of elements from the specified column.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified column index is less than 0 or greater than or equal to the current XSize.
    /// </exception>
    [Pure]
    public IEnumerable<T> GetAtX(int x) {
        for (var y = 0; y < _ySize; y++) {
            yield return _items[x][y];
        }
    }

    
    /// <summary>
    /// Retrieves all elements at the specified row (Y-coordinate) in the 2D list.
    /// </summary>
    /// <param name="y">The zero-based index of the row to retrieve elements from.</param>
    /// <returns>An enumerable collection of elements at the specified row.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified index is less than 0 or greater than or equal to the current YSize.
    /// </exception>
    [Pure]
    public IEnumerable<T> GetAtY(int y) {
        for (int x = 0; x < _xSize; x++) {
            yield return _items[x][y];
        }
    }

    
    /// <summary>
    /// Places a 2D matrix into this List2D at the specified offset. If the matrix extends beyond
    /// the current bounds of the List2D, the List2D is resized accordingly.
    /// </summary>
    /// <param name="matrix">The 2D array of elements to place into this List2D.</param>
    /// <param name="offsetPoint">
    /// An optional offset defining where the top-left corner of the matrix will be placed.
    /// If not provided, the matrix will be placed at the origin of the List2D.
    /// </param>
    public void Place(T[,] matrix, Point2D? offsetPoint = null) {
        var offset = offsetPoint ?? Point2D.Empty;

        var placedMax = offset + matrix.Size;

        var max = new Point(Math.Max(_xSize, placedMax.X), Math.Max(_ySize, placedMax.Y));
        var min = new Point(Math.Min(offset.X, 0), Math.Min(offset.Y, 0));
        
        var newSize = new Size(max.X - min.X, max.Y - min.Y);

        var isBelow = offset.X < 0 || offset.Y < 0;

        if (isBelow) {
            var newMatrix = new T[newSize.Width][];
            
            // copy the existing matrix
            for (int x = 0; x < newSize.Width; x++) {
                newMatrix[x] = new T[newSize.Height];
            }

            var newXOffset =  Math.Max(0, offset.X);
            var newYOffset =  Math.Max(0, offset.Y);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);

            // copy existing matrix
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    newMatrix[x + oldXOffset][y + oldYOffset] = _items[x][y];
                }
            }

            // copy input into the new matrix
            for (int x = 0; x < matrix.Len0; x++) {
                for (int y = 0; y < matrix.Len1; y++) {
                    newMatrix[x + newXOffset][y + newYOffset] = matrix[x, y];   
                }
            }
            
            // set _matrix to the new matrix and update size data
            _items     = newMatrix;
            _xSize     = newSize.Width;
            _ySize     = newSize.Height;
            _xCapacity = _xSize;
            _yCapacity = _ySize;
        }
        else {
            
            if (placedMax.X > _xSize || placedMax.Y > _ySize) {
                Expand(newSize.Width, newSize.Height);
            }
            
            // copy input into _matrix
            for (int x = 0; x < matrix.Len0; x++) {
                for (int y = 0; y < matrix.Len1; y++) {
                    _items[x + offset.X][y + offset.Y] = matrix[x, y];
                }
            }
        }
    }


    /// <summary>
    /// Places the contents of the specified 2D list into the current List2D instance, optionally offset by a specified point.
    /// </summary>
    /// <param name="matrix">The List2D instance containing the elements to be placed.</param>
    /// <param name="offsetPoint">
    /// An optional point specifying the offset at which the matrix should be placed.
    /// If null, the matrix will be placed starting at the origin (0, 0).
    /// </param>
    public void Place(List2D<T> matrix, Point2D? offsetPoint = null) {
        var offset = offsetPoint ?? Point2D.Empty;

        var placedMax = offset + matrix.Size;

        var max = new Point(Math.Max(_xSize, placedMax.X), Math.Max(_ySize, placedMax.Y));
        var min = new Point(Math.Min(offset.X, 0), Math.Min(offset.Y, 0));
        
        var newSize = new Size(max.X - min.X, max.Y - min.Y);

        var isBelow = offset.X < 0 || offset.Y < 0;

        if (isBelow) {
            var newMatrix = new T[newSize.Width][];
            
            // copy the existing matrix
            for (int x = 0; x < newSize.Width; x++) {
                newMatrix[x] = new T[newSize.Height];
            }

            var newXOffset =  Math.Max(0, offset.X);
            var newYOffset =  Math.Max(0, offset.Y);
            var oldXOffset = -Math.Min(0, offset.X);
            var oldYOffset = -Math.Min(0, offset.Y);

            // copy existing matrix
            for (int x = 0; x < _xSize; x++) {
                Array.Copy(_items[x], 0, newMatrix[x + oldXOffset], oldYOffset, _ySize);
            }

            // copy input into the new matrix
            for (int x = 0; x < matrix._ySize; x++) {
                Array.Copy(matrix._items[x], 0, newMatrix[x + newXOffset], newYOffset, matrix._ySize);
            }
            
            // set _matrix to the new matrix and update size data
            _items     = newMatrix;
            _xSize     = newSize.Width;
            _ySize     = newSize.Height;
            _xCapacity = _xSize;
            _yCapacity = _ySize;
        }
        else {
            
            if (placedMax.X > _xSize || placedMax.Y > _ySize) {
                Expand(newSize.Width, newSize.Height);
            }
            
            // copy input into _matrix
            for (int x = 0; x < matrix._xSize; x++) {
                Array.Copy(matrix._items[x], 0, _items[x + offset.X], offset.Y, matrix._ySize);
            }
        }
    }

    /// <summary>
    /// Clears all elements from the 2D list, resetting its size and capacity to their initial values.
    /// </summary>
    /// <remarks>
    /// After invoking this method, the 2D list will be empty with its capacity set to the default initial capacity.
    /// </remarks>
    public void Clear() {
        _xSize     = 0;
        _ySize     = 0;
        _xCapacity = INITIAL_CAPACITY;
        _yCapacity = INITIAL_CAPACITY;
        _items     = new T[INITIAL_CAPACITY][];
    }

    public IEnumerator<IEnumerable<T>> GetEnumerator() {
        for (int x = 0; x < _xSize; x++)
            yield return GetAtX(x);
    }
    
    IEnumerator<IEnumerable> IEnumerable2D.GetEnumerator() => GetEnumerator();


    /// <summary>
    /// Converts the 2D list into a two-dimensional array. (Creates a copy)
    /// </summary>
    /// <returns>A two-dimensional array containing the elements of the 2D list.</returns>
    [Pure]
    public T[,] ToArray() {
        var arr = new T[_xSize, _ySize];

        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                arr[x, y] = _items[x][y];
            }
        }
        
        return arr;
    }

    /// <summary>
    /// Converts the 2D list into a jagged array. (Creates a copy)
    /// </summary>
    /// <returns>A jagged array representation of the 2D list,
    /// where each inner array corresponds to a row of the 2D list.</returns>
    [Pure]
    public T[][] ToJagged() {
        var arr = new T[_xSize][];

        for (int x = 0; x < _xSize; x++) {
            arr[x] = new T[_ySize];
            
            for (int y = 0; y < _ySize; y++) {
                arr[x][y] = _items[x][y];
            }
        }
        
        return arr;
    }
}