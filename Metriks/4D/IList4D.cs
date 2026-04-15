// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

namespace Metriks;

public interface IList4D<T> : ICollection4D<T> {
    
    public T this[int w, int x, int y, int z] { get; set; }

    public void InsertAtW(int w);
    public void InsertAtX(int x);
    public void InsertAtY(int y);
    public void InsertAtZ(int z);

    public void RemoveAtW(int w);
    public void RemoveAtX(int x);
    public void RemoveAtY(int y);
    public void RemoveAtZ(int z);
}