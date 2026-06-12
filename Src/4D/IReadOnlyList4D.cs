// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

namespace Metriks;

/// <summary>
/// Represents a generic read-only four-dimensional list of elements.
/// </summary>
/// <typeparam name="T">The type of elements in the read-only four-dimensional list.</typeparam>
public interface IReadOnlyList4D<out T> : IReadOnlyCollection4D<T> {

    /// <summary>
    /// Gets the element at the specified 4D coordinates.
    /// </summary>
    /// <param name="w">The zero-based W index of the element to get.</param>
    /// <param name="x">The zero-based X index of the element to get.</param>
    /// <param name="y">The zero-based Y index of the element to get.</param>
    /// <param name="z">The zero-based Z index of the element to get.</param>
    /// <returns>The element at the specified coordinates.</returns>
    public T this[int w, int x, int y, int z] { get; }

}

/// <summary>
/// Represents a non-generic read-only four-dimensional list of elements.
/// </summary>
public interface IReadOnlyList4D : IReadOnlyCollection4D {

    /// <summary>
    /// Gets the element at the specified 4D coordinates.
    /// </summary>
    /// <param name="w">The zero-based W index of the element to get.</param>
    /// <param name="x">The zero-based X index of the element to get.</param>
    /// <param name="y">The zero-based Y index of the element to get.</param>
    /// <param name="z">The zero-based Z index of the element to get.</param>
    /// <returns>The element at the specified coordinates.</returns>
    public object? this[int w, int x, int y, int z] { get; }

}