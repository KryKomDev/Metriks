// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public interface IReadOnlyList2D<out T> : IReadOnlyCollection2D<T> {

    public T this[int x, int y] { get; }

}

public interface IReadOnlyList2D : IReadOnlyCollection2D {

    public object? this[int x, int y] { get; }

}