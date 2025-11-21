namespace Metriks;

public interface IReadOnlyList3D<out T> : IReadOnlyCollection3D<T> {
    public T this[int x, int y, int z] { get; }
}

public interface IReadOnlyList3D : IReadOnlyCollection3D {
    public object? this[int x, int y, int z] { get; }
}