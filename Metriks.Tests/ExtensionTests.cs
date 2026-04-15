namespace Metriks.Tests;

public class ExtensionTests {
    
    [Fact]
    public void ArrayDimensions_2D_ShouldReturnCorrectSizes() {
        int[,] arr = new int[10, 20];
        Assert.Equal(10, arr.Len0);
        Assert.Equal(20, arr.Len1);
        Assert.Equal(new Size2D(10, 20), arr.Size);
        Assert.Equal(new Size2D(10, 20), arr.GetSize());
    }

    [Fact]
    public void ArrayDimensions_3D_ShouldReturnCorrectSizes() {
        int[,,] arr = new int[10, 20, 30];
        Assert.Equal(10, arr.Len0);
        Assert.Equal(20, arr.Len1);
        Assert.Equal(30, arr.Len2);
        Assert.Equal(new Size3D(10, 20, 30), arr.Size);
    }

    [Fact]
    public void ArrayDimensions_4D_ShouldReturnCorrectSizes() {
        int[,,,] arr = new int[10, 20, 30, 40];
        Assert.Equal(10, arr.Len0);
        Assert.Equal(20, arr.Len1);
        Assert.Equal(30, arr.Len2);
        Assert.Equal(40, arr.Len3);
        Assert.Equal(new Size4D(10, 20, 30, 40), arr.Size);
    }

    [Fact]
    public void ArrayManipulation_2D_Fill_ShouldWork() {
        int[,] arr = new int[3, 3];
        arr.Fill(42);
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                Assert.Equal(42, arr[i, j]);
    }

    [Fact]
    public void ArrayManipulation_2D_FillRegion_ShouldWork() {
        int[,] arr = new int[5, 5];
        arr.Fill(1, 1, 2, 1, 2); // fill (1,1) to (2,2) with 1? 
        // Wait, the code says: for (int i0 = start0; i0 <= end0; i0++) { for (int i1 = start1; i1 < end1; i1++) {
        // end0 = start0 + count0 = 1 + 2 = 3. end1 = start1 + count1 = 1 + 2 = 3.
        // i0 goes 1, 2, 3. i1 goes 1, 2.
        
        Assert.Equal(1, arr[1, 1]);
        Assert.Equal(1, arr[1, 2]);
        Assert.Equal(1, arr[2, 1]);
        Assert.Equal(1, arr[2, 2]);
        Assert.Equal(1, arr[3, 1]);
        Assert.Equal(1, arr[3, 2]);
        Assert.Equal(0, arr[1, 3]);
        Assert.Equal(0, arr[4, 1]);
    }

    [Fact]
    public void ArrayManipulation_2D_SafeFill_ShouldWork() {
        int[,] arr = new int[3, 3];
        arr.SafeFill(1, -1, 5, -1, 5);
        // start0 = -1.Clamp(0, 2) = 0. end0 = (-1 + 5).Clamp(0, 2) = 2.
        // start1 = -1.Clamp(0, 2) = 0. end1 = (-1 + 5).Clamp(0, 2) = 2.
        // i0 goes 0, 1. i1 goes 0, 1.
        
        Assert.Equal(1, arr[0, 0]);
        Assert.Equal(1, arr[0, 1]);
        Assert.Equal(1, arr[1, 0]);
        Assert.Equal(1, arr[1, 1]);
        Assert.Equal(0, arr[2, 2]);
    }

    [Fact]
    public void ArrayManipulation_2D_ToJagged_ShouldWork() {
        int[,] arr = { { 1, 2 }, { 3, 4 } };
        var jagged = arr.ToJagged();
        Assert.Equal(1, jagged[0][0]);
        Assert.Equal(2, jagged[0][1]);
        Assert.Equal(3, jagged[1][0]);
        Assert.Equal(4, jagged[1][1]);
    }

    [Fact]
    public void ArrayManipulation_1D_Range_ShouldWork() {
        int[] arr = { 1, 2, 3, 4, 5 };
        var range = arr.Range(1, 4); // 1, 2, 3? loop: for (int i = start; i < end; i++)
        Assert.Equal(3, range.Length);
        Assert.Equal(2, range[0]);
        Assert.Equal(3, range[1]);
        Assert.Equal(4, range[2]);
    }

    [Fact]
    public void ArrayManipulation_Resize_ShouldWork() {
        int[,] arr = { { 1, 2 }, { 3, 4 } };
        ArrayManipulation.Resize(ref arr, 3, 3);
        Assert.Equal(3, arr.GetLength(0));
        Assert.Equal(3, arr.GetLength(1));
        Assert.Equal(1, arr[0, 0]);
        Assert.Equal(4, arr[1, 1]);
        Assert.Equal(0, arr[2, 2]);
    }

    [Fact]
    public void Linq2D_List2D_AllAtX_AllAtY_ShouldWork() {
        var list = new List2D<int>(new int[3, 3]);
        list.Fill(1);
        list[1, 1] = 2;
        
        Assert.True(list.AllAtX(0, x => x == 1));
        Assert.False(list.AllAtX(1, x => x == 1));
        Assert.True(list.AllAtY(0, x => x == 1));
        Assert.False(list.AllAtY(1, x => x == 1));
    }

    [Fact]
    public void Linq2D_IEnumerable2D_Select_ShouldWork() {
        var list = new List2D<int>(new int[2, 2]);
        list[0, 0] = 1; list[0, 1] = 2;
        list[1, 0] = 3; list[1, 1] = 4;
        
        var sums = list.Select(sub => sub.Sum()).ToList();
        Assert.Equal(2, sums.Count);
        Assert.Equal(3, sums[0]); // 1+2
        Assert.Equal(7, sums[1]); // 3+4
    }

    [Fact]
    public void Linq2D_IEnumerable2D_GetAtX_GetAtY_ShouldWork() {
        var list = new List2D<int>(new int[2, 2]);
        list[0, 0] = 1; list[0, 1] = 2;
        list[1, 0] = 3; list[1, 1] = 4;
        
        var col0 = list.GetAtX(0).ToList();
        Assert.Equal(new[] { 1, 2 }, col0);

        var row0 = list.GetAtY(0).ToList();
        Assert.Equal(new[] { 1, 3 }, row0);
    }
}