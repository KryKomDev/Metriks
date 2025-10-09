// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Metriks;

public class List2D<T> : IList2D<T> {

    private const int InitialCapacity = 4;
    private const float GrowthFactor = 2f;

    /// <summary>
    /// Represents the underlying two-dimensional array used to store the elements of the List2D instance.
    /// </summary>
    /// <remarks> 
    /// This variable is a jagged array (array of arrays), where each subarray represents a row in the
    /// two-dimensional structure. The size of the jagged array is determined by the x-capacity and
    /// y-capacity of the List2D instance. It is used internally
    /// to store and manage data in the two-dimensional list.
    /// The first index is the x-index, the second index is the y-index.
    /// </remarks>
    private T[][] _matrix;

    private int _xSize;
    private int _ySize;
    private int _xCapacity;
    private int _yCapacity;

    public List2D(int xCapacity = InitialCapacity, int yCapacity = InitialCapacity) {
        _matrix   = new T[yCapacity][];
        
        _xSize     = 0;
        _ySize     = 0;
        _xCapacity = xCapacity;
        _yCapacity = yCapacity;
    }

    public List2D(T[,] collection) : this(collection.GetLength(0), collection.GetLength(1)) {
        for (int x = 0; x < collection.GetLength(0); x++) {
            for (int y = 0; y < collection.GetLength(1); y++) {
                _matrix[x][y] = collection[x, y];
            }
        }
    }
    
    public int XSize => _xSize;
    public int YSize => _ySize;
    public int XCapacity => _xCapacity;
    public int YCapacity => _yCapacity;

    public T this[int x, int y] {
        get {
            if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            return _matrix[x][y];
        }
        set {
            if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            _matrix[x][y] = value;
        }
    }
    
    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    
    public T this[Index x, Index y] {
        get {
            int xo = x.GetOffset(_xSize);
            int yo = y.GetOffset(_ySize);
            if (xo < 0 || xo >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            return _matrix[xo][yo];
        }
        set {
            int xo = x.GetOffset(_xSize);
            int yo = y.GetOffset(_ySize);
            if (xo < 0 || xo >= _xSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= _ySize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            _matrix[xo][yo] = value;
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
    public void InsertXAt(int x) {
        if (x < 0 || x > _xSize) 
            throw new IndexOutOfRangeException("Index 'x' is out of range.");
        
        _xSize++;

        if (_xSize >= _xCapacity) {
            _xCapacity = (int)(_xCapacity * GrowthFactor);
        }
        
        var newMatrix = new T[_xCapacity][];
        
        Array.Copy(_matrix, newMatrix, x);
        Array.Copy(_matrix, x, newMatrix, x + 1, _xSize - x);
        
        newMatrix[x] = new T[_yCapacity];
        
        _matrix = newMatrix;
    }

    /// <summary>
    /// Inserts a new row at the specified index in the 2D list.
    /// </summary>
    /// <param name="y">The zero-based index at which the new row should be inserted.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified index is less than 0 or greater than the current YSize.
    /// </exception>
    public void InsertYAt(int y) {
        if (y < 0 || y > _ySize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");
        
        _ySize++;

        if (_ySize >= _yCapacity) {
            _yCapacity = (int)(_yCapacity * GrowthFactor);
        }

        for (int x = 0; x < _xSize; x++) {
            var newArray = new T[_yCapacity];
            Array.Copy(_matrix[x], newArray, y);
            Array.Copy(_matrix[x], y, newArray, y + 1, _ySize - y);
            _matrix[x] = newArray;
        }
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
        _xSize++;
        
        if (_xSize >= _xCapacity) {
            _xCapacity = (int)(_xCapacity * GrowthFactor);
            
            var newMatrix = new T[_xCapacity][];
        
            Array.Copy(_matrix, newMatrix, _xSize - 1);
        
            _matrix = newMatrix;
        }
         
        _matrix[_xSize - 1] = new T[_yCapacity]; 
    }

    /// <summary>
    /// Adds a new row at the end of the 2D list, increasing its YSize by 1.
    /// </summary>
    /// <remarks>
    /// If adding a new row exceeds the current YCapacity, the capacity is increased
    /// using a predefined growth factor, and the existing data is reallocated to fit the new capacity.
    /// </remarks>
    public void AddY() {
        _ySize++;

        if (_ySize >= _yCapacity) {
            _yCapacity = (int)(_yCapacity * GrowthFactor);

            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_yCapacity];
                
                Array.Copy(_matrix[x], newArray, _ySize - 1);
                
                _matrix[x] = newArray;
            }
        }
    }

    public void Expand(int xSize, int ySize) {
        if (xSize < _xSize) throw new ArgumentOutOfRangeException(nameof(xSize), "Cannot shrink the XSize of a List2D.");
        if (ySize < _ySize) throw new ArgumentOutOfRangeException(nameof(ySize), "Cannot shrink the YSize of a List2D.");

        if (xSize > _xCapacity) {
            _xCapacity = xSize + 1;
            
            var newMatrix = new T[_xCapacity][];
            Array.Copy(_matrix, newMatrix, _xSize);
            _matrix = newMatrix;
        }

        if (ySize > _yCapacity) {
            _yCapacity = ySize + 1;

            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_yCapacity];
                Array.Copy(_matrix[x], newArray, _ySize);
                _matrix[x] = newArray;
            }
        }

        for (int x = _xSize; x < xSize; x++) {
            _matrix[x] = new T[_yCapacity];
        }

        _xSize = xSize;
        _ySize = ySize;
    }

    public void RemoveXAt(int x) {
        if (x < 0 || x >= _xSize) 
            throw new IndexOutOfRangeException("Index 'x' is out of range.");
        
        _xSize--;
        
        var newMatrix = new T[_xSize][];
        
        Array.Copy(_matrix, newMatrix, x);
        Array.Copy(_matrix, x + 1, newMatrix, x, _xSize - x);
        
        _matrix = newMatrix;
    }
    
    public void RemoveYAt(int y) {
        if (y < 0 || y >= _ySize)
            throw new IndexOutOfRangeException("Index 'y' is out of range.");

        _ySize--;

        for (int x = 0; x < _xSize; x++) {
            var newArray = new T[_yCapacity];
            Array.Copy(_matrix[x], newArray, y);
            Array.Copy(_matrix[x], y + 1, newArray, y, _ySize - y);
            _matrix[x] = newArray;
        }
    }

    /// <summary>
    /// Removes the last column from the 2D list, reducing its XSize by one.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to remove a column from an empty 2D list.
    /// </exception>
    public void RemoveX() {
        if (_xSize == 0) throw new InvalidOperationException("Cannot remove X from an empty List2D.");
        
        _xSize--;
        
        _matrix[_xSize] = null!;
    }

    /// <summary>
    /// Removes the last row (Y-dimension) from the 2D list.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to remove a row from an empty 2D list.
    /// </exception>
    public void RemoveY() {
        if (_ySize == 0) throw new InvalidOperationException("Cannot remove Y from an empty List2D.");

        _ySize--;

        for (int x = 0; x < _xSize; x++) {
            _matrix[x][_ySize] = default!;
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
            if (!predicate(_matrix[x][y])) return false;
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
            if (!predicate(_matrix[x][y])) return false;
        }
        
        return true;
    }

    public IEnumerator<IEnumerable<T>> GetEnumerator() => _matrix.Cast<IEnumerable<T>>().GetEnumerator();
    IEnumerator<IEnumerable> IEnumerable2D.GetEnumerator() => GetEnumerator();
}