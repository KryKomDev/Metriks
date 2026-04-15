// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

namespace Metriks;

public interface IReadOnlyList4D<out T> : IReadOnlyCollection4D<T> {

    public T this[int w, int x, int y, int z] { get; }

}

public interface IReadOnlyList4D : IReadOnlyCollection4D {

    public object? this[int w, int x, int y, int z] { get; }

}