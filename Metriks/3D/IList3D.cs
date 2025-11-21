namespace Metriks;

public interface IList3D<T> : ICollection3D<T> {
    
    public T this[int x, int y, int z] { get; set; }
    
    public void InsertAtX(int x);
    public void InsertAtY(int y);
    public void InsertAtZ(int z);
    
    public void RemoveAtX(int x);
    public void RemoveAtY(int y);
    public void RemoveAtZ(int z);
}