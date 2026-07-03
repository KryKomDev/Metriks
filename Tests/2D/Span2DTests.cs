namespace Metriks.Tests;

public class Span2DTests {
    [Fact]
    public void Constructor_Array_ShouldInitializeCorrectly() {
        int[,] array = {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        var span = new Span2D<int>(array);

        Assert.Equal(2,                span.XSize);
        Assert.Equal(3,                span.YSize);
        Assert.Equal(2,                span.XCount);
        Assert.Equal(3,                span.YCount);
        Assert.Equal(new Size2D(2, 3), span.Size);
        Assert.Equal(6,                span.Length);
        Assert.Equal(6,                span.Count);
        Assert.False(span.IsEmpty);
        Assert.Equal(3, span.Stride);

        Assert.Equal(1, span[0, 0]);
        Assert.Equal(5, span[1, 1]);
        Assert.Equal(6, span[new Point2D(1, 2)]);
    }

    [Fact]
    public void Indexer_ShouldModifyOriginalArray() {
        int[,] array = {
            { 1, 2 },
            { 3, 4 }
        };

        var span = new Span2D<int>(array);
        span[0, 1]              = 42;
        span[new Point2D(1, 0)] = 99;

        Assert.Equal(42, array[0, 1]);
        Assert.Equal(99, array[1, 0]);
    }

    [Fact]
    public void Indexer_OutOfBounds_ShouldThrow() {
        var array = new int[2, 2];
        var span  = new Span2D<int>(array);

        var threw = false;

        try {
            var _ = span[2, 0];
        }
        catch (IndexOutOfRangeException) {
            threw = true;
        }

        Assert.True(threw, "Expected IndexOutOfRangeException for [2, 0]");

        threw = false;

        try {
            var _ = span[0, 2];
        }
        catch (IndexOutOfRangeException) {
            threw = true;
        }

        Assert.True(threw, "Expected IndexOutOfRangeException for [0, 2]");
    }

    [Fact]
    public void GetRow_ShouldReturnCorrectRowSpan() {
        int[,] array = {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        var span = new Span2D<int>(array);
        var row  = span.GetRow(1);

        Assert.Equal(3, row.Length);
        Assert.Equal(4, row[0]);
        Assert.Equal(5, row[1]);
        Assert.Equal(6, row[2]);
    }

    [Fact]
    public void Enumerator_ShouldIterateInRowMajorOrder() {
        int[,] array = {
            { 1, 2 },
            { 3, 4 }
        };

        var span     = new Span2D<int>(array);
        var elements = new List<int>();

        foreach (var item in span)
            elements.Add(item);

        Assert.Equal(new[] { 1, 2, 3, 4 }, elements);
    }

    [Fact]
    public void Slice_ShouldReturnCorrectSubSpan() {
        int[,] array = {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 }
        };

        var span  = new Span2D<int>(array);
        var slice = span.Slice(1, 1, 2, 2);

        Assert.Equal(2, slice.XSize);
        Assert.Equal(2, slice.YSize);
        Assert.Equal(4, slice.Stride);

        Assert.Equal(6,  slice[0, 0]);
        Assert.Equal(7,  slice[0, 1]);
        Assert.Equal(10, slice[1, 0]);
        Assert.Equal(11, slice[1, 1]);

        slice[0, 0] = 99;
        Assert.Equal(99, array[1, 1]);
    }

    [Fact]
    public void CopyTo_ShouldCopyElementsCorrectly() {
        int[,] sourceArray = {
            { 1, 2 },
            { 3, 4 }
        };

        var destArray = new int[2, 2];

        var srcSpan  = new Span2D<int>(sourceArray);
        var destSpan = new Span2D<int>(destArray);

        srcSpan.CopyTo(destSpan);

        Assert.Equal(1, destArray[0, 0]);
        Assert.Equal(2, destArray[0, 1]);
        Assert.Equal(3, destArray[1, 0]);
        Assert.Equal(4, destArray[1, 1]);
    }

    [Fact]
    public void FillAndClear_ShouldWorkCorrectly() {
        var array = new int[2, 2];
        var span  = new Span2D<int>(array);

        span.Fill(42);
        Assert.Equal(42, array[0, 0]);
        Assert.Equal(42, array[1, 1]);

        span.Clear();
        Assert.Equal(0, array[0, 0]);
        Assert.Equal(0, array[1, 1]);
    }
}