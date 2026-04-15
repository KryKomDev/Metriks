namespace Metriks.Tests;

public class Struct2DTests {
    
    [Fact]
    public void Point2D_Constructor_ShouldInitializeCorrectly() {
        var point = new Point2D(10, 20);
        Assert.Equal(10, point.X);
        Assert.Equal(20, point.Y);
    }

    [Fact]
    public void Point2D_Operations_ShouldWork() {
        var p1 = new Point2D(10, 20);
        var p2 = new Point2D(5, 5);
        
        Assert.Equal(new Point2D(15, 25), p1 + p2);
        Assert.Equal(new Point2D(5, 15), p1 - p2);
        Assert.Equal(new Point2D(50, 100), p1 * p2);
        Assert.Equal(new Point2D(2, 4), p1 / p2);
    }

    [Fact]
    public void Size2D_Constructor_ShouldInitializeCorrectly() {
        var size = new Size2D(10, 20);
        Assert.Equal(10, size.X);
        Assert.Equal(20, size.Y);
    }

    [Fact]
    public void Size2D_Operations_ShouldWork() {
        var s1 = new Size2D(10, 20);
        var s2 = new Size2D(5, 5);
        
        Assert.Equal(new Size2D(15, 25), s1 + s2);
        Assert.Equal(new Size2D(5, 15), s1 - s2);
        Assert.Equal(new Size2D(50, 100), s1 * s2);
        Assert.Equal(new Size2D(2, 4), s1 / s2);
    }

    [Fact]
    public void Area2D_Constructors_ShouldInitializeCorrectly() {
        var p1 = new Point2D(0, 0);
        var p2 = new Point2D(10, 10);
        var area1 = new Area2D(p1, p2);
        Assert.Equal(p1, area1.Lower);
        Assert.Equal(p2, area1.Higher);
        Assert.Equal(new Size2D(10, 10), area1.Size);

        var area2 = new Area2D(p2, p1);
        Assert.Equal(p1, area2.Lower);
        Assert.Equal(p2, area2.Higher);

        var size = new Size2D(5, 5);
        var area3 = new Area2D(p1, size);
        Assert.Equal(p1, area3.Lower);
        Assert.Equal(new Point2D(5, 5), area3.Higher);
        Assert.Equal(size, area3.Size);
    }

    [Fact]
    public void Area2D_Operations_ShouldWork() {
        var area = new Area2D(new Point2D(0, 0), new Point2D(10, 10));
        var size = new Size2D(5, 5);
        var point = new Point2D(2, 2);

        var plusSize = area + size;
        Assert.Equal(new Point2D(0, 0), plusSize.Lower);
        Assert.Equal(new Point2D(15, 15), plusSize.Higher);

        var minusSize = area - size;
        Assert.Equal(new Point2D(0, 0), minusSize.Lower);
        Assert.Equal(new Point2D(5, 5), minusSize.Higher);

        var plusPoint = area + point;
        Assert.Equal(new Point2D(2, 2), plusPoint.Lower);
        Assert.Equal(new Point2D(12, 12), plusPoint.Higher);

        var minusPoint = area - point;
        Assert.Equal(new Point2D(-2, -2), minusPoint.Lower);
        Assert.Equal(new Point2D(8, 8), minusPoint.Higher);
    }

    [Fact]
    public void Area2D_Deconstruct_ShouldWork() {
        var area = new Area2D(new Point2D(1, 2), new Point2D(3, 4));
        
        {
            area.Deconstruct(out Point2D p1, out Point2D p2);
            Assert.Equal(new Point2D(1, 2), p1);
            Assert.Equal(new Point2D(3, 4), p2);
        }

        {
            area.Deconstruct(out Point2D p_start, out Size2D size);
            Assert.Equal(new Point2D(1, 2), p_start);
            Assert.Equal(new Size2D(2, 2), size);
        }
    }

    [Fact]
    public void Area2D_ToString_ShouldReturnExpectedFormat() {
        var area = new Area2D(new Point2D(0, 0), new Point2D(10, 20));
        Assert.Equal("[0, 0]:[10, 20](10, 20)", area.ToString());
    }
}