// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

namespace Metriks;

public interface IReadOnlyCollection4D<out T> : IEnumerable4D<T> {

    public int Count { get; }
    public int WCount { get; }
    public int XCount { get; }
    public int YCount { get; }
    public int ZCount { get; }

}

public interface IReadOnlyCollection4D : IEnumerable4D {

    public int Count { get; }
    public int WCount { get; }
    public int XCount { get; }
    public int YCount { get; }
    public int ZCount { get; }

}