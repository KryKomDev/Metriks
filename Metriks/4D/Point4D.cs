namespace Metriks;

public readonly record struct Point4D {
    
    public int W {
        get;
        #if NET5_0_OR_GREATER
        init; 
        #endif
    }
    
    public int X {
        get;
        #if NET5_0_OR_GREATER
        init; 
        #endif
    }
    
    public int Y {
        get;
        #if NET5_0_OR_GREATER
        init; 
        #endif
    }
    
    public int Z {
        get;
        #if NET5_0_OR_GREATER
        init; 
        #endif
    }
    
    public Point4D(int w, int x, int y, int z) {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }
    
    public override string ToString() => $"[{W}, {X}, {Y}, {Z}]";

    public Size4D ToSize() => new(W, X, Y, Z);
    
    public static Point4D operator +(Point4D l, Point4D r) => new(l.W + r.W, l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Point4D operator -(Point4D l, Point4D r) => new(l.W - r.W, l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Point4D operator *(Point4D l, Point4D r) => new(l.W * r.W, l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Point4D operator /(Point4D l, Point4D r) => new(l.W / r.W, l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public static explicit operator Size4D(Point4D point) => new(point.W, point.X, point.Y, point.Z);
    
    public static Point4D Zero { get; } = new(0, 0, 0, 0);
    public static Point4D One  { get; } = new(1, 1, 1, 1);

    public void Deconstruct(out int w, out int x, out int y, out int z) {
        w = W;
        x = X;
        y = Y;
        z = Z;
    }
}