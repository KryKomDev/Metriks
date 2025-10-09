//
// Metriks
//  Copyright (c) MIT License 2025, KryKom & ZlomenyMesic
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Metriks;

/// <summary>
/// 2D dynamic array structure.
/// </summary>
public class List2DOld<T> : IEnumerable<T>, IDisposable {
    
    private T[][] _matrix;

    /// <summary>
    /// The current number of rows in the matrix
    /// </summary>
    public int Rows { get; private set; }
    
    /// <summary>
    /// The current number of columns in the matrix
    /// </summary>
    public int Columns { get; private set; }
    
    /// <summary>
    /// The total number of elements in the matrix
    /// </summary>
    public int Count => Rows * Columns;

    /// <summary>
    /// Gets the element at the specified row and column index.
    /// </summary>
    /// <param name="row">The zero-based row index</param>
    /// <param name="column">The zero-based column index</param>
    public T this[in int row, in int column] {
        
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _matrix[row][column];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _matrix[row][column] = value;
    }

    /// <summary>
    /// Gets the reference to the element at the specified row and column index.
    /// </summary>
    /// <remarks>
    /// Use this indexer with extra caution. The value-based indexer (<c>this[int, int]</c>)
    /// generally provides better performance, unless working with large structs where copies
    /// need to be avoided.
    /// </remarks>
    /// <param name="index">The zero-based row and column indices</param>
    public ref T this[in (int row, int column) index]
        => ref _matrix[index.row][index.column];

    /// <summary>
    /// Initializes a new instance of the <see cref="List2DOld{T}"/> class with the specified
    /// number of rows and columns. A custom default value is allowed, too.
    /// </summary>
    /// <remarks>The default value is only used once, it is not stored for further use.</remarks>
    /// <param name="rows">The default number of rows</param>
    /// <param name="columns">The default number of columns</param>
    /// <param name="defaultValue">The default value assigned to all elements in the matrix</param>
    public List2DOld(int rows = 0, int columns = 0, T? defaultValue = default) {
        ThrowHelper.ThrowIfLt(rows, 0, new ArgumentOutOfRangeException(nameof(rows)));
        ThrowHelper.ThrowIfLt(columns, 0, new ArgumentOutOfRangeException(nameof(columns)));

        Rows    = rows;
        Columns = columns;
        _matrix = new T[rows][];
        
        for (int i = 0; i < rows; i++) {
            _matrix[i] = new T[columns];
        }

        // custom default values in the matrix
        if (defaultValue is not null && !EqualityComparer<T>.Default.Equals(defaultValue, default!))
            Fill(defaultValue);
    }

    /// <summary>
    /// Creates a new instance of <see cref="List2DOld{T}"/> with dimensions and elements
    /// corresponding to the specified two-dimensional array. 
    /// </summary>
    /// <param name="array">The template 2D array from which the instance shall be created</param>
    /// <returns>The new instance of <see cref="List2DOld{T}"/></returns>
    public static List2DOld<T> FromArray(T[,] array) {
        var matrix = new List2DOld<T>(rows:    array.GetLength(0),
                                     columns: array.GetLength(1));

        for (int i = 0; i < matrix.Rows; i++) {
            for (int j = 0; j < matrix.Columns; j++) {
                matrix[i, j] = array[j, i];
            }
        }
        return matrix;
    }

    /// <summary>
    /// Converts this instance of <see cref="List2DOld{T}"/> to a two-dimensional array with
    /// corresponding dimensions and elements.
    /// </summary>
    /// <returns>The 2D array</returns>
    public T[,] ToArray() {
        var array = new T[Rows, Columns];

        for (int i = 0; i < Rows; i++) {
            for (int j = 0; j < Columns; j++) {
                array[i, j] = _matrix[i][j];
            }
        }
        return array;
    }

    /// <summary>
    /// Creates a new instance of <see cref="List2DOld{T}"/> with dimensions and elements
    /// corresponding to the specified rectangle-shaped region in the current instance.
    /// </summary>
    /// <param name="rowStart">The starting index of the rows to be copied (inclusive)</param>
    /// <param name="rowLength">The number of rows to be copied</param>
    /// <param name="colStart">The starting index of the columns to be copied (inclusive)</param>
    /// <param name="colLength">The number of columns to be copied</param>
    /// <returns>The new instance of <see cref="List2DOld{T}"/></returns>
    public List2DOld<T> Slice(int rowStart, int rowLength, int colStart, int colLength) {
        var newMatrix = new List2DOld<T>(rowLength, colLength);

        for (int i = 0; i < rowLength; i++) {
            for (int j = 0; j < colLength; j++) {
                newMatrix[i, j] = _matrix[rowStart + i][colStart + j];
            }
        }
        return newMatrix;
    }
    
