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
}