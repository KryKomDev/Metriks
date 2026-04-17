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
    
    public override string ToString() => $"{X}x{Y}";

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
    
    /// <summary>
    /// Returns a new <see cref="Size2D"/> with the maximum X and Y values
    /// from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size2D"/> to compare.</param>
    /// <param name="b">The second <see cref="Size2D"/> to compare.</param>
    /// <returns>A new <see cref="Size2D"/> containing the maximum X and Y values
    /// from the two input sizes.</returns>
    public static Size2D Max(Size2D a, Size2D b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

    /// <summary>
    /// Returns a new <see cref="Size2D"/> with the minimum X and Y values
    /// from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size2D"/> to compare.</param>
    /// <param name="b">The second <see cref="Size2D"/> to compare.</param>
    /// <returns>A new <see cref="Size2D"/> containing the minimum X and Y values
    /// from the two input sizes.</returns>
    public static Size2D Min(Size2D a, Size2D b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
}