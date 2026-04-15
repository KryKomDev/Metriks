namespace Metriks;

public readonly record struct Area2D {

    private readonly int _lx;
    private readonly int _ly;
    private readonly int _hx;
    private readonly int _hy;
    
    public Point2D Lower {
        get => new(_lx, _ly);
        #if NET5_0_OR_GREATER
        init {
            if (_hx < value.X) {
                _lx = _hx;
                _hx = value.X;
            }
            else {
                _lx = value.X;
            }
            
            if (_hy < value.Y) {
                _ly = _hy;
                _hy = value.Y;
            }
            else {
                _ly = value.Y;
            }
        } 
        #endif
    }
    
    public Point2D Higher {
        get => new(_hx, _hy);
        #if NET5_0_OR_GREATER
        init {
            if (_lx > value.X) {
                _hx = _lx;
                _lx = value.X;
            }
            else {
                _hx = value.X;
            }
            
            if (_ly < value.Y) {
                _hy = _ly;
                _ly = value.Y;
            }
            else {
                _hy = value.Y;
            }
        }
        #endif
    }
    
    public Size2D Size => new(Math.Abs(Lower.X - Higher.X), Math.Abs(Lower.Y - Higher.Y));

    public Area2D(Point2D lower, Point2D higher) {
        (_lx, _hx) = lower.X < higher.X ? (lower.X, higher.X) : (higher.X, lower.X);
        (_ly, _hy) = lower.Y < higher.Y ? (lower.Y, higher.Y) : (higher.Y, lower.Y);
    }

    public Area2D(Point2D lower, Size2D s) {
        (_lx, _ly) = lower;
        _hx = lower.X + s.X; 
        _hy = lower.Y + s.Y;
    }

    public override string ToString() => $"{Lower}:{Higher}{Size}";

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