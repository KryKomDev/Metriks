// Metriks
// Copyright (c) KryKom 2026

using System.Runtime.CompilerServices;

namespace Metriks;

/// <summary>
///     Represents a two-dimensional size structure (width and height) using X and Y components.
/// </summary>
public readonly record struct Size2D {

    public Size2D(int x, int y) {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Gets the horizontal component (width) of the size.
    /// </summary>
    public int X {
        get;
        #if NET5_0_OR_GREATER
        init;
        #endif
    }

    /// <summary>
    ///     Gets the vertical component (height) of the size.
    /// </summary>
    public int Y {
        get;
        #if NET5_0_OR_GREATER
        init;
        #endif
    }

    /// <summary>
    ///     Gets a <see cref="Size2D" /> with X and Y values set to 0.
    /// </summary>
    public static Size2D Zero { get; } = new(0, 0);

    /// <summary>
    ///     Gets a <see cref="Size2D" /> with X and Y values set to 1.
    /// </summary>
    public static Size2D One { get; } = new(1, 1);

    public override string ToString() => $"{X}x{Y}";

    public Point2D ToPoint() => new(X, Y);
    
    /// <summary>
    ///     Determines whether the specified 2D point is contained within the current 2D area.
    /// </summary>
    /// <param name="point">The 2D point to check for containment within the area.</param>
    /// <returns>True if the point is contained within the area; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsIn(Point2D point) => ContainsIn(point.X, point.Y);

    /// <summary>
    ///     Determines whether the specified 2D point is contained within the current 2D area.
    /// </summary>
    /// <param name="x">The X-coordinate of the 2D point to check.</param>
    /// <param name="y">The Y-coordinate of the 2D point to check.</param>
    /// <returns>True if the 2D point is contained within the area; otherwise, false.</returns>
    public bool ContainsIn(int x, int y) =>
        x >= 0 && x <= X &&
        y >= 0 && y <= Y;

    /// <summary>
    ///     Determines whether the specified 2D point is strictly within the bounds of the current 2D area,
    ///     excluding the border positions.
    /// </summary>
    /// <param name="point">The 2D point to check for containment within the extended area.</param>
    /// <returns>True if the point is contained within the extended area; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsEx(Point2D point) => ContainsEx(point.X, point.Y);

    /// <summary>
    ///     Determines whether the specified 2D coordinates are strictly within the bounds of the current 2D area,
    ///     excluding the border positions.
    /// </summary>
    /// <param name="x">The X-coordinate of the point to check.</param>
    /// <param name="y">The Y-coordinate of the point to check.</param>
    /// <returns>True if the coordinates are strictly within the bounds of the area; otherwise, false.</returns>
    public bool ContainsEx(int x, int y) =>
        x > 0 && x < X &&
        y > 0 && y < Y;
    
    public static Size2D operator +(Size2D p) => p;
    public static Size2D operator -(Size2D p) => new(-p.X, -p.Y);

    public static Size2D operator +(Size2D l, Size2D r) => new(l.X + r.X, l.Y + r.Y);
    public static Size2D operator -(Size2D l, Size2D r) => new(l.X - r.X, l.Y - r.Y);
    public static Size2D operator *(Size2D l, Size2D r) => new(l.X * r.X, l.Y * r.Y);
    public static Size2D operator /(Size2D l, Size2D r) => new(l.X / r.X, l.Y / r.Y);

    public static explicit operator Point2D(Size2D size) => new(size.X, size.Y);

    public void Deconstruct(out int x, out int y) {
        x = X;
        y = Y;
    }

    /// <summary>
    ///     Returns a new <see cref="Size2D" /> with the maximum X and Y values
    ///     from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size2D" /> to compare.</param>
    /// <param name="b">The second <see cref="Size2D" /> to compare.</param>
    /// <returns>
    ///     A new <see cref="Size2D" /> containing the maximum X and Y values
    ///     from the two input sizes.
    /// </returns>
    public static Size2D Max(Size2D a, Size2D b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

    /// <summary>
    ///     Returns a new <see cref="Size2D" /> with the minimum X and Y values
    ///     from the two specified sizes.
    /// </summary>
    /// <param name="a">The first <see cref="Size2D" /> to compare.</param>
    /// <param name="b">The second <see cref="Size2D" /> to compare.</param>
    /// <returns>
    ///     A new <see cref="Size2D" /> containing the minimum X and Y values
    ///     from the two input sizes.
    /// </returns>
    public static Size2D Min(Size2D a, Size2D b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
}