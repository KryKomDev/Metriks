// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public readonly struct Size2D {
    public int X { get; }
    public int Y { get; }
    
    public Size2D(int x, int y) {
        X = x;
        Y = y;
    }
    
    public static Size2D Empty => new(0, 0);
    
    public override string ToString() => $"({X}, {Y})";
    
    public static Size2D operator +(Size2D l, Size2D r) => new(l.X + r.X, l.Y + r.Y);
    public static Size2D operator -(Size2D l, Size2D r) => new(l.X - r.X, l.Y - r.Y);
    public static Size2D operator *(Size2D l, Size2D r) => new(l.X * r.X, l.Y * r.Y);
    public static Size2D operator /(Size2D l, Size2D r) => new(l.X / r.X, l.Y / r.Y);
    
    public static Size2D operator +(Size2D l, Point2D r) => new(l.X + r.X, l.Y + r.Y);
    public static Size2D operator -(Size2D l, Point2D r) => new(l.X - r.X, l.Y - r.Y);
    public static Size2D operator *(Size2D l, Point2D r) => new(l.X * r.X, l.Y * r.Y);
    public static Size2D operator /(Size2D l, Point2D r) => new(l.X / r.X, l.Y / r.Y);
    
    public static Size2D operator +(Point2D l, Size2D r) => new(l.X + r.X, l.Y + r.Y);
    public static Size2D operator -(Point2D l, Size2D r) => new(l.X - r.X, l.Y - r.Y);
    public static Size2D operator *(Point2D l, Size2D r) => new(l.X * r.X, l.Y * r.Y);
    public static Size2D operator /(Point2D l, Size2D r) => new(l.X / r.X, l.Y / r.Y);

    public bool Equals(Size2D other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) => obj is Size2D other && Equals(other);

    public override int GetHashCode() {
        unchecked {
            return (X * 397) ^ Y;
        }
    }
}