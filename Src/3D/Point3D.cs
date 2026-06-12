using System.Globalization;

namespace Metriks;

/// <summary>
/// Represents a three-dimensional point using X, Y, and Z coordinates.
/// </summary>
public readonly record struct Point3D {
    /// <summary>
    /// Gets the X coordinate of the point.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets the Y coordinate of the point.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Gets the Z coordinate of the point.
    /// </summary>
    public int Z { get; }
    
    public Point3D(int x, int y, int z) {
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

        return $"{startBrace}{X}{separator} {Y}{separator} {Z}{endBrace}";
    }
    
    public Size3D ToSize() => new(X, Y, Z);
    
    public static Point3D operator +(Point3D p) => p;
    public static Point3D operator -(Point3D p) => new(-p.X, -p.Y, -p.Z);
    
    public static Point3D operator +(Point3D l, Point3D r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Point3D operator -(Point3D l, Point3D r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Point3D operator *(Point3D l, Point3D r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Point3D operator /(Point3D l, Point3D r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    public void Deconstruct(out int x, out int y, out int z) {
        x = X;
        y = Y;
        z = Z;
    }

    /// <summary>
    /// Gets a <see cref="Point3D"/> with X, Y, and Z values set to 0.
    /// </summary>
    public static Point3D Zero { get; } = new(0, 0, 0);

    /// <summary>
    /// Gets a <see cref="Point3D"/> with X, Y, and Z values set to 1.
    /// </summary>
    public static Point3D One  { get; } = new(1, 1, 1);
    
    /// <summary>
    /// Returns a new <see cref="Point3D"/> with the maximum X, Y, and Z values
    /// from the two specified points.
    /// </summary>
    /// <param name="a">The first <see cref="Point3D"/> to compare.</param>
    /// <param name="b">The second <see cref="Point3D"/> to compare.</param>
    /// <returns>A new <see cref="Point3D"/> containing the maximum X, Y, and Z values
    /// from the two input points.</returns>
    public static Point3D Max(Point3D a, Point3D b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    /// <summary>
    /// Returns a new <see cref="Point3D"/> with the minimum X, Y, and Z values
    /// from the two specified points.
    /// </summary>
    /// <param name="a">The first <see cref="Point3D"/> to compare.</param>
    /// <param name="b">The second <see cref="Point3D"/> to compare.</param>
    /// <returns>A new <see cref="Point3D"/> containing the minimum X, Y, and Z values
    /// from the two input points.</returns>
    public static Point3D Min(Point3D a, Point3D b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
}