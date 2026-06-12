// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

namespace Metriks;

/// <summary>
/// Represents a generic four-dimensional list of elements that can be accessed by index.
/// </summary>
/// <typeparam name="T">The type of elements in the four-dimensional list.</typeparam>
public interface IList4D<T> : ICollection4D<T> {
    
    /// <summary>
    /// Gets or sets the element at the specified 4D coordinates.
    /// </summary>
    /// <param name="w">The zero-based W index of the element to get or set.</param>
    /// <param name="x">The zero-based X index of the element to get or set.</param>
    /// <param name="y">The zero-based Y index of the element to get or set.</param>
    /// <param name="z">The zero-based Z index of the element to get or set.</param>
    /// <returns>The element at the specified coordinates.</returns>
    public T this[int w, int x, int y, int z] { get; set; }

    public void InsertAtW(int w);
    public void InsertAtX(int x);
    public void InsertAtY(int y);
    public void InsertAtZ(int z);

    public void RemoveAtW(int w);
    public void RemoveAtX(int x);
    public void RemoveAtY(int y);
    public void RemoveAtZ(int z);
}