// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2026

using System.Runtime.CompilerServices;

namespace Metriks;

/// <summary>
///     Represents a three-dimensional size structure (width, height, and depth) using X, Y, and Z components.
/// </summary>
public readonly record struct Size3D {

    public Size3D(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    ///     Gets the horizontal component (width) of the size.
    /// </summary>
    public int X { get; init; }

    /// <summary>
    ///     Gets the vertical component (height) of the size.
    /// </summary>
    public int Y { get; init; }

    /// <summary>
    ///     Gets the depth component of the size.
    /// </summary>
    public int Z { get; init; }

    /// <summary>
    ///     Gets a <see cref="Size3D" /> with X, Y, and Z values set to 0.
    /// </summary>
    public static Size3D Zero { get; } = new(0, 0, 0);

    /// <summary>
    ///     Gets a <see cref="Size3D" /> with X, Y, and Z values set to 1.
    /// </summary>
    public static Size3D One { get; } = new(1, 1, 1);

    public override string  ToString() => $"{X}x{Y}x{Z}";
    public          Point3D ToPoint()  => new(X, Y, Z);

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

    /// <summary>
    ///     Returns a new <see cref="Size3D" /> with the maximum X, Y, and Z values
    ///     from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size3D" /> to compare.</param>
    /// <param name="b">The second <see cref="Size3D" /> to compare.</param>
    /// <returns>
    ///     A new <see cref="Size3D" /> containing the maximum X, Y, and Z values
    ///     from the two input sizes.
    /// </returns>
    public static Size3D Max(Size3D a, Size3D b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    /// <summary>
    ///     Returns a new <see cref="Size3D" /> with the minimum X, Y, and Z values
    ///     from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size3D" /> to compare.</param>
    /// <param name="b">The second <see cref="Size3D" /> to compare.</param>
    /// <returns>
    ///     A new <see cref="Size3D" /> containing the minimum X, Y, and Z values
    ///     from the two input sizes.
    /// </returns>
    public static Size3D Min(Size3D a, Size3D b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
    
    /// <summary>
    ///     Determines whether the specified 3D point is contained within the current 3D area.
    /// </summary>
    /// <param name="point">The 3D point to check for containment within the area.</param>
    /// <returns>True if the point is contained within the area; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsIn(Point3D point) => ContainsIn(point.X, point.Y, point.Z);

    /// <summary>
    ///     Determines whether the specified 3D point is contained within the current 3D area.
    /// </summary>
    /// <param name="x">The X-coordinate of the point to check.</param>
    /// <param name="y">The Y-coordinate of the point to check.</param>
    /// <param name="z">The Z-coordinate of the point to check.</param>
    /// <returns>True if the 3D point is contained within the area; otherwise, false.</returns>
    public bool ContainsIn(int x, int y, int z) =>
        x >= 0 && x <= X &&
        y >= 0 && y <= Y &&
        z >= 0 && z <= Z;

    /// <summary>
    ///     Determines whether the specified 3D point is strictly within the bounds of the current 3D area,
    ///     excluding the border positions.
    /// </summary>
    /// <param name="point">The 3D point to check for containment within the extended area.</param>
    /// <returns>True if the point is contained within the extended area; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsEx(Point3D point) => ContainsEx(point.X, point.Y, point.Z);

    /// <summary>
    ///     Determines whether the specified 3D coordinates are strictly within the bounds of the
    ///     current 3D area, excluding the border positions.
    /// </summary>
    /// <param name="x">The X-coordinate of the point to check.</param>
    /// <param name="y">The Y-coordinate of the point to check.</param>
    /// <param name="z">The Z-coordinate of the point to check.</param>
    /// <returns>True if the coordinates are strictly within the bounds of the area; otherwise, false.</returns>
    public bool ContainsEx(int x, int y, int z) =>
        x > 0 && x < X &&
        y > 0 && y < Y &&
        z > 0 && z < Z;

}