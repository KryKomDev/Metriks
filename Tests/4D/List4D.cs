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
        var list4D = new List4D<int>(
            new int[,,,] {
                {
                    { { 1, 2 }, { 3, 4 } },
                    { { 5, 6 }, { 7, 8 } }
                }
            }
        );

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

    [Fact]
    public void Constructor_FromOtherList4D_ShouldCopyDataAndSizes() {
        var original = new List4D<int>(2, 2, 2, 2);
        original.Expand(2, 2, 2, 2);
        int counter = 1;
        for (int w = 0; w < 2; w++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 2; y++) {
                    for (int z = 0; z < 2; z++) {
                        original[w, x, y, z] = counter++;
                    }
                }
            }
        }

        var copy = new List4D<int>(original);

        Assert.Equal(2, copy.WSize);
        Assert.Equal(2, copy.XSize);
        Assert.Equal(2, copy.YSize);
        Assert.Equal(2, copy.ZSize);
        Assert.Equal(original.Size, copy.Size);

        for (int w = 0; w < 2; w++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 2; y++) {
                    for (int z = 0; z < 2; z++) {
                        Assert.Equal(original[w, x, y, z], copy[w, x, y, z]);
                    }
                }
            }
        }

        original[0, 0, 0, 0] = 999;
        Assert.Equal(1, copy[0, 0, 0, 0]);
    }

    [Fact]
    public void Constructor_FromOtherList4D_Null_ShouldThrow() {
        Assert.Throws<ArgumentNullException>(() => new List4D<int>((List4D<int>)null!));
    }

    [Fact]
    public void Constructor_FromReadOnlyList4D_ShouldCopyCorrectly() {
        var orig = new List4D<int>(2, 2, 2, 2);
        orig.Expand(2, 2, 2, 2);
        orig[0, 0, 0, 0] = 10;
        orig[1, 1, 1, 1] = 80;

        IReadOnlyList4D<int> original = orig;
        var copy = new List4D<int>(original);

        Assert.Equal(2, copy.WSize);
        Assert.Equal(2, copy.XSize);
        Assert.Equal(2, copy.YSize);
        Assert.Equal(2, copy.ZSize);
        Assert.Equal(10, copy[0, 0, 0, 0]);
        Assert.Equal(80, copy[1, 1, 1, 1]);
    }

    [Fact]
    public void Constructor_FromReadOnlyList4D_Null_ShouldThrow() {
        Assert.Throws<ArgumentNullException>(() => new List4D<int>((IReadOnlyList4D<int>)null!));
    }

    [Fact]
    public void Clone_ShouldCreateIndependentCopy() {
        var original = new List4D<int>(2, 2, 2, 2);
        original.Expand(2, 2, 2, 2);
        original[0, 0, 0, 0] = 7;
        original[1, 1, 1, 1] = 42;

        var clone = original.Clone();

        Assert.Equal(original.Size, clone.Size);
        Assert.Equal(7, clone[0, 0, 0, 0]);
        Assert.Equal(42, clone[1, 1, 1, 1]);

        clone[0, 0, 0, 0] = 123;
        Assert.Equal(7, original[0, 0, 0, 0]);
    }

    [Fact]
    public void CopyFrom_List4D_ShouldResizeAndCopyAllData() {
        var source = new List4D<int>(2, 2, 2, 2);
        source.Expand(2, 2, 2, 2);
        source[0, 0, 0, 0] = 1;
        source[1, 1, 1, 1] = 99;

        var dest = new List4D<int>();
        dest.CopyFrom(source);

        Assert.Equal(source.WSize, dest.WSize);
        Assert.Equal(source.XSize, dest.XSize);
        Assert.Equal(source.YSize, dest.YSize);
        Assert.Equal(source.ZSize, dest.ZSize);
        Assert.Equal(1, dest[0, 0, 0, 0]);
        Assert.Equal(99, dest[1, 1, 1, 1]);

        source[0, 0, 0, 0] = 555;
        Assert.Equal(1, dest[0, 0, 0, 0]);
    }

    [Fact]
    public void CopyFrom_IReadOnlyList4D_ShouldCopyData() {
        var src = new List4D<int>(2, 2, 2, 2);
        src.Expand(2, 2, 2, 2);
        src[0, 0, 0, 0] = 5;
        src[1, 1, 1, 1] = 25;

        IReadOnlyList4D<int> source = src;
        var dest = new List4D<int>();

        dest.CopyFrom(source);

        Assert.Equal(2, dest.WSize);
        Assert.Equal(2, dest.XSize);
        Assert.Equal(2, dest.YSize);
        Assert.Equal(2, dest.ZSize);
        Assert.Equal(5, dest[0, 0, 0, 0]);
        Assert.Equal(25, dest[1, 1, 1, 1]);
    }

    [Fact]
    public void CopyFrom_WithOffset_ShouldPlaceElementsCorrectly() {
        var dest = new List4D<int>(3, 3, 3, 3);
        dest.Expand(3, 3, 3, 3);

        var source = new List4D<int>(2, 2, 2, 2);
        source.Expand(2, 2, 2, 2);
        source[0, 0, 0, 0] = 1;
        source[1, 1, 1, 1] = 8;

        dest.CopyFrom(source, new Point4D(1, 1, 1, 1));

        Assert.Equal(0, dest[0, 0, 0, 0]);
        Assert.Equal(1, dest[1, 1, 1, 1]);
        Assert.Equal(8, dest[2, 2, 2, 2]);
    }

    [Fact]
    public void CopyFrom_SubRegion_ShouldCopyExactSlice() {
        var source = new List4D<int>(3, 3, 3, 3);
        source.Expand(3, 3, 3, 3);
        int counter = 1;
        for (int w = 0; w < 3; w++) {
            for (int x = 0; x < 3; x++) {
                for (int y = 0; y < 3; y++) {
                    for (int z = 0; z < 3; z++) {
                        source[w, x, y, z] = counter++;
                    }
                }
            }
        }

        var dest = new List4D<int>(2, 2, 2, 2);
        dest.Expand(2, 2, 2, 2);

        dest.CopyFrom(source, new Point4D(1, 1, 1, 1), new Point4D(0, 0, 0, 0), new Size4D(2, 2, 2, 2));

        for (int w = 0; w < 2; w++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 2; y++) {
                    for (int z = 0; z < 2; z++) {
                        Assert.Equal(source[w + 1, x + 1, y + 1, z + 1], dest[w, x, y, z]);
                    }
                }
            }
        }
    }

    [Fact]
    public void CopyFrom_Self_OverlappingRegion_ShouldNotCorruptData() {
        var list = new List4D<int>(4, 4, 4, 4);
        list.Expand(4, 4, 4, 4);
        int counter = 1;
        for (int w = 0; w < 4; w++) {
            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    for (int z = 0; z < 4; z++) {
                        list[w, x, y, z] = counter++;
                    }
                }
            }
        }

        var expected = new int[2, 2, 2, 2];
        for (int w = 0; w < 2; w++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 2; y++) {
                    for (int z = 0; z < 2; z++) {
                        expected[w, x, y, z] = list[w, x, y, z];
                    }
                }
            }
        }

        // dest > src
        list.CopyFrom(list, new Point4D(0, 0, 0, 0), new Point4D(1, 1, 1, 1), new Size4D(2, 2, 2, 2));

        for (int w = 0; w < 2; w++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 2; y++) {
                    for (int z = 0; z < 2; z++) {
                        Assert.Equal(expected[w, x, y, z], list[w + 1, x + 1, y + 1, z + 1]);
                    }
                }
            }
        }

        // dest < src
        list.CopyFrom(list, new Point4D(1, 1, 1, 1), new Point4D(0, 0, 0, 0), new Size4D(2, 2, 2, 2));

        for (int w = 0; w < 2; w++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 2; y++) {
                    for (int z = 0; z < 2; z++) {
                        Assert.Equal(expected[w, x, y, z], list[w, x, y, z]);
                    }
                }
            }
        }
    }

    [Fact]
    public void CopyTo_List4D_EmptyDestination_ShouldPopulate() {
        var source = new List4D<int>(2, 2, 2, 2);
        source.Expand(2, 2, 2, 2);
        source[0, 0, 0, 0] = 10;
        source[1, 1, 1, 1] = 40;

        var dest = new List4D<int>();
        source.CopyTo(dest);

        Assert.Equal(2, dest.WSize);
        Assert.Equal(2, dest.XSize);
        Assert.Equal(2, dest.YSize);
        Assert.Equal(2, dest.ZSize);
        Assert.Equal(10, dest[0, 0, 0, 0]);
        Assert.Equal(40, dest[1, 1, 1, 1]);
    }

    [Fact]
    public void CopyTo_List4D_ExistingDestination_ShouldCopy() {
        var source = new List4D<int>(2, 2, 2, 2);
        source.Expand(2, 2, 2, 2);
        source[0, 0, 0, 0] = 1;
        source[1, 1, 1, 1] = 2;

        var dest = new List4D<int>(3, 3, 3, 3);
        dest.Expand(3, 3, 3, 3);

        source.CopyTo(dest, new Point4D(1, 1, 1, 1));

        Assert.Equal(1, dest[1, 1, 1, 1]);
        Assert.Equal(2, dest[2, 2, 2, 2]);
    }

    [Fact]
    public void CopyTo_List4D_SubRegion_ShouldCopy() {
        var source = new List4D<int>(3, 3, 3, 3);
        source.Expand(3, 3, 3, 3);
        source[1, 1, 1, 1] = 99;

        var dest = new List4D<int>(2, 2, 2, 2);
        dest.Expand(2, 2, 2, 2);

        source.CopyTo(dest, new Point4D(1, 1, 1, 1), new Point4D(0, 0, 0, 0), new Size4D(1, 1, 1, 1));

        Assert.Equal(99, dest[0, 0, 0, 0]);
    }

    [Fact]
    public void CopyTo_List4D_TooSmall_ShouldThrow() {
        var source = new List4D<int>(2, 2, 2, 2);
        source.Expand(2, 2, 2, 2);

        var dest = new List4D<int>(1, 1, 1, 1);
        dest.Expand(1, 1, 1, 1);

        Assert.Throws<ArgumentException>(() => source.CopyTo(dest));
    }
}