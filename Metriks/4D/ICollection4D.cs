// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

namespace Metriks;

public interface ICollection4D<T> : IEnumerable4D<T> {
    
    public int Count { get; }
    public int WCount { get; }
    public int XCount { get; }
    public int YCount { get; }
    public int ZCount { get; }
    public void CopyTo(T[,,,] array, Point4D index);
    public bool IsReadOnly { get; }
    
    public void Clear();
    public void AddW();
    public void AddX();
    public void AddY();
    public void AddZ();
    public void ShrinkW();
    public void ShrinkX();
    public void ShrinkY();
    public void ShrinkZ();
    public bool Contains(T value);
    public bool ContainsAtW(int w, T value);
    public bool ContainsAtX(int x, T value);
    public bool ContainsAtY(int y, T value);
    public bool ContainsAtZ(int z, T value);
}

public interface ICollection4D : IEnumerable4D {
    
    public int Count { get; }
    public int WCount { get; }
    public int XCount { get; }
    public int YCount { get; }
    public int ZCount { get; }
    public void CopyTo(Array array, Point4D index);
}