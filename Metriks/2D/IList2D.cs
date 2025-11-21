// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public interface IList2D<T> : ICollection2D<T> {

    public T this[int x, int y] { get; set; }

    public void InsertAtX(int x);
    public void InsertAtY(int y);

    public void RemoveAtX(int x);
    public void RemoveAtY(int y);
}