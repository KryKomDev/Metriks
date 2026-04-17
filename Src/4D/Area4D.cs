namespace Metriks;

public readonly record struct Area4D : IFormattable {

    private readonly int _lw;
    private readonly int _lx;
    private readonly int _ly;
    private readonly int _lz;
    private readonly int _hw;
    private readonly int _hx;
    private readonly int _hy;
    private readonly int _hz;
    
    public Point4D Lower {
        get => new(_lw, _lx, _ly, _lz);
        #if NET5_0_OR_GREATER
        init {
            (_lw, _hw) = value.W < _hw ? (value.W, _hw) : (_hw, value.W);
            (_lx, _hx) = value.X < _hx ? (value.X, _hx) : (_hx, value.X);
            (_ly, _hy) = value.Y < _hy ? (value.Y, _hy) : (_hy, value.Y);
            (_lz, _hz) = value.Z < _hz ? (value.Z, _hz) : (_hz, value.Z);
        } 
        #endif
    }
    
    public Point4D Higher {
        get => new(_hw, _hx, _hy, _hz);
        #if NET5_0_OR_GREATER
        init {
            (_lw, _hw) = _lw < value.W ? (_lw, value.W) : (value.W, _lw);
            (_lx, _hx) = _lx < value.X ? (_lx, value.X) : (value.X, _lx);
            (_ly, _hy) = _ly < value.Y ? (_ly, value.Y) : (value.Y, _ly);
            (_lz, _hz) = _lz < value.Z ? (_lz, value.Z) : (value.Z, _lz);
        }
        #endif
    }

    public Size4D Size => new(
        Math.Abs(_lw - _hw),
        Math.Abs(_lx - _hx),
        Math.Abs(_ly - _hy),
        Math.Abs(_lz - _hz)
    );

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