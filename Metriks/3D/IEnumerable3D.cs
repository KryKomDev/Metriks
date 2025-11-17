// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Collections;

namespace Metriks;

public interface IEnumerable3D<out T> : IEnumerable3D 
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    public new IEnumerator<IEnumerable<IEnumerable<T>>> GetEnumerator();
}

public interface IEnumerable3D {
    public IEnumerator<IEnumerable<IEnumerable>> GetEnumerator();
}