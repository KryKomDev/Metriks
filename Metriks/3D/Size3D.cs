// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

namespace Metriks;

public readonly struct Size3D : IEquatable<Size3D> {
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    
    public Size3D(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }
    
    public static Size3D operator +(Size3D l, Point3D r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Size3D operator -(Size3D l, Point3D r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Size3D operator *(Size3D l, Point3D r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Size3D operator /(Size3D l, Point3D r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);
    
    public static Size3D operator +(Point3D l, Size3D r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Size3D operator -(Point3D l, Size3D r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Size3D operator *(Point3D l, Size3D r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Size3D operator /(Point3D l, Size3D r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);
    
    public static Size3D operator +(Size3D l, Size3D r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Size3D operator -(Size3D l, Size3D r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Size3D operator *(Size3D l, Size3D r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Size3D operator /(Size3D l, Size3D r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public bool Equals(Size3D other) => X == other.X && Y == other.Y && Z == other.Z;
    public override bool Equals(object? obj) => obj is Size3D other && Equals(other);

    public override int GetHashCode() {
        unchecked {
            var hashCode = X;
            hashCode = (hashCode * 397) ^ Y;
            hashCode = (hashCode * 397) ^ Z;
            return hashCode;
        }
    }
}