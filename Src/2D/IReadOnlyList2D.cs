// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

/// <summary>
/// Represents a generic read-only two-dimensional list of elements.
/// </summary>
/// <typeparam name="T">The type of elements in the read-only two-dimensional list.</typeparam>
public interface IReadOnlyList2D<out T> : IReadOnlyCollection2D<T> {

    /// <summary>
    /// Gets the element at the specified 2D coordinates.
    /// </summary>
    /// <param name="x">The zero-based X index of the element to get.</param>
    /// <param name="y">The zero-based Y index of the element to get.</param>
    /// <returns>The element at the specified coordinates.</returns>
    public T this[int x, int y] { get; }

}

/// <summary>
/// Represents a non-generic read-only two-dimensional list of elements.
/// </summary>
public interface IReadOnlyList2D : IReadOnlyCollection2D {

    /// <summary>
    /// Gets the element at the specified 2D coordinates.
    /// </summary>
    /// <param name="x">The zero-based X index of the element to get.</param>
    /// <param name="y">The zero-based Y index of the element to get.</param>
    /// <returns>The element at the specified coordinates.</returns>
    public object? this[int x, int y] { get; }

}