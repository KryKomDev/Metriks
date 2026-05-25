namespace Metriks;

public readonly record struct Area2D : IFormattable {

    private readonly int _lx;
    private readonly int _ly;
    private readonly int _hx;
    private readonly int _hy;

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

    public Point2D Lower {
        get => new(_lx, _ly);
        #if NET5_0_OR_GREATER
        init {
            (_lx, _hx) = int.Order(value.X, _hx);
            (_ly, _hy) = int.Order(value.Y, _hy);
        } 
        #endif
    }
    
    public Point2D Higher {
        get => new(_hx, _hy);
        #if NET5_0_OR_GREATER
        init {
            (_lx, _hx) = int.Order(value.X, _lx);
            (_ly, _hy) = int.Order(value.Y, _ly);
        }
        #endif
    }
    
    public Size2D Size => new(Math.Abs(Lower.X - Higher.X), Math.Abs(Lower.Y - Higher.Y));

    public Range RangeX => new(Lower.X, Higher.X);
    public Range RangeY => new(Lower.Y, Higher.Y);

    public Area2D(int lowerX, int lowerY, int higherX, int higherY) {
        (_lx, _hx) = lowerX < higherX ? (lowerX, higherX) : (higherX, lowerX);
        (_ly, _hy) = lowerY < higherY ? (lowerY, higherY) : (higherY, lowerY);
    }
    
    public Area2D(Point2D lower, Point2D higher) {
        (_lx, _hx) = lower.X < higher.X ? (lower.X, higher.X) : (higher.X, lower.X);
        (_ly, _hy) = lower.Y < higher.Y ? (lower.Y, higher.Y) : (higher.Y, lower.Y);
    }

    public Area2D(Point2D lower, Size2D s) {
        (_lx, _ly) = lower;
        _hx = lower.X + s.X; 
        _hy = lower.Y + s.Y;
    }

    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider) => 
        $"[{Lower.ToString(format, formatProvider)}:{Higher.ToString(format, formatProvider)} | {Size}]";

    public static Area2D operator +(Area2D area, Size2D size) => new(area.Lower, area.Size + size);
    public static Area2D operator -(Area2D area, Size2D size) => new(area.Lower, area.Size - size);

    public static Area2D operator +(Area2D area, Point2D point) => new(area.Lower + point, area.Higher + point);
    public static Area2D operator -(Area2D area, Point2D point) => new(area.Lower - point, area.Higher - point);

    public void Deconstruct(out Point2D a, out Point2D b) {
        a = Lower;
        b = Higher;
    }

    public void Deconstruct(out Point2D a, out Size2D s) {
        a = Lower;
        s = Size;
    }
}