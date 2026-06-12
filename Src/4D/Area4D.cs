using System.Runtime.CompilerServices;

namespace Metriks;

/// <summary>
/// Represents a four-dimensional bounding area defined by lower and higher W, X, Y, and Z bounds.
/// </summary>
public readonly record struct Area4D : IFormattable {

    private readonly int _lw;
    private readonly int _lx;
    private readonly int _ly;
    private readonly int _lz;
    private readonly int _hw;
    private readonly int _hx;
    private readonly int _hy;
    private readonly int _hz;
    
    /// <summary>
    /// Gets the lower bound along the W-axis.
    /// </summary>
    public int LowerW {
        get => _lw;
        #if NET5_0_OR_GREATER
        init => (_lw, _hw) = int.Order(value, _hw);
        #endif
    }

    /// <summary>
    /// Gets the lower bound along the X-axis.
    /// </summary>
    public int LowerX {
        get => _lx;
        #if NET5_0_OR_GREATER
        init => (_lx, _hx) = int.Order(value, _hx);
        #endif
    }
    
    /// <summary>
    /// Gets the lower bound along the Y-axis.
    /// </summary>
    public int LowerY {
        get => _ly;
        #if NET5_0_OR_GREATER
        init => (_ly, _hy) = int.Order(value, _hy);
        #endif
    }

    /// <summary>
    /// Gets the lower bound along the Z-axis.
    /// </summary>
    public int LowerZ {
        get => _lz;
        #if NET5_0_OR_GREATER
        init => (_lz, _hz) = int.Order(value, _hz);
        #endif
    }
    
    /// <summary>
    /// Gets the higher bound along the W-axis.
    /// </summary>
    public int HigherW {
        get => _hw;
        #if NET5_0_OR_GREATER
        init => (_lw, _hw) = int.Order(value, _lw);
        #endif
    }

    /// <summary>
    /// Gets the higher bound along the X-axis.
    /// </summary>
    public int HigherX {
        get => _hx;
        #if NET5_0_OR_GREATER
        init => (_lx, _hx) = int.Order(value, _lx);
        #endif
    }
    
    /// <summary>
    /// Gets the higher bound along the Y-axis.
    /// </summary>
    public int HigherY {
        get => _hy;
        #if NET5_0_OR_GREATER
        init => (_ly, _hy) = int.Order(value, _ly);
        #endif
    }

    /// <summary>
    /// Gets the higher bound along the Z-axis.
    /// </summary>
    public int HigherZ {
        get => _hz;
        #if NET5_0_OR_GREATER
        init => (_lz, _hz) = int.Order(value, _lz);
        #endif
    }

    /// <summary>
    /// Gets the lower-bound point of the area.
    /// </summary>
    public Point4D Lower {
        get => new(_lw, _lx, _ly, _lz);
        #if NET5_0_OR_GREATER
        init {
            (_lw, _hw) = int.Order(value.W, _hw);
            (_lx, _hx) = int.Order(value.X, _hx);
            (_ly, _hy) = int.Order(value.Y, _hy);
            (_lz, _hz) = int.Order(value.Z, _hz);
        } 
        #endif
    }
    
    /// <summary>
    /// Gets the higher-bound point of the area.
    /// </summary>
    public Point4D Higher {
        get => new(_hw, _hx, _hy, _hz);
        #if NET5_0_OR_GREATER
        init {
            (_lw, _hw) = int.Order(value.W, _lw);
            (_lx, _hx) = int.Order(value.X, _lx);
            (_ly, _hy) = int.Order(value.Y, _ly);
            (_lz, _hz) = int.Order(value.Z, _lz);
        }
        #endif
    }

    /// <summary>
    /// Gets the size of the area.
    /// </summary>
    public Size4D Size => new(
        Math.Abs(_lw - _hw),
        Math.Abs(_lx - _hx),
        Math.Abs(_ly - _hy),
        Math.Abs(_lz - _hz)
    );

    /// <summary>
    /// Gets the size along the W-axis of the area.
    /// </summary>
    public int SizeW => Math.Abs(_lw - _hw);

    /// <summary>
    /// Gets the width (size along the X-axis) of the area.
    /// </summary>
    public int SizeX => Math.Abs(_lx - _hx);

    /// <summary>
    /// Gets the height (size along the Y-axis) of the area.
    /// </summary>
    public int SizeY => Math.Abs(_ly - _hy);

    /// <summary>
    /// Gets the depth (size along the Z-axis) of the area.
    /// </summary>
    public int SizeZ => Math.Abs(_lz - _hz);
    
    /// <summary>
    /// Gets a <see cref="Range"/> representing the bounds along the W-axis.
    /// </summary>
    public Range RangeW => new(_lw, _hw);

    /// <summary>
    /// Gets a <see cref="Range"/> representing the bounds along the X-axis.
    /// </summary>
    public Range RangeX => new(_lx, _hx);

    /// <summary>
    /// Gets a <see cref="Range"/> representing the bounds along the Y-axis.
    /// </summary>
    public Range RangeY => new(_ly, _hy);

    /// <summary>
    /// Gets a <see cref="Range"/> representing the bounds along the Z-axis.
    /// </summary>
    public Range RangeZ => new(_lz, _hz);

    public Area4D(int lowerW, int lowerX, int lowerY, int lowerZ, int higherW, int higherX, int higherY, int higherZ) {
        (_lw, _hw) = lowerW < higherW ? (lowerW, higherW) : (higherW, lowerW);
        (_lx, _hx) = lowerX < higherX ? (lowerX, higherX) : (higherX, lowerX);
        (_ly, _hy) = lowerY < higherY ? (lowerY, higherY) : (higherY, lowerY);
        (_lz, _hz) = lowerZ < higherZ ? (lowerZ, higherZ) : (higherZ, lowerZ);
    }

    public Area4D(Point4D lower, Point4D higher) {
        (_lw, _hw) = lower.W < higher.W ? (lower.W, higher.W) : (higher.W, lower.W);
        (_lx, _hx) = lower.X < higher.X ? (lower.X, higher.X) : (higher.X, lower.X);
        (_ly, _hy) = lower.Y < higher.Y ? (lower.Y, higher.Y) : (higher.Y, lower.Y);
        (_lz, _hz) = lower.Z < higher.Z ? (lower.Z, higher.Z) : (higher.Z, lower.Z);
    }

    public Area4D(Point4D lower, Size4D s) {
        (_lw, _lx, _ly, _lz) = lower;
        _hw = lower.W + s.W; 
        _hx = lower.X + s.X; 
        _hy = lower.Y + s.Y;
        _hz = lower.Z + s.Z;
    }

    /// <summary>
    /// Determines whether the specified 4D point is contained within the current 4D area.
    /// </summary>
    /// <param name="point">The 4D point to check for containment within the area.</param>
    /// <returns>True if the point is contained within the area; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsIn(Point4D point) {
        return ContainsIn(point.W, point.X, point.Y, point.Z);
    }

    /// <summary>
    /// Determines whether the specified 4D point is contained within the current 4D area.
    /// </summary>
    /// <param name="w">The W-coordinate of the 4D point to check.</param>
    /// <param name="x">The X-coordinate of the 4D point to check.</param>
    /// <param name="y">The Y-coordinate of the 4D point to check.</param>
    /// <param name="z">The Z-coordinate of the 4D point to check.</param>
    /// <returns>True if the 4D point is contained within the area; otherwise, false.</returns>
    public bool ContainsIn(int w, int x, int y, int z) {
        return
            w >= _lw && w <= _hw &&
            x >= _lx && x <= _hx &&
            y >= _ly && y <= _hy &&
            z >= _lz && z <= _hz;
    }

    /// <summary>
    /// Determines whether the specified 4D point is strictly within the bounds of the current 4D area,
    /// excluding the border positions.
    /// </summary>
    /// <param name="point">The 4D point to check for containment within the extended area.</param>
    /// <returns>True if the point is contained within the extended area; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsEx(Point4D point) {
        return ContainsEx(point.W, point.X, point.Y, point.Z);
    }
    
    /// <summary>
    /// Determines whether the specified 4D coordinates are strictly within the bounds of the current 4D area,
    /// excluding the border positions.
    /// </summary>
    /// <param name="w">The W-coordinate of the point to check.</param>
    /// <param name="x">The X-coordinate of the point to check.</param>
    /// <param name="y">The Y-coordinate of the point to check.</param>
    /// <param name="z">The Z-coordinate of the point to check.</param>
    /// <returns>True if the coordinates are strictly within the bounds of the area; otherwise, false.</returns>
    public bool ContainsEx(int w, int x, int y, int z) {
        return
            w > _lw && w < _hw &&
            x > _lx && x < _hx &&
            y > _ly && y < _hy &&
            z > _lz && z < _hz;
    }
    
    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider) => 
        $"[{Lower.ToString(format, formatProvider)}:{Higher.ToString(format, formatProvider)} | {Size}]";

    public static Area4D operator +(Area4D area, Size4D size) => new(area.Lower, area.Size + size);
    public static Area4D operator -(Area4D area, Size4D size) => new(area.Lower, area.Size - size);

    public static Area4D operator +(Area4D area, Point4D point) => new(area.Lower + point, area.Higher + point);
    public static Area4D operator -(Area4D area, Point4D point) => new(area.Lower - point, area.Higher - point);

    public void Deconstruct(out Point4D a, out Point4D b) {
        a = Lower;
        b = Higher;
    }

    public void Deconstruct(out Point4D a, out Size4D s) {
        a = Lower;
        s = Size;
    }
}