// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Collections;
using System.Collections.Generic;

namespace Metriks;

public interface IEnumerable3D<out T> : IEnumerable3D, IEnumerable<IEnumerable2D<T>>
#if NET9_0_OR_GREATER
    where T : allows ref struct 
#endif
{
    public new IEnumerator<IEnumerable2D<T>> GetEnumerator();

    public new IEnumerable2D<T> GetAtX(int x);
    public new IEnumerable2D<T> GetAtY(int y);
    public new IEnumerable2D<T> GetAtZ(int z);
}

public interface IEnumerable3D : IEnumerable {
    public new IEnumerator GetEnumerator();

    public IEnumerable2D GetAtX(int x);
    public IEnumerable2D GetAtY(int y);
    public IEnumerable2D GetAtZ(int z);
}