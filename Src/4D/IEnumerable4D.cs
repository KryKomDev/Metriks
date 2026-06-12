// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Collections;
using System.Collections.Generic;

namespace Metriks;

/// <summary>
/// Exposes the enumerator for a four-dimensional collection, supporting iteration over hyperplanes, layers, rows, and columns.
/// </summary>
/// <typeparam name="T">The type of elements in the four-dimensional sequence.</typeparam>
public interface IEnumerable4D<out T> : IEnumerable4D, IEnumerable<IEnumerable3D<T>>
#if NET9_0_OR_GREATER
    where T : allows ref struct 
#endif
{
    public new IEnumerator<IEnumerable3D<T>> GetEnumerator();

    public new IEnumerable3D<T> GetAtW(int w);
    public new IEnumerable3D<T> GetAtX(int x);
    public new IEnumerable3D<T> GetAtY(int y);
    public new IEnumerable3D<T> GetAtZ(int z);
}

/// <summary>
/// Exposes the non-generic enumerator for a four-dimensional collection.
/// </summary>
public interface IEnumerable4D : IEnumerable {
    public new IEnumerator GetEnumerator();

    public IEnumerable3D GetAtW(int w);
    public IEnumerable3D GetAtX(int x);
    public IEnumerable3D GetAtY(int y);
    public IEnumerable3D GetAtZ(int z);
}