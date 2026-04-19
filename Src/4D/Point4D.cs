using System.Globalization;

namespace Metriks;

public readonly record struct Point4D : IFormattable {
    
    public int W {
        get;
        #if NET5_0_OR_GREATER
        init; 
        #endif
    }
    
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
    
    public int Z {
        get;
        #if NET5_0_OR_GREATER
        init; 
        #endif
    }
    
    public Point4D(int w, int x, int y, int z) {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString() => ToString(null, null);
    
    public string ToString(string? format, IFormatProvider? formatProvider) {
        formatProvider ??= CultureInfo.CurrentCulture;
    
        var  culture = formatProvider as CultureInfo ?? CultureInfo.CurrentCulture;
        bool isCzech = culture.TwoLetterISOLanguageName.Equals("cs", StringComparison.OrdinalIgnoreCase);

        string startBrace = isCzech ? "[" : "(";
        string endBrace   = isCzech ? "]" : ")";

        string separator = culture.NumberFormat.NumberDecimalSeparator == "," ? ";" : ",";

        return $"{startBrace}{W}{separator} {X}{separator} {Y}{separator} {Z}{endBrace}";
    }

    public Size4D ToSize() => new(W, X, Y, Z);
    
    public static Point4D operator +(Point4D p) => p;
    public static Point4D operator -(Point4D p) => new(-p.W, -p.X, -p.Y, -p.Z);

    public static Point4D operator +(Point4D l, Point4D r) => new(l.W + r.W, l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Point4D operator -(Point4D l, Point4D r) => new(l.W - r.W, l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Point4D operator *(Point4D l, Point4D r) => new(l.W * r.W, l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Point4D operator /(Point4D l, Point4D r) => new(l.W / r.W, l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public static explicit operator Size4D(Point4D point) => new(point.W, point.X, point.Y, point.Z);
    
    public static Point4D Zero { get; } = new(0, 0, 0, 0);
    public static Point4D One  { get; } = new(1, 1, 1, 1);

    public void Deconstruct(out int w, out int x, out int y, out int z) {
        w = W;
        x = X;
        y = Y;
        z = Z;
    }
    
    /// <summary>
    /// Returns a new <see cref="Point4D"/> with the maximum W, X, Y, and Z values
    /// from the two specified points.
    /// </summary>
    /// <param name="a">The first <see cref="Point4D"/> to compare.</param>
    /// <param name="b">The second <see cref="Point4D"/> to compare.</param>
    /// <returns>A new <see cref="Point4D"/> containing the maximum W, X, Y, and Z values
    /// from the two input points.</returns>
    public static Point4D Max(Point4D a, Point4D b) =>
        new(Math.Max(a.W, b.W), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    /// <summary>
    /// Returns a new <see cref="Point4D"/> with the minimum W, X, Y, and Z values
    /// from the two specified points.
    /// </summary>
    /// <param name="a">The first <see cref="Point4D"/> to compare.</param>
    /// <param name="b">The second <see cref="Point4D"/> to compare.</param>
    /// <returns>A new <see cref="Point4D"/> containing the minimum W, X, Y, and Z values
    /// from the two input points.</returns>
    public static Point4D Min(Point4D a, Point4D b) =>
        new(Math.Min(a.W, b.W), Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
}