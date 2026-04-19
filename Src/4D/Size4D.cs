// Metriks
// Copyright (c) KryKom 2026

namespace Metriks;

public readonly record struct Size4D {
    
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
    
    public Size4D(int w, int x, int y, int z) {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }
    
    public override string ToString() => $"{W}x{X}x{Y}x{Z}";

    public Point4D ToPoint() => new(W, X, Y, Z);
    
    public static Size4D operator +(Size4D p) => p;
    public static Size4D operator -(Size4D p) => new(-p.W, -p.X, -p.Y, -p.Z);

    public static Size4D operator +(Size4D l, Size4D r) => new(l.W + r.W, l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Size4D operator -(Size4D l, Size4D r) => new(l.W - r.W, l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Size4D operator *(Size4D l, Size4D r) => new(l.W * r.W, l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Size4D operator /(Size4D l, Size4D r) => new(l.W / r.W, l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public static explicit operator Point4D(Size4D size) => new(size.W, size.X, size.Y, size.Z);
    
    public static Size4D Zero { get; } = new(0, 0, 0, 0);
    public static Size4D One  { get; } = new(1, 1, 1, 1);

    public void Deconstruct(out int w, out int x, out int y, out int z) {
        w = W;
        x = X;
        y = Y;
        z = Z;
    }
    
    /// <summary>
    /// Returns a new <see cref="Size4D"/> with the maximum W, X, Y, and Z values
    /// from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size4D"/> to compare.</param>
    /// <param name="b">The second <see cref="Size4D"/> to compare.</param>
    /// <returns>A new <see cref="Size4D"/> containing the maximum W, X, Y, and Z values
    /// from the two input sizes.</returns>
    public static Size4D Max(Size4D a, Size4D b) =>
        new(Math.Max(a.W, b.W), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    /// <summary>
    /// Returns a new <see cref="Size4D"/> with the minimum W, X, Y, and Z values
    /// from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size4D"/> to compare.</param>
    /// <param name="b">The second <see cref="Size4D"/> to compare.</param>
    /// <returns>A new <see cref="Size4D"/> containing the minimum W, X, Y, and Z values
    /// from the two input sizes.</returns>
    public static Size4D Min(Size4D a, Size4D b) => 
        new(Math.Min(a.W, b.W), Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
}