namespace Metriks.Tests;

public class List3DTests {

    [Fact]
    public void Constructor_Default_ShouldInitializeCorrectly() {
        var list3D = new List3D<int>();
        Assert.Equal(0, list3D.XSize);
        Assert.Equal(0, list3D.YSize);
        Assert.Equal(0, list3D.ZSize);
        Assert.Equal(4, list3D.XCapacity);
    }

    [Fact]
    public void Constructor_From3DArray_ShouldInitializeCorrectly() {
        var array = new int[2, 2, 2];
        array[0, 0, 0] = 1;
        array[1, 1, 1] = 8;

        var list3D = new List3D<int>(array);

        Assert.Equal(2, list3D.XSize);
        Assert.Equal(1, list3D[0, 0, 0]);
        Assert.Equal(8, list3D[1, 1, 1]);
    }

    [Fact]
    public void AddXYZ_ShouldIncreaseSizes() {
        var list3D = new List3D<int>();
        list3D.AddX();
        list3D.AddY();
        list3D.AddZ();

        Assert.Equal(1, list3D.XSize);
        Assert.Equal(1, list3D.YSize);
        Assert.Equal(1, list3D.ZSize);
    }

    [Fact]
    public void Indexer_Range_Slices_ShouldWork() {
        var list3D = new List3D<int>();
        list3D.Expand(3, 1, 1);
        list3D[0, 0, 0] = 1;
        list3D[1, 0, 0] = 2;
        list3D[2, 0, 0] = 3;

        #if NET5_0_OR_GREATER
        var slice = list3D[..2, 0, 0];
        Assert.Equal(2, slice.Length);
        Assert.Equal(1, slice[0]);
        Assert.Equal(2, slice[1]);
        #endif
    }

    [Fact]
    public void InsertAt_ShouldMaintainIntegrity() {
        var list3D = new List3D<int>();
        list3D.Expand(1, 1, 1);
        list3D[0, 0, 0] = 10;

        list3D.InsertAtX(0);
        list3D[0, 0, 0] = 5;

        Assert.Equal(2,  list3D.XSize);
        Assert.Equal(5,  list3D[0, 0, 0]);
        Assert.Equal(10, list3D[1, 0, 0]);

        list3D.InsertAtZ(0);
        list3D[0, 0, 0] = 1;
        Assert.Equal(2, list3D.ZSize);
        Assert.Equal(1, list3D[0, 0, 0]);
        Assert.Equal(5, list3D[0, 0, 1]);
    }

