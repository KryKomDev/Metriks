namespace Metriks.Tests;

public class Struct4DTests {
    
    [Fact]
    public void Point4D_Constructor_ShouldInitializeCorrectly() {
        var point = new Point4D(10, 20, 30, 40);
        Assert.Equal(10, point.W);
        Assert.Equal(20, point.X);
        Assert.Equal(30, point.Y);
        Assert.Equal(40, point.Z);
    }

    [Fact]
    public void Point4D_Operations_ShouldWork() {
        var p1 = new Point4D(10, 20, 30, 40);
        var p2 = new Point4D(5, 5, 5, 5);
        
        Assert.Equal(new Point4D(15, 25, 35, 45), p1 + p2);
        Assert.Equal(new Point4D(5, 15, 25, 35), p1 - p2);
        Assert.Equal(new Point4D(50, 100, 150, 200), p1 * p2);
        Assert.Equal(new Point4D(2, 4, 6, 8), p1 / p2);
    }

    [Fact]
    public void Size4D_Constructor_ShouldInitializeCorrectly() {
        var size = new Size4D(10, 20, 30, 40);
        Assert.Equal(10, size.W);
        Assert.Equal(20, size.X);
        Assert.Equal(30, size.Y);
        Assert.Equal(40, size.Z);
    }

    [Fact]
    public void Size4D_Operations_ShouldWork() {
        var s1 = new Size4D(10, 20, 30, 40);
        var s2 = new Size4D(5, 5, 5, 5);
        
        Assert.Equal(new Size4D(15, 25, 35, 45), s1 + s2);
        Assert.Equal(new Size4D(5, 15, 25, 35), s1 - s2);
        Assert.Equal(new Size4D(50, 100, 150, 200), s1 * s2);
        Assert.Equal(new Size4D(2, 4, 6, 8), s1 / s2);
    }

    [Fact]
    public void Area4D_Constructors_ShouldInitializeCorrectly() {
        var p1 = new Point4D(0, 0, 0, 0);
        var p2 = new Point4D(10, 10, 10, 10);
        var area1 = new Area4D(p1, p2);
        Assert.Equal(p1, area1.Lower);
        Assert.Equal(p2, area1.Higher);
        Assert.Equal(new Size4D(10, 10, 10, 10), area1.Size);

        var area2 = new Area4D(p2, p1);
        Assert.Equal(p1, area2.Lower);
        Assert.Equal(p2, area2.Higher);

        var size = new Size4D(5, 5, 5, 5);
        var area3 = new Area4D(p1, size);
        Assert.Equal(p1, area3.Lower);
        Assert.Equal(new Point4D(5, 5, 5, 5), area3.Higher);
        Assert.Equal(size, area3.Size);

        var area4 = new Area4D(10, 20, 30, 40, 0, 5, 15, 25);
        Assert.Equal(0, area4.LowerW);
        Assert.Equal(10, area4.HigherW);
        Assert.Equal(5, area4.LowerX);
        Assert.Equal(20, area4.HigherX);
        Assert.Equal(15, area4.LowerY);
        Assert.Equal(30, area4.HigherY);
        Assert.Equal(25, area4.LowerZ);
        Assert.Equal(40, area4.HigherZ);
    }

    [Fact]
    public void Area4D_CoordinateProperties_ShouldWork() {
        var area = new Area4D(new Point4D(1, 2, 3, 4), new Point4D(5, 6, 7, 8));
        Assert.Equal(1, area.LowerW);
        Assert.Equal(2, area.LowerX);
        Assert.Equal(3, area.LowerY);
        Assert.Equal(4, area.LowerZ);
        Assert.Equal(5, area.HigherW);
        Assert.Equal(6, area.HigherX);
        Assert.Equal(7, area.HigherY);
        Assert.Equal(8, area.HigherZ);

        #if NET5_0_OR_GREATER
        var updated = area with { LowerW = 9, HigherX = 1 };
        Assert.Equal(5, updated.LowerW);
        Assert.Equal(9, updated.HigherW);
        Assert.Equal(1, updated.LowerX);
        Assert.Equal(2, updated.HigherX);
        #endif
    }

    [Fact]
    public void Area4D_RangeProperties_ShouldBeCorrect() {
        var area = new Area4D(new Point4D(1, 2, 3, 4), new Point4D(10, 20, 30, 40));
        Assert.Equal(new Range(1, 10), area.RangeW);
        Assert.Equal(new Range(2, 20), area.RangeX);
        Assert.Equal(new Range(3, 30), area.RangeY);
        Assert.Equal(new Range(4, 40), area.RangeZ);
    }

    [Fact]
    public void Area4D_Operations_ShouldWork() {
        var area = new Area4D(new Point4D(0, 0, 0, 0), new Point4D(10, 10, 10, 10));
        var size = new Size4D(5, 5, 5, 5);
        var point = new Point4D(2, 2, 2, 2);

        var plusSize = area + size;
        Assert.Equal(new Point4D(0, 0, 0, 0), plusSize.Lower);
        Assert.Equal(new Point4D(15, 15, 15, 15), plusSize.Higher);

        var minusSize = area - size;
        Assert.Equal(new Point4D(0, 0, 0, 0), minusSize.Lower);
        Assert.Equal(new Point4D(5, 5, 5, 5), minusSize.Higher);

        var plusPoint = area + point;
        Assert.Equal(new Point4D(2, 2, 2, 2), plusPoint.Lower);
        Assert.Equal(new Point4D(12, 12, 12, 12), plusPoint.Higher);

        var minusPoint = area - point;
        Assert.Equal(new Point4D(-2, -2, -2, -2), minusPoint.Lower);
        Assert.Equal(new Point4D(8, 8, 8, 8), minusPoint.Higher);
    }

    [Fact]
    public void Area4D_Deconstruct_ShouldWork() {
        var area = new Area4D(new Point4D(1, 2, 3, 4), new Point4D(5, 6, 7, 8));
        
        {
            area.Deconstruct(out Point4D p1, out Point4D p2);
            Assert.Equal(new Point4D(1, 2, 3, 4), p1);
            Assert.Equal(new Point4D(5, 6, 7, 8), p2);
        }

        {
            area.Deconstruct(out Point4D p_start, out Size4D size);
            Assert.Equal(new Point4D(1, 2, 3, 4), p_start);
            Assert.Equal(new Size4D(4, 4, 4, 4), size);
        }
    }

    [Fact]
    public void Area4D_ToString_ShouldReturnExpectedFormat() {
        var area = new Area4D(new Point4D(0, 0, 0, 0), new Point4D(10, 20, 30, 40));
        Assert.Equal("[(0, 0, 0, 0):(10, 20, 30, 40) | 10x20x30x40]", area.ToString(null, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("[[0; 0; 0; 0]:[10; 20; 30; 40] | 10x20x30x40]", area.ToString(null, System.Globalization.CultureInfo.GetCultureInfo("cs-CZ")));
    }
}