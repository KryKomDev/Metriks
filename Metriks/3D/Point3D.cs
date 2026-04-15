namespace Metriks;

public readonly record struct Point3D {
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    
    public Point3D(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }
    
    public override string ToString() => $"[{X}, {Y}, {Z}]";
    public Size3D ToSize() => new(X, Y, Z);
    
    public static Point3D operator +(Point3D l, Point3D r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Point3D operator -(Point3D l, Point3D r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Point3D operator *(Point3D l, Point3D r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Point3D operator /(Point3D l, Point3D r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public void Deconstruct(out int x, out int y, out int z) {
        x = X;
        y = Y;
        z = Z;
    }

    public static Point3D Zero { get; } = new(0, 0, 0);
    public static Point3D One  { get; } = new(1, 1, 1);
}