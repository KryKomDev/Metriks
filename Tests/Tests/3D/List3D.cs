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
        var slice = list3D[0..2, 0, 0];
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
        
        Assert.Equal(2, list3D.XSize);
        Assert.Equal(5, list3D[0, 0, 0]);
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
        
        Assert.Equal(2, list3D.XSize);
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
        Assert.True(list3D.ContainsAtX(1, 99));
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
        var col = sliceX.GetAtY(1).ToList();
        
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
        Assert.Equal(7, list3D[0, 0, 0]);
        Assert.Equal(42, list3D[1, 1, 1]);
    }

    [Fact]
    public void Fill_Factory_ShouldWorkCorrectly() {
        var list3D = new List3D<int>();
        list3D.Expand(2, 2, 2);
        
        int counter = 0;
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
        list3D.Place(matrix, new Point3D(1, 1, 1), resize: false);

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
}