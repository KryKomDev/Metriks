namespace Metriks;

public readonly record struct Point2D {
    
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
    
    public Point2D(int x, int y) {
        X = x;
        Y = y;
    }
    
    public override string ToString() => $"[{X}, {Y}]";

    public Size2D ToSize() => new(X, Y);
    
    public static Point2D operator +(Point2D l, Point2D r) => new(l.X + r.X, l.Y + r.Y);
    public static Point2D operator -(Point2D l, Point2D r) => new(l.X - r.X, l.Y - r.Y);
    public static Point2D operator *(Point2D l, Point2D r) => new(l.X * r.X, l.Y * r.Y);
    public static Point2D operator /(Point2D l, Point2D r) => new(l.X / r.X, l.Y / r.Y);

    public static explicit operator Size2D(Point2D point) => new(point.X, point.Y);
    
    public static Point2D Zero { get; } = new(0, 0);
    public static Point2D One  { get; } = new(1, 1);

    public void Deconstruct(out int x, out int y) {
        x = X;
        y = Y;
    }

}