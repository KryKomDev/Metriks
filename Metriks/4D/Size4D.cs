namespace Metriks;

public readonly struct Size4D {
    public int W { get; }
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    
    public Size4D(int w, int x, int y, int z) {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }
    
}