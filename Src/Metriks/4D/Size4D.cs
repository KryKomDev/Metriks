// Metriks
// Copyright (c) KryKom 2026

using System.Runtime.CompilerServices;

namespace Metriks;

/// <summary>
///     Represents a four-dimensional size structure using W, X, Y, and Z components.
/// </summary>
public readonly record struct Size4D {

    public Size4D(int w, int x, int y, int z) {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    ///     Gets the component along the W-axis.
    /// </summary>
    public int W { get; init; }

    /// <summary>
    ///     Gets the horizontal component (width) along the X-axis.
    /// </summary>
    public int X { get; init; }

    /// <summary>
    ///     Gets the vertical component (height) along the Y-axis.
    /// </summary>
    public int Y { get; init; }

    /// <summary>
    ///     Gets the depth component along the Z-axis.
    /// </summary>
    public int Z { get; init; }

    /// <summary>
    ///     Gets a <see cref="Size4D" /> with W, X, Y, and Z values set to 0.
    /// </summary>
    public static Size4D Zero { get; } = new(0, 0, 0, 0);

    /// <summary>
    ///     Gets a <see cref="Size4D" /> with W, X, Y, and Z values set to 1.
    /// </summary>
    public static Size4D One { get; } = new(1, 1, 1, 1);

    public override string ToString() => $"{W}x{X}x{Y}x{Z}";

    public Point4D ToPoint() => new(W, X, Y, Z);

    public static Size4D operator +(Size4D p) => p;
    public static Size4D operator -(Size4D p) => new(-p.W, -p.X, -p.Y, -p.Z);

    public static Size4D operator +(Size4D l, Size4D r) => new(l.W + r.W, l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Size4D operator -(Size4D l, Size4D r) => new(l.W - r.W, l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Size4D operator *(Size4D l, Size4D r) => new(l.W * r.W, l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Size4D operator /(Size4D l, Size4D r) => new(l.W / r.W, l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public static explicit operator Point4D(Size4D size) => new(size.W, size.X, size.Y, size.Z);

    public void Deconstruct(out int w, out int x, out int y, out int z) {
        w = W;
        x = X;
        y = Y;
        z = Z;
    }

    /// <summary>
    ///     Returns a new <see cref="Size4D" /> with the maximum W, X, Y, and Z values
    ///     from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size4D" /> to compare.</param>
    /// <param name="b">The second <see cref="Size4D" /> to compare.</param>
    /// <returns>
    ///     A new <see cref="Size4D" /> containing the maximum W, X, Y, and Z values
    ///     from the two input sizes.
    /// </returns>
    public static Size4D Max(Size4D a, Size4D b) =>
        new(Math.Max(a.W, b.W), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    /// <summary>
    ///     Returns a new <see cref="Size4D" /> with the minimum W, X, Y, and Z values
    ///     from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size4D" /> to compare.</param>
    /// <param name="b">The second <see cref="Size4D" /> to compare.</param>
    /// <returns>
    ///     A new <see cref="Size4D" /> containing the minimum W, X, Y, and Z values
    ///     from the two input sizes.
    /// </returns>
    public static Size4D Min(Size4D a, Size4D b) =>
        new(Math.Min(a.W, b.W), Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

    /// <summary>
    ///     Determines whether the specified 4D point is contained within the current 4D area.
    /// </summary>
    /// <param name="point">The 4D point to check for containment within the area.</param>
    /// <returns>True if the point is contained within the area; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsIn(Point4D point) => ContainsIn(point.W, point.X, point.Y, point.Z);

    /// <summary>
    ///     Determines whether the specified 4D point is contained within the current 4D area.
    /// </summary>
    /// <param name="w">The W-coordinate of the point to check.</param>
    /// <param name="x">The X-coordinate of the point to check.</param>
    /// <param name="y">The Y-coordinate of the point to check.</param>
    /// <param name="z">The Z-coordinate of the point to check.</param>
    /// <returns>True if the 4D point is contained within the area; otherwise, false.</returns>
    public bool ContainsIn(int w, int x, int y, int z) =>
        w >= 0 && w <= W &&
        x >= 0 && x <= X &&
        y >= 0 && y <= Y &&
        z >= 0 && z <= Z;

    /// <summary>
    ///     Determines whether the specified 4D point is strictly within the bounds of the current 4D area,
    ///     excluding the border positions.
    /// </summary>
    /// <param name="point">The 4D point to check for containment within the extended area.</param>
    /// <returns>True if the point is contained within the extended area; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsEx(Point4D point) => ContainsEx(point.W, point.X, point.Y, point.Z);

    /// <summary>
    ///     Determines whether the specified 4D coordinates are strictly within the bounds of the
    ///     current 4D area, excluding the border positions.
    /// </summary>
    /// <param name="w">The W-coordinate of the point to check.</param>
    /// <param name="x">The X-coordinate of the point to check.</param>
    /// <param name="y">The Y-coordinate of the point to check.</param>
    /// <param name="z">The Z-coordinate of the point to check.</param>
    /// <returns>True if the coordinates are strictly within the bounds of the area; otherwise, false.</returns>
    public bool ContainsEx(int w, int x, int y, int z) =>
        w > 0 && w < W &&
        x > 0 && x < X &&
        y > 0 && y < Y &&
        z > 0 && z < Z;

}