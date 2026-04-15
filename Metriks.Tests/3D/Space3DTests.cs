namespace Metriks.Tests;

public class Space3DTests {
    
    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var space = new Space3D<int>();
        Assert.Equal(0, space.XSize);
        Assert.Equal(0, space.XOriginOffset);
    }

    [Fact]
    public void Indexer_WithOffset_ShouldWork() {
        var space = new Space3D<int>();
        space.Expand(1, 1, 1);
        space.MoveOrigin(1, 1, 1);
        
        // This means internal [0,0,0] is now at [-1, -1, -1] in coordinated space
        space[-1, -1, -1] = 42;
        Assert.Equal(42, space.UncoordinatedGet(0, 0, 0));
    }

    [Fact]
    public void Place_WithNegativeOffset_ShouldAdjustOrigin() {
        var space = new Space3D<int>();
        var matrix = new int[1, 1, 1];
        matrix[0, 0, 0] = 99;
        
        space.Place(matrix, new Point3D(-1, -1, -1));
        
        Assert.Equal(1, space.XOriginOffset);
        Assert.Equal(99, space[-1, -1, -1]);
        Assert.Equal(99, space.UncoordinatedGet(0, 0, 0));
    }

    [Fact]
    public void InsertAt_ShouldAdjustOrigin() {
        var space = new Space3D<int>();
        space.Expand(1, 1, 1);
        space[0, 0, 0] = 10;
        
        space.InsertAtX(-1); // Inserts at internal index 0
        Assert.Equal(1, space.XOriginOffset);
        Assert.Equal(10, space[0, 0, 0]);
        Assert.Equal(0, space[-1, 0, 0]); // New element

        space.InsertAtY(-1);
        Assert.Equal(1, space.YOriginOffset);

        space.InsertAtZ(-1);
        Assert.Equal(1, space.ZOriginOffset);
    }

    [Fact]
    public void RemoveAt_ShouldAdjustOrigin() {
        var space = new Space3D<int>();
        space.Expand(3, 3, 3);
        space.MoveOrigin(1, 1, 1);
        
        space.RemoveAtX(-1);
        Assert.Equal(0, space.XOriginOffset);

        space.RemoveAtY(-1);
        Assert.Equal(0, space.YOriginOffset);

        space.RemoveAtZ(-1);
        Assert.Equal(0, space.ZOriginOffset);
    }

    [Fact]
    public void Properties_ShouldReflectState() {
        var space = new Space3D<int>(new int[3, 3, 3]);
        space.MoveOrigin(1, 1, 1);
        
        Assert.Equal(-1, space.XStart);
        Assert.Equal(1, space.XEnd);
        Assert.Equal(new Point3D(1, 1, 1), space.OriginOffset);
    }

    [Fact]
    public void Clear_ShouldResetEverything() {
        var space = new Space3D<int>(new int[3, 3, 3]);
        space.MoveOrigin(1, 1, 1);
        space.Clear();
        
        Assert.Equal(0, space.XOriginOffset);
        Assert.Equal(0, space.YOriginOffset);
        Assert.Equal(0, space.ZOriginOffset);
        Assert.Equal(0, space.XSize);
    }

    [Fact]
    public void Indexer_WithDisableCoordinates_ShouldWork() {
        var space = new Space3D<int>(new int[3, 3, 3]);
        space.MoveOrigin(1, 1, 1);
        
        space[1, 1, 1, true] = 42; // This is internal [1,1,1]
        Assert.Equal(42, space[0, 0, 0]); // Coordinated [0,0,0] is internal [1,1,1]
    }
}