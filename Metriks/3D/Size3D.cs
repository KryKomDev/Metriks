// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public readonly record struct Size3D {
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    
    public Size3D(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
    public Point3D ToPoint() => new(X, Y, Z);

    public static Size3D operator +(Size3D l, Size3D r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Size3D operator -(Size3D l, Size3D r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Size3D operator *(Size3D l, Size3D r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Size3D operator /(Size3D l, Size3D r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public void Deconstruct(out int x, out int y, out int z) {
        x = X;
        y = Y;
        z = Z;
    }
}