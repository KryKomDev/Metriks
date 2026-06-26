namespace Metriks;

/// <summary>
/// Represents a generic read-only three-dimensional collection of elements.
/// </summary>
/// <typeparam name="T">The type of elements in the read-only three-dimensional collection.</typeparam>
public interface IReadOnlyCollection3D<out T> : IEnumerable3D<T> {
    
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

    /// <summary>
    /// Gets the number of elements along the Z-axis.
    /// </summary>
    public int ZCount { get; }
}

/// <summary>
/// Represents a non-generic read-only three-dimensional collection of elements.
/// </summary>
public interface IReadOnlyCollection3D : IEnumerable3D {

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

    /// <summary>
    /// Gets the number of elements along the Z-axis.
    /// </summary>
    public int ZCount { get; }
}