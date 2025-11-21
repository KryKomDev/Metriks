// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public interface IReadOnlyCollection2D<out T> : IEnumerable2D<T> {
    public int Count { get; }
    public int XCount { get; }
    public int YCount { get; }
}

public interface IReadOnlyCollection2D : IEnumerable2D {
    public int Count { get; }
    public int XCount { get; }
    public int YCount { get; }
}