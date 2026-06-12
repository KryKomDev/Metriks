// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

/// <summary>
/// Represents a generic read-only two-dimensional collection of elements.
/// </summary>
/// <typeparam name="T">The type of elements in the read-only two-dimensional collection.</typeparam>
public interface IReadOnlyCollection2D<out T> : IEnumerable2D<T> {
    
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
}

/// <summary>
/// Represents a non-generic read-only two-dimensional collection of elements.
/// </summary>
public interface IReadOnlyCollection2D : IEnumerable2D {

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
}