    /// <summary>
    /// Returns the row at the specified index as an array.
    /// </summary>
    /// <param name="index">The zero-based row index</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] GetRow(in int index)
        => _matrix[index];

    /// <summary>
    /// Returns the column at the specified index as an array.
    /// </summary>
    /// <param name="index">The zero-based column index</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] GetCol(in int index) {
        var column = new T[Rows];
        for (int i = 0; i < Rows; i++) {
            column[i] = _matrix[i][index];
        }
        return column;
    }

    /// <summary>
    /// Iterates over all rows of the matrix.
    /// </summary>
    /// <returns>All rows of the matrix</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<T[]> GetRows() {
        for (int i = 0; i < Rows; i++)
            yield return GetRow(i);
    }

    /// <summary>
    /// Iterates over all columns of the matrix.
    /// </summary>
    /// <returns>All columns of the matrix</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<T[]> GetCols() {
        for (int i = 0; i < Columns; i++)
            yield return GetCol(i);
    }
    
    /// <summary>
    /// Adds a new empty row to the bottom of the matrix.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRow() {
        Array.Resize(ref _matrix, Rows + 1);
        _matrix[Rows - 1] = new T[Columns];
        
        Rows++;
    }
    
    /// <summary>
    /// Adds a new row with the specified values to the bottom of the matrix.
    /// </summary>
    /// <param name="values">The new row values; must cover the whole row</param>
    public void AddRow(in T[] values) {
        if (values.Length < Columns)
            throw new ArgumentException($"{nameof(values)} doesn't cover the whole row.");
        
        // add an empty row first
        AddRow();
        
        // assign every item in the last row the corresponding value
        for (int i = 0; i < Columns; i++) {
            _matrix[Rows - 1][i] = values[i];
        }
    }
    
    /// <summary>
    /// Adds a new empty column to the right side of the matrix.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddCol() {
        for (int i = 0; i < Rows; i++) {
            Array.Resize(ref _matrix[i], Columns + 1);
        }
        
        Columns++;
    }

    /// <summary>
    /// Adds a new column with specified values to the right side of the matrix.
    /// </summary>
    /// <param name="values">The new column values; must cover the whole column</param>
    public void AddCol(in T[] values) {
        if (values.Length < Rows)
            throw new ArgumentException($"{nameof(values)} doesn't cover the whole column.");
        
        // add an empty column first
        AddCol();

        // assign the last item of each row the corresponding value
        for (int i = 0; i < Rows; i++) {
            _matrix[i][Columns - 1] = values[i];
        }
    }

    /// <summary>
    /// Removes a whole row of the matrix at the specified index.
    /// Remaining rows are shifted accordingly, and the matrix is resized.
    /// </summary>
    /// <param name="index">The zero-based index of the row to be removed</param>
    public void RemoveRow(in int index) {
        for (int i = index; i < Rows - 1; i++) {
            _matrix[i] = _matrix[i + 1];
        }
        ResizeRows(Rows - 1);
    }

    /// <summary>
    /// Removes whole column of the matrix at the specified index.
    /// Remaining columns are shifted accordingly, and the matrix is resized.
    /// </summary>
    /// <param name="index">The zero-based index of the column to be removed</param>
    public void RemoveCol(in int index) {
        for (int i = 0; i < Rows; i++) {
            for (int j = index; j < Columns - 1; j++) {
                _matrix[i][j] = _matrix[i][j + 1];
            }
        }
        ResizeCols(Columns - 1);
    }

    /// <summary>
    /// Resizes the matrix to match the specified number of rows.
    /// </summary>
    /// <remarks>Values are kept, unless shrinking the rows, where some values obviously have to be lost.</remarks>
    /// <param name="newRows">The new number of rows</param>
    public void ResizeRows(int newRows) {
        Array.Resize(ref _matrix, newRows);

        for (int i = Rows; i < newRows; i++)
            _matrix[i] = new T[Columns];

        Rows = newRows;
    }

    /// <summary>
    /// Resizes the matrix to match the specified number of columns.
    /// </summary>
    /// <remarks>Values are kept, unless shrinking the columns, where some values obviously have to be lost.</remarks>
    /// <param name="newCols">The new number of columns</param>
    public void ResizeCols(int newCols) {
        for (int i = 0; i < Rows; i++) {
            Array.Resize(ref _matrix[i], newCols);
        }
        
        Columns = newCols;
    }

    /// <summary>
    /// Resizes the matrix to match the specified number of rows and columns.
    /// </summary>
    /// <remarks>Values are kept, unless shrinking the matrix, where some values obviously have to be lost.</remarks>
    /// <param name="newRows">The new number of rows</param>
    /// <param name="newCols">The new number of columns</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Resize(int newRows, int newCols) {
        ResizeRows(newRows);
        ResizeCols(newCols);
    }

    /// <summary>
    /// Sets every element in the matrix to its default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() {
        for (int i = 0; i < Rows; i++) {
            Array.Clear(_matrix[i], 0, _matrix[i].Length);
        }
    }

    /// <summary>
    /// Assigns the specified value to every element in the matrix.
    /// </summary>
    /// <param name="value">The value to be assigned</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(in T value) {
        for (int i = 0; i < Rows; i++) {
            Extensions.Fill(_matrix[i], value);
        }
    }

    /// <summary>
    /// Assigns the specified value to every element in the specified row of the matrix.
    /// </summary>
    /// <param name="index">The zero-based row index</param>
    /// <param name="value">The value to be assigned</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FillRow(in int index, in T value) {
        Extensions.Fill(_matrix[index], value);
    }

    /// <summary>
    /// Assigns the specified value to every element in the specified column of the matrix.
    /// </summary>
    /// <param name="index">The zero-based column index</param>
    /// <param name="value">The value to be assigned</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FillCol(in int index, in T value) {
        for (int i = 0; i < Rows; i++) {
            _matrix[i][index] = value;
        }
    }

    /// <summary>
    /// Checks whether a value is present anywhere in the matrix.
    /// </summary>
    /// <param name="value">The value to be found</param>
    /// <returns>True if the value is present, otherwise false</returns>
    public bool Contains(T value) {
        for (int i = 0; i < Rows; i++) {
            if (_matrix[i].Contains(value))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the indices of the first occurence of the specified value in the matrix.
    /// </summary>
    /// <param name="value">The value to be found</param>
    /// <returns>The indices of the first occurence, or <c>(-1, -1)</c> if the value is not present.</returns>
    public (int Row, int Column) IndexOf(T value) {
        for (int i = 0; i < Rows; i++) {
            int index = Array.IndexOf(_matrix[i], value);
            
            // if the item is present, return the indices
            if (index != -1)
                return (Row: i, Column: index);
        }

        return (Row: -1, Column: -1);
    }

    /// <summary>
    /// Performs an action on every element of the matrix.
    /// </summary>
    /// <param name="action">The action to be performed; takes the element as a parameter.</param>
    public void ForEachAction(in Action<T> action) {
        for (int i = 0; i < Rows; i++) {
            for (int j = 0; j < Columns; j++) {
                action(_matrix[i][j]);
            }
        }
    }
    
    /// <summary>
    /// Performs a function/operation on every element of the matrix.
    /// </summary>
    /// <remarks>As opposed to <see cref="ForEachAction"/>, this method modifies the matrix.</remarks>
    /// <param name="function">The function to be performed; takes the element as a parameter and returns the new value.</param>
    public void ForEachFunc(in Func<T, T> function) {
        for (int i = 0; i < Rows; i++) {
            for (int j = 0; j < Columns; j++) {
                _matrix[i][j] = function(_matrix[i][j]);
            }
        }
    }
    
    /// <summary>
    /// Creates a deep clone of this instance.
    /// </summary>
    /// <returns>The new clone instance</returns>
    public List2DOld<T> Clone() {
        var clone = new List2DOld<T>(Rows, Columns);
        
        for (int i = 0; i < Rows; i++)
            Array.Copy(_matrix[i], clone.GetRow(i), Columns);

        return clone;
    }
    
#if DEBUG
    public void PrintContents() {
        for (int i = 0; i < Rows; i++) {
            foreach (var item in _matrix[i])
                Console.Write($"{item, 3} ");
            
            Console.WriteLine();
        }
    }
#endif
    
#region IENUMERABLE

    /// <summary>
    /// Iterates over all elements. To iterate over rows or columns, use <see cref="GetRows"/>
    /// or <see cref="GetCols"/> respectively.
    /// </summary>
    /// <returns>All elements of the matrix</returns>
    public IEnumerator<T> GetEnumerator() {
        for (int i = 0; i < Rows; i++) {
            for (int j = 0; j < Columns; j++) {
                yield return _matrix[i][j];
            }
        }
    }
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
    
#endregion    
    
#region IDISPOSABLE

    private bool _disposed;
    
    /// <summary>
    /// Allows the pretty <c>using</c> statement and slightly helps the GC.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Do not dispose more than once</exception>
    public void Dispose() {
        ThrowHelper.ThrowIf(_disposed, new ObjectDisposedException(nameof(List2DOld<T>)));

        for (int i = 0; i < Rows; i++) {
            _matrix[i] = null!;
        }
        
        _matrix = null!;
        Rows = Columns = 0;
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }
    
#endregion    
}