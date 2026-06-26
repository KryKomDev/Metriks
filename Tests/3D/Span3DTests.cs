namespace Metriks.Tests;

public class Span3DTests {
    [Fact]
    public void Constructor_Array_ShouldInitializeCorrectly() {
        int[,,] array = {
            {
                { 1, 2 },
                { 3, 4 }
            },
            {
                { 5, 6 },
                { 7, 8 }
            }
        };

        var span = new Span3D<int>(array);

        Assert.Equal(2, span.XSize);
        Assert.Equal(2, span.YSize);
        Assert.Equal(2, span.ZSize);
        Assert.Equal(new Size3D(2, 2, 2), span.Size);
        Assert.Equal(8, span.Length);
        Assert.False(span.IsEmpty);

        Assert.Equal(1, span[0, 0, 0]);
        Assert.Equal(4, span[0, 1, 1]);
        Assert.Equal(7, span[1, 1, 0]);
        Assert.Equal(8, span[new Point3D(1, 1, 1)]);
    }

    [Fact]
    public void Indexer_ShouldModifyOriginalArray() {
        int[,,] array = new int[2, 2, 2];
        var span = new Span3D<int>(array);

        span[0, 1, 0] = 42;
        Assert.Equal(42, array[0, 1, 0]);
    }

    [Fact]
    public void Slice_ShouldReturnCorrectSubSpan() {
        int[,,] array = {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        var span = new Span3D<int>(array);
        // Slice along Y and Z
        var slice = span.Slice(0, 0, 1, 2, 2, 2);

        Assert.Equal(2, slice.XSize);
        Assert.Equal(2, slice.YSize);
        Assert.Equal(2, slice.ZSize);

        Assert.Equal(2, slice[0, 0, 0]);
        Assert.Equal(3, slice[0, 0, 1]);
        Assert.Equal(5, slice[0, 1, 0]);
        Assert.Equal(6, slice[0, 1, 1]);

        Assert.Equal(8, slice[1, 0, 0]);
        Assert.Equal(11, slice[1, 1, 0]);
    }

    [Fact]
    public void GetPlaneAtX_ShouldReturnCorrect2DPlane() {
        int[,,] array = {
            {
                { 1, 2 },
                { 3, 4 }
            },
            {
                { 5, 6 },
                { 7, 8 }
            }
        };

        var span = new Span3D<int>(array);
        var plane = span.GetPlaneAtX(1);

        Assert.Equal(2, plane.XSize);
        Assert.Equal(2, plane.YSize);

        Assert.Equal(5, plane[0, 0]);
        Assert.Equal(6, plane[0, 1]);
        Assert.Equal(7, plane[1, 0]);
        Assert.Equal(8, plane[1, 1]);
    }

    [Fact]
    public void Enumerator_ShouldIterateCorrectly() {
        int[,,] array = {
            {
                { 1, 2 },
                { 3, 4 }
            },
            {
                { 5, 6 },
                { 7, 8 }
            }
        };

        var span = new Span3D<int>(array);
        var elements = new System.Collections.Generic.List<int>();

        foreach (var item in span) {
            elements.Add(item);
        }

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, elements);
    }
}
