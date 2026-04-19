using System.Globalization;

namespace Metriks;

public readonly record struct Point2D : IFormattable {

    public int X {
        get;
        #if NET5_0_OR_GREATER
        init;
        #endif
    }

    public int Y {
        get;
        #if NET5_0_OR_GREATER
        init;
        #endif
    }

    public Point2D(int x, int y) {
        X = x;
        Y = y;
    }

    public override string ToString() => ToString(null, null);
    
    public string ToString(string? format, IFormatProvider? formatProvider) {
        formatProvider ??= CultureInfo.CurrentCulture;
    
        var  culture = formatProvider as CultureInfo ?? CultureInfo.CurrentCulture;
        bool isCzech = culture.TwoLetterISOLanguageName.Equals("cs", StringComparison.OrdinalIgnoreCase);

        string startBrace = isCzech ? "[" : "(";
        string endBrace   = isCzech ? "]" : ")";

        string separator = culture.NumberFormat.NumberDecimalSeparator == "," ? ";" : ",";

        return $"{startBrace}{X}{separator} {Y}{endBrace}";
    }

    public Size2D ToSize() => new(X, Y);
    
    public static Point2D operator +(Point2D p) => p;
    public static Point2D operator -(Point2D p) => new(-p.X, -p.Y);

    public static Point2D operator +(Point2D l, Point2D r) => new(l.X + r.X, l.Y + r.Y);
    public static Point2D operator -(Point2D l, Point2D r) => new(l.X - r.X, l.Y - r.Y);
    public static Point2D operator *(Point2D l, Point2D r) => new(l.X * r.X, l.Y * r.Y);
    public static Point2D operator /(Point2D l, Point2D r) => new(l.X / r.X, l.Y / r.Y);
    
    public static explicit operator Size2D(Point2D point) => new(point.X, point.Y);

    public static Point2D Zero { get; } = new(0, 0);
    public static Point2D One { get; } = new(1, 1);

    public void Deconstruct(out int x, out int y) {
        x = X;
        y = Y;
    }

    /// <summary>
    /// Returns a new <see cref="Point2D"/> with the maximum X and Y values
    /// from the two specified points.
    /// </summary>
    /// <param name="a">The first <see cref="Point2D"/> to compare.</param>
    /// <param name="b">The second <see cref="Point2D"/> to compare.</param>
    /// <returns>A new <see cref="Point2D"/> containing the maximum X and Y values
    /// from the two input points.</returns>
    public static Point2D Max(Point2D a, Point2D b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

    /// <summary>
    /// Returns a new <see cref="Point2D"/> with the minimum X and Y values
    /// from the two specified points.
    /// </summary>
    /// <param name="a">The first <see cref="Point2D"/> to compare.</param>
    /// <param name="b">The second <see cref="Point2D"/> to compare.</param>
    /// <returns>A new <see cref="Point2D"/> containing the minimum X and Y values
    /// from the two input points.</returns>
    public static Point2D Min(Point2D a, Point2D b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
}