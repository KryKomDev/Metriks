namespace Metriks.Tests;

public class Span4DTests {
    [Fact]
    public void Constructor_Array_ShouldInitializeCorrectly() {
        int[,,,] array = new int[2, 2, 2, 2];
        array[0, 1, 0, 1] = 42;
        array[1, 0, 1, 0] = 99;

        var span = new Span4D<int>(array);

        Assert.Equal(2, span.WSize);
        Assert.Equal(2, span.XSize);
        Assert.Equal(2, span.YSize);
        Assert.Equal(2, span.ZSize);
        Assert.Equal(new Size4D(2, 2, 2, 2), span.Size);
        Assert.Equal(16, span.Length);

        Assert.Equal(42, span[0, 1, 0, 1]);
        Assert.Equal(99, span[new Point4D(1, 0, 1, 0)]);
    }

    [Fact]
    public void Slice_ShouldReturnCorrectSubSpan() {
        int[,,,] array = new int[3, 3, 3, 3];
        array[1, 1, 1, 1] = 100;

        var span = new Span4D<int>(array);
        var slice = span.Slice(1, 1, 1, 1, 2, 2, 2, 2);

        Assert.Equal(2, slice.WSize);
        Assert.Equal(2, slice.XSize);
        Assert.Equal(2, slice.YSize);
        Assert.Equal(2, slice.ZSize);

        Assert.Equal(100, slice[0, 0, 0, 0]);
        slice[0, 0, 0, 0] = 200;
        Assert.Equal(200, array[1, 1, 1, 1]);
    }

    [Fact]
    public void GetCubeAtW_ShouldReturnCorrect3DCube() {
        int[,,,] array = new int[2, 3, 3, 3];
        array[1, 1, 2, 0] = 42;

        var span = new Span4D<int>(array);
        var cube = span.GetCubeAtW(1);

        Assert.Equal(3, cube.XSize);
        Assert.Equal(3, cube.YSize);
        Assert.Equal(3, cube.ZSize);
        Assert.Equal(42, cube[1, 2, 0]);
    }

    [Fact]
    public void Enumerator_ShouldIterateCorrectly() {
        int[,,,] array = new int[2, 2, 2, 2];
        for (int w = 0; w < 2; w++)
            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 2; y++)
                    for (int z = 0; z < 2; z++)
                        array[w, x, y, z] = (w << 3) | (x << 2) | (y << 1) | z;

        var span = new Span4D<int>(array);
        var elements = new System.Collections.Generic.List<int>();

        foreach (var item in span) {
            elements.Add(item);
        }

        Assert.Equal(16, elements.Count);
        for (int i = 0; i < 16; i++) {
            Assert.Equal(i, elements[i]);
        }
    }
}
