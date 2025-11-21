// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public interface IReadonlyList2D<out T> : IReadOnlyCollection2D<T> {
    public T this[int x, int y] { get; }
}

public interface IReadonlyList2D : IReadOnlyCollection2D {
    public object? this[int x, int y] { get; }
}