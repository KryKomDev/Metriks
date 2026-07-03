namespace Metriks.Tests;

public class List4DTests {

    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var list4D = new List4D<int>(2, 3, 4, 5);
        Assert.Equal(0, list4D.WSize);
        Assert.Equal(2, list4D.WCapacity);
    }

    [Fact]
    public void AddWXYZ_ShouldIncreaseSizes() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();

        Assert.Equal(1, list4D.WSize);
        Assert.Equal(1, list4D.XSize);
        Assert.Equal(1, list4D.YSize);
        Assert.Equal(1, list4D.ZSize);
    }

    [Fact]
    public void Indexer_ShouldSetAndGet() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();

        list4D[0, 0, 0, 0] = 42;
        Assert.Equal(42, list4D[0, 0, 0, 0]);
    }

    [Fact]
    public void InsertAt_W_ShouldWork() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();
        list4D[0, 0, 0, 0] = 1;

        list4D.InsertAtW(0);
        list4D[0, 0, 0, 0] = 2;

        Assert.Equal(2, list4D.WSize);
        Assert.Equal(2, list4D[0, 0, 0, 0]);
        Assert.Equal(1, list4D[1, 0, 0, 0]);
    }

    [Fact]
    public void RemoveAt_X_ShouldWork() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.AddX();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();

        list4D[0, 0, 0, 0] = 1;
        list4D[0, 1, 0, 0] = 2;

        list4D.RemoveAtX(0);
        Assert.Equal(1, list4D.XSize);
        Assert.Equal(2, list4D[0, 0, 0, 0]);
    }

    [Fact]
    public void Contains_Methods_ShouldReturnCorrectResults() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();
        list4D[0, 0, 0, 0] = 42;

        Assert.True(list4D.Contains(42));
        Assert.True(list4D.ContainsAtW(0,  42));
        Assert.False(list4D.ContainsAtW(1, 42)); // Out of range but current implementation might handle differently, let's check
    }

    [Fact]
    public void Clear_ShouldReset() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.Clear();
        Assert.Equal(0, list4D.WSize);
    }

    [Fact]
    public void Enumerator_ShouldProvideSlices() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();
        list4D[0, 0, 0, 0] = 100;

        var count = 0;

        foreach (var slice3D in list4D)
        foreach (var slice2D in slice3D)
        foreach (var col in slice2D)
        foreach (var item in col) {
            Assert.Equal(100, item);
            count++;
        }

        Assert.Equal(1, count);
    }

    [Fact]
    public void Range_Indexer_ShouldWork() {
        var list4D = new List4D<int>(3, 1, 1, 1);
        list4D.AddW();
        list4D.AddW();
        list4D.AddW();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();
        list4D[0, 0, 0, 0] = 1;
        list4D[1, 0, 0, 0] = 2;
        list4D[2, 0, 0, 0] = 3;

        var slice = list4D[..2, 0, 0, 0];
        Assert.Equal(2, slice.Length);
        Assert.Equal(1, slice[0]);
        Assert.Equal(2, slice[1]);
    }

    [Fact]
    public void ToArray_ShouldWork() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();
        list4D[0, 0, 0, 0] = 42;

        var arr = list4D.ToArray();
        Assert.Equal(42, arr[0, 0, 0, 0]);
    }

    [Fact]
    public void ToJagged_ShouldWork() {
        var list4D = new List4D<int>();
        list4D.Expand(1, 1, 1, 1);
        list4D[0, 0, 0, 0] = 42;

        var jagged = list4D.ToJagged();
        Assert.Equal(42, jagged[0][0][0][0]);
    }

    [Fact]
    public void Fill_ShouldWork() {
        var list4D = new List4D<int>();
        list4D.Expand(2, 2, 2, 2);

        list4D.Fill(5);
        Assert.Equal(5, list4D[0, 0, 0, 0]);
        Assert.Equal(5, list4D[1, 1, 1, 1]);

        list4D.Fill(10, 1, 1, 1, 1, 1, 1, 1, 1);
        Assert.Equal(5,  list4D[0, 0, 0, 0]);
        Assert.Equal(10, list4D[1, 1, 1, 1]);
    }

    [Fact]
    public void Place_ShouldWork() {
        var list4D = new List4D<int>();
        var matrix = new int[1, 1, 1, 1];
        matrix[0, 0, 0, 0] = 77;

        list4D.Place(matrix, new Point4D(0, 0, 0, 0));
        Assert.Equal(77, list4D[0, 0, 0, 0]);
    }

    [Fact]
    public void Place_WithResizeFalse_ShouldWork() {
        var list4D = new List4D<int>();
        list4D.Expand(1, 1, 1, 1);
        var matrix = new int[1, 1, 1, 1];
        matrix[0, 0, 0, 0] = 77;

        list4D.Place(matrix, new Point4D(1, 1, 1, 1), false);
        Assert.Equal(1, list4D.WSize);

        // Should not have expanded and 77 should not be at 1,1,1,1
    }

    [Fact]
    public void Expand_ShouldIncreaseSizeAndKeepData() {
        var list4D = new List4D<int>();
        list4D.AddW();
        list4D.AddX();
        list4D.AddY();
        list4D.AddZ();
        list4D[0, 0, 0, 0] = 5;

        list4D.Expand(2, 2, 2, 2, 10);

        Assert.Equal(2,  list4D.WSize);
        Assert.Equal(5,  list4D[0, 0, 0, 0]);
        Assert.Equal(10, list4D[1, 1, 1, 1]);
    }

    [Fact]
    public void Resize_ShouldChangeSizeAndKeepData() {
        var list4D = new List4D<int>();
        list4D.Expand(2, 2, 2, 2, 1);
        list4D[0, 0, 0, 0] = 5;

        list4D.Resize(1, 1, 1, 1);

        Assert.Equal(1, list4D.WSize);
        Assert.Equal(5, list4D[0, 0, 0, 0]);
    }

    [Fact]
    public void Point4DIndexer_ShouldWork() {
        var list4D = new List4D<int>(1, 1, 1, 1);
        list4D.Expand(1, 1, 1, 1);
        var pt = new Point4D(0, 0, 0, 0);
        list4D[pt] = 99;
        Assert.Equal(99, list4D[pt]);
    }

    [Fact]
    public void SpanMethods_ShouldWork() {
        var list4D = new List4D<int>(new int[,,,] {
            {
                { { 1, 2 }, { 3, 4 } },
                { { 5, 6 }, { 7, 8 } }
            }
        });

        // 1. GetSpanAtWXY
        var zSpan = list4D.GetSpanAtWXY(0, 0, 1);
        Assert.Equal(2, zSpan.Length);
        Assert.Equal(3, zSpan[0]);
        Assert.Equal(4, zSpan[1]);

        // 2. CopyAtWXYTo
        var wxyDest = new int[2];
        list4D.CopyAtWXYTo(0, 0, 0, wxyDest);
        Assert.Equal(1, wxyDest[0]);
        Assert.Equal(2, wxyDest[1]);

        // 3. CopyAtWXZTo
        var wxzDest = new int[2];
        list4D.CopyAtWXZTo(0, 0, 1, wxzDest);
        Assert.Equal(2, wxzDest[0]);
        Assert.Equal(4, wxzDest[1]);

        // 4. CopyAtWYZTo
        var wyzDest2 = new int[2];
        list4D.CopyAtWYZTo(0, 1, 0, wyzDest2);
        Assert.Equal(3, wyzDest2[0]);
        Assert.Equal(7, wyzDest2[1]);

        // 5. CopyAtXYZTo
        var xyzDest = new int[1];
        list4D.CopyAtXYZTo(0, 0, 0, xyzDest);
        Assert.Equal(1, xyzDest[0]);

        // 6. CopySliceTo
        var sliceDest = new int[4];
        list4D.CopySliceTo(sliceDest, 0, 1, 0, 2, 0, 1, 0, 2);
        Assert.Equal(1, sliceDest[0]);
        Assert.Equal(2, sliceDest[1]);
        Assert.Equal(5, sliceDest[2]);
        Assert.Equal(6, sliceDest[3]);
    }
}