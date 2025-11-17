// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public interface IList2D<out T> : IEnumerable2D<T> {
    
    /// <summary>
    /// Gets the current number of columns (X-dimension) in the 2D list.
    /// </summary>
    /// <remarks>
    /// Represents the total count of columns (X-dimension) currently in the data structure.
    /// </remarks>
    public int XSize { get; }

    /// <summary>
    /// Gets the current number of rows (Y-dimension) in the 2D list.
    /// </summary>
    /// <remarks>
    /// Represents the total count of rows (Y-dimension) currently in the data structure.
    /// </remarks>
    public int YSize { get; }

    /// <summary>
    /// Gets or sets the value at the specified 2D index in the data structure.
    /// </summary>
    /// <param name="x">The zero-based index of the X-dimension (column).</param>
    /// <param name="y">The zero-based index of the Y-dimension (row).</param>
    /// <returns>The value at the specified 2D index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the specified X-dimension or Y-dimension index is out of range.
    /// </exception>
    public T this[int x, int y] { get; }

    /// <summary>
    /// Inserts a new column at the specified index in the 2D list.
    /// </summary>
    /// <param name="x">The zero-based index at which the new column should be inserted.
    /// Must be within the range [0, XSize].</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the provided index is less than 0 or greater than the current XSize.
    /// </exception>
    public void InsertAtX(int x);

    /// <summary>
    /// Inserts a new row at the specified index in the 2D list.
    /// </summary>
    /// <param name="y">The zero-based index at which the new row should be inserted.
    /// Must be within the range [0, YSize].</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the specified index is less than 0 or greater than the current YSize.
    /// </exception>
    public void InsertAtY(int y);

    /// <summary>
    /// Expands the 2D list to the specified sizes in the X and Y dimensions.
    /// </summary>
    /// <param name="xSize">The new size for the X dimension. Must be greater than or equal to the current XSize.</param>
    /// <param name="ySize">The new size for the Y dimension. Must be greater than or equal to the current YSize.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the specified xSize is less than the current XSize or the specified ySize is less than the current YSize.
    /// </exception>
    public void Expand(int xSize, int ySize);

    /// <summary>
    /// Removes the column at the specified index in the 2D list and shifts all subsequent columns to the left.
    /// </summary>
    /// <param name="x">The zero-based index of the column to remove. Must be within the range [0, XSize - 1].</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the specified index is less than 0 or greater than or equal to the current XSize.
    /// </exception>
    public void RemoveAtX(int x);

    /// <summary>
    /// Removes the row at the specified index in the 2D list.
    /// </summary>
    /// <param name="y">The zero-based index of the row to be removed. Must be within the range [0, YSize - 1].</param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the specified index is less than 0 or greater than or equal to the current YSize.
    /// </exception>
    public void RemoveAtY(int y);
    
    public bool IsReadonly { get; }
}