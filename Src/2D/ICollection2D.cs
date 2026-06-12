// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

/// <summary>
/// Defines size, capacity, and utility methods for generic two-dimensional collections.
/// </summary>
/// <typeparam name="T">The type of elements in the two-dimensional collection.</typeparam>
public interface ICollection2D<T> : IEnumerable2D<T> {
    
    /// <summary>
    /// Gets the total number of elements in the collection.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the number of elements along the X-axis.
    /// </summary>
    public int XCount { get; }

    /// <summary>
    /// Gets the number of elements along the Y-axis.
    /// </summary>
    public int YCount { get; }

    public void CopyTo(T[,] array, Point2D index);

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly { get; }
    
    public void Clear();
    public void AddX();
    public void AddY();
    public void ShrinkX();
    public void ShrinkY();
    public bool Contains(T value);
    public bool ContainsAtX(int x, T value);
    public bool ContainsAtY(int y, T value);
}

/// <summary>
/// Defines size and copy methods for non-generic two-dimensional collections.
/// </summary>
public interface ICollection2D : IEnumerable2D {

    /// <summary>
    /// Gets the total number of elements in the collection.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the number of elements along the X-axis.
    /// </summary>
    public int XCount { get; }

    /// <summary>
    /// Gets the number of elements along the Y-axis.
    /// </summary>
    public int YCount { get; }

    public void CopyTo(Array array, Point2D index);
}