    [Fact]
    public void Expand_ShouldWorkWithDefaultValue() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2, 42);

        Assert.Equal(2,  list3D.XSize);
        Assert.Equal(42, list3D[1, 1, 1]);
    }

    [Fact]
    public void Resize_ShouldPreserveData() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2);
        list3D[0, 0, 0] = 1;

        list3D.Resize(1, 1, 1);
        Assert.Equal(1, list3D.XSize);
        Assert.Equal(1, list3D[0, 0, 0]);
    }

    [Fact]
    public void Contains_Methods_ShouldReturnCorrectResults() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2);
        list3D[1, 0, 1] = 99;

        Assert.True(list3D.Contains(99));
        Assert.True(list3D.ContainsAtX(1,  99));
        Assert.False(list3D.ContainsAtX(0, 99));
        Assert.True(list3D.ContainsAtZ(1, 99));
    }

    [Fact]
    public void Clear_ShouldResetEverything() {
        var list3D = new List3D<int>();
        list3D.Expand(5, 5, 5);
        list3D.Clear();

        Assert.Equal(0, list3D.Count);
        Assert.Equal(0, list3D.XSize);
    }

    [Fact]
    public void GetAt_Slices_ShouldBeEnumerable() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2);
        list3D[0, 1, 0] = 5;

        var sliceX = list3D.GetAtX(0); // This is an IEnumerable2D
        var col    = sliceX.GetAtY(1).ToList();

        Assert.Equal(2, col.Count);
        Assert.Equal(5, col[0]);
    }

    [Fact]
    public void ToArray_ShouldCreateCorrectDeepCopy() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 1, 1);
        list3D[0, 0, 0] = 1;
        list3D[1, 0, 0] = 2;

        var arr = list3D.ToArray();
        Assert.Equal(1, arr[0, 0, 0]);
        Assert.Equal(2, arr[1, 0, 0]);
    }

    [Fact]
    public void ToJagged_ShouldCreateCorrectDeepCopy() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2);
        list3D[1, 1, 1] = 42;

        var jagged = list3D.ToJagged();
        Assert.Equal(42, jagged[1][1][1]);

        // Ensure it's a deep copy
        jagged[1][1][1] = 0;
        Assert.Equal(42, list3D[1, 1, 1]);
    }

    [Fact]
    public void Fill_ShouldWorkCorrectly() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2);

        list3D.Fill(7);
        Assert.Equal(7, list3D[0, 0, 0]);
        Assert.Equal(7, list3D[1, 1, 1]);

        list3D.Fill(42, 1, 1, 1, 1, 1, 1);
        Assert.Equal(7,  list3D[0, 0, 0]);
        Assert.Equal(42, list3D[1, 1, 1]);
    }

    [Fact]
    public void Fill_Factory_ShouldWorkCorrectly() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2);

        var counter = 0;
        list3D.Fill(() => counter++);

        Assert.Equal(0, list3D[0, 0, 0]);
        Assert.Equal(7, list3D[1, 1, 1]);
    }

    [Fact]
    public void Place_ShouldWorkCorrectly() {
        var list3D = new List3D<int>();
        var matrix = new int[2, 2, 2];
        matrix[0, 0, 0] = 1;
        matrix[1, 1, 1] = 2;

        list3D.Place(matrix, new Point3D(1, 1, 1));

        Assert.Equal(3, list3D.XSize);
        Assert.Equal(3, list3D.YSize);
        Assert.Equal(3, list3D.ZSize);
        Assert.Equal(1, list3D[1, 1, 1]);
        Assert.Equal(2, list3D[2, 2, 2]);
    }

    [Fact]
    public void Place_WithResizeFalse_ShouldNotExpand() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2);
        var matrix = new int[2, 2, 2];
        matrix[0, 0, 0] = 1;
        matrix[1, 1, 1] = 2;

        // Place at offset 1,1,1. Max would be 3,3,3 which is out of bounds.
        list3D.Place(matrix, new Point3D(1, 1, 1), false);

        Assert.Equal(2, list3D.XSize);
        Assert.Equal(1, list3D[1, 1, 1]);

        // list3D[2,2,2] should not exist and not be set
    }

    [Fact]
    public void RemoveAt_ShouldWorkForAllDimensions() {
        var list3D = new List3D<int>(2, 2, 2);
        list3D.Expand(2, 2, 2);
        list3D[0, 0, 0] = 1;
        list3D[1, 0, 0] = 2;
        list3D[0, 1, 0] = 3;
        list3D[0, 0, 1] = 4;

        list3D.RemoveAtX(0);
        Assert.Equal(1, list3D.XSize);
        Assert.Equal(2, list3D[0, 0, 0]);

        list3D.RemoveAtY(0);
        Assert.Equal(1, list3D.YSize);

        list3D.RemoveAtZ(0);
        Assert.Equal(1, list3D.ZSize);
    }

    [Fact]
    public void CopyTo_ShouldWork() {
        var list3D = new List3D<int>(2, 1, 1);
        list3D.Expand(2, 1, 1);
        list3D[0, 0, 0] = 1;
        list3D[1, 0, 0] = 2;

        var target = new int[3, 1, 1];
        list3D.CopyTo(target, new Point3D(1, 0, 0));

        Assert.Equal(1, target[1, 0, 0]);
        Assert.Equal(2, target[2, 0, 0]);
    }

    [Fact]
    public void Point3DIndexer_ShouldWork() {
        var list3D = new List3D<int>(1, 1, 1);
        list3D.Expand(1, 1, 1);
        var pt = new Point3D(0, 0, 0);
        list3D[pt] = 99;
        Assert.Equal(99, list3D[pt]);
    }

    [Fact]
    public void SpanMethods_ShouldWork() {
        var list3D = new List3D<int>(
            new int[,,] {
                { { 1, 2 }, { 3, 4 } },
                { { 5, 6 }, { 7, 8 } }
            }
        );

        // 1. GetSpanAtXY
        var zSpan = list3D.GetSpanAtXY(0, 1);
        Assert.Equal(2, zSpan.Length);
        Assert.Equal(3, zSpan[0]);
        Assert.Equal(4, zSpan[1]);

        // 2. CopyAtXYTo
        var xyDest = new int[2];
        list3D.CopyAtXYTo(1, 0, xyDest);
        Assert.Equal(5, xyDest[0]);
        Assert.Equal(6, xyDest[1]);

        // 3. CopyAtXZTo
        var xzDest = new int[2];
        list3D.CopyAtXZTo(0, 1, xzDest);
        Assert.Equal(2, xzDest[0]);
        Assert.Equal(4, xzDest[1]);

        // 4. CopyAtYZTo
        var yzDest = new int[2];
        list3D.CopyAtYZTo(1, 0, yzDest);
        Assert.Equal(3, yzDest[0]);
        Assert.Equal(7, yzDest[1]);

        // 5. CopySliceTo
        var sliceDest = new int[4];
        list3D.CopySliceTo(sliceDest, 0, 2, 0, 1, 0, 2);
        Assert.Equal(1, sliceDest[0]);
        Assert.Equal(2, sliceDest[1]);
        Assert.Equal(5, sliceDest[2]);
        Assert.Equal(6, sliceDest[3]);
    }

    [Fact]
    public void Constructor_FromOtherList3D_ShouldCopyDataAndSizes() {
        var original = new List3D<int>(2, 3, 4);
        original.Expand(2, 3, 4);
        int counter = 1;
        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 3; y++) {
                for (int z = 0; z < 4; z++) {
                    original[x, y, z] = counter++;
                }
            }
        }

        var copy = new List3D<int>(original);

        Assert.Equal(2, copy.XSize);
        Assert.Equal(3, copy.YSize);
        Assert.Equal(4, copy.ZSize);
        Assert.Equal(original.Size, copy.Size);

        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 3; y++) {
                for (int z = 0; z < 4; z++) {
                    Assert.Equal(original[x, y, z], copy[x, y, z]);
                }
            }
        }

        original[0, 0, 0] = 999;
        Assert.Equal(1, copy[0, 0, 0]);
    }

    [Fact]
    public void Constructor_FromOtherList3D_Null_ShouldThrow() {
        Assert.Throws<ArgumentNullException>(() => new List3D<int>((List3D<int>)null!));
    }

    [Fact]
    public void Constructor_FromReadOnlyList3D_ShouldCopyCorrectly() {
        var orig = new List3D<int>(2, 2, 2);
        orig.Expand(2, 2, 2);
        orig[0, 0, 0] = 10;
        orig[1, 1, 1] = 80;

        IReadOnlyList3D<int> original = orig;
        var copy = new List3D<int>(original);

        Assert.Equal(2, copy.XSize);
        Assert.Equal(2, copy.YSize);
        Assert.Equal(2, copy.ZSize);
        Assert.Equal(10, copy[0, 0, 0]);
        Assert.Equal(80, copy[1, 1, 1]);
    }

    [Fact]
    public void Constructor_FromReadOnlyList3D_Null_ShouldThrow() {
        Assert.Throws<ArgumentNullException>(() => new List3D<int>((IReadOnlyList3D<int>)null!));
    }

    [Fact]
    public void Clone_ShouldCreateIndependentCopy() {
        var original = new List3D<int>(2, 2, 2);
        original.Expand(2, 2, 2);
        original[0, 0, 0] = 7;
        original[1, 1, 1] = 42;

        var clone = original.Clone();

        Assert.Equal(original.Size, clone.Size);
        Assert.Equal(7, clone[0, 0, 0]);
        Assert.Equal(42, clone[1, 1, 1]);

        clone[0, 0, 0] = 123;
        Assert.Equal(7, original[0, 0, 0]);
    }

    [Fact]
    public void CopyFrom_List3D_ShouldResizeAndCopyAllData() {
        var source = new List3D<int>(2, 3, 2);
        source.Expand(2, 3, 2);
        source[0, 0, 0] = 1;
        source[1, 2, 1] = 99;

        var dest = new List3D<int>();
        dest.CopyFrom(source);

        Assert.Equal(source.XSize, dest.XSize);
        Assert.Equal(source.YSize, dest.YSize);
        Assert.Equal(source.ZSize, dest.ZSize);
        Assert.Equal(1, dest[0, 0, 0]);
        Assert.Equal(99, dest[1, 2, 1]);

        source[0, 0, 0] = 555;
        Assert.Equal(1, dest[0, 0, 0]);
    }

    [Fact]
    public void CopyFrom_IReadOnlyList3D_ShouldCopyData() {
        var src = new List3D<int>(2, 2, 2);
        src.Expand(2, 2, 2);
        src[0, 0, 0] = 5;
        src[1, 1, 1] = 25;

        IReadOnlyList3D<int> source = src;
        var dest = new List3D<int>();

        dest.CopyFrom(source);

        Assert.Equal(2, dest.XSize);
        Assert.Equal(2, dest.YSize);
        Assert.Equal(2, dest.ZSize);
        Assert.Equal(5, dest[0, 0, 0]);
        Assert.Equal(25, dest[1, 1, 1]);
    }

    [Fact]
    public void CopyFrom_WithOffset_ShouldPlaceElementsCorrectly() {
        var dest = new List3D<int>(4, 4, 4);
        dest.Expand(4, 4, 4);

        var source = new List3D<int>(2, 2, 2);
        source.Expand(2, 2, 2);
        source[0, 0, 0] = 1;
        source[1, 1, 1] = 8;

        dest.CopyFrom(source, new Point3D(1, 1, 1));

        Assert.Equal(0, dest[0, 0, 0]);
        Assert.Equal(1, dest[1, 1, 1]);
        Assert.Equal(8, dest[2, 2, 2]);
        Assert.Equal(0, dest[3, 3, 3]);
    }

    [Fact]
    public void CopyFrom_SubRegion_ShouldCopyExactSlice() {
        var source = new List3D<int>(3, 3, 3);
        source.Expand(3, 3, 3);
        int counter = 1;
        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 3; y++) {
                for (int z = 0; z < 3; z++) {
                    source[x, y, z] = counter++;
                }
            }
        }

        var dest = new List3D<int>(2, 2, 2);
        dest.Expand(2, 2, 2);

        dest.CopyFrom(source, new Point3D(1, 1, 1), new Point3D(0, 0, 0), new Size3D(2, 2, 2));

        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 2; y++) {
                for (int z = 0; z < 2; z++) {
                    Assert.Equal(source[x + 1, y + 1, z + 1], dest[x, y, z]);
                }
            }
        }
    }

    [Fact]
    public void CopyFrom_Self_OverlappingRegion_ShouldNotCorruptData() {
        var list = new List3D<int>(4, 4, 4);
        list.Expand(4, 4, 4);
        int counter = 1;
        for (int x = 0; x < 4; x++) {
            for (int y = 0; y < 4; y++) {
                for (int z = 0; z < 4; z++) {
                    list[x, y, z] = counter++;
                }
            }
        }

        // Copy region (0,0,0)-(2,2,2) to (1,1,1) -> dest > src
        var expected = new int[2, 2, 2];
        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 2; y++) {
                for (int z = 0; z < 2; z++) {
                    expected[x, y, z] = list[x, y, z];
                }
            }
        }

        list.CopyFrom(list, new Point3D(0, 0, 0), new Point3D(1, 1, 1), new Size3D(2, 2, 2));

        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 2; y++) {
                for (int z = 0; z < 2; z++) {
                    Assert.Equal(expected[x, y, z], list[x + 1, y + 1, z + 1]);
                }
            }
        }

        // Now reverse: dest < src
        list.CopyFrom(list, new Point3D(1, 1, 1), new Point3D(0, 0, 0), new Size3D(2, 2, 2));

        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 2; y++) {
                for (int z = 0; z < 2; z++) {
                    Assert.Equal(expected[x, y, z], list[x, y, z]);
                }
            }
        }
    }

    [Fact]
    public void CopyTo_List3D_EmptyDestination_ShouldPopulate() {
        var source = new List3D<int>(2, 2, 2);
        source.Expand(2, 2, 2);
        source[0, 0, 0] = 10;
        source[1, 1, 1] = 40;

        var dest = new List3D<int>();
        source.CopyTo(dest);

        Assert.Equal(2, dest.XSize);
        Assert.Equal(2, dest.YSize);
        Assert.Equal(2, dest.ZSize);
        Assert.Equal(10, dest[0, 0, 0]);
        Assert.Equal(40, dest[1, 1, 1]);
    }

    [Fact]
    public void CopyTo_List3D_ExistingDestination_ShouldCopy() {
        var source = new List3D<int>(2, 2, 2);
        source.Expand(2, 2, 2);
        source[0, 0, 0] = 1;
        source[1, 1, 1] = 2;

        var dest = new List3D<int>(3, 3, 3);
        dest.Expand(3, 3, 3);

        source.CopyTo(dest, new Point3D(1, 1, 1));

        Assert.Equal(1, dest[1, 1, 1]);
        Assert.Equal(2, dest[2, 2, 2]);
    }

    [Fact]
    public void CopyTo_List3D_SubRegion_ShouldCopy() {
        var source = new List3D<int>(3, 3, 3);
        source.Expand(3, 3, 3);
        source[1, 1, 1] = 99;

        var dest = new List3D<int>(2, 2, 2);
        dest.Expand(2, 2, 2);

        source.CopyTo(dest, new Point3D(1, 1, 1), new Point3D(0, 0, 0), new Size3D(1, 1, 1));

        Assert.Equal(99, dest[0, 0, 0]);
    }

    [Fact]
    public void CopyTo_List3D_TooSmall_ShouldThrow() {
        var source = new List3D<int>(2, 2, 2);
        source.Expand(2, 2, 2);

        var dest = new List3D<int>(1, 1, 1);
        dest.Expand(1, 1, 1);

        Assert.Throws<ArgumentException>(() => source.CopyTo(dest));
    }
}