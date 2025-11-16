namespace Metriks;

public readonly struct Point2D : IEquatable<Point2D> {
    public int X { get; }
    public int Y { get; }
    
    public Point2D(int x, int y) {
        X = x;
        Y = y;
    }
    
    public static Point2D Empty => new(0, 0);
    
    public override string ToString() => $"({X}, {Y})";
    
    public static Point2D operator +(Point2D l, Point2D r) => new(l.X + r.X, l.Y + r.Y);
    public static Point2D operator -(Point2D l, Point2D r) => new(l.X - r.X, l.Y - r.Y);
    public static Point2D operator *(Point2D l, Point2D r) => new(l.X * r.X, l.Y * r.Y);
    public static Point2D operator /(Point2D l, Point2D r) => new(l.X / r.X, l.Y / r.Y);

    public bool Equals(Point2D other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) => obj is Point2D other && Equals(other);

    public override int GetHashCode() {
        unchecked {
            return (X * 397) ^ Y;
        }
    }
}