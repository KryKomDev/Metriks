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
        var space  = new Space3D<int>();
        var matrix = new int[1, 1, 1];
        matrix[0, 0, 0] = 99;

        space.Place(matrix, new Point3D(-1, -1, -1));

        Assert.Equal(1,  space.XOriginOffset);
        Assert.Equal(99, space[-1, -1, -1]);
        Assert.Equal(99, space.UncoordinatedGet(0, 0, 0));
    }

    [Fact]
    public void InsertAt_ShouldAdjustOrigin() {
        var space = new Space3D<int>();
        space.Expand(1, 1, 1);
        space[0, 0, 0] = 10;

        space.InsertAtX(-1); // Inserts at internal index 0
        Assert.Equal(1,  space.XOriginOffset);
        Assert.Equal(10, space[0, 0, 0]);
        Assert.Equal(0,  space[-1, 0, 0]); // New element

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

        Assert.Equal(-1,                   space.XStart);
        Assert.Equal(1,                    space.XEnd);
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

        space[1, 1, 1, true] = 42;        // This is internal [1,1,1]
        Assert.Equal(42, space[0, 0, 0]); // Coordinated [0,0,0] is internal [1,1,1]
    }

    [Fact]
    public void Constructor_FromOtherSpace3D_ShouldCopyDataAndOffsets() {
        var original = new Space3D<int>();
        original.Expand(4, 4, 4);
        original.MoveOrigin(2, 1, 3);
        original[-2, -1, -3] = 100;
        original[0, 0, 0] = 200;
        original[1, 2, 0] = 300;

        var copy = new Space3D<int>(original);

        Assert.Equal(original.XOriginOffset, copy.XOriginOffset);
        Assert.Equal(original.YOriginOffset, copy.YOriginOffset);
        Assert.Equal(original.ZOriginOffset, copy.ZOriginOffset);
        Assert.Equal(original.OriginOffset, copy.OriginOffset);
        Assert.Equal(original.XStart, copy.XStart);
        Assert.Equal(original.YStart, copy.YStart);
        Assert.Equal(original.ZStart, copy.ZStart);
        Assert.Equal(original.XEnd, copy.XEnd);
        Assert.Equal(original.YEnd, copy.YEnd);
        Assert.Equal(original.ZEnd, copy.ZEnd);
        Assert.Equal(original.Size, copy.Size);

        Assert.Equal(100, copy[-2, -1, -3]);
        Assert.Equal(200, copy[0, 0, 0]);
        Assert.Equal(300, copy[1, 2, 0]);

        original[-2, -1, -3] = 999;
        Assert.Equal(100, copy[-2, -1, -3]);
    }

    [Fact]
    public void Constructor_FromOtherSpace3D_Null_ShouldThrow() {
        Assert.Throws<ArgumentNullException>(() => new Space3D<int>((Space3D<int>)null!));
    }

    [Fact]
    public void Clone_ShouldPreserveOriginOffsetsAndData() {
        var original = new Space3D<int>();
        original.Expand(3, 3, 3);
        original.MoveOrigin(1, 2, 1);
        original[-1, -2, -1] = 42;

        Space3D<int> clone = original.Clone();

        Assert.Equal(original.XOriginOffset, clone.XOriginOffset);
        Assert.Equal(original.YOriginOffset, clone.YOriginOffset);
        Assert.Equal(original.ZOriginOffset, clone.ZOriginOffset);
        Assert.Equal(42, clone[-1, -2, -1]);

        clone[-1, -2, -1] = 99;
        Assert.Equal(42, original[-1, -2, -1]);
    }
}