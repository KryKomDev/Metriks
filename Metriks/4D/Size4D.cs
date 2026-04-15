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
    
    public override string ToString() => $"({W}, {X}, {Y}, {Z})";

    public Point4D ToPoint() => new(W, X, Y, Z);
    
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
}