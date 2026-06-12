// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

namespace Metriks;

/// <summary>
/// Defines size, capacity, and utility methods for generic four-dimensional collections.
/// </summary>
/// <typeparam name="T">The type of elements in the four-dimensional collection.</typeparam>
public interface ICollection4D<T> : IEnumerable4D<T> {
    
    /// <summary>
    /// Gets the total number of elements in the collection.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the number of elements along the W-axis.
    /// </summary>
    public int WCount { get; }

    /// <summary>
    /// Gets the number of elements along the X-axis.
    /// </summary>
    public int XCount { get; }

    /// <summary>
    /// Gets the number of elements along the Y-axis.
    /// </summary>
    public int YCount { get; }

    /// <summary>
    /// Gets the number of elements along the Z-axis.
    /// </summary>
    public int ZCount { get; }

    public void CopyTo(T[,,,] array, Point4D index);

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly { get; }
    
    public void Clear();
    public void AddW();
    public void AddX();
    public void AddY();
    public void AddZ();
    public void ShrinkW();
    public void ShrinkX();
    public void ShrinkY();
    public void ShrinkZ();
    public bool Contains(T value);
    public bool ContainsAtW(int w, T value);
    public bool ContainsAtX(int x, T value);
    public bool ContainsAtY(int y, T value);
    public bool ContainsAtZ(int z, T value);
}

/// <summary>
/// Defines size and copy methods for non-generic four-dimensional collections.
/// </summary>
public interface ICollection4D : IEnumerable4D {
    
    /// <summary>
    /// Gets the total number of elements in the collection.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the number of elements along the W-axis.
    /// </summary>
    public int WCount { get; }

    /// <summary>
    /// Gets the number of elements along the X-axis.
    /// </summary>
    public int XCount { get; }

    /// <summary>
    /// Gets the number of elements along the Y-axis.
    /// </summary>
    public int YCount { get; }

    /// <summary>
    /// Gets the number of elements along the Z-axis.
    /// </summary>
    public int ZCount { get; }

    public void CopyTo(Array array, Point4D index);
}