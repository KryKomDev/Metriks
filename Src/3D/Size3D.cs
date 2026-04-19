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

    public override string ToString() => $"{X}x{Y}x{Z}";
    public Point3D ToPoint() => new(X, Y, Z);

    public static Size3D operator +(Size3D p) => p;
    public static Size3D operator -(Size3D p) => new(-p.X, -p.Y, -p.Z);

    public static Size3D operator +(Size3D l, Size3D r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Size3D operator -(Size3D l, Size3D r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Size3D operator *(Size3D l, Size3D r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Size3D operator /(Size3D l, Size3D r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public void Deconstruct(out int x, out int y, out int z) {
        x = X;
        y = Y;
        z = Z;
    }

    public static Size3D Zero { get; } = new(0, 0, 0);
    public static Size3D One  { get; } = new(1, 1, 1);
    
    /// <summary>
    /// Returns a new <see cref="Size3D"/> with the maximum X, Y, and Z values
    /// from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size3D"/> to compare.</param>
    /// <param name="b">The second <see cref="Size3D"/> to compare.</param>
    /// <returns>A new <see cref="Size3D"/> containing the maximum X, Y, and Z values
    /// from the two input sizes.</returns>
    public static Size3D Max(Size3D a, Size3D b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    /// <summary>
    /// Returns a new <see cref="Size3D"/> with the minimum X, Y, and Z values
    /// from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size3D"/> to compare.</param>
    /// <param name="b">The second <see cref="Size3D"/> to compare.</param>
    /// <returns>A new <see cref="Size3D"/> containing the minimum X, Y, and Z values
    /// from the two input sizes.</returns>
    public static Size3D Min(Size3D a, Size3D b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
}