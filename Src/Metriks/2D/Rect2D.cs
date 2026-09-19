using System.Runtime.CompilerServices;

namespace Metriks;

/// <summary>
///     Represents a two-dimensional bounding area defined by lower and higher X and Y bounds.
/// </summary>
public readonly record struct Rect2D : IFormattable {
    private readonly int _hx;
    private readonly int _hy;

    private readonly int _lx;
    private readonly int _ly;

    public Rect2D(int lowerX, int lowerY, int higherX, int higherY) {
        (_lx, _hx) = lowerX < higherX ? (lowerX, higherX) : (higherX, lowerX);
        (_ly, _hy) = lowerY < higherY ? (lowerY, higherY) : (higherY, lowerY);
    }

    public Rect2D(Point2D lower, Point2D higher) {
        (_lx, _hx) = lower.X < higher.X ? (lower.X, higher.X) : (higher.X, lower.X);
        (_ly, _hy) = lower.Y < higher.Y ? (lower.Y, higher.Y) : (higher.Y, lower.Y);
    }

    public Rect2D(Point2D lower, Size2D s) {
        (_lx, _ly) = lower;
        _hx        = lower.X + s.X;
        _hy        = lower.Y + s.Y;
    }

    /// <summary>
    ///     Gets the lower bound along the X-axis.
    /// </summary>
    public int LowerX {
        get => _lx;
        #if NET5_0_OR_GREATER
        init => (_lx, _hx) = int.Order(value, _hx);
        #endif
    }

    /// <summary>
    ///     Gets the lower bound along the Y-axis.
    /// </summary>
    public int LowerY {
        get => _ly;
        #if NET5_0_OR_GREATER
        init => (_ly, _hy) = int.Order(value, _hy);
        #endif
    }

    /// <summary>
    ///     Gets the higher bound along the X-axis.
    /// </summary>
    public int HigherX {
        get => _hx;
        #if NET5_0_OR_GREATER
        init => (_lx, _hx) = int.Order(value, _lx);
        #endif
    }

    /// <summary>
    ///     Gets the higher bound along the Y-axis.
    /// </summary>
    public int HigherY {
        get => _hy;
        #if NET5_0_OR_GREATER
        init => (_ly, _hy) = int.Order(value, _ly);
        #endif
    }

    /// <summary>
    ///     Gets the lower-bound point of the area.
    /// </summary>
    public Point2D Lower {
        get => new(_lx, _ly);
        #if NET5_0_OR_GREATER
        init {
            (_lx, _hx) = int.Order(value.X, _hx);
            (_ly, _hy) = int.Order(value.Y, _hy);
        }
        #endif
    }

    /// <summary>
    ///     Gets the higher-bound point of the area.
    /// </summary>
    public Point2D Higher {
        get => new(_hx, _hy);
        #if NET5_0_OR_GREATER
        init {
            (_lx, _hx) = int.Order(value.X, _lx);
            (_ly, _hy) = int.Order(value.Y, _ly);
        }
        #endif
    }

    /// <summary>
    ///     Gets the size (width and height) of the area.
    /// </summary>
    public Size2D Size => new(Math.Abs(_lx - _hx), Math.Abs(_ly - _hy));

    /// <summary>
    ///     Gets the width (size along the X-axis) of the area.
    /// </summary>
    public int SizeX => Math.Abs(_lx - _hx);

    /// <summary>
    ///     Gets the height (size along the Y-axis) of the area.
    /// </summary>
    public int SizeY => Math.Abs(_ly - _hy);

    /// <summary>
    ///     Gets a <see cref="Range" /> representing the bounds along the X-axis.
    /// </summary>
    public Range RangeX => new(_lx, _hx);

    /// <summary>
    ///     Gets a <see cref="Range" /> representing the bounds along the Y-axis.
    /// </summary>
    public Range RangeY => new(_ly, _hy);

    public string ToString(string? format, IFormatProvider? formatProvider) => $"[{Lower.ToString(format, formatProvider)}:{Higher.ToString(format, formatProvider)} | {Size}]";

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
        x >= _lx && x <= _hx &&
        y >= _ly && y <= _hy;

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
        x > _lx && x < _hx &&
        y > _ly && y < _hy;

    /// <summary>
    /// Calculates the intersection of the current 2D area with another specified 2D area.
    /// </summary>
    /// <param name="other">The other 2D area to intersect with the current area.</param>
    /// <returns>An <see cref="Rect2D"/> representing the intersecting area if an intersection
    /// exists; otherwise, null.</returns>
    public Rect2D? Intersect(Rect2D other) {
        var lx = Math.Max(_lx, other.LowerX);
        var ly = Math.Max(_ly, other.LowerY);
        var hx = Math.Min(_hx, other.HigherX);
        var hy = Math.Min(_hy, other.HigherY);

        return hx < lx || hy < ly
            ? null
            : new Rect2D(lx, ly, hx, hy);
    }

    public override string ToString() => ToString(null, null);

    public static Rect2D operator +(Rect2D area, Size2D size) => new(area.Lower, area.Size + size);
    public static Rect2D operator -(Rect2D area, Size2D size) => new(area.Lower, area.Size - size);

    public static Rect2D operator +(Rect2D area, Point2D point) => new(area.Lower + point, area.Higher + point);
    public static Rect2D operator -(Rect2D area, Point2D point) => new(area.Lower - point, area.Higher - point);

    public static Rect2D? operator &(Rect2D left, Rect2D right) => left.Intersect(right);

    public void Deconstruct(out Point2D a, out Point2D b) {
        a = Lower;
        b = Higher;
    }

    public void Deconstruct(out Point2D a, out Size2D s) {
        a = Lower;
        s = Size;
    }
}