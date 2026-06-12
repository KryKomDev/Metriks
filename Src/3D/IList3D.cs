// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

/// <summary>
/// Represents a generic three-dimensional list of elements that can be accessed by index.
/// </summary>
/// <typeparam name="T">The type of elements in the three-dimensional list.</typeparam>
public interface IList3D<T> : ICollection3D<T> {
    
    /// <summary>
    /// Gets or sets the element at the specified 3D coordinates.
    /// </summary>
    /// <param name="x">The zero-based X index of the element to get or set.</param>
    /// <param name="y">The zero-based Y index of the element to get or set.</param>
    /// <param name="z">The zero-based Z index of the element to get or set.</param>
    /// <returns>The element at the specified coordinates.</returns>
    public T this[int x, int y, int z] { get; set; }

    public void InsertAtX(int x);
    public void InsertAtY(int y);
    public void InsertAtZ(int z);

    public void RemoveAtX(int x);
    public void RemoveAtY(int y);
    public void RemoveAtZ(int z);
}