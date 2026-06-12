// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Collections;
using System.Collections.Generic;

namespace Metriks;

/// <summary>
/// Exposes the enumerator for a two-dimensional collection, supporting iteration over rows and columns.
/// </summary>
/// <typeparam name="T">The type of elements in the two-dimensional sequence.</typeparam>
public interface IEnumerable2D<out T> : IEnumerable2D, IEnumerable<IEnumerable<T>>
#if NET9_0_OR_GREATER
    where T : allows ref struct 
#endif
{
    public new IEnumerator<IEnumerable<T>> GetEnumerator();

    public new IEnumerable<T> GetAtX(int x);
    public new IEnumerable<T> GetAtY(int y);
}

/// <summary>
/// Exposes the non-generic enumerator for a two-dimensional collection.
/// </summary>
public interface IEnumerable2D : IEnumerable {
    public new IEnumerator GetEnumerator();

    public IEnumerable GetAtX(int x);
    public IEnumerable GetAtY(int y);
}