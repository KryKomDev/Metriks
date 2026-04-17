using System.Globalization;

namespace Metriks.Tests;

public class Struct3DTests {
    
    [Fact]
    public void Point3D_Constructor_ShouldInitializeCorrectly() {
        var point = new Point3D(10, 20, 30);
        Assert.Equal(10, point.X);
        Assert.Equal(20, point.Y);
        Assert.Equal(30, point.Z);
    }

    [Fact]
    public void Point3D_Operations_ShouldWork() {
        var p1 = new Point3D(10, 20, 30);
        var p2 = new Point3D(5, 5, 5);
        
        Assert.Equal(new Point3D(15, 25, 35), p1 + p2);
        Assert.Equal(new Point3D(5, 15, 25), p1 - p2);
        Assert.Equal(new Point3D(50, 100, 150), p1 * p2);
        Assert.Equal(new Point3D(2, 4, 6), p1 / p2);
    }

    [Fact]
    public void Size3D_Constructor_ShouldInitializeCorrectly() {
        var size = new Size3D(10, 20, 30);
        Assert.Equal(10, size.X);
        Assert.Equal(20, size.Y);
        Assert.Equal(30, size.Z);
    }

    [Fact]
    public void Size3D_Operations_ShouldWork() {
        var s1 = new Size3D(10, 20, 30);
        var s2 = new Size3D(5, 5, 5);
        
        Assert.Equal(new Size3D(15, 25, 35), s1 + s2);
        Assert.Equal(new Size3D(5, 15, 25), s1 - s2);
        Assert.Equal(new Size3D(50, 100, 150), s1 * s2);
        Assert.Equal(new Size3D(2, 4, 6), s1 / s2);
    }

    [Fact]
    public void Area3D_Constructor_ShouldNormalizePoints() {
        var p1 = new Point3D(10, 10, 10);
        var p2 = new Point3D(0, 20, 5);
        var area = new Area3D(p1, p2);
        
        Assert.Equal(0, area.Lower.X);
        Assert.Equal(10, area.Higher.X);
        Assert.Equal(10, area.Lower.Y);
        Assert.Equal(20, area.Higher.Y);
        Assert.Equal(5, area.Lower.Z);
        Assert.Equal(10, area.Higher.Z);
    }

    [Fact]
    public void Area3D_Size_ShouldBeCorrect() {
        var area = new Area3D(new Point3D(0, 0, 0), new Size3D(10, 20, 30));
        Assert.Equal(10, area.Size.X);
        Assert.Equal(20, area.Size.Y);
        Assert.Equal(30, area.Size.Z);
    }

    [Fact]
    public void Area3D_Operations_ShouldWork() {
        var area = new Area3D(new Point3D(0, 0, 0), new Point3D(10, 10, 10));
        var size = new Size3D(5, 5, 5);
        var point = new Point3D(2, 2, 2);

        var plusSize = area + size;
        Assert.Equal(new Point3D(0, 0, 0), plusSize.Lower);
        Assert.Equal(new Point3D(15, 15, 15), plusSize.Higher);

        var minusSize = area - size;
        Assert.Equal(new Point3D(0, 0, 0), minusSize.Lower);
        Assert.Equal(new Point3D(5, 5, 5), minusSize.Higher);

        var plusPoint = area + point;
        Assert.Equal(new Point3D(2, 2, 2), plusPoint.Lower);
        Assert.Equal(new Point3D(12, 12, 12), plusPoint.Higher);

        var minusPoint = area - point;
        Assert.Equal(new Point3D(-2, -2, -2), minusPoint.Lower);
        Assert.Equal(new Point3D(8, 8, 8), minusPoint.Higher);
    }

    [Fact]
    public void Area3D_Deconstruct_ShouldWork() {
        var area = new Area3D(new Point3D(1, 2, 3), new Point3D(4, 5, 6));
        
        {
            area.Deconstruct(out Point3D p1, out Point3D p2);
            Assert.Equal(new Point3D(1, 2, 3), p1);
            Assert.Equal(new Point3D(4, 5, 6), p2);
        }

        {
            area.Deconstruct(out Point3D p_start, out Size3D size);
            Assert.Equal(new Point3D(1, 2, 3), p_start);
            Assert.Equal(new Size3D(3, 3, 3), size);
        }
    }

    [Fact]
    public void Area3D_ToString_ShouldReturnExpectedFormat() {
        var area = new Area3D(new Point3D(0, 0, 0), new Point3D(10, 20, 30));
        Assert.Equal("[(0, 0, 0):(10, 20, 30) | 10x20x30]", area.ToString(null, CultureInfo.InvariantCulture));
        Assert.Equal("[[0; 0; 0]:[10; 20; 30] | 10x20x30]", area.ToString(null, CultureInfo.GetCultureInfo("cs-CZ")));
    }
}