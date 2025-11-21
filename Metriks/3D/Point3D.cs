namespace Metriks;

public readonly struct Point3D : IEquatable<Point3D> {
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    
    public Point3D(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }
    
    public static Point3D Empty => new(0, 0, 0);
    
    public override string ToString() => $"({X}, {Y})";
    
    public static Point3D operator +(Point3D l, Point3D r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Point3D operator -(Point3D l, Point3D r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Point3D operator *(Point3D l, Point3D r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Point3D operator /(Point3D l, Point3D r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public bool Equals(Point3D other) => X == other.X && Y == other.Y && Z == other.Z;

    public override bool Equals(object? obj) => obj is Point2D other && Equals(other);

    public override int GetHashCode() {
        unchecked {
            return (X * 397) ^ (Y * 16807) ^ Z;
        }
    }
}