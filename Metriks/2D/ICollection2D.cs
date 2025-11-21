// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public interface ICollection2D<T> : IEnumerable2D<T> {
    
    public int Count { get; }
    public int XCount { get; }
    public int YCount { get; }
    public void CopyTo(T[,] array, Point2D index);
    public bool IsReadOnly { get; }
    
    public void Clear();
    public void AddX();
    public void AddY();
    public void ShrinkX();
    public void ShrinkY();
    public bool Contains(T value);
    public bool ContainsAtX(int x, T value);
    public bool ContainsAtY(int y, T value);
}

public interface ICollection2D : IEnumerable2D {
    public int Count { get; }
    public int XCount { get; }
    public int YCount { get; }
    public void CopyTo(Array array, Point2D index);
}