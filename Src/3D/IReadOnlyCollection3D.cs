namespace Metriks;

public interface IReadOnlyCollection3D<out T> : IEnumerable3D<T> {
    public int Count { get; }
    public int XCount { get; }
    public int YCount { get; }
    public int ZCount { get; }
}

public interface IReadOnlyCollection3D : IEnumerable3D {
    public int Count { get; }
    public int XCount { get; }
    public int YCount { get; }
    public int ZCount { get; }
}