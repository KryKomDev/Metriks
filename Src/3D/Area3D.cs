namespace Metriks;

public readonly record struct Area3D : IFormattable {

    private readonly int _lx;
    private readonly int _ly;
    private readonly int _lz;
    private readonly int _hx;
    private readonly int _hy;
    private readonly int _hz;
    
    public int LowerX {
        get => _lx;
        #if NET5_0_OR_GREATER
        init => (_lx, _hx) = int.Order(value, _hx);
        #endif
    }
    
    public int LowerY {
        get => _ly;
        #if NET5_0_OR_GREATER
        init => (_ly, _hy) = int.Order(value, _hy);
        #endif
    }

    public int LowerZ {
        get => _lz;
        #if NET5_0_OR_GREATER
        init => (_lz, _hz) = int.Order(value, _hz);
        #endif
    }
    
    public int HigherX {
        get => _hx;
        #if NET5_0_OR_GREATER
        init => (_lx, _hx) = int.Order(value, _lx);
        #endif
    }
    
    public int HigherY {
        get => _hy;
        #if NET5_0_OR_GREATER
        init => (_ly, _hy) = int.Order(value, _ly);
        #endif
    }

    public int HigherZ {
        get => _hz;
        #if NET5_0_OR_GREATER
        init => (_lz, _hz) = int.Order(value, _lz);
        #endif
    }

    public Point3D Lower {
        get => new(_lx, _ly, _lz);
        #if NET5_0_OR_GREATER
        init {
            (_lx, _hx) = int.Order(value.X, _hx);
            (_ly, _hy) = int.Order(value.Y, _hy);
            (_lz, _hz) = int.Order(value.Z, _hz);
        } 
        #endif
    }
    
    public Point3D Higher {
        get => new(_hx, _hy, _hz);
        #if NET5_0_OR_GREATER
        init {
            (_lx, _hx) = int.Order(value.X, _lx);
            (_ly, _hy) = int.Order(value.Y, _ly);
            (_lz, _hz) = int.Order(value.Z, _lz);
        }
        #endif
    }

    public Size3D Size => new(
        Math.Abs(_lx - _hx),
        Math.Abs(_ly - _hy),
        Math.Abs(_lz - _hz)
    );

    public int SizeX => Math.Abs(_lx - _hx);
    public int SizeY => Math.Abs(_ly - _hy);
    public int SizeZ => Math.Abs(_lz - _hz);
    
    public Range RangeX => new(_lx, _hx);
    public Range RangeY => new(_ly, _hy);
    public Range RangeZ => new(_lz, _hz);

    public Area3D(int lowerX, int lowerY, int lowerZ, int higherX, int higherY, int higherZ) {
        (_lx, _hx) = lowerX < higherX ? (lowerX, higherX) : (higherX, lowerX);
        (_ly, _hy) = lowerY < higherY ? (lowerY, higherY) : (higherY, lowerY);
        (_lz, _hz) = lowerZ < higherZ ? (lowerZ, higherZ) : (higherZ, lowerZ);
    }

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

    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider) => 
        $"[{Lower.ToString(format, formatProvider)}:{Higher.ToString(format, formatProvider)} | {Size}]";

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