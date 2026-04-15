namespace Metriks;

public readonly record struct Area3D {

    private readonly int _lx;
    private readonly int _ly;
    private readonly int _lz;
    private readonly int _hx;
    private readonly int _hy;
    private readonly int _hz;
    
    public Point3D Lower {
        get => new(_lx, _ly, _lz);
        #if NET5_0_OR_GREATER
        init {
            (_lx, _hx) = value.X < _hx ? (value.X, _hx) : (_hx, value.X);
            (_ly, _hy) = value.Y < _hy ? (value.Y, _hy) : (_hy, value.Y);
            (_lz, _hz) = value.Z < _hz ? (value.Z, _hz) : (_hz, value.Z);
        } 
        #endif
    }
    
    public Point3D Higher {
        get => new(_hx, _hy, _hz);
        #if NET5_0_OR_GREATER
        init {
            (_lx, _hx) = _lx < value.X ? (_lx, value.X) : (value.X, _lx);
            (_ly, _hy) = _ly < value.Y ? (_ly, value.Y) : (value.Y, _ly);
            (_lz, _hz) = _lz < value.Z ? (_lz, value.Z) : (value.Z, _lz);
        }
        #endif
    }

    public Size3D Size => new(
        Math.Abs(_lx - _hx),
        Math.Abs(_ly - _hy),
        Math.Abs(_lz - _hz)
    );

    public Area3D(Point3D lower, Point3D higher) {
        (_lx, _hx) = lower.X < higher.X ? (lower.X, higher.X) : (higher.X, lower.X);
        (_ly, _hy) = lower.Y < higher.Y ? (lower.Y, higher.Y) : (higher.Y, lower.Y);
        (_lz, _hz) = lower.Z < higher.Z ? (lower.Z, higher.Z) : (higher.Z, lower.Z);
    }

    public Area3D(Point3D lower, Size3D s) {
        (_lx, _ly, _lz) = lower;
        _hx = lower.X + s.X; 
        _hy = lower.Y + s.Y;
        _hz = lower.Z + s.Z;
    }

    public override string ToString() => $"{Lower}:{Higher}{Size}";

    public static Area3D operator +(Area3D area, Size3D size) => new(area.Lower, area.Size + size);
    public static Area3D operator -(Area3D area, Size3D size) => new(area.Lower, area.Size - size);

    public static Area3D operator +(Area3D area, Point3D point) => new(area.Lower + point, area.Higher + point);
    public static Area3D operator -(Area3D area, Point3D point) => new(area.Lower - point, area.Higher - point);

    public void Deconstruct(out Point3D a, out Point3D b) {
        a = Lower;
        b = Higher;
    }

    public void Deconstruct(out Point3D a, out Size3D s) {
        a = Lower;
        s = Size;
    }
}