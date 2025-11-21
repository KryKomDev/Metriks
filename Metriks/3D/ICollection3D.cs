namespace Metriks;

public interface ICollection3D<T> : IEnumerable3D<T> {
    
    public int Count { get; }
    public int XCount { get; }
    public int YCount { get; }
    public int ZCount { get; }
    public void CopyTo(T[,,] array, Point3D index);
    public bool IsReadOnly { get; }
    
    public void Clear();
    public void AddX();
    public void AddY();
    public void AddZ();
    public void ShrinkX();
    public void ShrinkY();
    public void ShrinkZ();
    public bool Contains(T value);
    public bool ContainsAtX(int x, T value);
    public bool ContainsAtY(int y, T value);
    public bool ContainsAtZ(int z, T value);
}

public interface ICollection3D : IEnumerable3D {
    
    public int Count { get; }
    public int XCount { get; }
    public int YCount { get; }
    public int ZCount { get; }
    public void CopyTo(Array array, Point3D index);
}