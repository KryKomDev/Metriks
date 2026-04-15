// Metriks
// Copyright (c) KryKom 2026

namespace Metriks;

public readonly record struct Size2D {
    
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
    
    public Size2D(int x, int y) {
        X = x;
        Y = y;
    }
    
    public override string ToString() => $"({X}, {Y})";

    public Point2D ToPoint() => new(X, Y);
    
    public static Size2D operator +(Size2D l, Size2D r) => new(l.X + r.X, l.Y + r.Y);
    public static Size2D operator -(Size2D l, Size2D r) => new(l.X - r.X, l.Y - r.Y);
    public static Size2D operator *(Size2D l, Size2D r) => new(l.X * r.X, l.Y * r.Y);
    public static Size2D operator /(Size2D l, Size2D r) => new(l.X / r.X, l.Y / r.Y);

    public static explicit operator Point2D(Size2D size) => new(size.X, size.Y);
    
    public static Size2D Zero { get; } = new(0, 0);
    public static Size2D One  { get; } = new(1, 1);

    public void Deconstruct(out int x, out int y) {
        x = X;
        y = Y;
    }
}