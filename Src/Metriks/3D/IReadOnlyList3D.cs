namespace Metriks;

/// <summary>
/// Represents a generic read-only three-dimensional list of elements.
/// </summary>
/// <typeparam name="T">The type of elements in the read-only three-dimensional list.</typeparam>
public interface IReadOnlyList3D<out T> : IReadOnlyCollection3D<T> {

    /// <summary>
    /// Gets the element at the specified 3D coordinates.
    /// </summary>
    /// <param name="x">The zero-based X index of the element to get.</param>
    /// <param name="y">The zero-based Y index of the element to get.</param>
    /// <param name="z">The zero-based Z index of the element to get.</param>
    /// <returns>The element at the specified coordinates.</returns>
    public T this[int x, int y, int z] { get; }
}

/// <summary>
/// Represents a non-generic read-only three-dimensional list of elements.
/// </summary>
public interface IReadOnlyList3D : IReadOnlyCollection3D {

    /// <summary>
    /// Gets the element at the specified 3D coordinates.
    /// </summary>
    /// <param name="x">The zero-based X index of the element to get.</param>
    /// <param name="y">The zero-based Y index of the element to get.</param>
    /// <param name="z">The zero-based Z index of the element to get.</param>
    /// <returns>The element at the specified coordinates.</returns>
    public object? this[int x, int y, int z] { get; }
}