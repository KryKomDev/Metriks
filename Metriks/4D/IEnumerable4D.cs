// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Collections;
using System.Collections.Generic;

namespace Metriks;

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

public interface IEnumerable4D : IEnumerable {
    public new IEnumerator GetEnumerator();

    public IEnumerable3D GetAtW(int w);
    public IEnumerable3D GetAtX(int x);
    public IEnumerable3D GetAtY(int y);
    public IEnumerable3D GetAtZ(int z);
}