// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Runtime.CompilerServices;

namespace Metriks;

public class Plane<T> : List2D<T> {
    
    private int _xOriginOffset = 0;
    private int _yOriginOffset = 0;
    
    public int XStart => -_xOriginOffset;
    public int YStart => -_yOriginOffset;
    public int XEnd => XSize - 1 - _xOriginOffset;
    public int YEnd => YSize - 1 - _yOriginOffset;
    
    public new T this[int x, int y] {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => base[x + _xOriginOffset, y + _yOriginOffset];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => base[x + _xOriginOffset, y + _yOriginOffset] = value;
    }

    public T this[int x, int y, bool disableCoordinates] {
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => disableCoordinates ? base[x, y] : this[x, y];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            if (disableCoordinates)
                base[x, y] = value;
            else
                this[x, y] = value;
        }
    }

    #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    
    public new T this[Index x, Index y] {
        
        [Pure]
        get {
            int xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : _xOriginOffset);
            int yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : _yOriginOffset);
            if (xo < 0 || xo >= XSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= YSize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            return UnsafeGet(xo, yo);
        }
        
        set {
            int xo = x.GetOffset(XSize) + (x.IsFromEnd ? 0 : _xOriginOffset);
            int yo = y.GetOffset(YSize) + (y.IsFromEnd ? 0 : _yOriginOffset);
            if (xo < 0 || xo >= XSize) throw new IndexOutOfRangeException("Index 'x' is out of range.");
            if (yo < 0 || yo >= YSize) throw new IndexOutOfRangeException("Index 'y' is out of range.");
            UnsafeSet(xo, yo, value);
        }
    }
    
    #endif

    /// <summary>
    /// Retrieves the element at the specified x and y coordinates without applying origin offsets.
    /// </summary>
    /// <param name="x">The x-coordinate of the element to retrieve.</param>
    /// <param name="y">The y-coordinate of the element to retrieve.</param>
    /// <returns>The element of type <typeparamref name="T"/> at the specified coordinates.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T UncoordinatedGet(int x, int y) => base[x, y];

    /// <summary>
    /// Sets the element at the specified x and y coordinates without applying origin offsets.
    /// </summary>
    /// <param name="x">The x-coordinate of the element to set.</param>
    /// <param name="y">The y-coordinate of the element to set.</param>
    /// <param name="value">The value of type <typeparamref name="T"/> to set at the specified coordinates.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UncoordinatedSet(int x, int y, T value) => base[x, y] = value;

    public int XOriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _xOriginOffset;
    }

    public int YOriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _yOriginOffset;
    }
    
    public Point2D OriginOffset {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_xOriginOffset, _yOriginOffset);
    }

    public Plane(int xCapacity, int yCapacity) : base(xCapacity, yCapacity) { }
    public Plane(int capacity) : base(capacity) { }
    public Plane(T[,] arr) : base(arr) { }
    public Plane() { }

    /// <summary>
    /// Inserts a new column at the specified index in the 2D list while accounting for the origin offset.
    /// </summary>
    /// <param name="x">The x-coordinate at which the new column should be inserted.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified coordinate is out of bounds.
    /// </exception>
    public new void InsertAtX(int x) {
        base.InsertAtX(x + _xOriginOffset);
        if (x <= _xOriginOffset) _xOriginOffset++;
    }

    /// <summary>
    /// Inserts a new row at the specified index in the 2D list while accounting for the origin offset.
    /// </summary>
    /// <param name="y">The y-coordinate at which the new row should be inserted.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified coordinate is out of bounds.
    /// </exception>
    public new void InsertAtY(int y) {
        base.InsertAtY(y + _yOriginOffset);
        if (y <= _yOriginOffset) _yOriginOffset++;
    }

    /// <summary>
    /// Removes a column at the specified index in the 2D list while accounting for the origin offset.
    /// </summary>
    /// <param name="x">The x-coordinate at which the column should be removed.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified coordinate is out of bounds.
    /// </exception>
    public new void RemoveAtX(int x) {
        base.RemoveAtX(x + _xOriginOffset);
        if (x < _xOriginOffset) _xOriginOffset--;
    }

    /// <summary>
    /// Removes a row at the specified index in the 2D list while accounting for the origin offset.
    /// </summary>
    /// <param name="y">The y-coordinate at which the row should be removed.</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified coordinate is out of bounds.
    /// </exception>
    public new void RemoveAtY(int y) {
        base.RemoveAtY(y + _yOriginOffset);
        if (y < _yOriginOffset) _yOriginOffset--;
    }

    /// <summary>
    /// Retrieves all elements from the specified column in the 2D list while adjusting for the origin offset.
    /// </summary>
    /// <param name="x">The x-coordinate of the column to retrieve elements from.</param>
    /// <returns>An enumerable collection of elements from the specified column.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the adjusted column index is out of bounds.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new IEnumerable<T> GetAtX(int x) => base.GetAtX(x + _xOriginOffset);

    /// <summary>
    /// Retrieves all elements at the specified row (Y-coordinate) in the 2D list, accounting for the origin offset.
    /// </summary>
    /// <param name="y">The y-coordinate of the row to retrieve elements from.</param>
    /// <returns>An enumerable collection of elements at the specified row.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified y-coordinate is out of bounds.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new IEnumerable<T> GetAtY(int y) => base.GetAtY(y + _yOriginOffset);

    /// <summary>
    /// Determines whether all elements in the specified column, adjusted for the origin offset, satisfy the given
    /// predicate.
    /// </summary>
    /// <param name="x">The x-coordinate (adjusted for the origin offset) of the column to check.</param>
    /// <param name="predicate">The predicate to evaluate for each element in the column.</param>
    /// <returns>
    /// True if all elements in the specified column satisfy the predicate; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new bool AllAtX(int x, Predicate<T> predicate) => base.AllAtX(x + _xOriginOffset, predicate);

    /// <summary>
    /// Determines whether all elements in the specified row of the 2D list, with consideration to the origin offset,
    /// satisfy a given condition.
    /// </summary>
    /// <param name="y">The y-coordinate of the row to evaluate.</param>
    /// <param name="predicate">The condition to evaluate against each element in the row.</param>
    /// <returns>
    /// True if all elements in the specified row satisfy the condition; otherwise, false.
    /// </returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified row index is less than 0 or greater than or equal to the current YSize,
    /// considering the origin offset.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new bool AllAtY(int y, Predicate<T> predicate) => base.AllAtY(y + _yOriginOffset, predicate);

    /// <summary>
    /// Places a 2D matrix into the FixedOriginList2D at the specified offset while accounting for the origin offsets.
    /// Adjusts the origin offsets if the provided offset is less than the current offsets.
    /// </summary>
    /// <param name="matrix">The 2D array of elements to place into the FixedOriginList2D.</param>
    /// <param name="offsetPoint">
    /// An optional offset defining where the top-left corner of the matrix will be placed.
    /// If not provided, the matrix will be placed at the origin of the FixedOriginList2D.
    /// </param>
    public new void Place(T[,] matrix, Point2D? offsetPoint) {
        var offset = offsetPoint ?? Point2D.Empty;

        base.Place(matrix, new Point2D(offset.X + _xOriginOffset, offset.Y + _yOriginOffset));
        
        if (offset.X < -_xOriginOffset) _xOriginOffset = -offset.X;
        if (offset.Y < -_yOriginOffset) _yOriginOffset = -offset.Y;
    }

    public new void Clear() {
        base.Clear();
        _xOriginOffset = 0;
        _yOriginOffset = 0;
    }

    public void MoveOrigin(int xOffset, int yOffset) {
        _xOriginOffset += xOffset;
        _yOriginOffset += yOffset;
    }
}